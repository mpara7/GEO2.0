using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs
{
    public sealed class GlobalRecorder
    {
        // 使用 Lazy<T> 确保线程安全且延迟初始化
        private static readonly Lazy<GlobalRecorder> _instance = new Lazy<GlobalRecorder>(() => new GlobalRecorder());

        public static GlobalRecorder Instance => _instance.Value;

        // 用于存储各个方法产生的数字记录
        // key：方法名，value：该方法产生的数字列表
        private readonly ConcurrentDictionary<string, List<int>> _records = new ConcurrentDictionary<string, List<int>>();

        // 私有构造函数，确保外部无法直接实例化
        private GlobalRecorder() { }

        /// <summary>
        /// 记录指定方法产生的数字
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <param name="value">要记录的数字</param>
        public void Record(string methodName, int value)
        {
            // 如果字典中没有该方法的记录，则创建一个新的列表
            var list = _records.GetOrAdd(methodName, key => new List<int>());

            // List<T> 不是线程安全的，对列表操作时需要加锁
            lock (list)
            {
                list.Add(value);
            }
        }

        /// <summary>
        /// 获取指定方法的所有记录
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <returns>数字列表</returns>
        public IEnumerable<int> GetRecords(string methodName)
        {
            if (_records.TryGetValue(methodName, out var list))
            {
                lock (list)
                {
                    // 返回列表的副本
                    return new List<int>(list);
                }
            }
            return Array.Empty<int>();
        }

        /// <summary>
        /// 获取所有方法的记录
        /// </summary>
        /// <returns>包含方法名称及对应数字列表的字典</returns>
        public IReadOnlyDictionary<string, List<int>> GetAllRecords()
        {
            var snapshot = new Dictionary<string, List<int>>();
            foreach (var kvp in _records)
            {
                lock (kvp.Value)
                {
                    snapshot[kvp.Key] = new List<int>(kvp.Value);
                }
            }
            return snapshot;
        }

        /// <summary>
        /// 清除所有记录（全局清零）
        /// </summary>
        public void Clear()
        {
            _records.Clear();
        }

        /// <summary>
        /// 清除指定方法的记录
        /// </summary>
        /// <param name="methodName">方法名称</param>
        public void Clear(string methodName)
        {
            _records.TryRemove(methodName, out _);
        }
    }
}

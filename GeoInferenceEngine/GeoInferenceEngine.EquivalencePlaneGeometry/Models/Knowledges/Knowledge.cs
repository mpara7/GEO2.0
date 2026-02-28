using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.Knowledges
{
    public abstract class Knowledge
    {
        public static Dictionary<string, ulong> ClassIndexDict { get; set; } = new();
        public static void InitClassIndex(params Assembly[] assemblies)
        {
            ClassIndexDict.Clear();
            ulong classIndex = 0;
            //点作为通用的
            foreach (var assemly in assemblies)
            {
                foreach (var type in assemly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Knowledge)))
                    {
                        ClassIndexDict.Add(type.Name, classIndex);
                        classIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// 获取组成部分
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Knowledge this[int index] => Properties[index];
        /// <summary>
        /// 构成的谓词
        /// </summary>
        public List<Knowledge> Properties { get; set; } = new List<Knowledge>();
        public Expr Expr { get; set; }

        /// <summary>
        /// 条件谓词
        /// </summary>
        public List<Knowledge> Conditions { get; set; } = new();
        public string Reason { get; set; }

        /// <summary>
        /// 本知识在该类别的索引值
        /// </summary>
        public uint PosIndex
        {
            get;
            set;
        }
        public ulong HashCode { get; set; } = 0;
        public bool IsAvailable { get; set; } = true;
        public bool IsUpdated { get; set; } = false;

        #region 辅助构成知识的相关函数
        /// <summary>
        /// 添加组成部分
        /// </summary>
        /// <param name="objs"></param>
        protected void Add(params object[] objs)
        {
            foreach (var obj in objs)
            {
                if (obj is null)
                    throw new ArgumentNullException();
                else if (obj is Knowledge pred)
                    Properties.Add(pred);
                else if (obj is Expr expr)
                {
                    Expr = expr;
                }
                else
                    throw new ArgumentException();
            }
        }
        public void SetNameHashCode(string name)
        {
            HashCode = ClassIndexDict[GetType().Name] << 54 | (uint)name.GetHashCode();
        }
        public virtual void SetHashCode()
        {
            HashCode = ClassIndexDict[GetType().Name] << 54;
            for (int k = 0; k < Properties.Count && k < 9; ++k)
            {
                if (k == 0)
                    HashCode |= Properties[k].PosIndex;
                else
                    HashCode |= (ulong)Properties[k].PosIndex << k * 6;
            }
        }

        public void AddReason([CallerMemberName] string reason = "error")
        {
            Reason = reason;
        }
        public void AddCondition(IEnumerable<Knowledge> conditionPreds)
        {
            foreach (var condition in conditionPreds)
            {
                Conditions.Add(condition);
            }
        }
        public void AddCondition(params Knowledge[] conditionPreds)
        {
            foreach (var condition in conditionPreds)
            {
                Conditions.Add(condition);

            }
        }
        public void AddCondition(string reason, params Knowledge[] conditionPreds)
        {
            Reason = reason;
            foreach (var condition in conditionPreds)
            {
                Conditions.Add(condition);
            }
        }
        public static bool operator ==(Knowledge k1, Knowledge k2)
        {
            // 如果两个都是 null，则它们相等
            if (ReferenceEquals(k1, k2))
                return true;

            // 如果其中一个是 null，另一个不是，则它们不相等
            if (k1 is null || k2 is null)
                return false;
            return k1.HashCode == k2.HashCode;
        }
        public static bool operator !=(Knowledge k1, Knowledge k2)
        {
            return k1.HashCode != k2.HashCode;
        }
        
        public virtual Knowledge Clone()
        {
            var newPred = (Knowledge)RuntimeHelpers.GetUninitializedObject(GetType());
            newPred.Properties = new List<Knowledge>(Properties);
            newPred.Expr = Expr?.Clone();
            newPred.Conditions = new List<Knowledge>();
            return newPred;
        }
        #endregion

        #region 抽象函数
        /// <summary>
        /// 非正式的
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            throw new Exception("忘记重写ToString函数了吧(￣、￣)");
        }
        /// <summary>
        /// 几何信息归一化处理 并且负责判断信息是否可用 在推理引擎中调用 构造函数不管
        /// </summary>
        public abstract void Normalize();
        #endregion


        #region 归一化辅助函数
        /// <summary>
        /// 当谓词所有组成部分的类型都一致时且没有次序关系时使用
        /// </summary>
        /// <param name="pred"></param>
        public void Sort()
        {
            Properties.Sort(new Comparison<Knowledge>((pred1, pred2) =>
            {
                if (pred1.PosIndex == pred2.PosIndex)
                    return 0;
                else if (pred1.PosIndex > pred2.PosIndex)
                    return 1;
                else
                    return -1;
            }));
        }
        /// <summary>
        /// 对指定的位置进行排序
        /// </summary>
        /// <param name="pred"></param>
        /// <param name="postions"></param>
        public void Sort(int pos1, int pos2)
        {
            if (Properties[pos1].PosIndex > Properties[pos2].PosIndex)
            {
                var temp = Properties[pos1];
                Properties[pos1] = Properties[pos2];
                Properties[pos2] = temp;
            }
        }
        /// <summary>
        /// 对指定的位置进行排序
        /// </summary>
        /// <param name="pred"></param>
        /// <param name="postions"></param>
        public void Sort(params int[] postions)
        {
            List<Knowledge> knowledgesForSort = new List<Knowledge>();
            foreach (var pos in postions)
            {
                knowledgesForSort.Add(Properties[pos]);
            }
            knowledgesForSort.Sort(new Comparison<Knowledge>((pred1, pred2) =>
            {
                if (pred1.PosIndex == pred2.PosIndex)
                    return 0;
                else if (pred1.PosIndex > pred2.PosIndex)
                    return 1;
                else
                    return -1;
            }));
            int index = 0;
            foreach (var pos in postions)
            {
                Properties[pos] = knowledgesForSort[index];
                index++;
            }
        }
        #endregion

        #region Debug工具函数
        public bool StrContains(string content)
        {
            return ToString().Contains(content);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GeoInferenceEngine.Backbone
{
    public static class GlobalEngineConfigs
    {
        private static string savePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GeoInference\\Configs";

        public static void Save(string name, EngineConfig config)
        {
            Directory.CreateDirectory(savePath);
            var yaml = YAML.Serialize(config);
            File.WriteAllText(savePath + "\\" + name + ".zec", yaml);
        }

        public static string[] List()
        {
            var list = Directory.GetFiles(savePath).Select(p => p.Replace(savePath + "\\", "").Replace(".zec", ""));
            Console.WriteLine("推理全局设置包括：");
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
            return list.ToArray();
        }

        public static void Delete(string name)
        {
            File.Delete(savePath + "\\" + name);
        }

        public static void Clear()
        {
            foreach (var item in Directory.GetFiles(savePath))
            {
                File.Delete(item);
            }
        }

        public static EngineConfig? Load(string name)
        {
            try
            {
                var yaml = File.ReadAllText(savePath + "\\" + name + ".zec");
                return YAML.Deserialize<EngineConfig>(yaml);
            }
            catch { throw new InvalidOperationException(); }
        }
    }
}
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZTool.Tools;

namespace ZTool.UsefulInterfaces;
public interface IFileSingleton<T> where T : new()
{
    private static T _instance;
    private static readonly object _syslock = new object();
    public static string SaveDir { get; set; } = "Singletons";
    public static string SaveFileName { get; set; }
    public static string SavePath { get => SaveDir is not null ? $"{SaveDir}/{SaveFileName}.txt" : $"{SaveFileName}.txt"; }
    public static bool IsThrowException { get; set; } = false;
    public static bool IsUseJsonOrYAML { get; set; } = false;
    public static T Instance
    {
        get
        {
            if (_instance == null)//两重锁结构
            {
                lock (_syslock)
                {
                    if (_instance == null)
                    {
                        string runtimeContextInJson;
                        try
                        {
                            if (SaveFileName is null)
                            {
                                SaveFileName = $"{typeof(T).Name}";
                            }

                            runtimeContextInJson = File.ReadAllText(SavePath);
                            try
                            {
                                if (IsUseJsonOrYAML)
                                {
                                    _instance =JsonConvert.DeserializeObject<T>(runtimeContextInJson);
                                }
                                else
                                {
                                    _instance = YAML.Deserialize<T>(runtimeContextInJson);
                                }
                            }
                            catch (Exception ex)
                            {
                                _instance = new T();
                                if (IsThrowException)
                                    throw ex;
                            }
                        }
                        catch (IOException ex)
                        {
                            _instance = new T();
                            if (IsThrowException)
                                throw ex;
                        }
                    }
                }
            }
            return _instance;
        }
    }

    public virtual void Save()
    {
        if (!Directory.Exists(SaveDir))
            Directory.CreateDirectory(SaveDir);
        try
        {
            if (IsUseJsonOrYAML)
            {
                var str = JsonConvert.SerializeObject(this);
                File.WriteAllText(SavePath, str);
            }
            else
            {
                var str = YAML.Serialize(this);
                File.WriteAllText(SavePath, str);
            }

            
        }
        catch (Exception ex)
        {
            if (IsThrowException)
                throw ex;
        }

    }
    public void DeleteFile()
    {
        try
        {
            File.Delete(SavePath);
        }
        catch (Exception ex)
        {
            if (IsThrowException)
                throw ex;
        }

    }
}

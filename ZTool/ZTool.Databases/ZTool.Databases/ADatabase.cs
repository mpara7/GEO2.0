namespace ZTool.Databases;
public abstract class ADatabase
{
    public abstract bool IsCollectionExists(string collectionName);
    /// <summary>
    /// 创建数据库
    /// </summary>
    /// <param name="dataObject"></param>
    public abstract void Create(string collectionName);
    /// <summary>
    /// 删除数据库
    /// </summary>
    /// <param name="dataObject"></param>
    public abstract void Drop(string collectionName);
    /// <summary>
    /// 清空数据库
    /// </summary>
    /// <param name="dataObject"></param>
    public abstract void Clear(string collectionName);
    /// <summary>
    /// 列举所有数据库
    /// </summary>
    /// <returns></returns>
    public abstract List<string> GetCollectionNames();
}

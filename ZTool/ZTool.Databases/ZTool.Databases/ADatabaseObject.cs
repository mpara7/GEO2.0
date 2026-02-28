namespace ZTool.Databases;
/// <summary>
/// 数据库对象，其字段与数据库内能一一对应
/// 约定规则：
/// 1.子类只定义字段，不定义属性
/// 2.字段不能设置默认值,推荐基本类型设为可空基本类型
/// 3.字段遵循大驼峰
/// 说明：
/// 1.由于主要对数据库一一对应，没有必要做赋值检查等额外操作，如有需要，构造新类型处理并用DataMaper转换
/// 2.当使用部分映射时，没被映射字段为空，避免被初始值污染字段
/// 3.作为对外字段，一般使用大驼峰
/// </summary>
public abstract class ADatabaseObject
{
    public string Id { get; set; }
}

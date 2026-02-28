using MongoDB.Bson.Serialization.Attributes;

using ZTool.Databases;

namespace GeoDatabase.Shared.Models;
public class User : ADatabaseObject
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}

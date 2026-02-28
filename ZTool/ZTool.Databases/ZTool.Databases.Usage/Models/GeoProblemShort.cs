namespace GeoDatabase.Shared.Models;

public class GeoProblemShort
{
    public string Title { get; set; } = "猫猫";
    public string Id { get; set; }
    public DateTime LastUpdateTime { get; set; }
    public Dictionary<string, string> Labels { get; set; } = new();
    public List<string> ThumbNails { get; set; } = new List<string>();
}

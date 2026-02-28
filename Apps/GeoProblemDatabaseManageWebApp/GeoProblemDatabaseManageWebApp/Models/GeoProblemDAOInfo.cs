using GeoProblemDatabases.Models;

namespace GeoProblemDatabaseManageWebApp.Models;

public class GeoProblemInfo
{
    public GeoProblemDAO GeoProblemDAO { get; set; }
    public Dictionary<string, ImageDAO> ImageDAOs { get; set; } = new Dictionary<string, ImageDAO>();
    public InferenceInputDAO InferenceInputDAO { get; set; }
    public InferenceOutputDAO InferenceOutputDAO { get; set; }
}

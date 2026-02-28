namespace GeoWebsite.Shared.Models;
public class ProblemCondition
{
    public string ProblemType { get; set; }
    public long CurPageNum { get; set; }
    public long PageSize { get; set; }
    public bool IsDeleted { get; set; }
}

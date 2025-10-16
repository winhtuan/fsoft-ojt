namespace Plantpedia.DTO;

public class UserActivityItemDto
{
    public long ActivityId { get; set; }
    public string Type { get; set; } = ""; // "Comment" | "Reaction" | "Search"
    public string Description { get; set; } = ""; // mô tả gộp từ RefId/Metadata
    public DateTime CreatedAt { get; set; } // UTC
}

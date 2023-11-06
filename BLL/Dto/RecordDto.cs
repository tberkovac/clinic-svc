namespace BLL.Dto;

public class RecordDto {
    public int RecordId { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
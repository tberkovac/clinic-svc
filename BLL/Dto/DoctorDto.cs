namespace BLL.Dto;

public class DoctorDto {
    public int DoctorId { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public int TitleId { get; set; }
    public TitleDto? Title { get; set; }
    public required string Code { get; set; }
    public required string Password { get; set; }
}
namespace BLL.Dto;

public class PatientDto {
    public int? PatientId { get; set; }
    public required string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public required GenderDto Gender { get; set; }
    public required string UMCN { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
}
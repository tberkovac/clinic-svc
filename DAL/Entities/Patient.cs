namespace DAL.Entities;

public class Patient {
    public int PatientId { get; set; }
    public required string FullName { get; set; }
    public required DateTime DateOfBirth { get; set; }
    public required Gender Gender { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public required string UMCN { get; set; }
}
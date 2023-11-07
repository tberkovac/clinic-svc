namespace DAL.Entities;

public class Doctor {
    public int DoctorId { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public int TitleId { get; set; }
    public Title? Title { get; set; }
    public required string Code { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public bool IsDeleted { get; set; }
}
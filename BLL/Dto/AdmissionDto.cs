namespace BLL.Dto;

public class AdmissionDto {
    public int AdmissionId { get; set; }
    public DateTime AdmissionDate { get; set; }
    public int PatientId { get; set; }
    public PatientDto? Patient { get; set; }
    public int DoctorId { get; set; }
    public DoctorDto? Doctor { get; set; }
    public int? RecordId { get; set; }
    public RecordDto? Record { get; set; }
    public bool IsEmergency { get; set; }
}
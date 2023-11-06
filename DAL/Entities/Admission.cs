using System;
namespace DAL.Entities
{
	public class Admission
	{
        public int AdmissionId { get; set; }
        public DateTime AdmissionDate { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
        public int? RecordId { get; set; }
        public Record? Record { get; set; }
        public bool IsEmergency { get; set; }
    }
}


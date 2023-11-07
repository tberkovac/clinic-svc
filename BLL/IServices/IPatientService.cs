using BLL.Dto;
using DAL.Entities;

namespace BLL.IServices;

public interface IPatientService 
{
    Task<PatientDto> CreatePatient(PatientDto patientDto);
    Task<PatientDto> DeletePatient(int patientId);
    Task<List<PatientDto>> GetAllPatients();
    Task<ResponsePageDto<PatientDto>> GetPaginatedPatients(SearchParamsDto searchParamsDto);
}
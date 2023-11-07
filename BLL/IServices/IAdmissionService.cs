using BLL.Dto;

namespace BLL.IServices
{
    public interface IAdmissionService
	{
        Task<ResponsePageDto<AdmissionDto>> GetAllAdmissions(SearchParamsDto searchParams);
        Task<AdmissionDto> Create(AdmissionDto admissionDto);
        Task<List<AdmissionDto>> GetDoctorsAdmissions(int doctorId);
        Task<AdmissionDto> DeleteAdmission(int admissionId);
    }
}


using BLL.Dto;

namespace BLL.IServices
{
    public interface IAdmissionService
	{
        Task<ResponsePageDto<AdmissionDto>> GetAllAdmissions(SearchAdmissionsParamsDto searchParams);
        Task<AdmissionDto> Create(AdmissionDto admissionDto);
        Task<ResponsePageDto<AdmissionDto>> GetDoctorsAdmissions(SearchAdmissionsParamsDto searchParams, int doctorId);
        Task<AdmissionDto> DeleteAdmission(int admissionId);
    }
}


using System;
using BLL.Dto;

namespace BLL.IServices
{
	public interface IDoctorService
	{
        Task<DoctorDto> CreateDoctor(DoctorDto doctorDto);
        Task<DoctorDto> DeleteDoctor(int doctorId);
        Task<List<DoctorDto>> GetAllSpecialists();
        Task<List<DoctorDto>> GetDoctors();
        Task<ResponsePageDto<DoctorDto>> GetPaginatedDoctors(SearchParamsDto searchParamsDto);
    }
}


using System;
using BLL.Dto;
using DAL.Entities;

namespace BLL.IServices
{
	public interface IDoctorService
	{
        Task<DoctorDto> CreateDoctor(DoctorDto doctorDto);
        Task<List<DoctorDto>> GetDoctors();
        Task<ResponsePageDto<DoctorDto>> GetPaginatedDoctors(SearchParamsDto searchParamsDto);
    }
}


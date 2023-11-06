using System;
using BLL.Dto;
using BLL.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DoctorsController : ControllerBase
    {
		private readonly IDoctorService _doctorService;

		public DoctorsController(IDoctorService doctorService)
		{
			_doctorService = doctorService;
		}

		[HttpPost("Create")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<DoctorDto>> CreateDoctor(DoctorDto doctorDto)
		{
			var result = await _doctorService.CreateDoctor(doctorDto);
			return result;
		}

		[HttpGet("GetAll")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<List<DoctorDto>>> GetAll()
		{
			var result = await _doctorService.GetDoctors();
			return result;
		}

        [HttpGet("GetPage")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponsePageDto<DoctorDto>>> GetPage([FromQuery] SearchParamsDto searchParams)
        {
            var result = await _doctorService.GetPaginatedDoctors(searchParams);
            return result;
        }
    }
}


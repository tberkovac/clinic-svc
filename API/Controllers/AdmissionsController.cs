using System;
using BLL.Dto;
using BLL.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AdmissionsController : ControllerBase
	{
		private readonly IAdmissionService _admissionService;

		public AdmissionsController(IAdmissionService admissionService)
		{
			_admissionService = admissionService;
		}

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponsePageDto<AdmissionDto>>> GetAll([FromQuery] SearchParamsDto searchParams)
        {
            var admissions = await _admissionService.GetAllAdmissions(searchParams);
            return admissions;
        }

        [HttpPost("Create")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<AdmissionDto>> Create(AdmissionDto admissionDto)
		{
			var admission = await _admissionService.Create(admissionDto);
			return admission;
		}

		[HttpGet("GetDoctorsAdmissions/{doctorId}")]
		[Authorize(Roles = "Admin,Doctor")]
		public async Task<ActionResult<List<AdmissionDto>>> GetDoctorsAdmissions(int doctorId)
		{
			var admissions = await _admissionService.GetDoctorsAdmissions(doctorId);
			return admissions;
		}

		[HttpDelete("Delete/{admissionId}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<AdmissionDto>> DeleteAdmission(int admissionId)
		{
			var deletedAdmission = await _admissionService.DeleteAdmission(admissionId);
			return deletedAdmission;
		}

    }
}


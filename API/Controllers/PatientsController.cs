using Microsoft.AspNetCore.Mvc;
using DAL;
using BLL.Dto;
using MySql.Data.MySqlClient;
using BLL.IServices;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PatientDto>> CreatePatient(PatientDto patientDto)
    {
        var result = await _patientService.CreatePatient(patientDto);
        return result;
    }

    [HttpGet("GetAll")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PatientDto>>> GetAll()
    {
        var result = await _patientService.GetAllPatients();
        return result;
    }

    [HttpGet("GetPage")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResponsePageDto<PatientDto>>> GetPage([FromQuery] SearchParamsDto searchParams)
    {
        var result = await _patientService.GetPaginatedPatients(searchParams);
        return result;
    }
}

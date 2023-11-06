using BLL.Dto;
using BLL.IServices;
using DAL.IRepositories;
using DAL.Entities;
using AutoMapper;
using DAL.Repositories;

namespace BLL.Services;

public class PatientService : IPatientService 
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public PatientService(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<List<PatientDto>> GetAllPatients()
    {
        var patients = await _patientRepository.GetAll();
        return _mapper.Map<List<PatientDto>>(patients);
    }

    public async Task<PatientDto> CreatePatient(PatientDto patientDto) 
    {
        var patient = _mapper.Map<Patient>(patientDto);
        _patientRepository.Add(patient);
        await _patientRepository.SaveChangesAsync();
        return _mapper.Map<PatientDto>(patient);
    }

    public async Task<ResponsePageDto<PatientDto>> GetPaginatedPatients(SearchParamsDto searchParamsDto)
    {
        var searchParams = _mapper.Map<SearchParams>(searchParamsDto);
        var patients = await _patientRepository.GetWithIncludesPaginatedAsync(searchParams);

        return _mapper.Map<ResponsePageDto<PatientDto>>(patients);
    }
}
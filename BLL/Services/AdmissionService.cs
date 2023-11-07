using System;
using AutoMapper;
using BLL.Dto;
using BLL.IServices;
using DAL.Entities;
using DAL.IRepositories;

namespace BLL.Services
{
	public class AdmissionService : IAdmissionService
	{
        private readonly IAdmissionRepository _admissionRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

		public AdmissionService(IAdmissionRepository admissionRepository, IDoctorRepository doctorRepository, IMapper mapper)
		{
            _admissionRepository = admissionRepository;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
		}

        public async Task<ResponsePageDto<AdmissionDto>> GetAllAdmissions(SearchParamsDto searchParamsDto)
        {
            var searchParams = _mapper.Map<SearchParams>(searchParamsDto);
            var admissions = await _admissionRepository.GetWithIncludesPaginatedAsync(searchParams, x => x.Doctor, x => x.Patient, x => x.Record);    
            
            return _mapper.Map<ResponsePageDto<AdmissionDto>>(admissions);
        }

        public async Task<AdmissionDto> Create(AdmissionDto admissionDto)
        {
            var admission = _mapper.Map<Admission>(admissionDto);
            _admissionRepository.Add(admission);
            await _admissionRepository.SaveChangesAsync();
            admissionDto.AdmissionId = admission.AdmissionId;
            return admissionDto;
        }

        public async Task<List<AdmissionDto>> GetDoctorsAdmissions(int userId)
        {
            var doctor = await _doctorRepository.Find(x => x.UserId == userId);

            if (doctor == null)
            {
                throw new Exception("User with provided id is not doctor!");
            }

            var admissions = await _admissionRepository
                .GetFilteredWithIncludesAsync(x => x.DoctorId == doctor.DoctorId,
                x => x.Doctor, x => x.Patient, x => x.Record);
            return _mapper.Map<List<AdmissionDto>>(admissions);
        }

        public async Task<AdmissionDto> DeleteAdmission(int admissionId)
        {
            var admission = await _admissionRepository.GetById(admissionId);

            if (admission == null)
            {
                throw new Exception($"Admission with provided id {admissionId} does not exist");
            }

            if (admission.RecordId != null)
            {
                admission.IsDeleted = true;
            }
            else
            {
                _admissionRepository.Remove(admission);
            }

            await _admissionRepository.SaveChangesAsync();

            return _mapper.Map<AdmissionDto>(admission);
        }
    }
}


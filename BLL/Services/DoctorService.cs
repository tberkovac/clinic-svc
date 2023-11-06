using System;
using System.Text;
using AutoMapper;
using BLL.Dto;
using BLL.IServices;
using DAL.Entities;
using DAL.IRepositories;

namespace BLL.Services
{
	public class DoctorService: IDoctorService
	{
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

		public DoctorService(IDoctorRepository doctorRepository, IUserRepository userRepository, IMapper mapper)
		{
            _doctorRepository = doctorRepository;
            _userRepository = userRepository;
            _mapper = mapper;
		}

        public async Task<DoctorDto> CreateDoctor(DoctorDto doctorDto)
        {
            var user = CreateUser(doctorDto);
            _userRepository.Add(user);
            await _userRepository.SaveChangesAsync();

            var doctor = _mapper.Map<Doctor>(doctorDto);
            doctor.UserId = user.UserId;
            _doctorRepository.Add(doctor);
            await _doctorRepository.SaveChangesAsync();
            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<List<DoctorDto>> GetDoctors()
        {
            var doctors = await _doctorRepository.GetWithIncludesAsync(d => d.Title);
            return _mapper.Map<List<DoctorDto>>(doctors);
        }

        public async Task<ResponsePageDto<DoctorDto>> GetPaginatedDoctors(SearchParamsDto searchParamsDto)
        {
            var searchParams = _mapper.Map<SearchParams>(searchParamsDto);
            var doctors = await _doctorRepository.GetWithIncludesPaginatedAsync(searchParams, d => d.Title);
            return _mapper.Map<ResponsePageDto<DoctorDto>>(doctors);
        }

        private User CreateUser(DoctorDto doctorDto)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(doctorDto.Password);
            return new User
            {
                RoleId = 1,
                PasswordHash = Encoding.UTF8.GetBytes(passwordHash),
                Username = doctorDto.Code,
                IsDeleted = false,
                IsActivated = false
            };
        }
    }
}


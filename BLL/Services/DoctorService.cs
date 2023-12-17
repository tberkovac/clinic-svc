using System;
using System.Text;
using System.Text.RegularExpressions;
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
            var doctors = await _doctorRepository.GetWithIncludesAsync(d => d.Title, d => d.User);
            doctors.Sort((x, y) => x.Surname.CompareTo(y.Surname));
            return _mapper.Map<List<DoctorDto>>(doctors);
        }

        public async Task<List<DoctorDto>> GetAllSpecialists()
        {
            var specialists = await _doctorRepository
                .GetFilteredWithIncludesAsync(d => d.Title.TitleName == "Specialist", d => d.Title);
            return _mapper.Map<List<DoctorDto>>(specialists);
        }

        public async Task<ResponsePageDto<DoctorDto>> GetPaginatedDoctors(SearchParamsDto searchParamsDto)
        {
            var searchParams = _mapper.Map<SearchParams>(searchParamsDto);
            var doctors = await _doctorRepository.GetWithIncludesPaginatedAsync(searchParams, d => d.Title);
            return _mapper.Map<ResponsePageDto<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto> DeleteDoctor(int doctorId)
        {
            var doctorList = await _doctorRepository.GetFilteredWithIncludesAsync(d => d.DoctorId == doctorId, d => d.User);

            var doctor = doctorList.FirstOrDefault();

            if (doctor == null)
            {
                throw new Exception($"Doctor with provided id {doctorId} does not exist");
            }

            doctor.IsDeleted = true;

            if (doctor.User == null)
            {
                throw new Exception($"Did you forget to include User reference?");
            }

            doctor.User.IsDeleted = true;
            doctor.User.IsActivated = false;

            await _doctorRepository.SaveChangesAsync();

            return _mapper.Map<DoctorDto>(doctor);
        }

        public User CreateUser(DoctorDto doctorDto)
        {
            var password = doctorDto.Password;

            if (password.Length < 10)
            {
                throw new Exception("Password length is less than 10");
            }

            if (!Regex.IsMatch(password, @"[!$%&.,]"))
            {
                throw new Exception("Password must contain at least one of the following special characters: ! $ % & .,");
            }

            // Add code for checking if password contains not only lowercases
            if (!password.Any(char.IsUpper))
            {
                throw new Exception("Password must contain at least one uppercase letter");
            }
            
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


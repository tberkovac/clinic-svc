using System;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BLL.Dto;
using BLL.IServices;
using DAL.Entities;
using DAL.IRepositories;
using DAL.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services
{
	public class UserService : IUserService
	{
        private readonly IUserRepository _userRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IMapper _mapper;

		public UserService(IUserRepository userRepository, ILeaveRequestRepository leaveRequestRepository, IMapper mapper)
		{
            _userRepository = userRepository;
            _leaveRequestRepository = leaveRequestRepository;
            _mapper = mapper;
		}

        public async Task<TokenResponse> Login(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetUserByUsername(loginDto.Username);

            if (user == null)
            { 
                throw new Exception("User with username " + loginDto.Username + " not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, Encoding.UTF8.GetString(user.PasswordHash)))
            {
                throw new Exception("Invalid password");
            }

            var token = CreateToken(user);

            return token;
        }


        public async Task<LeaveRequestDto> CreateLeaveRequest(LeaveRequestDto leaveRequestDto, int userId)
        {
            var userList = await _userRepository.GetFilteredWithIncludesAsync(x=> x.UserId == userId, x=> x.LeaveRequest);

            var user = userList.FirstOrDefault();

            if (user == null)
            {
                throw new Exception($"User with provided id: {userId}, does not exist");
            }

            if (!user.IsActivated)
            {
                throw new Exception($"User with provided id: {userId}, is already on vacation");
            }

            if (user.LeaveRequest != null && user.IsActivated)
            {
                throw new Exception($"User with provided id: {userId}, have already scheduled leave request in future");
            }

            var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestDto);
            leaveRequest.OldUserId = user.UserId;
            leaveRequest.IsActive = false;
            user.LeaveRequest = leaveRequest;

            await _userRepository.SaveChangesAsync();

            return _mapper.Map<LeaveRequestDto>(leaveRequest);
        }

        public async Task<bool> ApproveLeaveRequest(int userId)
        {
            var userList = await _userRepository.GetFilteredWithIncludesAsync(x => x.UserId == userId, x => x.LeaveRequest);

            var user = userList.FirstOrDefault();

            if (user == null)
            {
                throw new Exception($"User with provided id: {userId}, does not exist");
            }

            if (user.LeaveRequest == null)
            {
                throw new Exception($"There is no opened vacation for user with  provided id: {userId}");
            }

            user.IsActivated = false;
            user.LeaveRequest.IsActive = true;

            await _userRepository.SaveChangesAsync();
;
            return true;
        }

        public async Task<bool> RevokeLeaveRequest(int userId)
        {
            var userList = await _userRepository.GetFilteredWithIncludesAsync(x => x.UserId == userId, x => x.LeaveRequest);

            var user = userList.FirstOrDefault();

            if (user == null)
            {
                throw new Exception($"User with provided id: {userId}, does not exist");
            }

            if (user.LeaveRequest == null)
            {
                throw new Exception($"User with provided id: {userId}, does not have open leave requests!");
            }

            user.IsActivated = true;
            user.LeaveRequest.IsActive = false;
            user.LeaveRequestId = null;

            await _userRepository.SaveChangesAsync();
            
            return true;
        }

        private TokenResponse CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF32.GetBytes("secret key secret keysecret key secret keysecret key secret keysecret key secret keysecret key secret key"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponse
            {
                Token = jwt,
                Role = user.Role.RoleName,
                UserId = user.UserId
            };
        }
    }
}


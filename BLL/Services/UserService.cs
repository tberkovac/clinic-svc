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
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services
{
	public class UserService : IUserService
	{
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

		public UserService(IUserRepository userRepository, IMapper mapper)
		{
            _userRepository = userRepository;
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


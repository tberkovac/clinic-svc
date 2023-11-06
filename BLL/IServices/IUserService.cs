using System;
using BLL.Dto;

namespace BLL.IServices
{
	public interface IUserService
	{
        Task<TokenResponse> Login(UserLoginDto loginDto);
	}
}


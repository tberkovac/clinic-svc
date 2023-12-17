using System;
using BLL.Dto;

namespace BLL.IServices
{
	public interface IUserService
	{
        Task<TokenResponse> Login(UserLoginDto loginDto);
        Task<LeaveRequestDto> CreateLeaveRequest(LeaveRequestDto leaveRequestDto, int userId);
        Task<bool> ApproveLeaveRequest(int userId);
        Task<bool> RevokeLeaveRequest(int userId);
    }
}


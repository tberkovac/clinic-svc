using System;
using System.Security.Claims;
using BLL.Dto;
using BLL.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
		private readonly IUserService _userService;

		public AuthController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost("login")]
		public async Task<ActionResult<TokenResponse>> Login(UserLoginDto loginDto)
		{
			var result = await _userService.Login(loginDto);
			return Ok(result);
		}

        [HttpGet("loggedUser")]
        [Authorize(Roles = "Admin,Doctor")]
        public ActionResult<UserDto> GetLoggedInUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var user = new UserDto
            {
                UserId = int.Parse(userId),
            };

            return Ok(user);
        }

        [HttpPost("CreateLeaveRequest")]
        [Authorize]
        public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest(LeaveRequestDto leaveRequestDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return BadRequest("Invalid user ID claim.");
            }

            var result = await _userService.CreateLeaveRequest(leaveRequestDto, userId);
            return Ok(result);
        }

        [HttpPut("ApproveLeaveRequest/{userId}")]
        [Authorize(Roles = "Admin,Doctor")] //Need to remove doctor from authrized roles
        public async Task<ActionResult<bool>> ApproveLeaveRequest(int userId)
        {
            var result = await _userService.ApproveLeaveRequest(userId);
            return Ok(result);
        }

        [HttpPut("RevokeLeaveRequest/{userId}")]
        [Authorize(Roles = "Admin,Doctor")] //Need to remove doctor from authrized roles
        public async Task<ActionResult<bool>> RevokeLeaveRequest(int userId)
        {
            var result = await _userService.RevokeLeaveRequest(userId);
            return Ok(result);
        }
    }
}


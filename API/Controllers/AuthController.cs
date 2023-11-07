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
    }
}


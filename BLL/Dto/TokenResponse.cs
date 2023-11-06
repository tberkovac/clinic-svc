using System;
using System.Security.Claims;

namespace BLL.Dto
{
	public class TokenResponse
	{
		public required string Token { get; set; }
		public required string Role { get; set; }
		public required int UserId { get; set; }
	}
}


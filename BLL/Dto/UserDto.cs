using System;

namespace BLL.Dto
{
	public class UserDto
	{
        public int UserId { get; set; }
        public string? Username { get; set; }
        public int RoleId { get; set; }
        public RoleDto? Role { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActivated { get; set; }
    }
}


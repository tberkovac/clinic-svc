using System;
namespace DAL.Entities
{
	public class User
	{
		public int UserId { get; set; }
        public string? Username { get; set; }
        public required byte[] PasswordHash { get; set; }
		public int RoleId { get; set; }
		public Role? Role { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsActivated { get; set; }
	}
}


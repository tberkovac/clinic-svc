using System;
using DAL.Entities;

namespace DAL.IRepositories
{
	public interface IUserRepository : IRepository<User>
	{
		Task<User?> GetUserByUsername(string username);
	}
}


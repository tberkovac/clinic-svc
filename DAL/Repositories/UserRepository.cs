using System;
using DAL.Data;
using DAL.Entities;
using DAL.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
	public class UserRepository : Repository<User>, IUserRepository
	{
        public ApplicationDbContext _dbContext;

		public UserRepository(ApplicationDbContext dbContext) : base (dbContext)
		{
            _dbContext = dbContext;
		}

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _dbContext.Set<User>().Include(x => x.Role).Where(u => u.Username == username)
                .FirstOrDefaultAsync();
        }
    }
}


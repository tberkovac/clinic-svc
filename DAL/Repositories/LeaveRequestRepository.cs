using System;
using DAL.Data;
using DAL.Entities;
using DAL.IRepositories;

namespace DAL.Repositories
{
	public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
    {
		public LeaveRequestRepository(ApplicationDbContext dbContext): base(dbContext)	{

		}
	}
}


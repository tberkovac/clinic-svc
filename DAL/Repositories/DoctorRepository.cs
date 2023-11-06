using System;
using DAL.Entities;
using DAL.IRepositories;
using DAL.Data;

namespace DAL.Repositories
{
	public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
		public DoctorRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
		}
	}
}


using System;
using DAL.Data;
using DAL.Entities;
using DAL.IRepositories;

namespace DAL.Repositories
{
	public class AdmissionRepository : Repository<Admission>, IAdmissionRepository
	{
		public AdmissionRepository(ApplicationDbContext dbContext): base(dbContext)
		{
		}
	}
}


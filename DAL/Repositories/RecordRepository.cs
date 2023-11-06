using System;
using DAL.Data;
using DAL.Entities;
using DAL.IRepositories;

namespace DAL.Repositories
{
	public class RecordRepository : Repository<Record>, IRecordRepository
	{
		public RecordRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
		}
	}
}


using DAL.IRepositories;
using DAL.Entities;
using DAL.Data;

namespace DAL.Repositories;

public class PatientRepository : Repository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    
}
using Microsoft.EntityFrameworkCore;
using DAL.Entities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;


namespace DAL.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {   
        optionsBuilder.UseMySql("Server=localhost;Port=8889;User ID=root;Password=password;Database=clinic",  new MySqlServerVersion(new Version(8, 0, 28)));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
        });

        builder.Entity<Doctor>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
        });

        builder.Entity<Patient>(entity =>
        {
            entity.HasIndex(e => e.UMCN).IsUnique();
        });
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Title> Titles { get; set; }
    public DbSet<Admission> Admissions { get; set; }
}
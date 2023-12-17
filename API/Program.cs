using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using DAL.Data;
using DAL.Repositories;
using DAL.IRepositories;
using DAL.Entities;
using BLL.Config;
using BLL.Services;
using BLL.IServices;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF32.GetBytes("secret key secret keysecret key secret keysecret key secret keysecret key secret keysecret key secret key")),
    };
});
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddTransient<MySqlConnection>(_ =>
{
    var connectionString = "Server=localhost;Port=8889;User ID=root;Password=password;Database=clinic";
    return new MySqlConnection(connectionString);
});
builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddScoped(typeof(IPatientRepository), typeof(PatientRepository));
builder.Services.AddScoped(typeof(IDoctorRepository), typeof(DoctorRepository));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IAdmissionRepository), typeof(AdmissionRepository));
builder.Services.AddScoped(typeof(IRecordRepository), typeof(RecordRepository));
builder.Services.AddScoped(typeof(ILeaveRequestRepository), typeof(LeaveRequestRepository));

builder.Services.AddScoped(typeof(IPatientService), typeof(PatientService));
builder.Services.AddScoped(typeof(IDoctorService), typeof(DoctorService));
builder.Services.AddScoped(typeof(IUserService), typeof(UserService));
builder.Services.AddScoped(typeof(IAdmissionService), typeof(AdmissionService));
builder.Services.AddScoped(typeof(IRecordService), typeof(RecordService));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();

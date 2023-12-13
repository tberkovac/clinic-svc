using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Dto;
using BLL.IServices;
using BLL.Services;
using DAL.Entities;
using DAL.IRepositories;
using Moq;
using Xunit;

public class DoctorServiceTests
{
    public Mock<IDoctorRepository> DoctorRepositoryMock { get; }
    public Mock<IUserRepository> UserRepositoryMock { get; }
    public Mock<IMapper> MapperMock { get; }
    public DoctorService DoctorServiceInstance { get; }

    public DoctorServiceTests()
    {
        // Initialize your mocks and services here
        DoctorRepositoryMock = new Mock<IDoctorRepository>();
        UserRepositoryMock = new Mock<IUserRepository>();
        MapperMock = new Mock<IMapper>();

        DoctorServiceInstance = new DoctorService(
            DoctorRepositoryMock.Object,
            UserRepositoryMock.Object,
            MapperMock.Object
        );
    }

    [Fact]
    public async Task CreateDoctor_ValidDoctorDto_ReturnsDoctorDto()
    {
        // Arrange
        var doctorService = new DoctorService(DoctorRepositoryMock.Object, UserRepositoryMock.Object, MapperMock.Object);

        var doctorDto = new DoctorDto
        {
            Code = "123",
            Name = "Mea",
            Surname = "Hrbenic",
            Password = "Mea123456789!"
        };

        var user = new User
        {
            PasswordHash = new byte[] {}
        };

        DoctorRepositoryMock.Setup(repo => repo.Add(It.IsAny<Doctor>())).Verifiable();
        DoctorRepositoryMock.Setup(repo => repo.Add(It.IsAny<Doctor>())).Verifiable();
        UserRepositoryMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);
        DoctorRepositoryMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);
        MapperMock.Setup(mapper => mapper.Map<Doctor>(It.IsAny<DoctorDto>())).Returns(It.IsAny<Doctor>()); // You may need to set up the mapper based on your actual mappings
        MapperMock.Setup(mapper => mapper.Map<DoctorDto>(It.IsAny<Doctor>())).Returns(It.IsAny<DoctorDto>()); // You may need to set up the mapper based on your actual mappings

        // Act
        var result = await doctorService.CreateDoctor(doctorDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DoctorDto>(result);

        DoctorRepositoryMock.Verify();
        UserRepositoryMock.Verify();
        MapperMock.Verify();
    }

    [Fact]
    public async Task GetDoctors_ReturnsSortedDoctorDtos()
    {
        // Arrange
        var doctorRepositoryMock = new Mock<IDoctorRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var mapperMock = new Mock<IMapper>();

        var doctorService = new DoctorService(doctorRepositoryMock.Object, userRepositoryMock.Object, mapperMock.Object);

        var doctors = new List<Doctor>
        {
            new Doctor
            {
                Code = "123",
                Name = "Mea",
                Surname ="Hrbenic"
            }
        };

        doctorRepositoryMock.Setup(repo => repo.GetWithIncludesAsync(It.IsAny<Expression<Func<Doctor, object>>[]>())).ReturnsAsync(doctors);
        mapperMock.Setup(mapper => mapper.Map<List<DoctorDto>>(It.IsAny<List<Doctor>>())).Returns(new List<DoctorDto>()); // You may need to set up the mapper based on your actual mappings

        // Act
        var result = await doctorService.GetDoctors();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<DoctorDto>>(result);

        doctorRepositoryMock.Verify();
        mapperMock.Verify();
    }

    // Add more tests for other methods as needed
}

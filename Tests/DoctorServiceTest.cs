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

    [TestClass]
public class DoctorServiceTests
{
    private Mock<IDoctorRepository> _doctorRepositoryMock;
    private IMapper _mapper;

    [TestInitialize]
    public void Initialize()
    {
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<YourMappingProfile>()));
    }

    [TestMethod]
    public async Task DeleteDoctor_ExistingDoctor_DeletesDoctorAndUser()
    {
        // Arrange
        var doctorId = 1;
        var doctor = new Doctor { DoctorId = doctorId, User = new User() };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        Assert.IsTrue(doctor.IsDeleted);
        Assert.IsTrue(doctor.User.IsDeleted);
        Assert.IsFalse(doctor.User.IsActivated);
        _doctorRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Doctor with provided id 2 does not exist")]
    public async Task DeleteDoctor_NonExistingDoctor_ThrowsException()
    {
        // Arrange
        var doctorId = 2;
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor>());

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        // Exception expected
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Did you forget to include User reference?")]
    public async Task DeleteDoctor_DoctorWithoutUser_ThrowsException()
    {
        // Arrange
        var doctorId = 3;
        var doctor = new Doctor { DoctorId = doctorId, User = null };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        // Exception expected
    }
}
[TestClass]
public class DoctorServiceTests
{
    private Mock<IDoctorRepository> _doctorRepositoryMock;
    private IMapper _mapper;

    [TestInitialize]
    public void Initialize()
    {
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<YourMappingProfile>()));
    }

    [TestMethod]
    public async Task DeleteDoctor_ExistingDoctor_DeletesDoctorAndUser()
    {
        // Arrange
        var doctorId = 1;
        var doctor = new Doctor { DoctorId = doctorId, User = new User() };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        Assert.IsTrue(doctor.IsDeleted);
        Assert.IsTrue(doctor.User.IsDeleted);
        Assert.IsFalse(doctor.User.IsActivated);
        _doctorRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Doctor with provided id 2 does not exist")]
    public async Task DeleteDoctor_NonExistingDoctor_ThrowsException()
    {
        // Arrange
        var doctorId = 2;
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor>());

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        // Exception expected
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Did you forget to include User reference?")]
    public async Task DeleteDoctor_DoctorWithoutUser_ThrowsException()
    {
        // Arrange
        var doctorId = 3;
        var doctor = new Doctor { DoctorId = doctorId, User = null };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        // Exception expected
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Doctor with provided id 4 does not exist")]
    public async Task DeleteDoctor_ExistingDoctorWithoutUser_ThrowsException()
    {
        // Arrange
        var doctorId = 4;
        var doctor = new Doctor { DoctorId = doctorId, User = null };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        // Exception expected
    }
}

[TestClass]
public class DoctorServiceTests
{
    private Mock<IDoctorRepository> _doctorRepositoryMock;
    private IMapper _mapper;

    [TestInitialize]
    public void Initialize()
    {
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<YourMappingProfile>()));
    }

    [TestMethod]
    public async Task DeleteDoctor_ExistingDoctor_DeletesDoctorAndUser()
    {
        // Arrange
        var doctorId = 1;
        var doctor = new Doctor { DoctorId = doctorId, User = new User() };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        Assert.IsTrue(doctor.IsDeleted);
        Assert.IsTrue(doctor.User.IsDeleted);
        Assert.IsFalse(doctor.User.IsActivated);
        _doctorRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Doctor with provided id 2 does not exist")]
    public async Task DeleteDoctor_NonExistingDoctor_ThrowsException()
    {
        // Arrange
        var doctorId = 2;
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor>());

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        // Exception expected
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Did you forget to include User reference?")]
    public async Task DeleteDoctor_DoctorWithoutUser_ThrowsException()
    {
        // Arrange
        var doctorId = 3;
        var doctor = new Doctor { DoctorId = doctorId, User = null };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        // Exception expected
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Doctor with provided id 4 does not exist")]
    public async Task DeleteDoctor_ExistingDoctorWithoutUser_ThrowsException()
    {
        // Arrange
        var doctorId = 4;
        var doctor = new Doctor { DoctorId = doctorId, User = null };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        // Exception expected
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Did you forget to include User reference?")]
    public async Task DeleteDoctor_ExistingDoctorWithoutActivatedUser_ThrowsException()
    {
        // Arrange
        var doctorId = 5;
        var doctor = new Doctor { DoctorId = doctorId, User = new User { IsActivated = false } };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        // Exception expected
    }
}
[TestClass]
public class DoctorServiceTests
{
    private Mock<IDoctorRepository> _doctorRepositoryMock;
    private IMapper _mapper;

    [TestInitialize]
    public void Initialize()
    {
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<YourMappingProfile>()));
    }

    [TestMethod]
    public async Task DeleteDoctor_ExistingDoctor_DeletesDoctorAndUser()
    {
        // Arrange
        var doctorId = 1;
        var doctor = new Doctor { DoctorId = doctorId, User = new User { IsActivated = true } };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        Assert.IsTrue(doctor.IsDeleted); // Data flow from method parameter to doctor
        Assert.IsTrue(doctor.User.IsDeleted); // Data flow from doctor to user
        Assert.IsFalse(doctor.User.IsActivated); // Data flow from doctor to user
        _doctorRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once); // Data flow from user to repository
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Doctor with provided id 2 does not exist")]
    public async Task DeleteDoctor_NonExistingDoctor_ThrowsException()
    {
        // Arrange
        var doctorId = 2;
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor>());

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await Assert.ThrowsExceptionAsync<Exception>(() => doctorService.DeleteDoctor(doctorId));

        // Assert
        // Exception expected
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Did you forget to include User reference?")]
    public async Task DeleteDoctor_DoctorWithoutUser_ThrowsException()
    {
        // Arrange
        var doctorId = 3;
        var doctor = new Doctor { DoctorId = doctorId, User = null };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await Assert.ThrowsExceptionAsync<Exception>(() => doctorService.DeleteDoctor(doctorId));

        // Assert
        // Exception expected
    }
}
[TestClass]
public class DoctorServiceTests
{
    private Mock<IDoctorRepository> _doctorRepositoryMock;
    private IMapper _mapper;

    [TestInitialize]
    public void Initialize()
    {
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<YourMappingProfile>()));
    }

    [TestMethod]
    public async Task DeleteDoctor_ExistingDoctor_DeletesDoctorAndUser()
    {
        // Arrange
        var doctorId = 1;
        var doctor = new Doctor { DoctorId = doctorId, User = new User { IsActivated = true } };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act
        await doctorService.DeleteDoctor(doctorId);

        // Assert
        Assert.IsTrue(doctor.IsDeleted); // Path: Existing doctor with activated user
        Assert.IsTrue(doctor.User.IsDeleted);
        Assert.IsFalse(doctor.User.IsActivated);
        _doctorRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Doctor with provided id 2 does not exist")]
    public async Task DeleteDoctor_NonExistingDoctor_ThrowsException()
    {
        // Arrange
        var doctorId = 2;
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor>());

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => doctorService.DeleteDoctor(doctorId)); // Path: Non-existing doctor
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Did you forget to include User reference?")]
    public async Task DeleteDoctor_DoctorWithoutUser_ThrowsException()
    {
        // Arrange
        var doctorId = 3;
        var doctor = new Doctor { DoctorId = doctorId, User = null };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => doctorService.DeleteDoctor(doctorId)); // Path: Doctor without user
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Doctor with provided id 4 does not exist")]
    public async Task DeleteDoctor_ExistingDoctorWithoutUser_ThrowsException()
    {
        // Arrange
        var doctorId = 4;
        var doctor = new Doctor { DoctorId = doctorId, User = null };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => doctorService.DeleteDoctor(doctorId)); // Path: Existing doctor without user
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Did you forget to include User reference?")]
    public async Task DeleteDoctor_ExistingDoctorWithoutActivatedUser_ThrowsException()
    {
        // Arrange
        var doctorId = 5;
        var doctor = new Doctor { DoctorId = doctorId, User = new User { IsActivated = false } };
        _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                             .ReturnsAsync(new List<Doctor> { doctor });

        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapper);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => doctorService.DeleteDoctor(doctorId)); // Path: Existing doctor with non-activated user
    }
}

}



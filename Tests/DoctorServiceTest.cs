using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using BLL.Config;
using BLL.Dto;
using BLL.IServices;
using BLL.Services;
using DAL.Entities;
using DAL.IRepositories;
using Moq;

namespace Tests
{

    public class DoctorServiceTests
    {
        public Mock<IDoctorRepository> _doctorRepositoryMock { get; }
        public Mock<IUserRepository> _userRepositoryMock { get; }
        public Mock<IMapper> _mapper { get; }
        public Fixture _fixture = new Fixture();

        public DoctorServiceTests()
        {
            // Initialize your mocks and services here
            _doctorRepositoryMock = new Mock<IDoctorRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task CreateDoctor_ValidDoctorDto_ReturnsDoctorDto()
        {
            // Arrange

            var doctorDto = _fixture.Build<DoctorDto>().With(x => x.Password, $"{GuidGenerator.Create}$").Create();
            var doctor = _fixture.Build<Doctor>().Create();
            var user = _fixture.Create<User>();
            _userRepositoryMock.Setup(repo => repo.Add(user));
            _userRepositoryMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);
            _doctorRepositoryMock.Setup(repo => repo.Add(doctor));
            _doctorRepositoryMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapper.Setup(mapper => mapper.Map<Doctor>(doctorDto)).Returns(doctor); // You may need to set up the mapper based on your actual mappings
            _mapper.Setup(mapper => mapper.Map<DoctorDto>(doctor)).Returns(doctorDto); // You may need to set up the mapper based on your actual mappings

            var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);

            // Act
            var result = await doctorService.CreateDoctor(doctorDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DoctorDto>(result);

            _doctorRepositoryMock.Verify();
            _userRepositoryMock.Verify();
            _mapper.Verify();
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

        //Whitebox testovi za metodu DeleteDoctor
        [Fact]
        public async Task DeleteDoctor_ExistingDoctor_DeletesDoctorAndUser()
        {
            // Arrange
            var doctorId = 1;
            var doctor = _fixture.Build<Doctor>().With(x => x.DoctorId, doctorId).Create();
            _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                                 .ReturnsAsync(new List<Doctor> { doctor });

            var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);

            // Act
            await doctorService.DeleteDoctor(doctorId);

            // Assert
            Assert.True(doctor.IsDeleted);
            Assert.True(doctor.User.IsDeleted);
            Assert.False(doctor.User.IsActivated);
            _doctorRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteDoctor_DoctorWithoutUser_ThrowsException()
        {
            // Arrange
            var doctorId = 3;
            var doctor = _fixture.Build<Doctor>().Without(x => x.User).Create();
            _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                                 .ReturnsAsync(new List<Doctor> { doctor });

            var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);

            // Act & Assert
            var result = await Assert.ThrowsAsync<Exception>(() => doctorService.DeleteDoctor(doctorId));
            Assert.Equal("Did you forget to include User reference?", result.Message);
        }

        [Fact]
        public async Task DeleteDoctor_ExistingDoctorWithoutUser_ThrowsException2()
        {
            // Arrange
            var doctorId = 4;
            var doctor = _fixture.Build<Doctor>().With(x => x.DoctorId, doctorId).Create();
            _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                                 .ReturnsAsync(new List<Doctor>() { });

            var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);

            // Assert
            var result = await Assert.ThrowsAsync<Exception>(() => doctorService.DeleteDoctor(doctorId));
            Assert.Equal("Doctor with provided id 4 does not exist", result.Message);
        }

        [Fact]
        public async Task DeleteDoctor_NonExistingDoctor_ThrowsException()
        {
            // Arrange
            var doctorId = 2;
            _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                                 .ReturnsAsync(new List<Doctor>());

            var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);

            // Act & Assert
            var result = await Assert.ThrowsAsync<Exception>(() => doctorService.DeleteDoctor(doctorId));
            Assert.Equal("Doctor with provided id 2 does not exist", result.Message);
        }

        [Fact]
        public async Task DeleteDoctor_DoctorWithoutUser_ThrowsException2()
        {
            // Arrange
            var doctorId = 3;
            var doctor = _fixture.Build<Doctor>().With(x => x.DoctorId, doctorId).Without(x => x.User).Create();
            _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                                 .ReturnsAsync(new List<Doctor> { doctor });

            var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);

            // Act & Assert
            var result = await Assert.ThrowsAsync<Exception>(() => doctorService.DeleteDoctor(doctorId));
            Assert.Equal("Did you forget to include User reference?", result.Message);
        }

        [Fact]
        public async Task DeleteDoctor_ExistingDoctorWithoutUser_ThrowsException()
        {
            // Arrange
            var doctorId = 4;
            var doctor = _fixture.Build<Doctor>().With(x => x.DoctorId, doctorId).Without(x => x.User).Create();
            _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                                 .ReturnsAsync(new List<Doctor> { doctor });

            var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);

            // Act & Assert
            var result = await Assert.ThrowsAsync<Exception>(() => doctorService.DeleteDoctor(doctorId));
            Assert.Equal("Did you forget to include User reference?", result.Message);
        }

        [Fact]
        public async Task DeleteDoctor_ExistingDoctor_DeletesDoctorAndUser_Active()
        {
            // Arrange
            var doctorId = 1;
            var user = _fixture.Build<User>().With(x => x.IsActivated, true).Create();
            var doctor = _fixture.Build<Doctor>().With(x => x.DoctorId, doctorId).With(x => x.User, user).Create();
            _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                                 .ReturnsAsync(new List<Doctor> { doctor });

            var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);

            // Act
            await doctorService.DeleteDoctor(doctorId);

            // Assert
            Assert.True(doctor.IsDeleted); // Data flow from method parameter to doctor
            Assert.True(doctor.User.IsDeleted); // Data flow from doctor to user
            Assert.False(doctor.User.IsActivated); // Data flow from doctor to user
            _doctorRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once); // Data flow from user to repository
        }



        [Fact]
        public async Task DeleteDoctor_DoctorWithoutUser_ThrowsException3()
        {
            // Arrange
            var doctorId = 3;
            var doctor = _fixture.Build<Doctor>().With(x => x.DoctorId, doctorId).Without(x => x.User).Create();
            _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<Expression<Func<Doctor, object>>>()))
                                 .ReturnsAsync(new List<Doctor> { doctor });

            // Act
            var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);

            // Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => doctorService.DeleteDoctor(doctorId));
            Assert.Equal("Did you forget to include User reference?", exception.Message);
        }

        //Whitebox testovi za metodu CreateUser
        [Fact]
    public void CreateUser_WithShortPassword_ThrowsException()
    {
        // Arrange
        var doctorDto = new DoctorDto {Code = "123",
        Name = "Mea",
        Surname = "Hrbenic", Password = "short" };

        // Act
        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);
        var exception = Assert.Throws<Exception>(() => doctorService.CreateUser(doctorDto));

        // Assert
        Assert.Equal("Password length is less than 10", exception.Message);
    }
    [Fact]
    public void CreateUser_WithNoSpecialCharacter_ThrowsException()
    {
        // Arrange
        var doctorDto = new DoctorDto
    {
        Code = "123",
        Name = "Mea",
        Surname = "Hrbenic",
        Password = "Mea123456789"
    };

        // Act
        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);
        var exception = Assert.Throws<Exception>(() => doctorService.CreateUser(doctorDto));

        // Assert
        Assert.Equal("Password must contain at least one of the following special characters: ! $ % & .,", exception.Message);
    }

    [Fact]
    public void CreateUser_WithNoUppercaseLetter_ThrowsException()
    {
        // Arrange
        var doctorDto = new DoctorDto { Code = "123",
        Name = "Mea",
        Surname = "Hrbenic",Password = "nouppercase123" };

        // Act
        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);
        var exception = Assert.Throws<Exception>(() => doctorService.CreateUser(doctorDto));

        // Assert
        Assert.Equal("Password must contain at least one uppercase letter", exception.Message);
    }

    [Fact]
    public void CreateUser_WithValidPassword_ReturnsUserObject()
    {
        // Arrange
        var doctorDto = new DoctorDto { Code = "123",
        Name = "Mea",
        Surname = "Hrbenic",Password = "ValidPassword1!" };

        // Act
        var doctorService = new DoctorService(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _mapper.Object);
        var user = doctorService.CreateUser(doctorDto);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(1, user.RoleId);
        Assert.False(user.IsDeleted);
        Assert.False(user.IsActivated);
        Assert.Equal(doctorDto.Code, user.Username);
        // Add more assertions if needed
    }

    }
}



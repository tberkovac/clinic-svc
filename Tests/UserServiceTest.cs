using System;
using System.Text;
using AutoFixture;
using AutoMapper;
using BLL.Dto;
using BLL.Services;
using DAL.Entities;
using DAL.IRepositories;
using Moq;

namespace Tests
{
	public class UserServiceTest
	{
        public Mock<IUserRepository> _userRepositoryMock;
        public Mock<IMapper> _mapper;
        public Fixture _fixture = new Fixture();

		public UserServiceTest()
		{
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapper = new Mock<IMapper>();
		}

        [Fact]
        public async Task Login_ValidUser_ReturnsTokenResponse()
        {
            // Arrange
            var password = $"{Guid.NewGuid}$";
            var loginDto = _fixture.Build<UserLoginDto>().With(x => x.Password, password).Create();
            var passwordHash = Encoding.UTF8.GetBytes(BCrypt.Net.BCrypt.HashPassword(password));
            var user = _fixture.Build<User>().With(x => x.PasswordHash, passwordHash).Create();

            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(loginDto.Username)).ReturnsAsync(user);
            _mapper.Setup(mapper => mapper.Map<TokenResponse>(It.IsAny<User>())).Returns(_fixture.Create<TokenResponse>());

            var userService = new UserService(_userRepositoryMock.Object, _mapper.Object);

            // Act
            var result = await userService.Login(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TokenResponse>(result);
        }

        [Fact]
        public async Task Login_InvalidUser_ThrowsException()
        {
            // Arrange
            var loginDto = _fixture.Create<UserLoginDto>();

            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(loginDto.Username)).ReturnsAsync(null as User);

            var userService = new UserService(_userRepositoryMock.Object, new Mock<IMapper>().Object);

            // Act and Assert
            var result = await Assert.ThrowsAsync<Exception>(() => userService.Login(loginDto));
            Assert.Equal("User with username " + loginDto.Username + " not found.", result.Message);

        }

        [Fact]
        public async Task Login_InvalidPassword_ThrowsException()
        {
            // Arrange
            var password = $"{Guid.NewGuid}$";
            var wrongPassword = $"{Guid.NewGuid}$123";
            var passwordHash = Encoding.UTF8.GetBytes(BCrypt.Net.BCrypt.HashPassword(password));

            var loginDto = _fixture.Build<UserLoginDto>().With(x => x.Password, wrongPassword).Create();
            var user = _fixture.Build<User>().With(x => x.PasswordHash, passwordHash).Create();

            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(loginDto.Username)).ReturnsAsync(user);

            var userService = new UserService(_userRepositoryMock.Object, new Mock<IMapper>().Object);

            // Act and Assert
            var result = await Assert.ThrowsAsync<Exception>(() => userService.Login(loginDto));
            Assert.Equal("Invalid password", result.Message);
        }
    }
}


using System;
using System.Linq.Expressions;
using System.Text;
using AutoFixture;
using AutoMapper;
using BLL.Dto;
using BLL.IServices;
using BLL.Services;
using DAL.Entities;
using DAL.IRepositories;
using DAL.Repositories;
using Moq;

namespace Tests
{
	public class UserServiceTest
	{
        public Mock<IUserRepository> _userRepositoryMock;
        public Mock<ILeaveRequestRepository> _leaveRequestRepositoryMock;
        public Mock<IMapper> _mapper;
        public Fixture _fixture = new Fixture();

		public UserServiceTest()
		{
            _userRepositoryMock = new Mock<IUserRepository>();
            _leaveRequestRepositoryMock = new Mock<ILeaveRequestRepository>();
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

            var userService = new UserService(_userRepositoryMock.Object, _leaveRequestRepositoryMock.Object, _mapper.Object);

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

            var userService = new UserService(_userRepositoryMock.Object, _leaveRequestRepositoryMock.Object, new Mock<IMapper>().Object);

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

            var userService = new UserService(_userRepositoryMock.Object, _leaveRequestRepositoryMock.Object, new Mock<IMapper>().Object);

            // Act and Assert
            var result = await Assert.ThrowsAsync<Exception>(() => userService.Login(loginDto));
            Assert.Equal("Invalid password", result.Message);
        }

        [Fact]
        public async Task CreateLeaveRequest_UserDoesNotExist_ThrowsExceptption()
        {
            var leaveRequestDto = _fixture.Create<LeaveRequestDto>();
                
            _userRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(
                            It.IsAny<Expression<Func<User, bool>>>(),
                            It.IsAny<Expression<Func<User, object>>[]>()))
                            .ReturnsAsync(new List<User> { });

            var userService = new UserService(_userRepositoryMock.Object, _leaveRequestRepositoryMock.Object, new Mock<IMapper>().Object);

            var result = await Assert.ThrowsAsync<Exception>(() => userService.CreateLeaveRequest(leaveRequestDto,3));
            Assert.Equal("User with provided id: 3, does not exist", result.Message);
        }

        [Fact]
        public async Task CreateLeaveRequest_UserIsOnVacation_ThrowsExceptption()
        {
            var leaveRequestDto = _fixture.Create<LeaveRequestDto>();
            var user = _fixture.Build<User>().With(x => x.IsActivated, false).Create();

            _userRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(
                            It.IsAny<Expression<Func<User, bool>>>(),
                            It.IsAny<Expression<Func<User, object>>[]>()))
                            .ReturnsAsync(new List<User> { user });

            var userService = new UserService(_userRepositoryMock.Object, _leaveRequestRepositoryMock.Object, new Mock<IMapper>().Object);

            var result = await Assert.ThrowsAsync<Exception>(() => userService.CreateLeaveRequest(leaveRequestDto, 3));
            Assert.Equal("User with provided id: 3, does not exist", result.Message);
        }

        [Fact]
        public async Task CreateLeaveRequest_UserAlreadyScheduledVacation_ThrowsExceptption()
        {
            var leaveRequestDto = _fixture.Create<LeaveRequestDto>();
            var user = _fixture.Build<User>().With(x => x.IsActivated, false).Create();

            _userRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(
                            It.IsAny<Expression<Func<User, bool>>>(),
                            It.IsAny<Expression<Func<User, object>>[]>()))
                            .ReturnsAsync(new List<User> { user });

            var userService = new UserService(_userRepositoryMock.Object, _leaveRequestRepositoryMock.Object, new Mock<IMapper>().Object);

            var result = await Assert.ThrowsAsync<Exception>(() => userService.CreateLeaveRequest(leaveRequestDto, 3));
            Assert.Equal("User with provided id: 3, is already on vacation", result.Message);
        }

        [Fact]
        public async Task CreateLeaveRequest_CreatesSuccesfully_ReturnsLeaveRequest()
        {
            var leaveRequestDto = _fixture.Create<LeaveRequestDto>();
            var leaveRequest = _fixture.Create<LeaveRequest>();
            var user = _fixture.Build<User>().With(x => x.IsActivated, true).Create();

            _userRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(
                            It.IsAny<Expression<Func<User, bool>>>(),
                            It.IsAny<Expression<Func<User, object>>[]>()))
                            .ReturnsAsync(new List<User> { user });

            var userService = new UserService(_userRepositoryMock.Object, _leaveRequestRepositoryMock.Object, new Mock<IMapper>().Object);

            var result = await Assert.ThrowsAsync<Exception>(() => userService.CreateLeaveRequest(leaveRequestDto, 3));
            Assert.Equal("User with provided id: 3, have already scheduled leave request in future", result.Message);
        }

        [Fact]
        public async Task ApproveLeaveRequest_ApprovedSuccessfully_ReturnsTrue()
        {
            var leaveRequest = _fixture.Build<LeaveRequest>().With(x => x.IsActive, false).Create();
            var user = _fixture.Build<User>().With(x => x.UserId, 3)
                    .With(x => x.IsActivated, true)
                    .With(x => x.LeaveRequestId, leaveRequest.LeaveRequestId)
                    .With(x => x.LeaveRequest, leaveRequest).Create();

            _userRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(
                            It.IsAny<Expression<Func<User, bool>>>(),
                            It.IsAny<Expression<Func<User, object>>[]>()))
                            .ReturnsAsync(new List<User> { user });

            var userService = new UserService(_userRepositoryMock.Object, _leaveRequestRepositoryMock.Object, new Mock<IMapper>().Object);

            var result = await userService.ApproveLeaveRequest(3);


            Assert.True(result);
            Assert.False(user.IsActivated);
            Assert.NotNull(user.LeaveRequestId);
            Assert.True(user.LeaveRequest.IsActive);
        }

        [Fact]
        public async Task RevokeLeaveRequest_RevokedSuccessfully_ReturnsTrue()
        {
            var leaveRequest = _fixture.Build<LeaveRequest>().With(x => x.IsActive, true).Create();
            var user = _fixture.Build<User>().With(x => x.UserId, 3)
                    .With(x => x.IsActivated, false)
                    .With(x => x.LeaveRequest, leaveRequest).Create();

            _userRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(
                            It.IsAny<Expression<Func<User, bool>>>(),
                            It.IsAny<Expression<Func<User, object>>[]>()))
                            .ReturnsAsync(new List<User> { user });

            var userService = new UserService(_userRepositoryMock.Object, _leaveRequestRepositoryMock.Object, new Mock<IMapper>().Object);

            var result = await userService.RevokeLeaveRequest(3);


            Assert.True(result);
            Assert.True(user.IsActivated);
            Assert.Null(user.LeaveRequestId);
            Assert.False(user.LeaveRequest.IsActive);
        }
    }
}


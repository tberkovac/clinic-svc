using System;
using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using BLL.Dto;
using BLL.Services;
using DAL.Entities;
using DAL.IRepositories;
using Moq;

namespace Tests
{
	public class AdmissionServiceTest
	{
		public Mock<IAdmissionRepository> _admissionRepositoryMock;
		public Mock<IDoctorRepository> _doctorRepositoryMock;
        public Mock<IUserRepository> _userRepositoryMock;
		public Mock<IMapper> _mapper;
		public Fixture _fixture = new Fixture();
        public AdmissionService admissionService;

		public AdmissionServiceTest()
		{
			_admissionRepositoryMock = new Mock<IAdmissionRepository>();
			_doctorRepositoryMock = new Mock<IDoctorRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
			_mapper = new Mock<IMapper>();
            admissionService = new AdmissionService(_admissionRepositoryMock.Object, _doctorRepositoryMock.Object,
                                _userRepositoryMock.Object, _mapper.Object);
		}

		[Fact]
		public async Task GetAllAdmissions_PaginatesResponse_ReturnsPaginatedAdmissions()
		{
            // Arrange
            var searchParamsDto = _fixture.Create<SearchAdmissionsParamsDto>();
            var searchParams = _fixture.Create<SearchParams>();
            var admissions = _fixture.Create<ResponsePage<Admission>>();
            var admissionDtos = _fixture.Create<ResponsePageDto<AdmissionDto>>();

            _mapper.Setup(mapper => mapper.Map<SearchParams>(searchParamsDto)).Returns(searchParams);
            _mapper.Setup(mapper => mapper.Map<ResponsePageDto<AdmissionDto>>(admissions)).Returns(admissionDtos);

            _admissionRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesPaginatedAsync(
                It.IsAny<List<Expression<Func<Admission, bool>>>>(),
                searchParams,
                It.IsAny<Expression<Func<Admission, object>>[]>()))
                .ReturnsAsync(admissions);

            // Act
            var result = await admissionService.GetAllAdmissions(searchParamsDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResponsePageDto<AdmissionDto>>(result);
        }

        [Fact]
        public async Task Create_CreatesAdmission_ReturnsCreatedAdmission()
        {
            //Arrange
            var admissionDto = _fixture.Build<AdmissionDto>().With(x => x.AdmissionDate, DateTime.Now).Create();
            var admission = _fixture.Build<Admission>().With(x => x.AdmissionDate, DateTime.Now).With(x => x.DoctorId, 3).Create();
            var user = _fixture.Build<User>().With(x => x.IsActivated, true).Create();
            var doctor = _fixture.Build<Doctor>().With(x => x.DoctorId, 3).With(x => x.User, user).Create();

            _mapper.Setup(x => x.Map<Admission>(admissionDto)).Returns(admission);
            _admissionRepositoryMock.Setup(repo => repo.Add(admission));
            _admissionRepositoryMock.Setup(repo => repo.SaveChangesAsync());
            _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(
                It.IsAny<Expression<Func<Doctor, bool>>>(),
                It.IsAny<Expression<Func<Doctor, object>>[]>()))
                .ReturnsAsync(new List<Doctor> { doctor });
            _userRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync(new List<User> { user });

            //  var admissionService = new AdmissionService(_admissionRepositoryMock.Object, _doctorRepositoryMock.Object, _mapper.Object);

            //Act
            var result = await admissionService.Create(admissionDto);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(admission.AdmissionId, result.AdmissionId);
        }

        [Fact]
        public async Task Create_CreatesAdmissionForNonExistingDoctor_ThrowsException()
        {
            //Arrange
            var admissionDto = _fixture.Build<AdmissionDto>().With(x => x.AdmissionDate, DateTime.Now).Create();
            var admission = _fixture.Build<Admission>().With(x => x.AdmissionDate, DateTime.Now).With(x => x.DoctorId, 3).Create();
            var user = _fixture.Build<User>().With(x => x.IsActivated, true).Create();
            var doctor = _fixture.Build<Doctor>().With(x => x.DoctorId, 3).With(x => x.User, user).Create();

            _mapper.Setup(x => x.Map<Admission>(admissionDto)).Returns(admission);
            _admissionRepositoryMock.Setup(repo => repo.Add(admission));
            _admissionRepositoryMock.Setup(repo => repo.SaveChangesAsync());
            _doctorRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesAsync(
                It.IsAny<Expression<Func<Doctor, bool>>>(),
                It.IsAny<Expression<Func<Doctor, object>>[]>()))
                .ReturnsAsync(new List<Doctor> { });

            //  var admissionService = new AdmissionService(_admissionRepositoryMock.Object, _doctorRepositoryMock.Object, _mapper.Object);

            //Act and assert
            var result = await Assert.ThrowsAsync<Exception>(() => admissionService.Create(admissionDto));
            Assert.Equal("Doctor with provided id: 3, could not be found!", result.Message);
        }

        [Fact]
        public async Task Create_CreatesAdmissionInPast_ThrowsException()
        {
            //Arrange
            var admissionDto = _fixture.Build<AdmissionDto>().With(x => x.AdmissionDate, DateTime.Now.AddDays(-1)).Create();
            var admission = _fixture.Build<Admission>().With(x => x.AdmissionDate, DateTime.Now.AddDays(-1)).Create();

            _mapper.Setup(x => x.Map<Admission>(admissionDto)).Returns(admission);

          //  var admissionService = new AdmissionService(_admissionRepositoryMock.Object, _doctorRepositoryMock.Object, _mapper.Object);

            //Act & Assert
            var result = await Assert.ThrowsAsync<Exception>(() => admissionService.Create(admissionDto));
            Assert.Equal("Admission cannot be scheduled in past!", result.Message);
        }

        [Fact]
        public async Task GetDoctorAdmissions_GetsDoctorsAdmissions_ReturnsAdmissions()
        {
            //Arrange
            var userID = 12;
            var searchParamsDto = _fixture.Create<SearchAdmissionsParamsDto>();
            var searchParams = _fixture.Create<SearchParams>();
            var doctor = _fixture.Build<Doctor>().With(x => x.User, _fixture.Build<User>().With(x => x.UserId, userID).Create()).Create();
            var admissions = _fixture.Create<ResponsePage<Admission>>();
            var admissionDtos = _fixture.Create<ResponsePageDto<AdmissionDto>>();

            _doctorRepositoryMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(doctor);
            _admissionRepositoryMock.Setup(repo => repo.GetFilteredWithIncludesPaginatedAsync(
                It.IsAny<List<Expression<Func<Admission, bool>>>>(),
                searchParams,
                It.IsAny<Expression<Func<Admission, object>>[]>()))
                .ReturnsAsync(admissions);

            _mapper.Setup(x => x.Map<SearchParams>(searchParamsDto)).Returns(searchParams);
            _mapper.Setup(mapper => mapper.Map<ResponsePageDto<AdmissionDto>>(admissions)).Returns(admissionDtos);

         //   var admissionService = new AdmissionService(_admissionRepositoryMock.Object, _doctorRepositoryMock.Object, _mapper.Object);

            //Act
            var result = await admissionService.GetDoctorsAdmissions(searchParamsDto, userID);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(admissionDtos, result);

        }

        [Fact]
        public async Task GetDoctorsAdmissions_DoctorIsNull_ThrowsException()
        {
            //Arrange
            var userID = 12;
            var searchParamsDto = _fixture.Create<SearchAdmissionsParamsDto>();
            _doctorRepositoryMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(null as Doctor);

       //     var admissionService = new AdmissionService(_admissionRepositoryMock.Object, _doctorRepositoryMock.Object, _mapper.Object);

            //Act & Assert
            var result = await Assert.ThrowsAsync<Exception>(() => admissionService.GetDoctorsAdmissions(searchParamsDto, userID));
            Assert.Equal("User with provided id is not doctor!", result.Message);
        }

        [Fact]
        public async Task DeleteAdmission_DeletesAdmission_ReturnsDeletedAdmission()
        {
            //Arrange
            var admissionId = 14;
            var recordId = 1;
            var admission = _fixture.Build<Admission>().With(x => x.AdmissionId, admissionId).With(x => x.RecordId, recordId).Create();
            var admissionDto = _fixture.Build<AdmissionDto>().With(x => x.AdmissionId, admissionId).With(x => x.RecordId, recordId).Create();

            _admissionRepositoryMock.Setup(repo => repo.GetById(admissionId)).ReturnsAsync(admission);
            _admissionRepositoryMock.Setup(repo => repo.SaveChangesAsync());
            _mapper.Setup(x => x.Map<AdmissionDto>(admission)).Returns(admissionDto);

         //   var admissionService = new AdmissionService(_admissionRepositoryMock.Object, _doctorRepositoryMock.Object, _mapper.Object);

            //Act
            var result = await admissionService.DeleteAdmission(admissionId);

            //Assert
            Assert.NotNull(result);
            Assert.True(admission.IsDeleted);
            Assert.Equal(admissionDto, result);

        }

        [Fact]
        public async Task DeleteAdmission_DeletesAdmissionWithoutRecordId_ReturnsDeletedAdmission()
        {
            //Arrange
            var admissionId = 14;
            var admission = _fixture.Build<Admission>().With(x => x.AdmissionId, admissionId).Without(x => x.RecordId).Create();
            var admissionDto = _fixture.Build<AdmissionDto>().With(x => x.AdmissionId, admissionId).Without(x => x.RecordId).Create();

            _admissionRepositoryMock.Setup(repo => repo.GetById(admissionId)).ReturnsAsync(admission);
            _admissionRepositoryMock.Setup(repo => repo.SaveChangesAsync());
            _admissionRepositoryMock.Setup(repo => repo.Remove(admission));
            _mapper.Setup(x => x.Map<AdmissionDto>(admission)).Returns(admissionDto);

        //    var admissionService = new AdmissionService(_admissionRepositoryMock.Object, _doctorRepositoryMock.Object, _mapper.Object);

            //Act
            var result = await admissionService.DeleteAdmission(admissionId);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(admissionDto, result);
            _admissionRepositoryMock.Verify(repo => repo.Remove(It.IsAny<Admission>()), Times.AtLeastOnce);

        }


        [Fact]
        public async Task DeleteAdmission_AdmissionIsNull_ThrowsException()
        {
            //Arrange
            var admissionId = 14;
          
            _admissionRepositoryMock.Setup(repo => repo.GetById(admissionId)).ReturnsAsync(null as Admission);

        //    var admissionService = new AdmissionService(_admissionRepositoryMock.Object, _doctorRepositoryMock.Object, _mapper.Object);

            //Act & Assert
            var result = await Assert.ThrowsAsync<Exception>(() => admissionService.DeleteAdmission(admissionId));
            Assert.Equal($"Admission with provided id {admissionId} does not exist", result.Message);
        }

    }
}


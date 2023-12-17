using System;
using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using BLL.Dto;
using BLL.Services;
using DAL.Entities;
using DAL.IRepositories;
using Moq;
using Xunit;

namespace Tests
{
	public class PatientServiceTest
	{
		public Mock<IPatientRepository> _patientRepositoryMock;
		public Mock<IMapper> _mapper;
		public Fixture _fixture = new Fixture();


		public PatientServiceTest()
		{
			_patientRepositoryMock = new Mock<IPatientRepository>();
			_mapper = new Mock<IMapper>();
		}

		[Fact]
		public async Task GetAllPatients_PatientsList_ReturnsPatients()
		{
			// Arrange
			var patients = _fixture.Create<List<Patient>>();
			var patientDtos = _fixture.Create<List<PatientDto>>();
			_patientRepositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(patients);
			_mapper.Setup(mapper => mapper.Map<List<PatientDto>>(patients)).Returns(patientDtos);

			var patientService = new PatientService(_patientRepositoryMock.Object, _mapper.Object);

			// Act
			var result = await patientService.GetAllPatients();

			// Assert
			Assert.Equal(patientDtos, result);

		}

		[Fact]
		public async Task CreatePatient_CreatesNewPatients_ReturnsPatient()
		{
			//Arrange
			var patientDto = _fixture.Create<PatientDto>();
			var patient = _fixture.Create<Patient>();
			_mapper.Setup(x => x.Map<Patient>(patientDto)).Returns(patient);
			_mapper.Setup(x => x.Map<PatientDto>(patient)).Returns(patientDto);
			_patientRepositoryMock.Setup(repo => repo.Add(patient));
			_patientRepositoryMock.Setup(repo => repo.SaveChangesAsync());

			var patientService = new PatientService(_patientRepositoryMock.Object, _mapper.Object);

			//Act
			var result = await patientService.CreatePatient(patientDto);

			//Assert
			Assert.Equal(patientDto, result);
		}

		[Fact]
		public async Task GetPaginatedPatients_PaginatesResponse_returnsPaginatesPatients()
		{
			//Arrange
			var searchParametersDto = _fixture.Create<SearchParamsDto>();
			var searchParameters = _fixture.Create<SearchParams>();
            var patients = _fixture.Create<ResponsePage<Patient>>();
            var patientDtos = _fixture.Create<ResponsePageDto<PatientDto>>();

            _mapper.Setup(x => x.Map<SearchParams>(searchParametersDto)).Returns(searchParameters);
            _mapper.Setup(mapper => mapper.Map<ResponsePageDto<PatientDto>>(patients)).Returns(patientDtos);
            _patientRepositoryMock.Setup(x => x.GetWithIncludesPaginatedAsync(searchParameters)).ReturnsAsync(patients);

			var patientService = new PatientService(_patientRepositoryMock.Object, _mapper.Object);

			//Act
			var result = await patientService.GetPaginatedPatients(searchParametersDto);

			//Assert
			Assert.NotNull(result);
			Assert.IsType<ResponsePageDto<PatientDto>>(result);

		}

		[Fact]
		public async Task DeletePatient_DeletesPatient_ReturnsDeletedPatient()
		{
			//Arrange
			var id = 1;
			var patient = _fixture.Build<Patient>().With(x => x.PatientId, id).Create();
			var patientDto = _fixture.Build<PatientDto>().With(x => x.PatientId, id).Create();
			_mapper.Setup(x => x.Map<PatientDto>(patient)).Returns(patientDto);
			_patientRepositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync(patient);
			_patientRepositoryMock.Setup(repo => repo.SaveChangesAsync());
			var patientService = new PatientService(_patientRepositoryMock.Object, _mapper.Object);

			//Act
			var result = await patientService.DeletePatient(id);

			//Assert
			Assert.Equal(id, result.PatientId);
			Assert.True(patient.IsDeleted);
		}

		[Fact]
		public async Task DeletePatient_NullValuePatient_ThrowsException()
		{
			//Arrange
            var id = 1;
            _patientRepositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync(null as Patient);

            var patientService = new PatientService(_patientRepositoryMock.Object, _mapper.Object);


			//Act & Assert
			var result  = await Assert.ThrowsAsync<Exception>(() => patientService.DeletePatient(id));
			Assert.Equal(result.Message, $"Patient with id : {id} does not exist");
		}
	}
}


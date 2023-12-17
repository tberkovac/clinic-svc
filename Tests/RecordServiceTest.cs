using System;
using AutoFixture;
using AutoMapper;
using BLL.Dto;
using BLL.Services;
using DAL.Entities;
using DAL.IRepositories;
using Moq;
using Record = DAL.Entities.Record;

namespace Tests
{
	public class RecordServiceTest
	{
		public Mock<IRecordRepository> _recordRepositoryMock;
		public Mock<IAdmissionRepository> _admissionRepositoryMock;
		public Mock<IMapper> _mapper;
		public Fixture _fixture = new Fixture();

		public RecordServiceTest()
		{
			_recordRepositoryMock = new Mock<IRecordRepository>();
			_admissionRepositoryMock = new Mock<IAdmissionRepository>();
			_mapper = new Mock<IMapper>();
		}

		[Fact]
		public async Task CreateRecord_RecordCreated_ReturnsRecord()
		{
			//Arrange
			var recordDto = _fixture.Create<RecordDto>();
			var admissionID = 1;
			var admission = _fixture.Build<Admission>().With(x => x.AdmissionId, admissionID).Create();
			var record = _fixture.Create<Record>();
			_mapper.Setup(x => x.Map<Record>(recordDto)).Returns(record);
			_mapper.Setup(x => x.Map<RecordDto>(record)).Returns(recordDto);
			_admissionRepositoryMock.Setup(repo => repo.GetById(admissionID)).ReturnsAsync(admission);
			_admissionRepositoryMock.Setup(repo => repo.SaveChangesAsync());

			var recordService = new RecordService(_recordRepositoryMock.Object, _admissionRepositoryMock.Object, _mapper.Object);

			//Act
			var result = await recordService.CreateRecord(recordDto, admissionID);

			//Assert
			Assert.NotNull(result);
			Assert.Equal(result, recordDto);
			Assert.Equal(admission.Record, record);
			Assert.Equal(DateTime.Now.Date, record.CreatedAt.Date);

		}

		[Fact]
		public async Task CreateRecord_NullValue_ThrowsException()
		{
            //Arrange
            var recordDto = _fixture.Create<RecordDto>();
            var admissionID = 1;
            _admissionRepositoryMock.Setup(repo => repo.GetById(admissionID)).ReturnsAsync(null as Admission);

            var recordService = new RecordService(_recordRepositoryMock.Object, _admissionRepositoryMock.Object, _mapper.Object);

			//Act & Assert
			var result = await Assert.ThrowsAsync<Exception>(() => recordService.CreateRecord(recordDto, admissionID));
			Assert.Equal("Admission with provided id does not exist", result.Message);

        }

	}
}


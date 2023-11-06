using System;
using AutoMapper;
using BLL.Dto;
using BLL.IServices;
using DAL.Entities;
using DAL.IRepositories;

namespace BLL.Services
{
	public class RecordService : IRecordService
	{
        private readonly IRecordRepository _recordRepository;
        private readonly IAdmissionRepository _admissionRepository;
        private readonly IMapper _mapper;

		public RecordService(IRecordRepository recordRepository, IAdmissionRepository admissionRepository,
            IMapper mapper)
		{
            _recordRepository = recordRepository;
            _admissionRepository = admissionRepository;
            _mapper = mapper;
		}

        public async Task<RecordDto> CreateRecord(RecordDto recordDto, int admissionId)
        {
            var admission = await _admissionRepository.GetById(admissionId);
            if (admission == null)
            {
                throw new Exception("Admission with provided id does not exist");
            }

            var record = _mapper.Map<Record>(recordDto);
            record.CreatedAt = DateTime.Now;

            admission.Record = record;
            await _admissionRepository.SaveChangesAsync();

            return _mapper.Map<RecordDto>(record);
        }
    }
}


using System;
using BLL.Dto;

namespace BLL.IServices
{
	public interface IRecordService
	{
		Task<RecordDto> CreateRecord(RecordDto recordDto, int admissionId);
	}
}


using System;
using BLL.Dto;
using BLL.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecordsController : ControllerBase
	{
		private readonly IRecordService _recordService;

		public RecordsController(IRecordService recordService)
		{
			_recordService = recordService;
		}

		[HttpPost("CreateRecord/{admissionId}")]
		public async Task<RecordDto> CreateRecord(RecordDto recordDto, int admissionId)
		{
			var record = await _recordService.CreateRecord(recordDto, admissionId);
			return record;
		}
	}
}


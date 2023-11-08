using System;
namespace BLL.Dto
{
	public class SearchAdmissionsParamsDto : SearchParamsDto
	{
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}
}


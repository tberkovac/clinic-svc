using System;
namespace BLL.Dto
{
	public class ResponsePageDto<TDto> where TDto : class
    {
		public required List<TDto> Data { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int NumberOfElements { get; set; }
	}
}


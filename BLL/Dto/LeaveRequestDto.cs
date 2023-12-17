using System;
namespace BLL.Dto
{
	public class LeaveRequestDto
	{
        public required string LeaveMessage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}


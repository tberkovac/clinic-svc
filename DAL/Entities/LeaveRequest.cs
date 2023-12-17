using System;
namespace DAL.Entities
{
	public class LeaveRequest
	{
		public int LeaveRequestId { get; set; }
		public int OldUserId { get; set; }
		public required string LeaveMessage { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; } = false;
	}
}


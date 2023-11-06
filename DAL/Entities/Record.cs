using System;
namespace DAL.Entities
{
	public class Record
	{
        public int RecordId { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}


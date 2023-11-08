using System;
namespace DAL.Entities
{
	public class SearchAdmissionParams : SearchParams
	{
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}


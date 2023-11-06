using System;
namespace DAL.Entities
{
	public class ResponsePage<TEntity> where TEntity : class
    {
		public required List<TEntity> Data { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int NumberOfElements { get; set; }
	}
}


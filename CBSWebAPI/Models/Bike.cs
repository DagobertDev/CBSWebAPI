using System.ComponentModel.DataAnnotations;

namespace CBSWebAPI.Models
{
	public class Bike
	{
		public Bike(long id, string name)
		{
			Id = id;
			Name = name;
		}

		public long Id { get; set; }
		[MaxLength(32)]
		public string Name { get; set; }
	}
}

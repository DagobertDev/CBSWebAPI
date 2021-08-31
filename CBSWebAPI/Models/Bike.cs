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

		public Bike(string name)
		{
			Name = name;
		}

		public long Id { get; set; }

		[MaxLength(32)]
		public string Name { get; set; }
	}

	public record BikeWrite([property: Required, MaxLength(32)] string Name);

	public record BikeRead(long Id, string Name);
}

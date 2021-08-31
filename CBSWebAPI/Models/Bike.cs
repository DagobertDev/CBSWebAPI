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
		public string Name { get; set; }
	}
}

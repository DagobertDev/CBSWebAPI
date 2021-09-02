using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBSWebAPI.Models
{
	public class Bike
	{
		public Bike(long id, long communityId, string name)
		{
			Id = id;
			CommunityId = communityId;
			Name = name;
		}

		public Bike(long communityId, string name)
		{
			CommunityId = communityId;
			Name = name;
		}

		public long Id { get; set; }

		[MaxLength(32)]
		public string Name { get; set; }
		
		[ForeignKey(nameof(Community))]
		public long CommunityId { get; set; }

		public Community Community { get; set; } = null!;
	}

	public record BikeWrite(long CommunityId, [MaxLength(32)] string Name);

	public record BikeRead(long Id, long CommunityId, string Name);
}

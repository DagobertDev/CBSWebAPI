using System.ComponentModel.DataAnnotations.Schema;

namespace CBSWebAPI.Models
{
	public class CommunityMembership
	{
		[ForeignKey(nameof(User))]
		public string UserId { get; set; } = null!;

		[ForeignKey(nameof(Community))]
		public long CommunityId { get; set; }

		public ApplicationUser User { get; set; } = null!;
		public Community Community { get; set; } = null!;

		public CommunityRole Role { get; set; } = CommunityRole.User;
	}

	public enum CommunityRole
	{
		Admin,
		User
	}
}

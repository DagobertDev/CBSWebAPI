using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CBSWebAPI.Models
{
	public class Community
	{
		public Community(long id, string name)
		{
			Id = id;
			Name = name;
		}

		public long Id { get; set; }

		[MaxLength(32)]
		public string Name { get; set; }

		private ICollection<CommunityMembership>? _members;

		public ICollection<CommunityMembership> Members
		{
			get => _members ??= new List<CommunityMembership>();
			private set => _members = value;
		}
	}
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CBSWebAPI.Models
{
	[Index(nameof(Email), IsUnique = true)]
	public class ApplicationUser
	{
		public ApplicationUser(string id, string email, string username)
		{
			Id = id;
			Email = email;
			Username = username;
		}

		public string Id { get; set; }

		[EmailAddress]
		public string Email { get; set; }

		[MaxLength(32)]
		public string Username { get; set; }

		private ICollection<CommunityMembership>? _memberships;

		public ICollection<CommunityMembership> Memberships
		{
			get => _memberships ??= new List<CommunityMembership>();
			set => _memberships = value;
		}
		
		private ICollection<Bike>? _bikes;

		public ICollection<Bike> Bikes
		{
			get => _bikes ??= new List<Bike>();
			set => _bikes = value;
		}
	}

	public record UserWrite([EmailAddress] string Email, [MaxLength(32)] string Username, [MinLength(6)] string Password);
	
	public record UserRead(string Id, string Email, string Username);
}

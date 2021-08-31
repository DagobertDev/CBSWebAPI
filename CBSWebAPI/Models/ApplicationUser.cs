using System.ComponentModel.DataAnnotations;

namespace CBSWebAPI.Models
{
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
	}
}

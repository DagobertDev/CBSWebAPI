using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace CBSWebAPI.Controllers
{
	public static class ControllerExtensions
	{
		public static bool IsUser(this ControllerBase controller, string id) =>
			controller.User.Claims.Single(claim => claim.Type == "user_id").Value == id;

		public static string GetUserEmail(this ControllerBase controller)
		{
			var firebase = controller.User.Claims.SingleOrDefault(claim => claim.Type == "firebase");

			return firebase == null
				? null
				: JsonSerializer.Deserialize<FirebaseClaim>(firebase.Value)?.identities?.email?.SingleOrDefault();
		}

		private class FirebaseClaim
		{
			public Identities identities { get; set; }

			public class Identities
			{
				public string[] email { get; set; }
			}
		}
	}
}

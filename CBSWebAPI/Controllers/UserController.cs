using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CBSWebAPI.Models;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CBSWebAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UserController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly FirebaseAuth _auth;

		public UserController(AppDbContext context, FirebaseAuth auth)
		{
			_context = context;
			_auth = auth;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<UserRead>> Get(string id)
		{
			var user = await _context.Users
				.Where(u => u.Id == id)
				.Select(u => new UserRead(u.Id, u.Email, u.Username))
				.SingleOrDefaultAsync();

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		[HttpGet]
		public async Task<ActionResult<UserRead>> GetByEmail([FromQuery] [EmailAddress] string email)
		{
			var user = await _context.Users
				.Where(u => u.Email == email)
				.Select(u => new UserRead(u.Id, u.Email, u.Username))
				.SingleOrDefaultAsync();

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<ActionResult<UserRead>> Post(UserWrite user)
		{
			try
			{
				var record = await _auth.CreateUserAsync(new UserRecordArgs
				{
					Uid = Guid.NewGuid().ToString(),
					Email = user.Email,
					Password = user.Password,
					DisplayName = user.Username,
				});

				_context.Users.Add(new ApplicationUser(record.Uid, record.Email, record.DisplayName));
				await _context.SaveChangesAsync();

				return CreatedAtAction(nameof(Get), new { Id = record.Uid },
					new UserRead(record.Uid, record.Email, record.DisplayName));
			}
			catch (FirebaseAuthException e)
			{
				if (e.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
				{
					return Conflict("Email already exists");
				}

				throw;
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			if (!this.IsUser(id))
			{
				return Unauthorized();
			}

			var user = await _context.Users.FindAsync(id);

			if (user == null)
			{
				return NotFound();
			}


			_context.Users.Remove(user);
			await _context.SaveChangesAsync();
			await _auth.DeleteUserAsync(id);
			return NoContent();
		}
	}
}

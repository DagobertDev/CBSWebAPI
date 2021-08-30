using System.Data;
using System.Threading.Tasks;
using CBSWebAPI.Models;
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

		public UserController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ApplicationUser>> Get(string id)
		{
			var user = await _context.Users.FindAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		[HttpGet("email/{email}")]
		public async Task<ActionResult<ApplicationUser>> GetByEmail(string email)
		{
			var user = await _context.Users.SingleOrDefaultAsync(user => user.Email == email);

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		[HttpPost]
		public async Task<ActionResult<ApplicationUser>> Post(ApplicationUser user)
		{
			if (!this.IsUser(user.Id))
			{
				return Unauthorized();
			}
			
			if (this.GetUserEmail() != user.Email)
			{
				return BadRequest();
			}
			
			if (await UserExists(user.Id))
			{
				return BadRequest();
			}

			_context.Users.Add(user);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(Get), new { user.Id }, user);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Put(string id, ApplicationUser user)
		{ 
			if (!this.IsUser(id))
			{
				return Unauthorized();
			}
			
			if (id != user.Id)
			{
				return BadRequest();
			}
			
			if (this.GetUserEmail() != user.Email)
			{
				return BadRequest();
			}

			_context.Entry(user).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DBConcurrencyException)
			{
				if (!await UserExists(id))
				{
					return NotFound();
				}

				throw;
			}

			return NoContent();
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

			return NoContent();
		}

		private Task<bool> UserExists(string id) => _context.Users.AnyAsync(user => user.Id == id);
	}
}

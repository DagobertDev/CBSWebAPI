using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CBSWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CBSWebAPI.Controllers
{
	[ApiController]
	[Authorize]
	[Route("[controller]")]
	public class BikesController : ControllerBase
	{
		private readonly AppDbContext _context;

		public BikesController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet("{id:long}")]
		public async Task<ActionResult<BikeRead>> Get(long id)
		{
			var bike = await _context.Bikes
				.Where(b => b.Id == id)
				.Select(b => BikeRead.From(b))
				.SingleOrDefaultAsync();

			if (bike == null)
			{
				return NotFound();
			}

			return bike;
		}

		[HttpGet]
		public async Task<ActionResult<ICollection<BikeRead>>> GetQuery([FromQuery] long? communityId, [FromQuery] string? userId)
		{
			if (communityId.HasValue)
			{
				return await GetByCommunity(communityId.Value);
			}

			if (userId != null)
			{
				return await GetByUser(userId);
			}

			return BadRequest();
		}

		private async Task<ActionResult<ICollection<BikeRead>>> GetByCommunity([FromQuery] long communityId)
		{
			var userId = this.GetUserId();
			
			var isMember = await _context.Memberships
				.Where(m => m.UserId == userId && m.CommunityId == communityId)
				.AnyAsync();

			if (!isMember)
			{
				return Unauthorized();
			}
			
			return await _context.Bikes
				.Where(b => b.CommunityId == communityId)
				.Select(b => BikeRead.From(b))
				.ToListAsync();
		}


		private async Task<ActionResult<ICollection<BikeRead>>> GetByUser([FromQuery] string userId)
		{
			if (this.GetUserId() != userId)
			{
				return Unauthorized();
			}

			var query = from b in _context.Bikes
			            join c in _context.Communities on b.CommunityId equals c.Id
			            where c.Members.Any(m => m.UserId == userId)
			            where b.UserId == userId || b.UserId == null
			            select BikeRead.From(b);

			return await query.ToListAsync();
		}


		[HttpPost]
		public async Task<ActionResult<BikeRead>> Post(BikeWrite request)
		{
			var bike = new Bike(request.CommunityId, request.Name);
			_context.Bikes.Add(bike);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(Get), new { bike.Id }, BikeRead.From(bike));
		}

		[HttpPut("{id:long}")]
		public async Task<IActionResult> Put(long id, BikeWrite bike)
		{
			_context.Update(new Bike(id, bike.CommunityId, bike.Name, bike.Position));

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DBConcurrencyException)
			{
				if (!BikeExists(id))
				{
					return NotFound();
				}

				throw;
			}

			return NoContent();
		}

		[HttpDelete("{id:long}")]
		public async Task<IActionResult> Delete(long id)
		{
			var bike = await _context.Bikes.FindAsync(id);

			if (bike == null)
			{
				return NotFound();
			}

			_context.Bikes.Remove(bike);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpPost("{id:long}/lend")]
		public async Task<ActionResult<BikeRead>> Lend(long id)
		{
			var bike = await _context.Bikes.FindAsync(id);

			if (bike == null)
			{
				return NotFound();
			}

			var userId = this.GetUserId();

			if (bike.UserId != null)
			{
				return Unauthorized("Bike is already in use");
			}

			bike.UserId = userId;
			bike.Position = null;
			await _context.SaveChangesAsync();

			return BikeRead.From(bike);
		}
		
		[HttpPost("{id:long}/return")]
		public async Task<ActionResult<BikeRead>> Return(long id, GeoPosition position)
		{
			var bike = await _context.Bikes.FindAsync(id);

			if (bike == null)
			{
				return NotFound();
			}

			if (bike.UserId != this.GetUserId())
			{
				return Unauthorized();
			}

			bike.UserId = null;
			bike.Position = position;
			await _context.SaveChangesAsync();

			return BikeRead.From(bike);
		}

		private bool BikeExists(long id) => _context.Bikes.Any(bike => bike.Id == id);
	}
}

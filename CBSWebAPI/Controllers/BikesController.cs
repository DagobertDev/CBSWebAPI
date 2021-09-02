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
				.Select(b => new BikeRead(b.Id,  b.CommunityId, b.Name))
				.SingleOrDefaultAsync();

			if (bike == null)
			{
				return NotFound();
			}

			return bike;
		}
		
		[HttpGet]
		public async Task<ActionResult<IEnumerable<BikeRead>>> GetByCommunity([FromQuery] long communityId) => await _context.Bikes
			.Where(b => b.CommunityId == communityId)
			.Select(b => new BikeRead(b.Id, b.CommunityId, b.Name))
			.ToListAsync();


		[HttpPost]
		public async Task<ActionResult<BikeRead>> Post(BikeWrite request)
		{
			var bike = new Bike(request.CommunityId, request.Name);
			_context.Bikes.Add(bike);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(Get), new { bike.Id }, new BikeRead(bike.Id, bike.CommunityId, bike.Name));
		}

		[HttpPut("{id:long}")]
		public async Task<IActionResult> Put(long id, BikeWrite bike)
		{
			_context.Update(new Bike(id, bike.CommunityId, bike.Name));

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

		private bool BikeExists(long id) => _context.Bikes.Any(bike => bike.Id == id);
	}
}

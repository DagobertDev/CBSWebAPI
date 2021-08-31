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

		[HttpGet]
		public async Task<ActionResult<IEnumerable<BikeRead>>> GetAll() => await _context.Bikes
			.Select(b => new BikeRead(b.Id, b.Name))
			.ToListAsync();

		[HttpGet("{id:long}")]
		public async Task<ActionResult<BikeRead>> Get(long id)
		{
			var bike = await _context.Bikes
				.Where(b => b.Id == id)
				.Select(b => new BikeRead(b.Id, b.Name))
				.SingleOrDefaultAsync();

			if (bike == null)
			{
				return NotFound();
			}

			return bike;
		}

		[HttpPost]
		public async Task<ActionResult<BikeRead>> Post(BikeWrite bikeWrite)
		{
			var bike = new Bike(bikeWrite.Name);
			_context.Bikes.Add(bike);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(Get), new { bike.Id }, new BikeRead(bike.Id, bike.Name));
		}

		[HttpPut("{id:long}")]
		public async Task<IActionResult> Put(long id, BikeWrite bike)
		{
			_context.Update(new Bike(id, bike.Name));

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

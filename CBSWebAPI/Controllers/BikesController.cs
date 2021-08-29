using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CBSWebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class BikesController : ControllerBase
	{
		private readonly AppDbContext _context;

		public BikesController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Bike>>> Get() => await _context.Bikes.ToListAsync();

		[HttpGet("{id:long}")]
		public async Task<ActionResult<Bike>> Get(long id)
		{
			var bike = await _context.Bikes.FindAsync(id);

			if (bike == null)
			{
				return NotFound();
			}

			return bike;
		}

		[HttpPost]
		public async Task<ActionResult<Bike>> Post(Bike bike)
		{
			_context.Bikes.Add(bike);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(Get), new { bike.Id }, bike);
		}

		[HttpPut("{id:long}")]
		public async Task<IActionResult> Put(long id, Bike bike)
		{
			if (id != bike.Id)
			{
				return BadRequest();
			}

			_context.Entry(bike).State = EntityState.Modified;

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

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

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
        public IEnumerable<Bike> Get() => _context.Bikes.ToList();
    }
}

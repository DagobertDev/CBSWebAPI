using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CBSWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BikesController : ControllerBase
    {
	    private readonly ILogger<BikesController> _logger;

        public BikesController(ILogger<BikesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Bike> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, rng.Next(1, 6)).Select(index => new Bike
            {
	            Id = index,
	            Name = $"Fahrrad Nr. {index}"
            })
            .ToArray();
        }
    }
}

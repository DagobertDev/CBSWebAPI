using System.Collections.Generic;
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
	public class CommunitiesController : ControllerBase
	{
		private readonly AppDbContext _context;

		public CommunitiesController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet("{id:long}")]
		public async Task<ActionResult<CommunityRead>> Get(long id)
		{
			var userId = this.GetUserId();

			var result = await _context.Communities.Where(c => c.Id == id).Select(c => 
					new 
					{ 
						Community = new CommunityRead(c.Id, c.Name), 
						IsMember = c.Members.Any(m => m.UserId == userId) 
					})
				.SingleOrDefaultAsync();

			if (result == null)
			{
				return NotFound();
			}

			if (!result.IsMember)
			{
				return Unauthorized();
			}

			return result.Community;
		}

		[HttpGet]
		public async Task<ActionResult<ICollection<CommunityRead>>> GetByUser([FromQuery] string member)
		{
			if (!this.IsUser(member))
			{
				return Unauthorized();
			}

			return await _context.Communities.Where(community => 
					community.Members.Any(m => m.UserId == member))
				.Select(community => new CommunityRead(community.Id, community.Name))
				.ToListAsync();
		}

		[HttpPost]
		public async Task<ActionResult<CommunityRead>> Post(CommunityWrite communityWrite)
		{
			var community = new Community(0, communityWrite.Name);
			
			_context.Communities.Add(community);

			var userId = this.GetUserId();

			community.Members.Add(new CommunityMembership
			{
				UserId = userId,
				Community = community,
				Role = CommunityRole.Admin,
			});

			await _context.SaveChangesAsync();

			return new CommunityRead(community.Id, community.Name);
		}
	}
}

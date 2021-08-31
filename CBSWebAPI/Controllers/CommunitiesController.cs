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
		public async Task<ActionResult<Community>> Get(long id)
		{
			var community = await _context.Communities.Include(community => community.Members)
				.SingleOrDefaultAsync(community => community.Id == id);

			if (community == null)
			{
				return NotFound();
			}

			var userId = this.GetUserId();

			if (community.Members.None(member => member.UserId == userId))
			{
				return Unauthorized();
			}

			return community;
		}

		[HttpGet]
		public async Task<ActionResult<ICollection<Community>>> GetByUser([FromQuery] string member)
		{
			if (!this.IsUser(member))
			{
				return Unauthorized();
			}

			return await _context.Communities.Where(community => community.Members.Any(m => m.UserId == member))
				.ToListAsync();
		}

		[HttpPost]
		public async Task<ActionResult<CommunityWrite>> Post(Community community)
		{
			_context.Communities.Add(community);
			
			var userId = this.GetUserId();

			community.Members.Add(new CommunityMembership
			{
				UserId = userId,
				Community = community,
				Role = CommunityRole.Admin,
			});

			await _context.SaveChangesAsync();

			return new CommunityWrite(community.Id, community.Name);
		}

		public record CommunityWrite(long Id, string Name);
	}
}

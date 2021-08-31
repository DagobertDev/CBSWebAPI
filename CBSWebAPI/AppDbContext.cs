using CBSWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CBSWebAPI
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<Bike> Bikes => Set<Bike>();
		public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
		public DbSet<Community> Communities => Set<Community>();

		protected override void OnModelCreating(ModelBuilder model)
		{
			model.Entity<CommunityMembership>(membership =>
			{
				membership.HasKey(m => new { m.UserId, m.CommunityId });
			});
		}
	}
}

using System.Text.Json;
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
		public DbSet<CommunityMembership> Memberships => Set<CommunityMembership>();

		protected override void OnModelCreating(ModelBuilder model)
		{
			model.Entity<CommunityMembership>(membership =>
			{
				membership.HasKey(m => new { m.UserId, m.CommunityId });
			});

			model.Entity<Bike>().Property(b => b.Position).HasConversion(
				p => JsonSerializer.Serialize(p, new JsonSerializerOptions()),
				p => JsonSerializer.Deserialize<GeoPosition>(p, new JsonSerializerOptions()));
		}
	}
}

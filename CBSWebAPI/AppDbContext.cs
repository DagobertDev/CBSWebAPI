using CBSWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CBSWebAPI
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<Bike> Bikes { get; set; }
		public DbSet<ApplicationUser> Users { get; set; }
	}
}

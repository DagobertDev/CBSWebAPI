using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CBSWebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            using (var scope = host.Services.CreateScope())
            {
	            var firebaseAuth = scope.ServiceProvider.GetRequiredService<FirebaseAuth>();
	            
	            var pagedEnumerable = firebaseAuth.ListUsersAsync(null);
	            var responses = pagedEnumerable.AsRawResponses().GetAsyncEnumerator();
	            
	            while (await responses.MoveNextAsync())
	            {
		            ExportedUserRecords response = responses.Current;
		            var users = response.Users;

		            if (users == null)
		            {
			            continue;
		            }
		            
		            await firebaseAuth.DeleteUsersAsync(users.Select(user => user.Uid).ToList());
	            }

	            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	            await db.Database.EnsureDeletedAsync();
	            await db.Database.EnsureCreatedAsync();
            }
            
	        await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
	            .ConfigureAppConfiguration((_, config) =>
	            {
		            config.AddEnvironmentVariables("CBS_");
	            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

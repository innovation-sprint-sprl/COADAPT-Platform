using Contracts.Logger;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace UserManagement.WebAPI {

	public static class Program {

		public static void Main(string[] args) {
			var host = CreateWebHostBuilder(args).Build();

			using (var scope = host.Services.CreateScope()) {
				var services = scope.ServiceProvider;
				var logger = services.GetRequiredService<ILoggerManager>();
				try {
					//var config = host.Services.GetRequiredService<IConfiguration>();
					//var testUserPw = config["SeedUserPW"];
					//logger.LogInfo($"Initializing users with password: '{testUserPw}'");
					SeedData.Initialize(services, "a1!B2@c3#").Wait();
				} catch (Exception ex) {
					logger.LogError($"An error occurred seeding the DB: {ex.Message}");
				}
				try {
					SeedData.Decrypt();
				} catch (Exception ex) {
					logger.LogError($"An error occurred decoding the data: {ex.Message}");
				}
			}

			host.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();

	}

}

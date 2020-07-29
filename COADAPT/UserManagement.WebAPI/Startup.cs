using System.IO;
using Contracts.Logger;
using Entities;
using Entities.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using UserManagement.WebAPI.Extensions;

namespace UserManagement.WebAPI {

	public class Startup {

		private readonly IConfigurationSection _appSettings;
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration) {
			LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
			Configuration = configuration;
			_appSettings = Configuration.GetSection("AppConfiguration");
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			// Used for various Services
			services.ConfigureCors();
			services.ConfigureIISIntegration();

			services.ConfigureMySqlContext(_appSettings["MySQLConnectionString"]);
			services.ConfigureCOADAPTService();

			services.ConfigureIdentitySupport();
			services.ConfigureIdentityOptions();

			services.ConfigureLoggerService();

			services.ConfigureAuthentication();
			services.ConfigureAuthorization();

			services.ConfigureSwagger(_appSettings["APIVersion1_0"]);

			services.Configure<AppSettings>(_appSettings);

			services.AddMvc(config => {
				var policy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.Build();
				config.Filters.Add(new AuthorizeFilter(policy));
			}).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerManager logger, COADAPTContext coadaptContext) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			} else {
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseCors(Constants.ApplicationOptions.CorsPolicyAllowAll);
			app.UseSwaggerDocumentation(_appSettings["APIVersion1_0"]);
			app.ConfigureExceptionHandler(logger);

			// HTTPS is handled by reverse proxy, see https://tinyurl.com/net-core-https
			// app.UseHttpsRedirection();

			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseAuthentication();

			app.UseMvc();

			// ===== Ensure Created =====
			// coadaptContext.Database.EnsureCreated();
		}

	}

}

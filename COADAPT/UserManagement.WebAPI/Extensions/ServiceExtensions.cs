using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Constants;
using Contracts.Logger;
using Contracts.Repository;
using Entities;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Swashbuckle.AspNetCore.Swagger;

namespace UserManagement.WebAPI.Extensions {

	public static class ServiceExtensions {

		/**
		 * Configure CORS Policy to consume data from third party server.
		 */
		public static void ConfigureCors(this IServiceCollection services) {
			services.AddCors(options => {
				options.AddPolicy(ApplicationOptions.CorsPolicyAllowAll,
					builder => builder.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowCredentials());
			});
		}

		/*
		 * Configure IIS Integration for the Web API.
		 *
		 * Edit only if you know what you are doing.
		 */
		public static void ConfigureIISIntegration(this IServiceCollection services) {
			services.Configure<IISOptions>(options => {
				/* Include Configuration here */
			});
		}

		/*
		 * Setup MySQL support for EFCore by passing the connection string
		 * to the DBContext.
		 *
		 * @param string MySQL Connection String
		 */
		public static void ConfigureMySqlContext(this IServiceCollection services, string mySqlConnectionString) {
			services.AddDbContext<COADAPTContext>(o => o.UseMySql(mySqlConnectionString));
		}

		public static void ConfigureIdentitySupport(this IServiceCollection services) {
			services.AddIdentity<IdentityUser, IdentityRole>()
				.AddEntityFrameworkStores<COADAPTContext>()
				.AddDefaultTokenProviders();
		}

		/**
		 * Helper method with the most common settings for Identity.
		 *
		 * @documentation: https://bit.ly/2WQNNUK
		 */
		public static void ConfigureIdentityOptions(this IServiceCollection services) {
			services.Configure<IdentityOptions>(options => {
				// Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 1;

				// Lockout settings.
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;

				// User settings.
				options.User.AllowedUserNameCharacters =
					"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
				options.User.RequireUniqueEmail = false;
			});
		}

		/*
		 * Initialize Repository Wrapper.
		 * Create global instance in order to access data from Repository.
		 */
		public static void ConfigureCOADAPTService(this IServiceCollection services) {
			services.AddScoped<IRepositoryWrapper, COADAPTService>();
		}

		/*
		 * Initialize NLog Configuration Model.
		 * *nlog.config* file is being server via Startup.cs/Startup()
		 */
		public static void ConfigureLoggerService(this IServiceCollection services) {
			services.AddSingleton<ILoggerManager, LoggerManager>();
		}

		public static void ConfigureAuthentication(this IServiceCollection services) {
			services.AddAuthentication(x => {
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(x => {
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters {
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("af4fXtXz3LJzjQVRKVlBjnXOo8tChET4")),
					ValidateIssuer = false,
					ValidateAudience = false,
					RequireExpirationTime = false,
					ValidateLifetime = true
				};
			});
		}

		public static void ConfigureAuthorization(this IServiceCollection services) {
			services.AddAuthorization(x => {
				x.AddPolicy("Administrator", builder => builder.RequireRole(Role.AdministratorRole));
				x.AddPolicy("SubAdministrator", builder => builder.RequireRole(Role.AdministratorRole, Role.SubAdministratorRole));
				x.AddPolicy("Supervisor", builder => builder.RequireRole(Role.AdministratorRole, Role.SubAdministratorRole, Role.SupervisorRole));
				x.AddPolicy("Therapist", builder => builder.RequireRole(Role.AdministratorRole, Role.SubAdministratorRole, Role.SupervisorRole, Role.TherapistRole));
				x.AddPolicy("Participant", builder => builder.RequireRole(Role.AdministratorRole, Role.SubAdministratorRole, Role.SupervisorRole, Role.ParticipantRole));
				x.AddPolicy("Everyone", builder => builder.RequireRole(Role.AdministratorRole, Role.SubAdministratorRole, Role.SupervisorRole, Role.TherapistRole, Role.ParticipantRole));
			});
		}

		/*
		 * Swagger document Configuration method.
		 *
		 * @param string Current API Version
		 */
		public static void ConfigureSwagger(this IServiceCollection services, string apiVersion) {
			services.AddSwaggerGen(c => {
				c.SwaggerDoc(apiVersion, new Info {
					Version = apiVersion,
					Title = "COADAPT User Management API",
					TermsOfService = "ToS Here",
					License = new License {
						Name = "OpenSource License Pending",
						Url = "https://example.com/license"
					}
				});

				c.AddSecurityDefinition("Bearer", new ApiKeyScheme {
					Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
					Name = "Authorization",
					In = "header",
					Type = "apiKey"
				});

				c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
					{ "Bearer", Array.Empty<string>() }
				});

				// Set the comments path for the Swagger JSON and UI.
				// Requires to enable .csproj property <GenerateDocumentationFile>
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});
		}

	}

}

using Microsoft.AspNetCore.Builder;

namespace UserManagement.WebAPI.Extensions {

	public static class ApplicationBuilderExtensions {

		public static void UseSwaggerDocumentation(this IApplicationBuilder app, string apiVersion) {
			app.UseSwagger();
			app.UseSwaggerUI(c => {
				c.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", $"COADAPT API {apiVersion}");

				// Clears Swagger routing, this makes swagger to be served @Index
				c.RoutePrefix = string.Empty;
			});
		}

	}

}

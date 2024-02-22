using IdentityWebApp.Data;
using IdentityWebApp.FileUploadService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityWebApp
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();

			//Srinath added this line to support Razor pages.
			services.AddRazorPages();
			//Srinath added DBContext to connect to sql db.
			services.AddDbContext<IdentityDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AuthDBContextConnection")));
			//Srinath added to remove the uppercase requirement
			services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequireUppercase = false;
				options.SignIn.RequireConfirmedEmail = true;
			});
			//Srinath, DI for the Interface Fileuploadservice created under the folder Fileuploadservice.
			services.AddScoped<IFileUploadService, LocalFileUploadService>();		

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();

			app.UseRouting();

			//srinath added below line for authentication and display of the user details like email.
			app.UseAuthentication();

			app.UseAuthorization();

			app.UseStaticFiles();

			

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
				//Srinath added this line to support Razor pages. All the routes are managed by Identity Library.
				endpoints.MapRazorPages();
			});
		}
	}
}

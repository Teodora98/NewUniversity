using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewUniversity.Data;
using NewUniversity.Models;

namespace NewUniveristy

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
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        );
            });
            //Json result for api client
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0)
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.WriteIndented = true; //pretty json
                });
            //services.AddCors();
            services.AddControllersWithViews();
            services.AddDefaultIdentity<AppUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<NewUniversityContext>().AddDefaultTokenProviders();
            services.AddDbContext<NewUniversityContext>(options => options.UseSqlServer(Configuration.GetConnectionString("NewUniversityContext")));
            // services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Authenticate /Login");
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

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

                app.UseHsts();

            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseStatusCodePages();
            app.UseDeveloperExceptionPage();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>

            {
                endpoints.MapControllerRoute(

                    name: "default",

                    pattern: "{controller=Students}/{action=Index}/{id?}");

            });
            app.UseCors("CorsPolicy");

        }

    }

}

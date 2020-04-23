using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewUniversity.Data;
using NewUniversity.Models;
namespace NewUniveristy
{
    public class Program
    {
        public static void Main(string[] args)

        {

            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())

            {

                var services = scope.ServiceProvider; try

                {

                    var context = services.GetRequiredService<NewUniversityContext>();

                    context.Database.Migrate();

                    SeedData.Initialize(services);

                }

                catch (Exception ex)

                {

                    var logger = services.GetRequiredService<ILogger<Program>>();

                    logger.LogError(ex, "An error occurred seeding the DB.");

                }

            }

            host.Run();

        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>

                Host.CreateDefaultBuilder(args)

                    .ConfigureWebHostDefaults(webBuilder =>

                    {

                        webBuilder.UseStartup<Startup>();

                    });
    }
}
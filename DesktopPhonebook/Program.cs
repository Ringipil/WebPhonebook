using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows.Forms;
using WebPhonebook;
using WebPhonebook.Models;

namespace DesktopPhonebook
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var host = CreateHostBuilder().Build();
            var services = host.Services;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var scope = services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PeopleDbContext>();
                //dbContext.Database.Migrate(); // ✅ Ensures the database is created and migrated
            }

            Application.Run(services.GetRequiredService<Form1>());
        }

        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<PeopleDbContext>(options =>
                        options.UseSqlServer("Server=.\\SQLEXPRESS;Database=PeopleDB;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False"));

                    services.AddScoped<EfDatabaseHandler>(); // ✅ Register EfDatabaseHandler
                    services.AddScoped<SqlDatabaseHandler>(); // ✅ Register SqlDatabaseHandler
                    services.AddSingleton<Form1>(); // ✅ Inject Form1
                });
        }
    }
}

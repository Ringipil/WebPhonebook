using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebPhonebook;

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

                    services.AddScoped<EfDatabaseHandler>();
                    services.AddScoped<SqlDatabaseHandler>();
                    services.AddSingleton<Form1>();
                });
        }
    }
}

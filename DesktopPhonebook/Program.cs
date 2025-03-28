﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhonebookServices;
using WebPhonebook;

namespace DesktopPhonebook
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<PeopleDbContext>(options =>
                    {
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    });
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddSingleton<SqlDatabaseHandler>();
                    services.AddSingleton<EfDatabaseHandler>();
                    services.AddSingleton<NameLoader>();
                    services.AddSingleton<Form1>();
                })
                .Build();

            var services = host.Services;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var scope = services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PeopleDbContext>();
                dbContext.Database.EnsureCreated();
            }

            Application.Run(services.GetRequiredService<Form1>());
        }
    }
}

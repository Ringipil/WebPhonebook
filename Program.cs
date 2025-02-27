using Microsoft.EntityFrameworkCore;
using WebPhonebook.Interfaces;
using WebPhonebook.Models;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing.");
}

builder.Services.AddDbContext<PeopleDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IDatabaseHandler, SqlDatabaseHandler>();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseSession();
app.UseRouting();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=People}/{action=Index}/{id?}");
});

app.Run();



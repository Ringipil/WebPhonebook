using Microsoft.EntityFrameworkCore;
using WebPhonebook;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing.");
}

builder.Services.AddDbContext<PeopleDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<SqlDatabaseHandler>();
builder.Services.AddScoped<EfDatabaseHandler>();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var sqlDbHandler = scope.ServiceProvider.GetRequiredService<SqlDatabaseHandler>();
    sqlDbHandler.InitializeDatabase();
}

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

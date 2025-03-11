using Microsoft.EntityFrameworkCore;
using WebPhonebook;
using WebPhonebook.Interfaces;

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
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

//builder.Services.AddScoped<IDatabaseHandler>(provider =>
//{
//    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
//    var selectedHandler = httpContextAccessor.HttpContext?.Session.GetString("SelectedHandler") ?? "ef";

//    return selectedHandler == "sql"
//        ? provider.GetRequiredService<SqlDatabaseHandler>()
//        : provider.GetRequiredService<EfDatabaseHandler>();
//});

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

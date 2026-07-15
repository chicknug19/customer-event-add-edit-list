using Microsoft.EntityFrameworkCore;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Data.Repositories;
using JPP.Services.Interfaces;
using JPP.Services.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var mvcBuilder = builder.Services.AddControllersWithViews();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

var aiConnectionString = GetRequiredSqlConnectionString(builder.Configuration, "AiConnection");
var crmConnectionString = GetRequiredSqlConnectionString(builder.Configuration, "CrmConnection");

builder.Services.AddSingleton<IAiDbConnectionFactory>(
    new AiDbConnectionFactory(aiConnectionString)
);

builder.Services.AddSingleton<ICrmDbConnectionFactory>(
    new CrmDbConnectionFactory(crmConnectionString)
);

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IDatabaseConnectionRepository, DatabaseConnectionRepository>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<IStoreListRepository, StoreListRepository>();
builder.Services.AddScoped<IStoreListService, StoreListService>();

builder.Services.AddScoped<IEmployeeListRepository, EmployeeListRepository>();
builder.Services.AddScoped<IEmployeeListService, EmployeeListService>();

builder.Services.AddScoped<IEmployeeDepartmentRepository, EmployeeDepartmentRepository>();
builder.Services.AddScoped<IEmployeeDepartmentService, EmployeeDepartmentService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<ICustomerListService, CustomerListService>();

builder.Services.AddScoped<IOtpService, CellboxOtpService>();
builder.Services.AddScoped<IAccountEmailService, AccountEmailService>();


var app = builder.Build();


//using (var scope = app.Services.CreateScope())
//{
//    var databaseConnectionRepository = scope.ServiceProvider
//        .GetRequiredService<IDatabaseConnectionRepository>();

//    var connectionResult = await databaseConnectionRepository.CheckAllConnectionsAsync();

//    if (connectionResult.StatusCode != 200)
//    {
//        throw new InvalidOperationException(connectionResult.StatusMessage);
//    }

//    Console.WriteLine(connectionResult.StatusMessage);
//}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();


static string GetRequiredSqlConnectionString(IConfiguration configuration, string name)
{
    var connectionString = configuration.GetConnectionString(name);

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException($"{name} is missing in appsettings.json.");
    }

    return connectionString;
}
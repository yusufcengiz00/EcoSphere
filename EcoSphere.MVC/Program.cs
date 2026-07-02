using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Serilog;
using EcoSphere.Core.Repositories;
using EcoSphere.Core.Models;
using EcoSphere.Data;
using EcoSphere.Data.Repositories;
using OfficeOpenXml;
using QuestPDF.Infrastructure;
using System.IO;

// Initialize Serilog Logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/ecosphere_log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("EcoSphere uygulaması başlatılıyor...");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog
    builder.Host.UseSerilog();

    // Register License contexts
    ExcelPackage.License.SetNonCommercialPersonal("EcoSphere");
    QuestPDF.Settings.License = LicenseType.Community;

    // Register DbContext with SQL Server Connection String
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<EcoSphereDbContext>(options =>
        options.UseSqlServer(connectionString));

    // Register Repositories & Unit of Work
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ISustainabilityGoalRepository, SustainabilityGoalRepository>();
    builder.Services.AddScoped<IEcoCertificateRepository, EcoCertificateRepository>();
    builder.Services.AddScoped<ISustainabilityAuditRepository, SustainabilityAuditRepository>();
    builder.Services.AddScoped<IEnergyConsumptionRepository, EnergyConsumptionRepository>();
    builder.Services.AddScoped<IWasteManagementRepository, WasteManagementRepository>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Add MVC controllers with views
    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    // Auto-create database & apply seed data on startup
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<EcoSphereDbContext>();
            Log.Information("LocalDB veritabanı denetleniyor...");
            
            var databaseCreator = context.Database.GetService<Microsoft.EntityFrameworkCore.Storage.IRelationalDatabaseCreator>();
            if (!databaseCreator.Exists())
            {
                Log.Information("Yeni veritabanı dosyası oluşturuluyor...");
                databaseCreator.Create();
            }
            
            Log.Information("Göçler (migrations) uygulanıyor...");
            context.Database.Migrate();
            Log.Information("Veritabanı başarıyla doğrulandı/göçürüldü.");
            
            // Create upload folder if not exists
            var uploadsPath = Path.Combine(app.Environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
                Log.Information("Görsel yükleme klasörü oluşturuldu: {Path}", uploadsPath);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Veritabanı başlatılırken hata oluştu!");
        }
    }

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    
    // Serving files from wwwroot
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama beklenmedik şekilde sonlandı!");
}
finally
{
    Log.CloseAndFlush();
}

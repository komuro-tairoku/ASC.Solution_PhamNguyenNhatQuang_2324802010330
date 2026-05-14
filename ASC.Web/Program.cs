//using ASC.DataAccess;
//using ASC.DataAccess.Interfaces;
using ASC.Buisiness;
using ASC.Buisiness.Interfaces;
using ASC.Business.Interfaces;
using ASC.DataAccess;
using ASC.Model.Models;
using ASC.Solution.Services;
using ASC.Web.Configuration;
using ASC.Web.Data;
using ASC.Web.ServiceHub;
using ASC.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConfig(builder.Configuration)
    .AddMyDependencyGroup(builder.Configuration);

// Add services to the container.
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found."
    );

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<IdentityUser>(options =>
//    options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddOptions();

builder.Services.Configure<ApplicationSettings>(
    builder.Configuration.GetSection("AppSetting")
);

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

//
// SIGNALR
//
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddTransient<IEmailSender, AuthMessageSender>();

builder.Services.AddTransient<ISmsSender, AuthMessageSender>();

builder.Services.AddSingleton<IIdentitySeed, IdentitySeed>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

builder.Services.AddSingleton<
    INavigationCacheOperations,
    NavigationCacheOperations>();

builder.Services.AddScoped<
    IServiceRequestMessageOperations,
    ServiceRequestMessageOperations>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["CacheSettings:CacheConnectionString"];

    options.InstanceName =
        builder.Configuration["CacheSettings:CacheInstance"];
});

var app = builder.Build();

//
// DATABASE MIGRATION
//
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();
}

//
// Configure the HTTP request pipeline.
//
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

//
// WEBSOCKET
//
app.UseWebSockets();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

//
// ROUTES
//
app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

//
// SIGNALR HUB
//
app.MapHub<ServiceMessagesHub>(
    "/serviceMessagesHub"
);

//
// IDENTITY SEED
//
using (var scope = app.Services.CreateScope())
{
    var storageSeed =
        scope.ServiceProvider
            .GetRequiredService<IIdentitySeed>();

    await storageSeed.Seed(
        scope.ServiceProvider
            .GetService<UserManager<IdentityUser>>(),

        scope.ServiceProvider
            .GetService<RoleManager<IdentityRole>>(),

        scope.ServiceProvider
            .GetService<IOptions<ApplicationSettings>>()
    );
}

//
// MASTER DATA SEED
//
using (var scope = app.Services.CreateScope())
{
    var masterDataOperations =
        scope.ServiceProvider
            .GetRequiredService<IMasterDataOperations>();

    // Check if data exists
    var existingData = await masterDataOperations.GetAllMasterValuesAsync();
    
    if (existingData == null || !existingData.Any())
    {
        var masterDataList = new List<MasterDataValue>
        {
            // Vehicle Types
            new MasterDataValue 
            { 
                PartitionKey = "VehicleType", 
                RowKey = Guid.NewGuid().ToString(), 
                Name = "Motorcycle", 
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                CreatedBy = "system",
                UpdatedBy = "system"
            },
            new MasterDataValue 
            { 
                PartitionKey = "VehicleType", 
                RowKey = Guid.NewGuid().ToString(), 
                Name = "Car", 
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                CreatedBy = "system",
                UpdatedBy = "system"
            },
            new MasterDataValue 
            { 
                PartitionKey = "VehicleType", 
                RowKey = Guid.NewGuid().ToString(), 
                Name = "Truck", 
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                CreatedBy = "system",
                UpdatedBy = "system"
            },
            // Vehicle Names
            new MasterDataValue 
            { 
                PartitionKey = "VehicleName", 
                RowKey = Guid.NewGuid().ToString(), 
                Name = "Honda CB 500", 
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                CreatedBy = "system",
                UpdatedBy = "system"
            },
            new MasterDataValue 
            { 
                PartitionKey = "VehicleName", 
                RowKey = Guid.NewGuid().ToString(), 
                Name = "Toyota Camry", 
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                CreatedBy = "system",
                UpdatedBy = "system"
            },
            new MasterDataValue 
            { 
                PartitionKey = "VehicleName", 
                RowKey = Guid.NewGuid().ToString(), 
                Name = "Ford Ranger", 
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                CreatedBy = "system",
                UpdatedBy = "system"
            }
        };

        await masterDataOperations.UploadBulkMasterData(masterDataList);
    }
}

//
// NAVIGATION CACHE
//
using (var scope = app.Services.CreateScope())
{
    var navigationCacheOperations =
        scope.ServiceProvider
            .GetRequiredService<INavigationCacheOperations>();

    await navigationCacheOperations
        .CreateNavigationCacheAsync();
}

//
// MASTER DATA CACHE
//
using (var scope = app.Services.CreateScope())
{
    var masterDataCacheOperations =
        scope.ServiceProvider
            .GetRequiredService<IMasterDataCacheOperations>();

    await masterDataCacheOperations
        .CreateMasterDataCacheAsync();
}

app.Run();
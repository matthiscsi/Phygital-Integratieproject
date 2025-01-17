using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PIP.BL;
using PIP.BL.IManagers;
using PIP.BL.Managers;
using PIP.DAL;
using PIP.DAL.EF;
using PIP.DAL.EF.Repositories;
using PIP.DAL.IRepositories;
using PIP.Domain.User;
using StackExchange.Redis;
using UI.MVC.Controllers;
using UI.MVC.Hub;
using Companion = PIP.Domain.User.Companion;

var builder = WebApplication.CreateBuilder(args);

string secretIdSql = "connectionstring";

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    var blDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "BL");
    var configPath = Path.Combine(blDirectory, "Configuration", "ProfanityFilter.json");
    builder.Configuration.AddJsonFile(configPath);
}
else
{
    var mvcDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "phygitalapp");
    var configPath = Path.Combine(mvcDirectory, "Configuration", "ProfanityFilter.json");
    builder.Configuration.AddJsonFile(configPath);
}


string adminPassSecretId = "appuseradminpass";
string companionPassSecretId = "appusercompanionpass";
string subplatformAdminPassSecretId = "appusersubplatformadminpass";

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
// Set the path to the service account key file
    string pathToServiceAccountKeyFile = "./secrets/googleappcred.json";
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToServiceAccountKeyFile);
}

SecretManager secretManager = new SecretManager();
CloudBucketManager cloudBucketManager = new CloudBucketManager();


// Add services to the container.
builder.Services.AddScoped<IFlowStepRepository, FlowStepRepository>();
builder.Services.AddScoped<IFlowRepository, FlowRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IResponseRepository, ResponseRepository>();
builder.Services.AddScoped<IFlowsessionRepository, FlowsessionRepository>();
builder.Services.AddScoped<ISubthemeRepository, SubthemeRepository>();
builder.Services.AddScoped<ISubPlatformRepository, SubPlatformRepository>();
builder.Services.AddScoped<IIdeaRepository, IdeaRepository>();
builder.Services.AddScoped<IProjectManager, ProjectManager>();
builder.Services.AddScoped<IFlowStepManager, FlowStepManager>();
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IIdeaManager, IdeaManager>();
builder.Services.AddScoped<IResponseManager, ResponseManager>();
builder.Services.AddScoped<IFlowSessionManager, FlowSessionManager>();
builder.Services.AddScoped<IFlowManager, FlowManager>();
builder.Services.AddScoped<ISubthemeManager, SubthemeManager>();
builder.Services.AddScoped<ISubPlatformManager, SubplatformManager>();
builder.Services.AddScoped<ICloudBucketManager, CloudBucketManager>();
builder.Services.AddScoped<UnitOfWork, UnitOfWork>();
builder.Services.AddScoped<FlowStepHelper>();
builder.Services.AddScoped<IProfanityFilter, JsonProfanityFilter>();
builder.Services.AddSingleton<SecretManager>();
builder.Services.AddSignalR();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

var supportedCultures = new List<CultureInfo>
{
    new CultureInfo("nl"),
    new CultureInfo("en")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("nl");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});


builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PhygitalDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    string connectionString = builder.Configuration.GetConnectionString("PhygitalDbContextConnection") ??
                              throw new InvalidOperationException(
                                  "Connection string 'PhygitalDbContextConnection' not found.");

    builder.Services.AddDbContext<PhygitalDbContext>(optionsBuilder =>
        optionsBuilder.UseSqlite(connectionString));
}
else
{
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

    string connectionString = await secretManager.GetSecretAsync(secretIdSql);

    builder.Services.AddDbContext<PhygitalDbContext>(optionsBuilder =>
        optionsBuilder.UseNpgsql(connectionString));
}

// Redis
if (builder.Environment.IsProduction())
{
    string redisAddress = await secretManager.GetSecretAsync("redisaddress");   
    
    var redis = ConnectionMultiplexer
        .Connect(redisAddress);
    builder.Services.AddDataProtection()
        .PersistKeysToStackExchangeRedis(redis, "DataProtectionKeys");
    builder.Services.AddStackExchangeRedisCache(option =>
    {
        option.Configuration = redisAddress;
        option.InstanceName = "phygitalredisinstance";
    });
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.Name = "PhygitalCookie";
    });
}


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    bool isCreated;
    PhygitalDbContext dbCtx = scope.ServiceProvider.GetRequiredService<PhygitalDbContext>();

    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
    {
        isCreated = true;
        dbCtx.CreateDatabase(dropDatabase: isCreated);
    }
    else
    {
        isCreated = false;
        isCreated = dbCtx.CreateDatabase(dropDatabase: isCreated);
    }

    if (dbCtx.CreateDatabase(isCreated))
    {
        UserManager<IdentityUser> userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        RoleManager<IdentityRole> roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        SeedIdentity(userManager, roleManager);

        DataSeeder.Seed(dbCtx, userManager).Wait();

        //QR Codes genereren voor de eerste twee flows, deze flows hebben we manueel geseed
        cloudBucketManager.GenerateQrCode("flow", 1);
        cloudBucketManager.GenerateQrCode("flow", 2);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();   
app.UseAuthentication();
app.UseAuthorization();
if (builder.Environment.IsProduction())
{
    app.UseSession();
}

app.MapHub<FlowHub>("flowStepHub");

var options = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();

app.UseRequestLocalization(options.Value);


app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();

app.Run();

void SeedIdentity(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
{
    var subplatformadministratorRole = new IdentityRole
    {
        Name = "subplatformadministrator"
    };
    roleManager.CreateAsync(subplatformadministratorRole).Wait();
    var adminRole = new IdentityRole
    {
        Name = "admin"
    };
    roleManager.CreateAsync(adminRole).Wait();

    var companionRole = new IdentityRole
    {
        Name = "companion"
    };
    roleManager.CreateAsync(companionRole).Wait();


    var admin = new IdentityUser
    {
        UserName = "admin@kdg.be",
        Email = "admin@kdg.be",
        EmailConfirmed = true
    };
    string adminPass = secretManager.GetSecretAsync(adminPassSecretId).Result;
    userManager.CreateAsync(admin, adminPass).Wait();


    var subplatformadministrator = new SubPlatformAdministrator()
    {
        UserName = "Sadmin@kdg.be",
        Email = "Sadmin@kdg.be",
        EmailConfirmed = true,

        OrganizationName = "Stad Mechelen"
    };
    string subplatformAdminPass = secretManager.GetSecretAsync(subplatformAdminPassSecretId).Result;
    userManager.CreateAsync(subplatformadministrator, subplatformAdminPass).Wait();
    userManager.AddToRoleAsync(subplatformadministrator, "subplatformadministrator").Wait();

    var companion = new Companion()
    {
        UserName = "companion@kdg.be",
        Email = "companion@kdg.be",
        EmailConfirmed = true
    };
    string companionPass = secretManager.GetSecretAsync(companionPassSecretId).Result;

    userManager.CreateAsync(companion, companionPass).Wait();


    userManager.AddToRoleAsync(admin, adminRole.Name).Wait();
    userManager.AddToRoleAsync(subplatformadministrator, subplatformadministratorRole.Name).Wait();
    userManager.AddToRoleAsync(companion, companionRole.Name).Wait();
}
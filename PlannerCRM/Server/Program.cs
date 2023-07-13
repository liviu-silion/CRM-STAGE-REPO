var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration
            .GetConnectionString("ConnString")
                ?? throw new InvalidOperationException("ConnString not found!"))
);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddHttpClient();

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddUserManager<UserManager<IdentityUser>>()
    .AddDefaultTokenProviders();

builder.Services.Configure<CookiePolicyOptions>(options => 
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services
    .Configure<IdentityOptions>(o =>
    {
        o.SignIn.RequireConfirmedEmail = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequireDigit = false;
        o.Password.RequiredLength = 8;
        o.Lockout.AllowedForNewUsers = true;
        o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        o.Lockout.MaxFailedAccessAttempts = 5;
    });

builder.Services.AddScoped<ApplicationUserRepository>();
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<WorkOrderRepository>();
builder.Services.AddScoped<ActivityRepository>();
builder.Services.AddScoped<WorkTimeRecordRepository>();
builder.Services.AddScoped<CalculateService>();

builder.Services.AddScoped<Logger<EmployeeRepository>>();
builder.Services.AddScoped<Logger<WorkOrderRepository>>();
builder.Services.AddScoped<Logger<ActivityRepository>>();
builder.Services.AddScoped<Logger<WorkTimeRecordRepository>>();
builder.Services.AddScoped<Logger<CalculateService>>();
builder.Services.AddScoped<Logger<ApplicationUserRepository>>();

builder.Services.AddScoped<Logger<DtoValidatorService>>();

builder.Logging.AddConfiguration(
    builder.Configuration.GetSection("Logging"));
var app = builder.Build();

var accountManagerEmail = "account.manager@gmail.com";
var accountManagerPassword = "Qwerty123";

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    if (!await db.Users.AnyAsync()) {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        
        var accountManager = new IdentityUser {
            Email = accountManagerEmail,
            EmailConfirmed = true,
            UserName = accountManagerEmail
        };

        var accountManagerResult = await userManager.CreateAsync(accountManager, accountManagerPassword);

        await roleManager.CreateAsync(new IdentityRole { Name = nameof(Roles.ACCOUNT_MANAGER) });
        await roleManager.CreateAsync(new IdentityRole { Name = nameof(Roles.PROJECT_MANAGER) });
        await roleManager.CreateAsync(new IdentityRole { Name = nameof(Roles.OPERATION_MANAGER) });
        await roleManager.CreateAsync(new IdentityRole { Name = nameof(Roles.SENIOR_DEVELOPER) });
        await roleManager.CreateAsync(new IdentityRole { Name = nameof(Roles.JUNIOR_DEVELOPER) });

        await userManager.AddToRoleAsync(accountManager, nameof(Roles.ACCOUNT_MANAGER));
    }
}

if (app.Environment.IsDevelopment()) {
    app.UseWebAssemblyDebugging();
    
    var developerExceptionPageOptions = new DeveloperExceptionPageOptions {
        SourceCodeLineCount = 5
    };
    app.UseDeveloperExceptionPage(developerExceptionPageOptions);
} else {
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthentication();

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
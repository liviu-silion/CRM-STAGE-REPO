using PlannerCRM.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration
            .GetConnectionString("ConnString")
                ?? throw new InvalidOperationException("ConnString not found!"))
);

builder.Services.AddHttpClient();

builder.Services
    .AddIdentity<Employee, EmployeeRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddUserManager<UserManager<Employee>>()
    .AddDefaultTokenProviders();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services
    .Configure<IdentityOptions>(o =>
    {
        o.User.RequireUniqueEmail = true;
        o.SignIn.RequireConfirmedEmail = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequireDigit = false;
        o.Password.RequiredLength = 8;
        o.Lockout.AllowedForNewUsers = true;
        o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        o.Lockout.MaxFailedAccessAttempts = 5;
    });

builder.Services.AddScoped<IRepository<EmployeeRepository>, EmployeeRepository>();
builder.Services.AddScoped<IRepository<WorkOrderRepository>, WorkOrderRepository>();
builder.Services.AddScoped<IRepository<ActivityRepository>, ActivityRepository>();
builder.Services.AddScoped<IRepository<WorkTimeRecordRepository>, WorkTimeRecordRepository>();
builder.Services.AddScoped<IRepository<ClientRepository>, ClientRepository>();
builder.Services.AddScoped<IRepository<CalculatorService>, CalculatorService>();
builder.Services.AddScoped<IRepository<DtoValidatorUtillity>, DtoValidatorUtillity>();

builder.Logging.AddConfiguration(
    builder.Configuration.GetSection("Logging"));
var app = builder.Build();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

if (!await context.Users.AnyAsync()) {
    await app.SeedDataAsync();
}

if (app.Environment.IsDevelopment()) {
    app.UseWebAssemblyDebugging();
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
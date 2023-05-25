using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PlannerCRM.Client;
using PlannerCRM.Client.Services;
using PlannerCRM.Client.Services.Crud;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<AuthenticationStateService>());
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<AuthenticationStateService>();
builder.Services.AddScoped<CurrentUserInfoService>();
builder.Services.AddScoped<NavigationLockService>();

builder.Services.AddScoped<OperationManagerCrudService>();
builder.Services.AddScoped<AccountManagerCrudService>();
builder.Services.AddScoped<DeveloperService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
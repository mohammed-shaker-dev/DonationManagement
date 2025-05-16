using Dashboard.BlazorApp;
using Dashboard.BlazorApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SharedKernel;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


var baseUrlConfig = new BaseUrlConfiguration();
builder.Configuration.Bind(BaseUrlConfiguration.CONFIG_NAME, baseUrlConfig);
builder.Services.AddScoped(sp => baseUrlConfig);
builder.Services.AddScoped(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var client = new HttpClient { BaseAddress = new Uri(baseUrlConfig.ApiBase) };
    client.DefaultRequestHeaders.Add("x-api-key", baseUrlConfig.apiKey);
    return client;
});
builder.Services.AddScoped<HttpService>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddOidcAuthentication(options =>
{
    // Configure your authentication provider options here.
    // For more information, see https://aka.ms/blazor-standalone-auth
    builder.Configuration.Bind("Local", options.ProviderOptions);
});
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();
//builder.Services.AddPWAServices();
await builder.Build().RunAsync();

using Donation.Web;
using Donation.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SharedKernel;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Configuration.AddJsonFile("appsettings.json");
//builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);
var baseUrlConfig = new BaseUrlConfiguration();
builder.Configuration.Bind(BaseUrlConfiguration.CONFIG_NAME, baseUrlConfig);
builder.Services.AddScoped(sp => baseUrlConfig);
 
builder.Services.AddScoped(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var client = new HttpClient { BaseAddress = new Uri(baseUrlConfig.ApiBase) };
    client.DefaultRequestHeaders.Add("x-api-key", baseUrlConfig.apiKey );
    return client;
});
builder.Services.AddScoped<HttpService>();
builder.Services.AddScoped<DonationService>();
builder.Services.AddMemoryCache();

await builder.Build().RunAsync();

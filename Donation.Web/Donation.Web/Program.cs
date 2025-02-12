using Donation.Web;
using Donation.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SharedKernel;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var baseUrlConfig = new BaseUrlConfiguration();
builder.Configuration.Bind(BaseUrlConfiguration.CONFIG_NAME, baseUrlConfig);
builder.Services.AddScoped(sp => baseUrlConfig);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseUrlConfig.ApiBase) });
builder.Services.AddScoped<HttpService>();
builder.Services.AddScoped<DonationService>();
builder.Services.AddMemoryCache();

await builder.Build().RunAsync();

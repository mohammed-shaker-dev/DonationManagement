using Dashboard.Blazor.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SharedKernel;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
var baseUrlConfig = new BaseUrlConfiguration();
builder.Configuration.Bind(BaseUrlConfiguration.CONFIG_NAME, baseUrlConfig);
builder.Services.AddScoped(sp => baseUrlConfig);
await builder.Build().RunAsync();

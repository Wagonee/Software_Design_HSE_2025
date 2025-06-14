using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Frontend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiGatewayUrl = "http://localhost:8000";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiGatewayUrl) });

await builder.Build().RunAsync();
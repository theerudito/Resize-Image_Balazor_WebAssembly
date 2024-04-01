using Blazorise;
using Blazorise.Bootstrap;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Resize_Image;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost/api/") });


await builder.Build().RunAsync();

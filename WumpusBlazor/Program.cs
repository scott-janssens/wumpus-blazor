using Blazored.Modal;
using Lea;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WumpusBlazor;
using WumpusBlazor.Helpers;
using WumpusEngine;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredModal();
builder.Services.AddSingleton<IEventAggregator, EventAggregator>();
builder.Services.AddSingleton<ISvgHelper, SvgHelper>();
builder.Services.AddTransient<IRandom, RandomHelper>();
builder.Services.AddSingleton(DifficultyOptions.Normal);
builder.Services.AddSingleton<Engine>();

await builder.Build().RunAsync();

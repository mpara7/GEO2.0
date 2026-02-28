using Blazored.LocalStorage;

using GeoProblemDatabases;

using GeoProblemDatabaseManageWebApp.Components;
using GeoProblemDatabaseManageWebApp.Models;

using MudBlazor.Services;
using GeoProblemDatabases.Databases;
DatabaseGlobalSetting.DatabaseSource = DatabaseSource.Remote;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped(sp => GeoProblemDatabase.Get());
builder.Services.AddScoped(sp => ImageDatabase.Get());
builder.Services.AddScoped(sp => InferenceInputDatabase.Get());
builder.Services.AddScoped(sp => InferenceOutputDatabase.Get());


builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMudServices();
builder.Services.AddBlazorContextMenu();

// 配置 Kestrel 服务器端口  
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001); // HTTP 端口
});

var app = builder.Build();
#if DEBUG
DatabaseGlobalSetting.DatabaseSource = DatabaseSource.Remote;
#else
DatabaseGlobalSetting.DatabaseSource = DatabaseSource.ServerLocal;
#endif
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
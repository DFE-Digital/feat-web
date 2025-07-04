using System.Text.Json.Serialization;
using feat.web.Repositories;
using GovUk.Frontend.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<RouteOptions>(option =>
{
    option.LowercaseUrls = true;
    option.LowercaseQueryStrings = true;
});

builder.Services.AddGovUkFrontend(options =>
{
    options.Rebrand = true;
    options.FrontendPackageHostingOptions = FrontendPackageHostingOptions.None;
    options.CompiledContentPath = "/js";
    options.StaticAssetsContentPath = "/assets";
});
builder.Services.AddRazorPages().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


// Setup our HTTP client
builder.Services.AddHttpClient("httpClient")
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler()
        {
            AllowAutoRedirect = true,
            UseDefaultCredentials = false
        };
    });
builder.Services.AddSingleton<HttpClientRepository>();

// Setup our session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();
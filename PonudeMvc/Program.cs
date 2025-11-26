using Application.DataParser;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.Options;
using Ponude.Config;
using Ponude.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IOfferItemApiClient, OfferItemApiClient>();

builder.Services.Configure<ApiClientSettings>(builder.Configuration.GetSection("ApiClientSettings"));

builder.Services.AddHttpClient<IOfferItemApiClient, OfferItemApiClient>((serviceProvider, client) =>
{
    var apiClientSettings = serviceProvider.GetRequiredService<IOptions<ApiClientSettings>>().Value;

    client.BaseAddress = new Uri(apiClientSettings.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", apiClientSettings.AcceptHeader);
    client.Timeout = TimeSpan.FromSeconds(90);
});

builder.Services.AddScoped<IUniversalExcelParser, UniversalExcelParser>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"),
    app => app.UseMiddleware<ApiExceptionHandlingMiddleware>());

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.Run();
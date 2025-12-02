using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Application.Validations;
using Core.Interfaces;
using Core.Processors;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Processing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PonudeWebApi.Config;
using PonudeWebApi.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection(nameof(DbSettings)));

builder.Services.AddControllers();
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextFactory<ApplicationDbContext>((serviceProvider, options) =>
{
    var dbSettings = serviceProvider.GetRequiredService<IOptions<DbSettings>>().Value;
    options.UseSqlServer(dbSettings.ConnectionString);
});

builder.Services.AddScoped<IOfferRepository, OfferRepository>();
builder.Services.AddScoped<IOfferService, OfferService>();
builder.Services.AddTransient<IValidator<OfferItemDto>, OfferItemValidator>();
builder.Services.AddTransient<IValidator<OfferDto>, OfferDtoValidator>();
builder.Services.AddScoped<IOfferImportService, OfferImportService>();
builder.Services.AddScoped<IOfferImportProcessor, OfferImportProcessor>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
using Kata.Wallet.Database;
using Kata.Wallet.Api.AutoMapper;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Kata.Wallet.Services.Interfaces;
using Kata.Wallet.Services;
using Microsoft.AspNetCore.Mvc;
using Kata.Wallet.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure application settings from appsettings.json and environment variables
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the DI container
builder.Services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("WalletDb"));

var mvcBuilder = builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        // Serialize enums as strings in API responses
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Custom response for model validation errors
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value != null && e.Value.Errors.Count > 0)
            .Select(e => new { Field = e.Key, Error = e.Value.Errors.First().ErrorMessage })
            .ToArray();

        return new BadRequestObjectResult(new
        {
            Message = "One or more errors occurred while validating the data.",
            Errors = errors
        });
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services and repositories for Dependency Injection
builder.Services.AddScoped<IWalletService, WalletService>(); // Wallet service for business logic
builder.Services.AddScoped<ITransactionService, TransactionService>(); // Transaction service for business logic
builder.Services.AddScoped<IWalletRepository, WalletRepository>(); // Wallet repository for database access
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>(); // Transaction repository for database access

// Register AutoMapper profiles for DTO-to-entity mapping
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger in development environment
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Enforce HTTPS

app.UseAuthorization(); // Middleware for handling authorization

app.MapControllers(); // Map API controllers to routes

app.Run(); // Run the application

public partial class Program { }
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Interfaces;
using ShoppingStore.Application.Services;
using ShoppingStore.Infrastructure.Data;
using ShoppingStore.Infrastructure.Repositories;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Register ShoppingStoreContext with SQL Server
builder.Services.AddDbContext<ShoppingStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IShoppingCartManager, DbShoppingCartManager>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

// Register Serilog ILogger
builder.Services.AddSingleton(Log.Logger);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

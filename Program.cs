using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Caching;
using Infrastructure.Repositories;
using StackExchange.Redis;
using Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var endPoints = builder.Configuration["RedisConfiguration:EndPoints"];
var port = builder.Configuration["RedisConfiguration:Port"];
var username = builder.Configuration["RedisConfiguration:Username"];
var password = builder.Configuration["RedisConfiguration:Password"];

var options = new ConfigurationOptions
{
    User = username,
    Password = password,
    AbortOnConnectFail = false
};

options.EndPoints.Add(endPoints!, int.Parse(port!));

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(options));

builder.Services.AddScoped<ICacheService,
    RedisCacheService>();

builder.Services.AddScoped<ProductReviewRepository>();

builder.Services.AddScoped<IProductReviewRepository>(
    provider =>
    {
        var repository =
            provider.GetRequiredService<ProductReviewRepository>();

        var cacheService =
            provider.GetRequiredService<ICacheService>();

        return new CachedProductReviewRepository(
            repository,
            cacheService);
    });

builder.Services.AddScoped<ProductReviewService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();




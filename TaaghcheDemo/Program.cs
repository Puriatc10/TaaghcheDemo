using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Net;
using TaaghcheDemo.CacheServices;
using TaaghcheDemo.Consumers;
using TaaghcheDemo.Infrastructure;
using TaaghcheDemo.Services;
using TaaghcheDemo.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});

// Add services to the container.

//builder.WebHost.UseKestrel(options =>
//{
//    options.Listen(IPAddress.Any, 443, listenOptions =>
//    {
//        var path = Path.Combine(Directory.GetCurrentDirectory(), "httpscert.cer");
//        listenOptions.UseHttps(path);
//    });
//});

builder.WebHost.UseUrls("http://*:5000");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//var op = new ConfigurationOptions
//{
//    AbortOnConnectFail = false,
//    EndPoints = { builder.Configuration.GetConnectionString("Redis") }
//};
//builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
//    ConnectionMultiplexer.Connect(op));
////////////////
//var config = new ConfigurationOptions
//{
//    EndPoints = {
//        new DnsEndPoint("localhost", 6379),
//    },
//    ResolveDns = true,
//    AbortOnConnectFail = false,
//    KeepAlive = 180,
//    ConnectTimeout = 10000,
//};
//builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
//    ConnectionMultiplexer.Connect(config));

////////////////
///
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<CacheHandler<MemoryCacheService>>();
builder.Services.AddScoped<CacheHandler<RedisCacheService>>();
builder.Services.AddScoped<MemoryCacheService>();
builder.Services.AddScoped<RedisCacheService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

builder.Services.Configure<MemoryCacheSettings>(builder.Configuration.GetSection("MemoryCacheSettings"));
builder.Services.Configure<RedisCacheSettings>(builder.Configuration.GetSection("RedisCacheSettings"));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddMemoryCache();

builder.Services.AddMassTransit(op =>
{
    op.SetKebabCaseEndpointNameFormatter();
    op.SetInMemorySagaRepositoryProvider();

    var assembly = typeof(Program).Assembly;

    op.AddConsumer<BookUpdatedConsumer>();
    //op.AddSagaStateMachine(assembly);
    op.AddSagas(assembly);
    op.AddActivities(assembly);

    op.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        var rabbitUsername = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER") ?? "guest";
        var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS") ?? "guest";
        var rabbit = builder.Configuration.GetSection("RabbitMQ");
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(rabbitUsername);
            h.Password(rabbitPass);
        });
        //cfg.ConfigureEndpoints(context);
    });
});

//builder.Services.AddStackExchangeRedisCache(redisOptions =>
//{
//    string connection = builder.Configuration.GetConnectionString("Redis");
//    redisOptions.Configuration = connection;
//});

builder.Services.AddHttpClient<ApiService>();

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

using EventBus.Abstractions;
using EventBus.RabbitMQ.Extensions;
using Log.Application.Events;
using Log.Domain.Repositories;
using Log.Infrastructure.Persistence;
using Log.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Serilog.Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "LogService")
    .WriteTo.Console()
    .WriteTo.File("logs/log-service-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Log API", Version = "v1" });
});

builder.Services.AddDbContext<LogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Log.Application.Commands.CreateLogCommand).Assembly));

builder.Services.AddScoped<ILogRepository, LogRepository>();

builder.Services.AddRabbitMQEventBus(
    builder.Configuration["RabbitMQ:HostName"] ?? "rabbitmq",
    builder.Configuration["RabbitMQ:ExchangeName"] ?? "microservice_exchange",
    builder.Configuration["RabbitMQ:QueueName"] ?? "log_queue",
    builder.Configuration["RabbitMQ:UserName"] ?? "guest",
    builder.Configuration["RabbitMQ:Password"] ?? "guest"
);

builder.Services.AddScoped<ProductCreatedEventHandler>();
builder.Services.AddScoped<ProductUpdatedEventHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LogDbContext>();
    dbContext.Database.Migrate();
}

var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<ProductCreatedEvent, ProductCreatedEventHandler>();
eventBus.Subscribe<ProductUpdatedEvent, ProductUpdatedEventHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseCors("AllowAll");

app.MapControllers();

Serilog.Log.Information("Log Service started successfully");
app.Run();


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "ApiGateway")
    .WriteTo.Console()
    .WriteTo.File("logs/api-gateway-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AuthService";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "MicroserviceApp";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
    });

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var userIdentifier = context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userIdentifier,
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            });
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests. Please try again later.",
            retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                ? retryAfter.ToString()
                : "60 seconds"
        }, cancellationToken: token);
    };
});

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

app.UseSerilogRequestLogging();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

// API Gateway üzerinden merkezi kimlik doğrulama: /api/products ve /api/logs JWT gerektirir, /api/auth açık kalır
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    if (path.StartsWith("/api/auth", StringComparison.OrdinalIgnoreCase))
    {
        await next();
        return;
    }
    if (path.StartsWith("/api/products", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/api/logs", StringComparison.OrdinalIgnoreCase))
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized", message = "Valid JWT token required" });
            return;
        }
    }
    await next();
});

app.MapReverseProxy();

Log.Information("API Gateway started successfully");

app.Run();


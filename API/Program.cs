using API.Extensions;
using API.Helpers.Errors;
using AspNetCoreRateLimit;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .CreateLogger();

// builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());

builder.Services.ConfigureRateLimiting();


// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.AddApplicationServices();
builder.Services.ConfigureApiVersioning();
builder.Services.AddJwt(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
}).AddXmlSerializerFormatters();

builder.Services.AddValidationErrors();

builder.Services.AddDbContext<StoreContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseIpRateLimiting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<StoreContext>();
        await context.Database.MigrateAsync();
        await StoreContextSeed.SeedAsync(context, loggerFactory);
        await StoreContextSeed.SeedRolesAsync(context, loggerFactory);
    }
    catch (Exception ex)
    {
        var _logger = loggerFactory.CreateLogger<Program>();
        _logger.LogError(ex, "An error ocurred during migration");
    }
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

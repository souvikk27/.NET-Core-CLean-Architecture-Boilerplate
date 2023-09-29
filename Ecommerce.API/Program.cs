using Ecommerce.API.Extensions;
using Ecommerce.LoggerService;
using Ecommerce.Presentation.ActionFilters;
using Ecommerce.Presentation.Extensions;
using Ecommerce.Service;
using Microsoft.AspNetCore.Mvc.Formatters;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.OutputFormatters.Add(new XmlSerializerOutputFormatter());
    config.ReturnHttpNotAcceptable = true;
}).AddApplicationPart(typeof(Ecommerce.Presentation.AssemblyReference).Assembly);
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.ConfigureInfrastructure();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureSqlContext(configuration);
builder.Services.ConfigureEntityContext(configuration);
builder.Services.ConfigureDbSeed();
builder.Services.ConfigureSwaggerGen();
builder.Services.ConfigureCors();
builder.Services.ConfigureLogging();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerManager>();

app.ConfigureExceptionHandler(logger);
app.ConfigureDatabaseSeed();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


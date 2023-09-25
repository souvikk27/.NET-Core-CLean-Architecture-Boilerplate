using Ecommerce.API.Extensions;
using Ecommerce.LoggerService;
using Ecommerce.Presentation.ActionFilters;
using Ecommerce.Presentation.Extensions;
using Ecommerce.Service;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.ConfigureInfrastructure();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureSqlContext(configuration);
builder.Services.ConfigureEntityContext(configuration);
builder.Services.ConfigureSwaggerGen();
builder.Services.ConfigureLogging();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerManager>();

app.ConfigureExceptionHandler(logger);
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


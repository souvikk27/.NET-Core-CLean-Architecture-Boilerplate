
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
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddMappingConfig();
builder.Services.ConfigureOpenIddict();
builder.Services.ConfigureHostedservice();
builder.Services.ConfigureQuartz();
builder.Services.ConfigureInfrastructure();
builder.Services.ConfigureIdentity();
builder.Services.TriggerOpenIdAuthentication();
builder.Services.ConfigureSqlContext(configuration);
builder.Services.ConfigureDbSeed();
builder.Services.InvokeOauthClient();
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
app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/error");

app.UseDeveloperExceptionPage();
app.UseCors("AllowAllOrigins");


app.MapControllers();

app.Run();


using APEX.Business.Services;
using APEX.Core.Interfaces;
using APEX.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Logging seviyesini ayarla
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Logo ayarlarını al ve null kontrolü yap
var logoConnectionString = builder.Configuration["LogoSettings:ConnectionString"];
var logoFirmaNo = builder.Configuration["LogoSettings:FirmaNo"];

if (string.IsNullOrEmpty(logoConnectionString))
    throw new InvalidOperationException("LogoSettings:ConnectionString konfigürasyonu bulunamadı.");

if (string.IsNullOrEmpty(logoFirmaNo))
    throw new InvalidOperationException("LogoSettings:FirmaNo konfigürasyonu bulunamadı.");

// Services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddScoped<ILogoRepository>(provider =>
    new LogoRepository(logoConnectionString, logoFirmaNo));
builder.Services.AddScoped<ILogoErpService, LogoErpService>();
builder.Services.AddScoped<SayimService>();

var app = builder.Build();

// Request logging middleware ekle
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("=== HTTP Request: {Method} {Path} ===", context.Request.Method, context.Request.Path);
    
    if (context.Request.ContentType?.Contains("application/json") == true)
    {
        context.Request.EnableBuffering();
        var body = await new StreamReader(context.Request.Body, System.Text.Encoding.UTF8).ReadToEndAsync();
        context.Request.Body.Position = 0;
        logger.LogInformation("Request Body: {Body}", body);
    }
    
    await next();
});

// Swagger (test için)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

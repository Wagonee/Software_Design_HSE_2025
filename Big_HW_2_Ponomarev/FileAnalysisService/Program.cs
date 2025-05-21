using FileAnalysisService.Data; 
using FileAnalysisService.Clients;
using FileAnalysisService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AppDatabase");
_ = connectionString ?? throw new InvalidOperationException("Connection string 'AppDatabase' not found.");
builder.Services.AddDbContext<FileAnalysisDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHttpClient<IFileStoringServiceClient, FileStoringServiceClient>(client =>
{
    string? fileStoringServiceUrl = builder.Configuration["ServiceUrls:FileStoringService"];
    _ = fileStoringServiceUrl ?? throw new InvalidOperationException("FileStoringService URL (ServiceUrls:FileStoringService) is not configured.");

    if (!fileStoringServiceUrl.EndsWith("/"))
    {
        fileStoringServiceUrl += "/";
    }
    client.BaseAddress = new Uri(fileStoringServiceUrl);
});

builder.Services.AddHttpClient<IWordCloudApiClient, WordCloudApiClient>(client => 
{
    string? wordCloudApiUrl = builder.Configuration["WordCloudApi:BaseUrl"];
    if (!string.IsNullOrEmpty(wordCloudApiUrl))
    {
        if (!wordCloudApiUrl.EndsWith("/"))
        {
            wordCloudApiUrl += "/";
        }
        client.BaseAddress = new Uri(wordCloudApiUrl);
    }
});


builder.Services.AddScoped<IFileAnalysisOrchestratorService, FileAnalysisOrchestratorService>(); 
builder.Services.AddScoped<ITextStatisticsService, TextStatisticsService>();
builder.Services.AddScoped<IPlagiarismDetectionService, PlagiarismDetectionService>(); 
builder.Services.AddScoped<IWordCloudGenerationService, WordCloudGenerationService>(); 
builder.Services.Configure<FileAnalysisService.Services.FileStorageSettings>( 
    builder.Configuration.GetSection("FileStorageForAnalysis"));
builder.Services.AddScoped<FileAnalysisService.Services.IFileStorageProvider, FileAnalysisService.Services.LocalFileStorageProvider>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "FileAnalysisOrchestratorService API",
        Version = "v1"
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileAnalysisOrchestratorService API V1"));

    
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<FileAnalysisDbContext>();
            dbContext.Database.Migrate();
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("FileAnalysisDB migrations applied successfully.");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the FileAnalysisDB.");
        }
    }
    
}

// app.UseHttpsRedirection(); 
app.UseAuthorization();
app.MapControllers();

app.Run();

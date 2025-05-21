using FileStoringService.Data;
using FileStoringService.Services;
using Microsoft.EntityFrameworkCore; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FileStorageSettings>(
    builder.Configuration.GetSection("FileStorage"));

var connectionString = builder.Configuration.GetConnectionString("AppDatabase");
builder.Services.AddDbContext<FileStoringDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IFileStorageProvider, LocalFileStorageProvider>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileStoringService API"));
}

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<FileStoringDbContext>();
        try
        {
            dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}
app.UseHttpsRedirection(); 
app.UseAuthorization();
app.MapControllers();
app.Run();

public class FileStorageSettings
{
    public string BasePath { get; set; }
}
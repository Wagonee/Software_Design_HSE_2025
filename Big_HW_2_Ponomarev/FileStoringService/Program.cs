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

app.UseHttpsRedirection(); 
app.UseAuthorization();
app.MapControllers();
app.Run();

public class FileStorageSettings
{
    public string BasePath { get; set; }
}
using Microsoft.EntityFrameworkCore;
using OrdersService.Data;

namespace OrdersService.Extensions;

public static class MigrationExtensions
{
    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        const int maxAttempts = 10;
        var currentAttempt = 0;
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>(); 

        while (currentAttempt < maxAttempts)
        {
            try
            {
                dbContext.Database.Migrate();
                logger.LogInformation("Миграции успешно применены.");
                break;  
            }
            catch (Exception ex)
            {
                currentAttempt++;
                logger.LogError(ex, "Попытка {Attempt} из {MaxAttempts} применить миграции не удалась.", currentAttempt, maxAttempts);
                Thread.Sleep(3000);  
            }
        }
        
        return app;
    }
}
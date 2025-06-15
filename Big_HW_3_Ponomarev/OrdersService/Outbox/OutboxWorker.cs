using System.Text;
using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace OrdersService.Outbox;

public class OutboxWorker(IServiceScopeFactory scopeFactory, ILogger<OutboxWorker> logger)
    : BackgroundService
{
    private IConnection? _connection;
    private IModel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
        
            if (_channel == null || _channel.IsClosed)
            {
                ConnectToRabbitMq();
            }

            if (_channel == null)
            {
                logger.LogWarning("Канал RabbitMQ недоступен. Повторная попытка через 5 секунд.");
                await Task.Delay(5000, stoppingToken);
                continue;
            }

            await ProcessOutboxMessages(stoppingToken);

            await Task.Delay(1000, stoppingToken); 
        }
    }

    private void ConnectToRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RabbitMQ__Host") ?? "rabbitmq",
            Port = int.Parse(Environment.GetEnvironmentVariable("RabbitMQ__Port") ?? "5672"),
            UserName = Environment.GetEnvironmentVariable("RabbitMQ__Username") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RabbitMQ__Password") ?? "guest",
            DispatchConsumersAsync = true
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("order-created", durable: true, exclusive: false, autoDelete: false);
            logger.LogInformation("Успешное подключение к RabbitMQ.");
        }
        catch (BrokerUnreachableException ex)
        {
            logger.LogError(ex, "Не удалось подключиться к RabbitMQ. Будет выполнена повторная попытка.");
            
            _channel?.Dispose();
            _connection?.Dispose();
            _channel = null;
            _connection = null;
        }
    }
    
    private async Task ProcessOutboxMessages(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

        var messages = await db.OutboxMessages
            .Where(x => !x.Processed)
            .OrderBy(x => x.OccurredOn)
            .Take(10)
            .ToListAsync(stoppingToken);

        foreach (var message in messages)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message.Payload);
                _channel.BasicPublish(
                    exchange: "",
                    routingKey: "order-created",
                    basicProperties: null,
                    body: body
                );

                message.Processed = true;
                logger.LogInformation("Сообщение {MessageId} успешно отправлено.", message.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при публикации сообщения {MessageId}", message.Id);
            }
        }

        if (messages.Count > 0)
        {
            await db.SaveChangesAsync(stoppingToken);
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
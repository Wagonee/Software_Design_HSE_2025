using System.Text;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace PaymentsService.Outbox;

public class OutboxWorker(IServiceScopeFactory scopeFactory, ILogger<OutboxWorker> logger)
    : BackgroundService
{
    private IConnection? _connection;
    private IModel? _channel;
    private const string ExchangeName = "payment-results-exchange";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_channel == null || _channel.IsClosed)
            {
                ConnectToRabbitMq();
            }

            if (_channel != null)
            {
                await ProcessOutboxMessages(stoppingToken);
            }
            else
            {
                logger.LogWarning("Канал RabbitMQ недоступен для Outbox. Повторная попытка через 1 секунду.");
            }
            
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
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
            logger.LogInformation("Outbox Worker успешно подключился к RabbitMQ.");
        }
        catch (BrokerUnreachableException ex)
        {
            logger.LogError(ex, "Не удалось подключиться к RabbitMQ для Outbox Worker.");
            _channel = null;
        }
    }
    
    private async Task ProcessOutboxMessages(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();

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
                    exchange: ExchangeName,
                    routingKey: "", 
                    basicProperties: null,
                    body: body
                );

                message.Processed = true;
                logger.LogInformation("Сообщение {MessageId} из Outbox успешно отправлено.", message.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при публикации сообщения {MessageId} из Outbox.", message.Id);
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
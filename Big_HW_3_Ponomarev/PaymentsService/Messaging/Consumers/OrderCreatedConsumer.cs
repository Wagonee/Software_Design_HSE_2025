using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Inbox;
using PaymentsService.Messaging.Events;
using PaymentsService.Outbox;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;  

namespace PaymentsService.Messaging.Consumers;

public class OrderCreatedConsumer(IServiceScopeFactory scopeFactory, ILogger<OrderCreatedConsumer> logger)
    : BackgroundService
{
    private IConnection? _connection;
    private IModel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_channel == null || _channel.IsClosed)
                {
                    logger.LogInformation("Попытка подключения OrderCreatedConsumer к RabbitMQ...");
                    
                    var factory = new ConnectionFactory
                    {
                        HostName = Environment.GetEnvironmentVariable("RabbitMQ__Host") ?? "rabbitmq",
                        Port = int.Parse(Environment.GetEnvironmentVariable("RabbitMQ__Port") ?? "5672"),
                        UserName = Environment.GetEnvironmentVariable("RabbitMQ__Username") ?? "guest",
                        Password = Environment.GetEnvironmentVariable("RabbitMQ__Password") ?? "guest",
                        DispatchConsumersAsync = true
                    };

                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    
                    _channel.QueueDeclare(queue: "order-created", durable: true, exclusive: false, autoDelete: false);

                    var consumer = new AsyncEventingBasicConsumer(_channel);
                    consumer.Received += OnMessageReceived;

                    _channel.BasicConsume(queue: "order-created", autoAck: false, consumer: consumer);
                    logger.LogInformation("OrderCreatedConsumer успешно подключен и слушает очередь 'order-created'.");
                }
            }
            catch (BrokerUnreachableException ex)
            {
                logger.LogError(ex, "OrderCreatedConsumer не смог подключиться к RabbitMQ. Повторная попытка через 5 секунд.");
                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Произошла непредвиденная ошибка в OrderCreatedConsumer. Попытка восстановить работу через 10 секунд.");
                await Task.Delay(10000, stoppingToken);
            }
            
            await Task.Delay(1000, stoppingToken);
        }
    }
    
    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var messageJson = Encoding.UTF8.GetString(body);
        
        var @event = JsonSerializer.Deserialize<OrderCreatedEvent>(messageJson);
        if (@event is null) return;
        
        var messageEnvelope = JsonSerializer.Deserialize<JsonElement>(messageJson);
        if (!messageEnvelope.TryGetProperty("MessageId", out var idElement) || !idElement.TryGetGuid(out var messageId))
        {
            logger.LogError("Не удалось извлечь MessageId из сообщения.");
            _channel.BasicNack(eventArgs.DeliveryTag, false, false);
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
        
        if (await db.InboxMessages.AnyAsync(m => m.Id == messageId))
        {
            _channel.BasicAck(eventArgs.DeliveryTag, false);
            logger.LogWarning("Сообщение {MessageId} уже было обработано.", messageId);
            return;
        }

        var account = await db.Accounts.FirstOrDefaultAsync(a => a.UserId == @event.UserId);
        
        string paymentStatus;
        string eventType;

        if (account == null)
        {
            paymentStatus = "failed_no_account";
            eventType = "OrderPaymentFailed";
        }
        else if (account.Balance < @event.Amount)
        {
            paymentStatus = "failed_insufficient_funds";
            eventType = "OrderPaymentFailed";
        }
        else
        {
            account.Balance -= @event.Amount;
            paymentStatus = "success";
            eventType = "OrderPaymentSucceeded";
        }
        
        var paymentResultEvent = new
        {
            OrderId = @event.OrderId,
            UserId = @event.UserId,
            Status = paymentStatus,
            Balance = account?.Balance ?? 0
        };
        
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = eventType,
            Payload = JsonSerializer.Serialize(paymentResultEvent),
            OccurredOn = DateTime.UtcNow
        };

        db.OutboxMessages.Add(outboxMessage);
        db.InboxMessages.Add(new InboxMessage { Id = messageId, ProcessedAt = DateTime.UtcNow });

        await db.SaveChangesAsync();
        
        _channel.BasicAck(eventArgs.DeliveryTag, false);
        logger.LogInformation("Платеж для заказа {OrderId} обработан со статусом {Status}", @event.OrderId, paymentStatus);
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
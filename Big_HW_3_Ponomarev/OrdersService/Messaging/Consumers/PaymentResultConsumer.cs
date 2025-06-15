using System.Text;
using System.Text.Json;
using OrdersService.Data;
using OrdersService.Data.Entities;
using OrdersService.Messaging.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrdersService.Messaging.Consumers;

public class PaymentResultConsumer(IServiceScopeFactory scopeFactory, ILogger<PaymentResultConsumer> logger, SocketManager socketManager)
    : BackgroundService
{
    private IModel? _channel;
    private const string ExchangeName = "payment-results-exchange";
    private const string QueueName = "orders-service-payment-results";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_channel == null || _channel.IsClosed)
                {
                     _channel = ConnectToRabbitMq();
                }

                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка в PaymentResultConsumer. Повторная попытка через 5 секунд.");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private IModel ConnectToRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RabbitMQ__Host") ?? "rabbitmq",
            Port = int.Parse(Environment.GetEnvironmentVariable("RabbitMQ__Port") ?? "5672"),
            UserName = Environment.GetEnvironmentVariable("RabbitMQ__Username") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RabbitMQ__Password") ?? "guest",
            DispatchConsumersAsync = true
        };
        
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
        channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: "");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += OnMessageReceived;
        
        channel.BasicQos(0, 1, false);
        channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        
        logger.LogInformation("PaymentResultConsumer успешно подключен и слушает очередь '{QueueName}'.", QueueName);
        return channel;
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var @event = JsonSerializer.Deserialize<PaymentResultEvent>(message);

            if (@event == null)
            {
                logger.LogWarning("Получено пустое или некорректное сообщение о результате платежа.");
                _channel.BasicAck(eventArgs.DeliveryTag, false);
                return;
            }

            logger.LogInformation("Получен результат оплаты для заказа {OrderId} со статусом {Status}", @event.OrderId, @event.Status);

            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var order = await dbContext.Orders.FindAsync(@event.OrderId);
            if (order == null)
            {
                logger.LogWarning("Заказ с ID {OrderId} не найден.", @event.OrderId);
                _channel.BasicAck(eventArgs.DeliveryTag, false);
                return;
            }

            order.Status = @event.Status == "success" ? OrderStatus.Paid : OrderStatus.Failed;
            await dbContext.SaveChangesAsync();
            
            logger.LogInformation("Статус заказа {OrderId} обновлен на {Status}", order.Id, order.Status);
            var payload = new
            {
                Message = $"Статус вашего заказа {order.Id} изменен на '{order.Status}'.",
                Balance = @event.Balance,
                Status = order.Status.ToString()
            };
            var json = JsonSerializer.Serialize(payload);
            var sent = await socketManager.SendMessage(order.UserId, json);

            if (!sent)
            {
                logger.LogWarning("WebSocket для пользователя {UserId} недоступен. Сообщение будет повторено.", order.UserId);
                _channel.BasicNack(eventArgs.DeliveryTag, false, true);
                return;
            }

            _channel.BasicAck(eventArgs.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обработке сообщения о результате платежа.");
            _channel.BasicNack(eventArgs.DeliveryTag, false, true);
        }
    }
}
using System.Net.WebSockets;
using Microsoft.EntityFrameworkCore;
using OrdersService;
using OrdersService.Data;
using OrdersService.Extensions;
using OrdersService.Messaging.Consumers;
using OrdersService.Outbox;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<SocketManager>();
builder.Services.AddDbContext<OrdersDbContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("OrdersDb"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(3), null)
    ));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<OutboxWorker>();
builder.Services.AddHostedService<PaymentResultConsumer>();
builder.Services.AddSwaggerGen();

var app = builder.Build();



app.UseWebSockets();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var socketManager = app.Services.GetRequiredService<SocketManager>();
            var userId = context.Request.Query["userId"];

            if (!string.IsNullOrEmpty(userId))
            {
                socketManager.AddSocket(userId, webSocket);
                await HandleWebSocket(webSocket);
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else
    {
        await next();
    }
});

async Task HandleWebSocket(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];
    var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    while (!receiveResult.CloseStatus.HasValue)
    {
        receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
}


app.ApplyMigrations();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
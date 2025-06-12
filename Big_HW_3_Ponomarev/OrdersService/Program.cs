using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Extensions;
using OrdersService.Messaging.Consumers;
using OrdersService.Outbox;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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

app.ApplyMigrations();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
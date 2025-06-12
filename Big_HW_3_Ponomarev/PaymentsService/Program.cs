using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Messaging.Consumers;
using PaymentsService.Outbox;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<PaymentsDbContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("PaymentsDb"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(3), null)
    ));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<OrderCreatedConsumer>();
builder.Services.AddHostedService<OutboxWorker>();
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
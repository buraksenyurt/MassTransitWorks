using CommonLib;
using MassTransit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:dd.MM.yyyy-HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("scothtiger");
            h.Password("123456");
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/publish-score", async (NewGameScore payload, IBus bus) =>
{
    Log.Information("Publishing new game score for PlayerID {PlayerID} with score {}", payload.PlayerID, payload.Point);
    await bus.Publish(payload);
    Log.Information("Score published successfully!");

    return Results.Ok("Score published successfully!");
})
.WithName("PublishGamersScore")
.WithOpenApi();

app.Run();
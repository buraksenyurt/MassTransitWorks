using CommonLib;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

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

app.MapPost("/publish-score", async (NewGameScore newGameScore, IBus bus) =>
{
    await bus.Publish(newGameScore);
    return Results.Ok("Score published successfully!");
})
.WithName("PublishGamersScore")
.WithOpenApi();

app.Run();
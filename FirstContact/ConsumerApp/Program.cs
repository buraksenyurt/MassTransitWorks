using ConsumerApp;
using MassTransit;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:dd.MM.yyyy-HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var yellowBus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost", "/", h =>
    {
        h.Username("scothtiger");
        h.Password("123456");
    });

    cfg.ReceiveEndpoint("msg_gamers_score", e =>
    {
        e.Consumer<ConsumerService>();
    });
});

await yellowBus.StartAsync();

try
{
    Log.Information("I am listening to event messages.\nPress any key to exit the program.");
    await Task.Run(Console.ReadKey);
}
catch (Exception exp)
{
    Log.Error(exp, "An error occurred while listening to messages.");
}
finally
{
    await yellowBus.StopAsync();
    Log.Information("Consumer application stopped.");
}
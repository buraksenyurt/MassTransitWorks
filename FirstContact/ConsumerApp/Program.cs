using ConsumerApp;
using MassTransit;

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
    Console.WriteLine("I am listening the event messages.\nPress any key to exit program.");
    await Task.Run(Console.ReadKey);
}
catch (Exception exp)
{
    Console.WriteLine(exp.Message);
}
finally
{
    await yellowBus.StopAsync();
}
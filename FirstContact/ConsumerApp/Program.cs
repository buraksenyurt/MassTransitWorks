using ConsumerApp;
using MassTransit;

var yellowBus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost", "/", h =>
    {
        h.Username("scothtiger");
        h.Password("123456");
    });

    cfg.ReceiveEndpoint("gamers_score_line", e =>
    {
        e.Consumer<ConsumerService>();
    });
});

await yellowBus.StartAsync();

try
{
    Console.WriteLine("Press any key to exit");
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
using CommonLib;
using MassTransit;

var yellowBus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost", "/", h =>
    {
        h.Username("scothtiger");
        h.Password("123456");
    });
});

await yellowBus.StartAsync();

try
{
    Console.WriteLine("Press any key to send message");
    Console.ReadLine();

    var message = new NewGameScore
    {
        GameID = 1,
        PlayerID = 902,
        Nickname = "RougeOne-1903",
        Point = 8.5
    };

    await yellowBus.Publish(message);
    await Task.Delay(500);

    Console.WriteLine("Message published.");
}
catch (Exception exp)
{
    Console.WriteLine(exp.Message);
}
finally
{
    await yellowBus.StopAsync();
}

using CommonLib;
using MassTransit;

namespace ConsumerApp;

public class ConsumerService : IConsumer<NewGameScore>
{
    public Task Consume(ConsumeContext<NewGameScore> context)
    {
        Console.WriteLine($"Received new game score info. Message Id : {context.MessageId}");
        Console.WriteLine($"{context.Message.Nickname},{context.Message.Nickname} {context.Message.Point}");
        return Task.CompletedTask;
    }
}
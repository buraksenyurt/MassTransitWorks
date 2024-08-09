using CommonLib;
using MassTransit;
using Serilog;

namespace ConsumerApp;

public class ConsumerService : IConsumer<NewGameScore>
{
    public Task Consume(ConsumeContext<NewGameScore> context)
    {
        Log.Information("Received new game score info. Message Id : {MessageId}", context.MessageId);
        Log.Information("Player is {Nickname} and the current scores are {Point}", context.Message.Nickname, context.Message.Point);
        return Task.CompletedTask;
    }
}
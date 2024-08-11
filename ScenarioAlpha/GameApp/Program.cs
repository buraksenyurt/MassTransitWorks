using System.Text.Json;
using CommonLib;
using MassTransit;
using Serilog;

namespace GameApp;

class Program
{
    static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo
            .Console(outputTemplate: "[{Timestamp:dd.MM.yyyy-HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Log.Information("Game App starting...");

        try
        {
            Game game = new()
            {
                GameID = 16,
                Name = "Geography Quiz"
            };
            Player player = new()
            {
                PlayerID = 23
            };
            Log.Information("Game created with ID: {GameID} and Name: {GameName}", game.GameID, game.Name);

            Console.WriteLine("Welcome to the world's most popular geography quiz!");
            Console.WriteLine("What is your name?");
            player.Nickname = Console.ReadLine();
            Console.WriteLine($"Welcome, {player.Nickname}");

            var questions = LoadQuestions(Path.Combine(Environment.CurrentDirectory, "Questions.json"));
            if (!questions.Any())
            {
                Log.Error("No questions loaded. Exiting the game.");
                Console.WriteLine("Questions could not be loaded. Please contact the game developer.");
                return;
            }

            player.GameScore = RunGame(questions);

            var newGameScore = new NewGameScore(
                game.GameID,
                player.PlayerID,
                player.Nickname ?? "Anonymous",
                player.GameScore,
                DateTime.Now
            );

            Console.WriteLine($"The quiz is over, dear '{player.Nickname}'");
            Console.WriteLine($"You scored a total of {newGameScore.Point} points.");
            Console.WriteLine("See you next time.");

            await PublishMessage(newGameScore);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unhandled exception occurred.");
        }
        finally
        {
            Log.Information("Game App ending...");
            Log.CloseAndFlush();
        }
    }

    static IEnumerable<Question> LoadQuestions(string filePath)
    {
        var data = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var questions = JsonSerializer.Deserialize<IEnumerable<Question>>(data, options);
        Log.Information("{Count} questions loaded.", questions?.Count());
        return questions ?? [];
    }

    static double RunGame(IEnumerable<Question> questions)
    {
        double score = 0;
        int counter = 1;

        var shuffledQuestions = questions.OrderBy(q => Guid.NewGuid()).ToList();

        foreach (Question q in shuffledQuestions)
        {
            Console.WriteLine($"{counter} - {q.Text}");
            counter++;
            Console.WriteLine("What is your answer?");
            string answer = Console.ReadLine() ?? "no answer";
            if (q.Answer.ToLower().Equals(answer.Trim().ToLower()))
            {
                Console.WriteLine($"Congratulations, you got it right. You earned {q.Point} points.");
                score += q.Point;
            }
            else
            {
                Console.WriteLine($"Sorry, that's incorrect. The correct answer was '{q.Answer}'.");
                score -= 1;
            }
            Console.WriteLine();
        }
        return score;
    }

    static async Task PublishMessage(NewGameScore newGameScore)
    {
        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("rabbitmq://localhost", h =>
            {
                h.Username("scothtiger");
                h.Password("123456");
            });
        });

        await busControl.StartAsync();

        try
        {
            await busControl.Publish(newGameScore);
            Log.Information("NewGameScore message sent to RabbitMQ. GameID: {GameID}, PlayerID: {PlayerID}, Points: {Points}", newGameScore.GameID, newGameScore.PlayerID, newGameScore.Point);
        }
        finally
        {
            await busControl.StopAsync();
        }
    }
}

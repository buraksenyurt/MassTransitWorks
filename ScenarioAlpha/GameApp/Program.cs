using System.Text.Json;
using CommonLib;
using MassTransit;

namespace GameApp;

class Program
{
    static async Task Main()
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
        Console.WriteLine("Welcome to the world's most popular geography quiz!");
        Console.WriteLine("What is your name?");
        player.Nickname = Console.ReadLine();
        Console.WriteLine($"Welcome, {player.Nickname}\n");
        var questions = LoadQuestions(Path.Combine(Environment.CurrentDirectory, "Questions.json"));
        if (!questions.Any())
        {
            Console.WriteLine("Questions could not be loaded. Please contact the game developer.");
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

    static IEnumerable<Question> LoadQuestions(string filePath)
    {
        var data = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var questions = JsonSerializer.Deserialize<IEnumerable<Question>>(data, options);
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
            Console.WriteLine("NewGameScore message sent to RabbitMQ.");
        }
        finally
        {
            await busControl.StopAsync();
        }
    }
}
using System.Text.Json;
using CommonLib;
using System.Linq;

namespace GameApp;

class Program
{
    static void Main()
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
        Console.WriteLine($"Welcome, {player.Nickname}");
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
}
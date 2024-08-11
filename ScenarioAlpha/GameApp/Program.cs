using System.Text.Json;
using CommonLib;

namespace GameApp;

class Program
{
    static void Main()
    {
        Game game = new()
        {
            GameID = 16,
            Name = "Coğrafya Oyunu"
        };
        Player player = new()
        {
            PlayerID = 23
        };
        Console.WriteLine("Dünyanın en popüler coğrafya bilgi yarışmasına hoşgeldin");
        Console.WriteLine("Hangi isimle yarışmaya katılıyorsun?");
        player.Nickname = Console.ReadLine();
        Console.WriteLine($"Hoşgeldin {player.Nickname}");
        var questions = LoadQuestions(Path.Combine(Environment.CurrentDirectory, "Questions.json"));
        if (!questions.Any())
        {
            Console.WriteLine("Sorular yüklenemedi. Lütfen oyun geliştirici ile irtibata geçin.");
        }
        player.GameScore = RunGame(questions);

        var newGameScore = new NewGameScore(
            game.GameID
            , player.PlayerID
            , player.Nickname ?? "Anonymous"
            , player.GameScore
            , DateTime.Now
        );

        Console.WriteLine($"Yarışma sona erdi sevgili '{player.Nickname}'");
        Console.WriteLine($"Toplam {newGameScore.Point} puan kazandın.");
        Console.WriteLine("Tekrardan görüşmek üzere.");
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
        foreach (Question q in questions)
        {
            Console.WriteLine($"{counter} - {q.Text}");
            counter++;
            Console.WriteLine("Cevabın nedir ?");
            string answer = Console.ReadLine() ?? "cevapsız";
            if (q.Answer.ToLower().Equals(answer.Trim().ToLower()))
            {
                Console.WriteLine($"Tebrikler, bildin. Tam {q.Point} puan kazandın.");
                score += q.Point;
            }
            else
            {
                Console.WriteLine($"Maalesef bilemedin. Doğru cevap '{q.Answer}' olacaktı.");
                score -= 1;
            }
        }
        return score;
    }
}

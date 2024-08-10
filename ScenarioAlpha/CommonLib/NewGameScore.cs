namespace CommonLib;

public class NewGameScore
{
    public int PlayerID { get; set; }
    public int GameID { get; set; }
    public string Nickname { get; set; } = "Anonymous";
    public double Point { get; set; }
    public DateTime RecordTime { get; set; } = DateTime.Now;
}

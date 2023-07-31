namespace RockPaperScissors.Model.Entity;

public class RoundEntity
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int Number { get; set; }
    public int? FirstPlayerChoice { get; set; }
    public int? SecondPlayerChoice { get; set; }
}
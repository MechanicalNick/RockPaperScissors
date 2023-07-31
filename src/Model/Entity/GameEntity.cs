namespace RockPaperScissors.Model.Entity;

public class GameEntity
{
    public int Id { get; set; }
    public int FirstPlayerId { get; set; }
    public int? SecondPlayerId { get; set; }
    public int RoundsCount { get; set; }
    public bool WithBot { get; set; }
}
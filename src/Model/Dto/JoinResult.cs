namespace RockPaperScissors.Model.Dto;

public enum JoinResult
{
    Ok,
    PlayerExists, // firstPlayer == secondPlayer
    GameIsFull,   // secondPlayer != null
    GameNotFound
}
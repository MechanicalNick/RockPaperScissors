namespace RockPaperScissors.Model.Dto;

public record StatisticResponse(List<StatisticItem> Result, int FirstWinsCount, 
    int SecondWinsCount, int DrawCount);
using RockPaperScissors.Model.Dto;

namespace RockPaperScissors.Service;

public static class TurnSolver
{
    public static TurnResponse Solve(Turn first, Turn second)
    {
        if (first == second)
            return TurnResponse.Draw;

        if (first == Turn.Rock && second == Turn.Scissors)
            return TurnResponse.FirstPlayerWins;
        if (first == Turn.Paper && second == Turn.Rock)
            return TurnResponse.FirstPlayerWins;
        if (first == Turn.Scissors && second == Turn.Paper)
            return TurnResponse.FirstPlayerWins;

        return TurnResponse.SecondPlayerWins;
    }

    public static int? SolveNullable(int? first, int? second)
    {
        return (first != null && second != null) ? (int) Solve((Turn) first, (Turn) second) : null;
    }
}
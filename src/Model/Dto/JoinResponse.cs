namespace RockPaperScissors.Model.Dto;

public record JoinResponse(int PlayerId)
{
    public JoinResponse(): this(Int32.MinValue){}
}
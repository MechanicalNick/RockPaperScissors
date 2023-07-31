using RockPaperScissors.Model.Entity;

namespace RockPaperScissors.Repository.Interface;

public interface IPlayerRepository
{
    Task<PlayerEntity?> FindPlayerByNameAsync(string playerName, CancellationToken ct);
    Task<PlayerEntity> GetBotAsync(CancellationToken ct);
    Task<PlayerEntity> SavePlayerAsync(string playerName, CancellationToken ct);
}
using System.Data;
using System.Data.Common;

namespace RockPaperScissors.Infrastructure;

public interface IConnectionFactory
{
    DbConnection GetConnection();
}
using RockPaperScissors.Model.Dto;

namespace RockPaperScissors.Infrastructure;

public static class EnumHelper
{
    public static string ToReadableValue<T>(int? value) where T: Enum
    {
        return value.HasValue ? ((T)(object)value.Value).ToString() : "-";
    }
}
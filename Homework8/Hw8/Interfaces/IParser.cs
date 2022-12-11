using Hw8.DTO;

namespace Hw8.Interfaces;

public interface IParser
{
    string TryParseValues(string? firstValue, string? operation, string? secondValue, out Values result);
}
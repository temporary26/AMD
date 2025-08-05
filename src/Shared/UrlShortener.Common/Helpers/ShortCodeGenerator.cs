using Microsoft.Extensions.Logging;

namespace UrlShortener.Common.Helpers;

public static class ShortCodeGenerator
{
    private const string Alphabet = "23456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz";
    private const int Base = 57; // Length of alphabet
    private static readonly Random Random = new();

    public static string GenerateShortCode(int length = 7)
    {
        var shortCode = new char[length];
        for (int i = 0; i < length; i++)
        {
            shortCode[i] = Alphabet[Random.Next(Base)];
        }
        return new string(shortCode);
    }

    public static string GenerateFromId(long id, int minLength = 5)
    {
        if (id == 0) return Alphabet[0].ToString();

        var result = "";
        while (id > 0)
        {
            result = Alphabet[(int)(id % Base)] + result;
            id /= Base;
        }

        // Pad with random characters to meet minimum length
        while (result.Length < minLength)
        {
            result = Alphabet[Random.Next(Base)] + result;
        }

        return result;
    }
}

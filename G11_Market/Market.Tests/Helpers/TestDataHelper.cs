namespace Market.Tests.Helpers;

public static class TestDataHelper
{
    public static string GenerateName(string prefix)
    {
        return $"{prefix}_{Guid.NewGuid():N}";
    }

    public static string GenerateCode(int length = 3)
    {
        var random = Random.Shared;
        var chars = new char[length];

        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = (char)random.Next('A', 'Z' + 1);
        }

        return new string(chars);
    }
}
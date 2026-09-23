namespace Market.Extensions;

public static class StringHelper
{
    public static string ToPlural(this string singular)
    {
        if (string.IsNullOrEmpty(singular))
            return singular;

        // TODO: განაზოგადე
        // თუ სახელი უკვე მთავრდება "Details"-ით,
        // მას plural suffix აღარ უნდა დაემატოს.
        if (singular.EndsWith("Details", StringComparison.OrdinalIgnoreCase))
        {
            return singular;
        }


        if (singular.EndsWith("y", StringComparison.OrdinalIgnoreCase)
            && !IsVowel(singular[^2]))
        {
            return singular[..^1] + "ies";
        }

        if (singular.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            singular.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            singular.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
            singular.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            singular.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
        {
            return singular + "es";
        }

        return singular + "s";
    }

    private static bool IsVowel(char c) => "aeiouAEIOU".IndexOf(c) >= 0;
}
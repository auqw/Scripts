using Newtonsoft.Json.Linq;

/// <summary>Small helpers shared by the developer generator/collector scripts.</summary>
public static class GeneratorSupportUtils
{
    /// <summary>Finds the index of the brace that closes the one at <paramref name="openingBrace"/>.</summary>
    public static int FindClosingBrace(string source, int openingBrace)
    {
        int depth = 0;
        for (int index = openingBrace; index < source.Length; index++)
        {
            if (source[index] == '{') depth++;
            else if (source[index] == '}' && --depth == 0) return index;
        }
        return -1;
    }

    /// <summary>Reads the first of <paramref name="names"/> present on <paramref name="value"/> as an int.</summary>
    public static int ReadInt(JToken value, params string[] names)
    {
        foreach (string name in names)
            if (int.TryParse(value[name]?.ToString(), out int parsed))
                return parsed;
        return 0;
    }
}
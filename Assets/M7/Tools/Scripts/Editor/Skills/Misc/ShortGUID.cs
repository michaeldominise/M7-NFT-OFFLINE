using System;

public static class ShortGUID
{
    /// <summary>
    /// GUID extension to shorten it so that it can be used in names
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static string Shorten(this Guid guid)
    {
        string encoded = Convert.ToBase64String(guid.ToByteArray());
        encoded = encoded
            .Replace("/", "_")
            .Replace("+", "-");

        return encoded.Substring(0, 22);
    }
}

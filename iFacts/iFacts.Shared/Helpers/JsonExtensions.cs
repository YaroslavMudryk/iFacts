using System.Text.Json;

namespace iFacts.Shared.Helpers;

public static class JsonExtensions
{
    private static JsonSerializerOptions _default = Settings.EntityFramework;
    public static string ToJson(this object obj)
    {
        return JsonSerializer.Serialize(obj, _default);
    }

    public static T FromJson<T>(this string content)
    {
        return JsonSerializer.Deserialize<T>(content, _default);
    }
}

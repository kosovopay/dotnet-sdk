using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KosovoPay.Enums;

/// <summary>
/// A generic JSON converter for string-backed enums that have an <c>Unknown</c> member.
/// Any unrecognised wire value deserialises to <c>Unknown</c> rather than throwing.
/// </summary>
/// <typeparam name="TEnum">The enum type. Must have a member named <c>Unknown</c>.</typeparam>
public sealed class ForwardCompatibleEnumConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private static readonly Dictionary<string, TEnum> _byValue;
    private static readonly TEnum _unknown;
    private static readonly Dictionary<TEnum, string> _toValue;

    static ForwardCompatibleEnumConverter()
    {
        _byValue = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);
        _toValue = new Dictionary<TEnum, string>();

        foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var enumVal = (TEnum)field.GetValue(null)!;
            var attr = field.GetCustomAttribute<JsonPropertyNameAttribute>();
            var wireName = attr?.Name ?? field.Name;
            _byValue[wireName] = enumVal;
            _toValue[enumVal] = wireName;
        }

        if (!Enum.TryParse<TEnum>("Unknown", ignoreCase: true, out var unknown))
        {
            throw new InvalidOperationException($"Enum {typeof(TEnum).Name} must have an 'Unknown' member for forward-compatibility.");
        }
        _unknown = unknown;
    }

    /// <inheritdoc />
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return _unknown;

        var raw = reader.GetString();
        if (raw is null)
            return _unknown;

        return _byValue.TryGetValue(raw, out var found) ? found : _unknown;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        if (_toValue.TryGetValue(value, out var wireName))
            writer.WriteStringValue(wireName);
        else
            writer.WriteNullValue();
    }
}

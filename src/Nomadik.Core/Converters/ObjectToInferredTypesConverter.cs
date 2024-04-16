using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nomadik.Core.Converters;

/// <summary>
/// Json Converter for deserializing of generic json to a loosely typed object?
/// </summary>
public class ObjectToInferredTypesConverter : JsonConverter<object>
{
    public override object? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) => reader.TokenType switch
    {
        JsonTokenType.Null => null,
        JsonTokenType.True => true,
        JsonTokenType.False => false,
        JsonTokenType.Number when reader.TryGetInt32(out int i) => i,
        JsonTokenType.Number => reader.GetDecimal(),
        JsonTokenType.String when reader.TryGetDateTime(out DateTime datetime) => 
            datetime,
        JsonTokenType.String => reader.GetString()!,
        _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
    };

    public override void Write(
        Utf8JsonWriter writer,
        object objectToWrite,
        JsonSerializerOptions options
    ) => JsonSerializer.Serialize(
        writer, objectToWrite, objectToWrite.GetType(), options
    );
}

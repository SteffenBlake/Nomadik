using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace Nomadik.Core.Converters;

/// <summary>
/// Json Converter for deserializing of <see cref="SearchFilter"/>s
/// </summary>
public class SearchFilterConverter : JsonConverter<SearchFilter>
{
    public override SearchFilter? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert, 
        JsonSerializerOptions options
    )
    {
        var raw = JsonSerializer.Deserialize<JsonObject>(ref reader, options)!;
        if (raw.ContainsKey(nameof(SearchFilterAnd.And)))
        {
            return raw.Deserialize<SearchFilterAnd>(options);
        }
        if (raw.ContainsKey(nameof(SearchFilterOr.Or)))
        {
            return raw.Deserialize<SearchFilterOr>(options);
        }
        if (raw.ContainsKey(nameof(SearchFilterWhere.Where)))
        {
            return raw.Deserialize<SearchFilterWhere>(options);
        }
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, SearchFilter value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(
            writer, value, value.GetType(), options
        );    
    }
}

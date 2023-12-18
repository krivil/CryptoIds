namespace CryptoIds;

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

public class CryptoIdJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => 
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(CryptoId<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type valueType = typeToConvert.GetGenericArguments()[0];

        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(CryptoIdJsonConverter<>).MakeGenericType(valueType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null)!;

        return converter;
    }

    private class CryptoIdJsonConverter<T>
        : JsonConverter<CryptoId<T>> where T : unmanaged
    {
        public override CryptoId<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType != JsonTokenType.String
                ? throw new JsonException()
                : CryptoId<T>.TryParse(reader.ValueSpan, out CryptoId<T> result)
                    ? result
                    : throw new JsonException("Invalid CryptoId");

        public override void Write(Utf8JsonWriter writer, CryptoId<T> value, JsonSerializerOptions options)
        {
            Span<byte> bytes = stackalloc byte[CryptoId<T>.GetLengthWhenEncoded()];
            value.TryEncode(bytes);
            writer.WriteStringValue(bytes);
        }
    }
}
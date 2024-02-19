namespace CryptoIds;

using System.Buffers;
using System.Text;
using System.Text.Json.Serialization;

public static class CryptoId
{
    public static CryptoId<T> From<T>(T id) where T : unmanaged => CryptoId<T>.From(id);
}

[JsonConverter(typeof(CryptoIdJsonConverterFactory))]
public readonly partial struct CryptoId<T> where T : unmanaged
{
    private readonly T _value;

    private CryptoId(T value) => _value = value;

    public T GetValue() => _value;
}

public partial struct CryptoId<T>
{
    private const int StackAllocThreshold = 128;

    public static CryptoId<T> From(T id) => new(id);

    public static CryptoId<T> Decode(string s) =>
    TryParse(s, out var result)
        ? result
        : throw new ArgumentException("Invalid CryptoId", nameof(s));

    public static CryptoId<T> Decode(string s, CryptoIdContext context) =>
        TryParse(s, context, out var result)
            ? result
            : throw new ArgumentException("Invalid CryptoId", nameof(s));

    public static CryptoId<T> Decode(ReadOnlySpan<char> s) =>
        TryParse(s, out var result)
            ? result
            : throw new ArgumentException("Invalid CryptoId", nameof(s));

    public static CryptoId<T> Decode(ReadOnlySpan<char> s, CryptoIdContext context) =>
        TryParse(s, context, out var result)
            ? result
            : throw new ArgumentException("Invalid CryptoId", nameof(s));

    public static CryptoId<T> Decode(ReadOnlySpan<byte> s) =>
        TryParse(s, out var result)
            ? result
            : throw new ArgumentException("Invalid CryptoId", nameof(s));

    public static CryptoId<T> Decode(ReadOnlySpan<byte> s, CryptoIdContext context) =>
        TryParse(s, context, out var result)
            ? result
            : throw new ArgumentException("Invalid CryptoId", nameof(s));

    public static int GetLengthWhenEncoded() => CryptoIdContext.Default.GetRequiredLengthForEncode<T>();

    public static int GetLengthWhenEncoded(CryptoIdContext context) => context.GetRequiredLengthForEncode<T>();

    public static bool TryParse(string s, out CryptoId<T> result) => TryParse((ReadOnlySpan<char>) s, out result);

    public static bool TryParse(string s, CryptoIdContext context, out CryptoId<T> result) => TryParse((ReadOnlySpan<char>) s, context, out result);

    public static bool TryParse(ReadOnlySpan<char> s, out CryptoId<T> result) 
    {
        int arraySizeRequired = Encoding.UTF8.GetByteCount(s);

        byte[]? bufferToReturnToPool = null;

        Span<byte> buffer = arraySizeRequired <= StackAllocThreshold
            ? stackalloc byte[arraySizeRequired]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(arraySizeRequired);

        try
        {
            _ = Encoding.UTF8.GetBytes(s, buffer);
            return TryParse(buffer, out result);
        }
        finally
        {
            if (bufferToReturnToPool != null) ArrayPool<byte>.Shared.Return(bufferToReturnToPool);
        }
    }
    
    public static bool TryParse(ReadOnlySpan<char> s, CryptoIdContext context, out CryptoId<T> result)
    {
        int arraySizeRequired = Encoding.UTF8.GetByteCount(s);

        byte[]? bufferToReturnToPool = null;

        Span<byte> buffer = arraySizeRequired <= StackAllocThreshold
            ? stackalloc byte[arraySizeRequired]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(arraySizeRequired);

        try
        {
            _ = Encoding.UTF8.GetBytes(s, buffer);
            return TryParse(buffer, context, out result);
        }
        finally
        {
            if (bufferToReturnToPool != null) ArrayPool<byte>.Shared.Return(bufferToReturnToPool);
        }
    }

    public static bool TryParse(ReadOnlySpan<byte> s, CryptoIdContext context, out CryptoId<T> result) =>
        context.TryDecode(s, out result);
    
    public static bool TryParse(ReadOnlySpan<byte> s, out CryptoId<T> result) =>
        CryptoIdContext.Default.TryDecode(s, out result);

    public char[] Encode() => Encode(CryptoIdContext.Default);

    public char[] Encode(CryptoIdContext context)
    {
        char[] result = new char[context.GetRequiredLengthForEncode<T>()];

        _ = context.TryEncode(_value, result);

        return result;
    }

    public char[] Encode(Guid userKey) => Encode(userKey, CryptoIdContext.Default);

    public char[] Encode(Guid userKey, CryptoIdContext context)
    {
        char[] result = new char[context.GetRequiredLengthForEncode<T>()];

        _ = context.TryEncode(_value, userKey, result);

        return result;
    }

    public byte[] EncodeBytes() => EncodeBytes(CryptoIdContext.Default);

    public byte[] EncodeBytes(CryptoIdContext context)
    {
        byte[] result = new byte[context.GetRequiredLengthForEncode<T>()];

        _ = context.TryEncode(_value, result);

        return result;
    }

    public byte[] EncodeBytes(Guid userKey) => EncodeBytes(userKey, CryptoIdContext.Default);

    public byte[] EncodeBytes(Guid userKey, CryptoIdContext context)
    {
        byte[] result = new byte[context.GetRequiredLengthForEncode<T>()];

        _ = context.TryEncode(_value, userKey, result);

        return result;
    }

    public bool TryEncode(Span<char> result) => TryEncode(result, CryptoIdContext.Default);

    public bool TryEncode(Span<char> result, CryptoIdContext context)
    {
        int lengthForEncode = context.GetRequiredLengthForEncode<T>();
        return result.Length < lengthForEncode
            ? throw new ArgumentException("Destination buffer is too small", nameof(result))
            : lengthForEncode == context.TryEncode(_value, result);
    }

    public bool TryEncode(Span<char> result, Guid userKey) => TryEncode(result, userKey, CryptoIdContext.Default);
    
    public bool TryEncode(Span<char> result, Guid userKey, CryptoIdContext context)
    {
        int lengthForEncode = context.GetRequiredLengthForEncode<T>();
        return result.Length < lengthForEncode
            ? throw new ArgumentException("Destination buffer is too small", nameof(result))
            : lengthForEncode == context.TryEncode(_value, userKey, result);
    }

    public bool TryEncode(Span<byte> result) => TryEncode(result, CryptoIdContext.Default);

    public bool TryEncode(Span<byte> result, CryptoIdContext context)
    {
        int lengthForEncode = context.GetRequiredLengthForEncode<T>();
        return result.Length < lengthForEncode
            ? throw new ArgumentException("Destination buffer is too small", nameof(result))
            : lengthForEncode == context.TryEncode(_value, result);
    }

    public bool TryEncode(Span<byte> result, Guid userKey) => TryEncode(result, userKey, CryptoIdContext.Default);

    public bool TryEncode(Span<byte> result, Guid userKey, CryptoIdContext context)
    {
        int lengthForEncode = context.GetRequiredLengthForEncode<T>();
        return result.Length < lengthForEncode
            ? throw new ArgumentException("Destination buffer is too small", nameof(result))
            : lengthForEncode == context.TryEncode(_value, userKey, result);
    }

    public override string ToString() => new(Encode());

    public string ToString(CryptoIdContext context) => new(Encode(context));

    public static implicit operator CryptoId<T>(T value) => new(value);
    public static implicit operator CryptoId<T>?(T? value) => value.HasValue ? new(value.Value) : null;
    public static implicit operator T(CryptoId<T> cryptoId) => cryptoId._value;
    public static implicit operator T?(CryptoId<T>? cryptoId) => cryptoId?._value;

    public static explicit operator CryptoId<T>(byte[] encoded) => Decode(encoded);
    public static explicit operator CryptoId<T>(Span<byte> encoded) => Decode(encoded);
    public static explicit operator CryptoId<T>(ReadOnlySpan<byte> encoded) => Decode(encoded);
    public static explicit operator CryptoId<T>?(byte[]? encoded) => encoded is null ? null : Decode(encoded);
    public static explicit operator CryptoId<T>(char[] encoded) => Decode(encoded);
    public static explicit operator CryptoId<T>?(char[]? encoded) => encoded is null ? null : Decode(encoded);
    public static explicit operator CryptoId<T>(Span<char> encoded) => Decode(encoded);
    public static explicit operator CryptoId<T>(ReadOnlySpan<char> encoded) => Decode(encoded);
    public static explicit operator CryptoId<T>(string encoded) => Decode(encoded);
    public static explicit operator CryptoId<T>?(string? encoded) => encoded is null ? null : Decode(encoded);
    public static explicit operator char[](CryptoId<T> cryptoId) => cryptoId.Encode();
    public static explicit operator char[]?(CryptoId<T>? cryptoId) => cryptoId?.Encode();
    public static explicit operator byte[](CryptoId<T> cryptoId) => cryptoId.EncodeBytes();
    public static explicit operator byte[]?(CryptoId<T>? cryptoId) => cryptoId?.EncodeBytes();
    public static explicit operator string(CryptoId<T> cryptoId) => cryptoId.ToString();
    public static explicit operator string?(CryptoId<T>? cryptoId) => cryptoId?.ToString();
}
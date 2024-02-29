namespace CryptoIds;

using System.Buffers;
using System.Text;
using System.Text.Json.Serialization;

public static class CryptoId
{
    public static CryptoId<T> From<T>(T id) where T : unmanaged => CryptoId<T>.From(id);
    
    public static CryptoId<T> Parse<T>(string s) where T : unmanaged => CryptoId<T>.Decode(s);

    public static CryptoId<T> Parse<T>(ReadOnlySpan<char> s) where T : unmanaged => CryptoId<T>.Decode(s);

    public static bool TryParse<T>(string? s, out CryptoId<T> result) where T : unmanaged
    {
        if (s != null) return CryptoId<T>.TryParse(s, out result);

        result = default;
        return false;
    }

    public static bool TryParse<T>(ReadOnlySpan<char> s, out CryptoId<T> result) where T : unmanaged => 
        CryptoId<T>.TryParse(s, out result);
}

[JsonConverter(typeof(CryptoIdJsonConverterFactory))]
public readonly partial struct CryptoId<T> : ISpanParsable<CryptoId<T>> where T : unmanaged
{
    private readonly T _value;

    private CryptoId(T value) => _value = value;

    public T GetValue() => _value;

    public static CryptoId<T> Parse(string s, IFormatProvider? provider) => Decode(s);

    public static bool TryParse(string? s, IFormatProvider? provider, out CryptoId<T> result)
    {
        if (s != null) return TryParse(s, out result);

        result = default;
        return false;
    }

    public static CryptoId<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Decode(s);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CryptoId<T> result) => TryParse(s, out result);
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

    public static bool TryParse(string s, CryptoIdContext context, out CryptoId<T> result) =>
        TryParse((ReadOnlySpan<char>) s, context, out result);

    public static bool TryParse(ReadOnlySpan<char> s, out CryptoId<T> result) =>
        TryParse(s, CryptoIdContext.Default, out result);
    
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

    public static bool TryParse(ReadOnlySpan<byte> s, CryptoIdContext context, out CryptoId<T> result)
    {
        if (context.TryDecode(s, out T value))
        {
            result = From(value);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<byte> s, out CryptoId<T> result) =>
        TryParse(s, CryptoIdContext.Default, out result);

    public char[] Encode() => Encode(CryptoIdContext.Default);

    public char[] Encode(CryptoIdContext context)
    {
        char[] result = new char[context.GetRequiredLengthForEncode<T>()];

        int resultLength = context.TryEncode(_value, result);

        return result[..resultLength];
    }

    public char[] Encode(Guid userKey) => Encode(userKey, CryptoIdContext.Default);

    public char[] Encode(Guid userKey, CryptoIdContext context)
    {
        char[] result = new char[context.GetRequiredLengthForEncode<T>()];

        int resultLength = context.TryEncode(_value, userKey, result);

        return result[..resultLength];
    }

    public byte[] EncodeBytes() => EncodeBytes(CryptoIdContext.Default);

    public byte[] EncodeBytes(CryptoIdContext context)
    {
        byte[] result = new byte[context.GetRequiredLengthForEncode<T>()];

        int resultLength = context.TryEncode(_value, result);

        return result[..resultLength];
    }

    public byte[] EncodeBytes(Guid userKey) => EncodeBytes(userKey, CryptoIdContext.Default);

    public byte[] EncodeBytes(Guid userKey, CryptoIdContext context)
    {
        byte[] result = new byte[context.GetRequiredLengthForEncode<T>()];

        int resultLength = context.TryEncode(_value, userKey, result);

        return result[..resultLength];
    }

    public bool TryEncode(Span<char> result, out int charsWritten) => TryEncode(result, out charsWritten, CryptoIdContext.Default);

    public bool TryEncode(Span<char> result, out int charsWritten, CryptoIdContext context)
    {
        int lengthForEncode = context.GetRequiredLengthForEncode<T>();
        if(result.Length < lengthForEncode)
            throw new ArgumentException("Destination buffer is too small", nameof(result));
        
        charsWritten = context.TryEncode(_value, result);
        return true;
    }

    public bool TryEncode(Span<char> result, out int charsWritten, Guid userKey) => TryEncode(result, out charsWritten, userKey, CryptoIdContext.Default);

    public bool TryEncode(Span<char> result, out int charsWritten, Guid userKey, CryptoIdContext context)
    {
        int lengthForEncode = context.GetRequiredLengthForEncode<T>();
        if (result.Length < lengthForEncode)
            throw new ArgumentException("Destination buffer is too small", nameof(result));

        charsWritten = context.TryEncode(_value, userKey, result);
        return true;
    }

    public bool TryEncode(Span<byte> result, out int bytesWritten) => TryEncode(result, out bytesWritten, CryptoIdContext.Default);

    public bool TryEncode(Span<byte> result, out int bytesWritten, CryptoIdContext context)
    {
        int lengthForEncode = context.GetRequiredLengthForEncode<T>();
        if(result.Length < lengthForEncode)
            throw new ArgumentException("Destination buffer is too small", nameof(result));

        bytesWritten = context.TryEncode(_value, result);
        return true;
    }

    public bool TryEncode(Span<byte> result, out int bytesWritten, Guid userKey) => TryEncode(result, out bytesWritten, userKey, CryptoIdContext.Default);

    public bool TryEncode(Span<byte> result, out int bytesWritten, Guid userKey, CryptoIdContext context)
    {
        int lengthForEncode = context.GetRequiredLengthForEncode<T>();
        if(result.Length < lengthForEncode)
            throw new ArgumentException("Destination buffer is too small", nameof(result));
        
        bytesWritten = context.TryEncode(_value, userKey, result);
        return true;
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
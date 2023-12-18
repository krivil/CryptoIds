namespace CryptoIds;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET8_0_OR_GREATER

[InlineArray(256)]
public struct Crypto2048BitKey
{
    public static Crypto2048BitKey From(ReadOnlySpan<byte> source256Bytes)
    {
        if (source256Bytes.Length != 256)
            throw new ArgumentException("Source must be 256 bytes long", nameof(source256Bytes));

        Crypto2048BitKey result = new();
        source256Bytes.CopyTo(MemoryMarshal.CreateSpan(ref result._element0, 256));
        return result;
    }

    public static Crypto2048BitKey From(ReadOnlySpan<byte> source128BytesA, ReadOnlySpan<byte> source128BytesB)
    {
        if (source128BytesA.Length != 128)
            throw new ArgumentException("SourceA must be 128 bytes long", nameof(source128BytesA));

        if (source128BytesB.Length != 128)
            throw new ArgumentException("SourceB must be 128 bytes long", nameof(source128BytesB));

        Crypto2048BitKey result = new();
        source128BytesA.CopyTo(MemoryMarshal.CreateSpan(ref result._element0, 128));
        source128BytesB.CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.Add(ref result._element0, 128), 128));
        return result;
    }

    public static Crypto2048BitKey From(Crypto1024BitKey keyA, Crypto1024BitKey keyB)
    {
        Crypto2048BitKey result = new();
        keyA.AsSpan().CopyTo(MemoryMarshal.CreateSpan(ref result._element0, 128));
        keyB.AsSpan().CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.Add(ref result._element0, 128), 128));
        return result;
    }

    private byte _element0;

    public ReadOnlySpan<byte> KeyA => MemoryMarshal.CreateReadOnlySpan(ref _element0, 128);
    public ReadOnlySpan<byte> KeyB => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref _element0, 128), 128);
}

#endif
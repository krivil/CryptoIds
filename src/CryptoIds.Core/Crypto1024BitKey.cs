namespace CryptoIds.Core;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


#if NET8_0_OR_GREATER

[InlineArray(128)]
public struct Crypto1024BitKey
{
    public static Crypto1024BitKey From(ReadOnlySpan<byte> source128Bytes)
    {
        if (source128Bytes.Length != 128)
            throw new ArgumentException("Source must be 128 bytes long", nameof(source128Bytes));

        Crypto1024BitKey result = new();
        source128Bytes.CopyTo(MemoryMarshal.CreateSpan(ref result._element0, 128));
        return result;
    }

    private byte _element0;

    public ReadOnlySpan<byte> AsSpan() => MemoryMarshal.CreateReadOnlySpan(ref _element0, 128);
}

#endif
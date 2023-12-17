namespace CryptoIds.Core;

using System.Runtime.CompilerServices;

public static class XorEncryptor
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> Xor(ReadOnlySpan<byte> input, ReadOnlySpan<byte> seed, ReadOnlySpan<byte> key)
    {
        Span<byte> output = new byte[input.Length];
        TryXor(input, seed, key, output);
        return output;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> Xor(ReadOnlySpan<byte> input, ReadOnlySpan<byte> key)
    {
        Span<byte> output = new byte[input.Length];
        TryXor(input, key, output);
        return output;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void XorInline(Span<byte> bytes, ReadOnlySpan<byte> seed, ReadOnlySpan<byte> key, ReadOnlySpan<byte> publicKey)
    {
        InternalXor(bytes, seed, key, publicKey);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void XorInline(Span<byte> bytes, ReadOnlySpan<byte> seed, ReadOnlySpan<byte> key)
    {
        InternalXor(bytes, seed, key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void XorInline(Span<byte> bytes, ReadOnlySpan<byte> key)
    {
        InternalXor(bytes, key);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryXor(ReadOnlySpan<byte> input, ReadOnlySpan<byte> seed, ReadOnlySpan<byte> key, Span<byte> output)
    {
        if (input.Length != output.Length)
        {
            return false;
        }

        input.CopyTo(output);
        InternalXor(output, seed, key);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryXor(ReadOnlySpan<byte> input, ReadOnlySpan<byte> key, Span<byte> output)
    {
        if (input.Length != output.Length)
        {
            return false;
        }

        input.CopyTo(output);
        InternalXor(output, key);
        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InternalXor(Span<byte> bytes, ReadOnlySpan<byte> seed, ReadOnlySpan<byte> key, ReadOnlySpan<byte> publicKey)
    {
        for (var i = 0; i < bytes.Length; i++)
        {
            byte b = bytes[i];
            b ^= seed[i % seed.Length];
            b ^= key[i % key.Length];
            b ^= publicKey[i % publicKey.Length];
            bytes[i] = b;
        }

        // TODO: Optimize this with SIMD
        // var seedVector = new Vector<byte>(seed[..input.Length]);
        // var keyVector = new Vector<byte>(key[..input.Length]);
        // var inputVector = new Vector<byte>(input);
        // var resultVector = inputVector ^ seedVector ^ keyVector;
        // resultVector.CopyTo(input);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InternalXor(Span<byte> bytes, ReadOnlySpan<byte> seed, ReadOnlySpan<byte> key)
    {
        for (var i = 0; i < bytes.Length; i++)
        {
            byte b = bytes[i];
            b ^= seed[i % seed.Length];
            b ^= key[i % key.Length];
            bytes[i] = b;
        }

        // TODO: Optimize this with SIMD
        // var seedVector = new Vector<byte>(seed[..input.Length]);
        // var keyVector = new Vector<byte>(key[..input.Length]);
        // var inputVector = new Vector<byte>(input);
        // var resultVector = inputVector ^ seedVector ^ keyVector;
        // resultVector.CopyTo(input);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InternalXor(Span<byte> bytes, ReadOnlySpan<byte> key)
    {
        for (var i = 0; i < bytes.Length; i++)
        {
            byte b = bytes[i];
            b ^= key[i % key.Length];
            bytes[i] = b;
        }

        // TODO: Optimize this with SIMD
    }
}
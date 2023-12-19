﻿namespace CryptoIds.Signature;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

public sealed class HmacSha3_256SignatureProvider : ISignatureProvider
{
    private const int ConstSignatureLengthInBytes = 32;
    public static int SignatureLengthInBytes => ConstSignatureLengthInBytes;

    public static readonly HmacSha3_256SignatureProvider Instance = new();

    public int SignatureLength => ConstSignatureLengthInBytes;

    public int Sign<T>(T id, ReadOnlySpan<byte> key, Span<byte> destination) where T : unmanaged
    {
        if (destination.Length < ConstSignatureLengthInBytes)
        {
            return 0;
        }

        Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<T>()];
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), id);

        _ = HMACSHA3_256.TryHashData(key, buffer, destination, out int retVal);
        return retVal;
    }

    public bool Verify<T>(T id, ReadOnlySpan<byte> key, ReadOnlySpan<byte> signature) where T : unmanaged
    {
        Span<byte> destination = stackalloc byte[SignatureLength];
        return Sign(id, key, destination) == signature.Length && signature.SequenceEqual(destination);
    }
}
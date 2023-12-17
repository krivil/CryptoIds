﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace CryptoIds.Core.Signature;

public sealed class HmacSha512SignatureProvider : ISignatureProvider
{
    private const int ConstSignatureLengthInBytes = 64;
    public static int SignatureLengthInBytes => ConstSignatureLengthInBytes;

    public static readonly HmacSha512SignatureProvider Instance = new();

    public int SignatureLength => ConstSignatureLengthInBytes;

    public int Sign<T>(T id, ReadOnlySpan<byte> key, Span<byte> destination) where T : unmanaged
    {
        if (destination.Length < ConstSignatureLengthInBytes)
        {
            return 0;
        }

        Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<T>()];
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), id);

        HMACSHA512.TryHashData(key, buffer, destination, out var retVal);
        return retVal;
    }

    public bool Verify<T>(T id, ReadOnlySpan<byte> key, ReadOnlySpan<byte> signature) where T : unmanaged
    {
        Span<byte> destination = stackalloc byte[SignatureLength];
        return Sign(id, key, destination) == signature.Length && signature.SequenceEqual(destination);
    }
}
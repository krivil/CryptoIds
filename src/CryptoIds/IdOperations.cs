namespace CryptoIds;

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

public static class IdOperations
{
    private const int StackAllocThreshold = 128;

    public static bool TryDecodeAndValidate<T>(ReadOnlySpan<char> encoded, ReadOnlySpan<byte> key, ISignatureProvider signer, out T result) where T : unmanaged
    {
        byte[]? bufferToReturnToPool = null;
        var encodedLen = Encoding.UTF8.GetByteCount(encoded);

        Span<byte> encodedBytes = encodedLen <= StackAllocThreshold
            ? stackalloc byte[encodedLen]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(encodedLen);

        try
        {
            Encoding.UTF8.GetBytes(encoded, encodedBytes);

            Span<byte> signature = stackalloc byte[signer.SignatureLength]; // maximum is 64 bytes

            bool success = Base64.TryDecodeFromUtf8(encodedBytes, out result, signature, out int signatureBytesWritten);

            if (!success) return false;

            signature = signature[..signatureBytesWritten];

            return signer.Verify(result, key, signature);
        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<byte>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static bool TryDecodeAndValidate<T>(ReadOnlySpan<byte> encoded, ReadOnlySpan<byte> key, ISignatureProvider signer, out T result) where T : unmanaged
    {
        Span<byte> signature = stackalloc byte[signer.SignatureLength]; // maximum is 64 bytes

        bool success = Base64.TryDecodeFromUtf8(encoded, out result, signature, out int signatureBytesWritten);

        if (!success) return false;

        signature = signature[..signatureBytesWritten];

        return signer.Verify(result, key, signature);
    }

    public static bool TryDecodeAndXorAndValidate<T>(ReadOnlySpan<char> encoded, ReadOnlySpan<byte> key, ReadOnlySpan<byte> keyXor, ISignatureProvider signer, out T result) where T : unmanaged
    {
        byte[]? bufferToReturnToPool = null;
        int byteCount = Encoding.UTF8.GetByteCount(encoded);
        Span<byte> buffer = byteCount <= StackAllocThreshold
            ? stackalloc byte[byteCount]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(byteCount);
        try
        {
            int bytesWritten = Encoding.UTF8.GetBytes(encoded, buffer);
            if (bytesWritten != 0) return TryDecodeAndXorAndValidate(buffer, key, keyXor, signer, out result);
            result = default;
            return false;

        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<byte>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static bool TryDecodeAndXorAndValidate<T>(ReadOnlySpan<byte> encoded, ReadOnlySpan<byte> key, ReadOnlySpan<byte> keyXor, ISignatureProvider signer, out T result) where T : unmanaged
    {
        int sizeOfId = Unsafe.SizeOf<T>();
        int arraySizeRequired = Base64.GetMaxByteCountForDecoding(encoded.Length);

        byte[]? bufferToReturnToPool = null;
        Span<byte> buffer = arraySizeRequired <= StackAllocThreshold
            ? stackalloc byte[arraySizeRequired]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(arraySizeRequired);
        try
        {
            Base64.DecodeFromUtf8(encoded, buffer, out int bytesConsumed, out int bytesWritten);
            if (bytesWritten == 0 || bytesWritten < sizeOfId)
            {
                result = default;
                return false;
            }

            // id -> (id,signature) -> id = id ^ signature ^ keyXor
            XorEncryptor.XorInline(buffer[..sizeOfId], buffer[sizeOfId..bytesWritten], keyXor);

            result = Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(buffer[..sizeOfId]));

            var signature = buffer[sizeOfId..bytesWritten];

            return signer.Verify(result, key, signature);
        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<byte>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static bool TryDecodeAndXorAndValidate<T>(ReadOnlySpan<char> encoded, ReadOnlySpan<byte> key, ReadOnlySpan<byte> keyXor, Guid session, ISignatureProvider signer, out T result) where T : unmanaged
    {
        byte[]? bufferToReturnToPool = null;
        int byteCount = Encoding.UTF8.GetByteCount(encoded);
        Span<byte> buffer = byteCount <= StackAllocThreshold
            ? stackalloc byte[byteCount]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(byteCount);
        try
        {
            int bytesWritten = Encoding.UTF8.GetBytes(encoded, buffer);
            if (bytesWritten == 0)
            {
                result = default;
                return false;
            }

            return TryDecodeAndXorAndValidate(buffer[..bytesWritten], key, keyXor, session, signer, out result);
        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<byte>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static bool TryDecodeAndXorAndValidate<T>(ReadOnlySpan<byte> encoded, ReadOnlySpan<byte> key, ReadOnlySpan<byte> keyXor, Guid session, ISignatureProvider signer, out T result) where T : unmanaged
    {
        int sizeOfId = Unsafe.SizeOf<T>();
        int arraySizeRequired = Base64.GetMaxByteCountForDecoding(encoded.Length);

        byte[]? bufferToReturnToPool = null;
        Span<byte> buffer = arraySizeRequired <= StackAllocThreshold
            ? stackalloc byte[arraySizeRequired]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(arraySizeRequired);
        try
        {
            Base64.DecodeFromUtf8(encoded, buffer, out int bytesConsumed, out int bytesWritten);
            if (bytesWritten == 0 || bytesWritten < sizeOfId)
            {
                result = default;
                return false;
            }

            Span<byte> sessionKeySpan = stackalloc byte[16];
            _ = session.TryWriteBytes(sessionKeySpan);

            // id -> (id,signature) -> id = id ^ signature ^ keyXor
            XorEncryptor.XorInline(buffer[..sizeOfId], buffer[sizeOfId..bytesWritten], keyXor, sessionKeySpan);

            if (!MemoryMarshal.TryRead(buffer[..sizeOfId], out T id))
            {
                result = default;
                return false;
            }
            result = id;

            var signature = buffer[sizeOfId..bytesWritten];

            return signer.Verify(id, key, signature);
        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<byte>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static int GetRequiredLengthForEncode<T>(ISignatureProvider signer) where T : unmanaged
    {
        int requiredLengthForEncode = Base64.GetMaxByteCountForEncoding<T>(signer.SignatureLength);
        return requiredLengthForEncode;
    }

    public static int TrySignAndEncode<T>(T id, ReadOnlySpan<byte> key, ISignatureProvider signer, Span<char> encodedResult) where T : unmanaged
    {
        int sizeOfBuffer = Base64.GetMaxByteCountForEncoding<T>(signer.SignatureLength);
        if (encodedResult.Length < sizeOfBuffer)
        {
            return -1 * sizeOfBuffer;
        }

        Span<byte> signature = stackalloc byte[signer.SignatureLength]; // maximum is 64 bytes
        _ = signer.Sign(id, key, signature);

        byte[]? bufferToReturnToPool = null;
        Span<byte> buffer = sizeOfBuffer <= StackAllocThreshold
            ? stackalloc byte[sizeOfBuffer]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(sizeOfBuffer);

        try
        {
            bool success = Base64.TryEncodeToUtf8(id, signature, buffer, out int bytesWritten);

            if (!success) return bytesWritten;

            Encoding.UTF8.GetChars(buffer[..bytesWritten], encodedResult);

            return bytesWritten;
        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<byte>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static int TrySignAndEncode<T>(T id, ReadOnlySpan<byte> key, ISignatureProvider signer, Span<byte> encodedResult) where T : unmanaged
    {
        int sizeOfBuffer = Base64.GetMaxByteCountForEncoding<T>(signer.SignatureLength);
        if (encodedResult.Length < sizeOfBuffer)
        {
            return -1 * sizeOfBuffer;
        }

        Span<byte> signature = stackalloc byte[signer.SignatureLength]; // maximum is 64 bytes
        _ = signer.Sign(id, key, signature);

        byte[]? bufferToReturnToPool = null;
        Span<byte> buffer = sizeOfBuffer <= StackAllocThreshold
            ? stackalloc byte[sizeOfBuffer]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(sizeOfBuffer);

        try
        {
            bool success = Base64.TryEncodeToUtf8(id, signature, buffer, out int bytesWritten);

            if (!success) return bytesWritten;

            buffer[..bytesWritten].CopyTo(encodedResult);

            return bytesWritten;
        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<byte>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static int TrySignAndXorAndEncode<T>(T id, ReadOnlySpan<byte> key, ReadOnlySpan<byte> keyXor, ISignatureProvider signer, Span<char> encodedResult) where T : unmanaged
    {
        Span<byte> resultBytes = stackalloc byte[encodedResult.Length];

        int bytesWritten = TrySignAndXorAndEncode(id, key, keyXor, signer, resultBytes);

        if (bytesWritten <= 0) return bytesWritten;

        return Encoding.UTF8.GetChars(resultBytes[..bytesWritten], encodedResult);
    }

    public static int TrySignAndXorAndEncode<T>(T id, ReadOnlySpan<byte> key, ReadOnlySpan<byte> keyXor, ISignatureProvider signer, Span<byte> encodedResult) where T : unmanaged
    {
        int sizeOfIdInBytes = Unsafe.SizeOf<T>();
        int sizeOfAllInBytes = sizeOfIdInBytes + signer.SignatureLength;
        int requiredMaxLengthForEncode = Base64.GetMaxByteCountForEncoding(sizeOfAllInBytes);

        if (encodedResult.Length < requiredMaxLengthForEncode)
        {
            return -1 * requiredMaxLengthForEncode;
        }

        Span<byte> spanIdAsBytes = stackalloc byte[sizeOfIdInBytes];
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(spanIdAsBytes), id);

        Span<byte> signature = stackalloc byte[signer.SignatureLength]; // maximum is 64 bytes
        _ = signer.Sign(id, key, signature);

        Span<byte> buffer = stackalloc byte[sizeOfAllInBytes];
        spanIdAsBytes.CopyTo(buffer);
        signature.CopyTo(buffer[spanIdAsBytes.Length..]);

        // id -> (id,signature) -> id = id ^ signature ^ keyXor
        XorEncryptor.XorInline(buffer[..sizeOfIdInBytes], buffer[sizeOfIdInBytes..], keyXor);

        Base64.EncodeToUtf8(buffer, encodedResult, out int bytesConsumed, out int bytesWritten);

        return bytesWritten;
    }

    public static int TrySignAndXorAndEncode<T>(T id, ReadOnlySpan<byte> key, ReadOnlySpan<byte> keyXor, Guid sessionKey, ISignatureProvider signer, Span<char> encodedResult) where T : unmanaged
    {
        Span<byte> resultBytes = stackalloc byte[encodedResult.Length];

        int bytesWritten = TrySignAndXorAndEncode(id, key, keyXor, sessionKey, signer, resultBytes);

        if (bytesWritten <= 0) return bytesWritten;

        return Encoding.UTF8.GetChars(resultBytes[..bytesWritten], encodedResult);
    }

    public static int TrySignAndXorAndEncode<T>(T id, ReadOnlySpan<byte> key, ReadOnlySpan<byte> keyXor, Guid sessionKey, ISignatureProvider signer, Span<byte> encodedResult) where T : unmanaged
    {
        int sizeOfIdInBytes = Unsafe.SizeOf<T>();
        int sizeOfAllInBytes = sizeOfIdInBytes + signer.SignatureLength;
        int requiredMaxLengthForEncode = Base64.GetMaxByteCountForEncoding(sizeOfAllInBytes);

        if (encodedResult.Length < requiredMaxLengthForEncode)
        {
            return -1 * requiredMaxLengthForEncode;
        }

        Span<byte> spanIdAsBytes = stackalloc byte[sizeOfIdInBytes];
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(spanIdAsBytes), id);

        Span<byte> signature = stackalloc byte[signer.SignatureLength]; // maximum is 64 bytes
        _ = signer.Sign(id, key, signature);

        Span<byte> buffer = stackalloc byte[sizeOfAllInBytes];
        spanIdAsBytes.CopyTo(buffer);
        signature.CopyTo(buffer[spanIdAsBytes.Length..]);

        Span<byte> sessionKeySpan = stackalloc byte[16];
        _ = sessionKey.TryWriteBytes(sessionKeySpan);

        // id -> (id,signature) -> id = id ^ signature ^ keyXor
        XorEncryptor.XorInline(buffer[..sizeOfIdInBytes], buffer[sizeOfIdInBytes..], keyXor, sessionKeySpan);

        Base64.EncodeToUtf8(buffer, encodedResult, out int bytesConsumed, out int bytesWritten);

        return bytesWritten;
    }
}
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
        int sizeOfId = Unsafe.SizeOf<T>();
        int arraySizeRequired = Base64UrlStringEncoder.GetMaximumRequiredLengthForDecode(encoded);

        byte[]? bufferToReturnToPool = null;

        Span<byte> buffer = arraySizeRequired <= StackAllocThreshold
            ? stackalloc byte[arraySizeRequired]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(arraySizeRequired);
        try
        {
            int bytesWritten = Base64UrlStringEncoder.Decode(encoded, buffer);
            if (bytesWritten == 0 || bytesWritten < sizeOfId)
            {
                result = default;
                return false;
            }

            Span<byte> idSpan = buffer[..sizeOfId];
            idSpan.Reverse(); // little endian to big endian - helps with sorting

            result = Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(idSpan));

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

    public static bool TryDecodeAndValidate<T>(ReadOnlySpan<byte> encoded, ReadOnlySpan<byte> key, ISignatureProvider signer, out T result) where T : unmanaged
    {
        int sizeOfId = Unsafe.SizeOf<T>();
        int arraySizeRequired = Base64UrlStringEncoder.GetMaximumRequiredLengthForDecode(encoded);

        byte[]? bufferToReturnToPool = null;

        Span<byte> buffer = arraySizeRequired <= StackAllocThreshold
            ? stackalloc byte[arraySizeRequired]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(arraySizeRequired);
        try
        {
            int bytesWritten = Base64UrlStringEncoder.Decode(encoded, buffer);
            if (bytesWritten == 0 || bytesWritten < sizeOfId)
            {
                result = default;
                return false;
            }

            Span<byte> idSpan = buffer[..sizeOfId];
            idSpan.Reverse(); // little endian to big endian - helps with sorting

            result = Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(idSpan));

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
            if (bytesWritten == 0)
            {
                result = default;
                return false;
            }

            return TryDecodeAndXorAndValidate(buffer[..bytesWritten], key, keyXor, signer, out result);
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
        int arraySizeRequired = Base64UrlStringEncoder.GetMaximumRequiredLengthForDecode(encoded);

        byte[]? bufferToReturnToPool = null;

        Span<byte> buffer = arraySizeRequired <= StackAllocThreshold
            ? stackalloc byte[arraySizeRequired]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(arraySizeRequired);
        try
        {
            int bytesWritten = Base64UrlStringEncoder.Decode(encoded, buffer);
            if (bytesWritten == 0 || bytesWritten < sizeOfId)
            {
                result = default;
                return false;
            }

            // id -> (id,signature) -> id = id ^ signature ^ keyXor
            XorEncryptor.XorInline(buffer[..sizeOfId], buffer[sizeOfId..bytesWritten], keyXor);

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
        int arraySizeRequired = Base64UrlStringEncoder.GetMaximumRequiredLengthForDecode(encoded);

        byte[]? bufferToReturnToPool = null;

        Span<byte> buffer = arraySizeRequired <= StackAllocThreshold
            ? stackalloc byte[arraySizeRequired]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(arraySizeRequired);
        try
        {
            int bytesWritten = Base64UrlStringEncoder.Decode(encoded, buffer);
            if (bytesWritten == 0 || bytesWritten < sizeOfId)
            {
                result = default;
                return false;
            }

            Span<byte> sessionKeySpan = stackalloc byte[16];
            session.TryWriteBytes(sessionKeySpan);

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
        int sizeOfIdInBytes = Unsafe.SizeOf<T>();
        int requiredLengthForEncode = Base64UrlStringEncoder.GetRequiredLengthForEncode(sizeOfIdInBytes + signer.SignatureLength);
        return requiredLengthForEncode;
    }

    public static int TrySignAndEncode<T>(T id, ReadOnlySpan<byte> key, ISignatureProvider signer, Span<char> encodedResult) where T : unmanaged
    {
        int sizeOfIdInBytes = Unsafe.SizeOf<T>();
        int requiredLengthForEncode = Base64UrlStringEncoder.GetRequiredLengthForEncode(sizeOfIdInBytes + signer.SignatureLength);

        if (encodedResult.Length < requiredLengthForEncode)
        {
            return -1 * requiredLengthForEncode;
        }

        byte[]? bufferToReturnToPool = null;

        Span<byte> encoded = requiredLengthForEncode <= StackAllocThreshold
            ? stackalloc byte[requiredLengthForEncode]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(requiredLengthForEncode);

        try
        {
            int bytesWritten = TrySignAndEncode(id, key, signer, encoded);

            return Encoding.UTF8.GetChars(encoded[..bytesWritten], encodedResult);
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
        int sizeOfIdInBytes = Unsafe.SizeOf<T>();
        int requiredLengthForEncode = Base64UrlStringEncoder.GetRequiredLengthForEncode(sizeOfIdInBytes + signer.SignatureLength);

        if (encodedResult.Length < requiredLengthForEncode)
        {
            return -1 * requiredLengthForEncode;
        }

        Span<byte> spanIdAsBytes = stackalloc byte[sizeOfIdInBytes];
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(spanIdAsBytes), id);
        spanIdAsBytes.Reverse(); // little endian to big endian - helps with sorting

        Span<byte> signature = stackalloc byte[signer.SignatureLength]; // maximum is 64 bytes
        signer.Sign(id, key, signature);

        Span<byte> buffer = stackalloc byte[spanIdAsBytes.Length + signature.Length];
        spanIdAsBytes.CopyTo(buffer);
        signature.CopyTo(buffer[spanIdAsBytes.Length..]);

        return Base64UrlStringEncoder.Encode(buffer, encodedResult);
    }

    public static int TrySignAndXorAndEncode<T>(T id, ReadOnlySpan<byte> key, ReadOnlySpan<byte> keyXor, ISignatureProvider signer, Span<char> encodedResult) where T : unmanaged
    {
        int sizeOfIdInBytes = Unsafe.SizeOf<T>();
        int sizeOfAllInBytes = sizeOfIdInBytes + signer.SignatureLength;
        int requiredLengthForEncode = Base64UrlStringEncoder.GetRequiredLengthForEncode(sizeOfAllInBytes);

        if (encodedResult.Length < requiredLengthForEncode)
        {
            return -1 * requiredLengthForEncode;
        }

        Span<byte> spanIdAsBytes = stackalloc byte[sizeOfIdInBytes];
        MemoryMarshal.Write(spanIdAsBytes, in id);

        Span<byte> signature = stackalloc byte[signer.SignatureLength]; // maximum is 64 bytes
        signer.Sign(id, key, signature);

        Span<byte> buffer = stackalloc byte[sizeOfAllInBytes];
        spanIdAsBytes.CopyTo(buffer);
        signature.CopyTo(buffer[spanIdAsBytes.Length..]);

        byte[]? bufferToReturnToPool = null;

        Span<byte> encoded = requiredLengthForEncode <= StackAllocThreshold
            ? stackalloc byte[requiredLengthForEncode]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(requiredLengthForEncode);

        try
        {
            // id -> (id,signature) -> id = id ^ signature ^ keyXor
            XorEncryptor.XorInline(buffer[..sizeOfIdInBytes], buffer[sizeOfIdInBytes..], keyXor);

            int bytesWritten = Base64UrlStringEncoder.Encode(buffer, encoded);
            if (bytesWritten != requiredLengthForEncode || bytesWritten == 0)
            {
                return 0;
            }

            return Encoding.UTF8.GetChars(encoded[..bytesWritten], encodedResult);
        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<byte>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static int TrySignAndXorAndEncode<T>(T id, ReadOnlySpan<byte> key, ReadOnlySpan<byte> keyXor, ISignatureProvider signer, Span<byte> encodedResult) where T : unmanaged
    {
        int sizeOfIdInBytes = Unsafe.SizeOf<T>();
        int sizeOfAllInBytes = sizeOfIdInBytes + signer.SignatureLength;
        int requiredLengthForEncode = Base64UrlStringEncoder.GetRequiredLengthForEncode(sizeOfAllInBytes);

        if (encodedResult.Length < requiredLengthForEncode)
        {
            return -1 * requiredLengthForEncode;
        }

        Span<byte> spanIdAsBytes = stackalloc byte[sizeOfIdInBytes];
        MemoryMarshal.Write(spanIdAsBytes, in id);

        Span<byte> signature = stackalloc byte[signer.SignatureLength]; // maximum is 64 bytes
        signer.Sign(id, key, signature);

        Span<byte> buffer = stackalloc byte[sizeOfAllInBytes];
        spanIdAsBytes.CopyTo(buffer);
        signature.CopyTo(buffer[spanIdAsBytes.Length..]);

        // id -> (id,signature) -> id = id ^ signature ^ keyXor
        XorEncryptor.XorInline(buffer[..sizeOfIdInBytes], buffer[sizeOfIdInBytes..], keyXor);

        return Base64UrlStringEncoder.Encode(buffer, encodedResult);
    }

    public static int TrySignAndXorAndEncode<T>(T id, ReadOnlySpan<byte> key, ReadOnlySpan<byte> keyXor, Guid sessionKey, ISignatureProvider signer, Span<char> encodedResult) where T : unmanaged
    {
        int sizeOfIdInBytes = Unsafe.SizeOf<T>();
        int sizeOfAllInBytes = sizeOfIdInBytes + signer.SignatureLength;
        int requiredLengthForEncode = Base64UrlStringEncoder.GetRequiredLengthForEncode(sizeOfAllInBytes);

        if (encodedResult.Length < requiredLengthForEncode)
        {
            return -1 * requiredLengthForEncode;
        }

        Span<byte> spanIdAsBytes = stackalloc byte[sizeOfIdInBytes];
        MemoryMarshal.Write(spanIdAsBytes, in id);

        Span<byte> signature = stackalloc byte[signer.SignatureLength]; // maximum is 64 bytes
        signer.Sign(id, key, signature);

        Span<byte> buffer = stackalloc byte[sizeOfAllInBytes];
        spanIdAsBytes.CopyTo(buffer);
        signature.CopyTo(buffer[spanIdAsBytes.Length..]);

        byte[]? bufferToReturnToPool = null;

        Span<byte> encoded = requiredLengthForEncode <= StackAllocThreshold
            ? stackalloc byte[requiredLengthForEncode]
            : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(requiredLengthForEncode);

        try
        {
            Span<byte> sessionKeySpan = stackalloc byte[16];
            sessionKey.TryWriteBytes(sessionKeySpan);
            // id -> (id,signature) -> id = id ^ signature ^ keyXor
            XorEncryptor.XorInline(buffer[..sizeOfIdInBytes], buffer[sizeOfIdInBytes..], keyXor, sessionKeySpan);

            int bytesWritten = Base64UrlStringEncoder.Encode(buffer, encoded);
            if (bytesWritten != requiredLengthForEncode || bytesWritten == 0)
            {
                return 0;
            }

            return Encoding.UTF8.GetChars(encoded[..bytesWritten], encodedResult);
        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<byte>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static int TrySignAndXorAndEncode<T>(T id, ReadOnlySpan<byte> key, ReadOnlySpan<byte> keyXor, Guid sessionKey, ISignatureProvider signer, Span<byte> encodedResult) where T : unmanaged
    {
        int sizeOfIdInBytes = Unsafe.SizeOf<T>();
        int sizeOfAllInBytes = sizeOfIdInBytes + signer.SignatureLength;
        int requiredLengthForEncode = Base64UrlStringEncoder.GetRequiredLengthForEncode(sizeOfAllInBytes);

        if (encodedResult.Length < requiredLengthForEncode)
        {
            return -1 * requiredLengthForEncode;
        }

        Span<byte> spanIdAsBytes = stackalloc byte[sizeOfIdInBytes];
        MemoryMarshal.Write(spanIdAsBytes, in id);

        Span<byte> signature = stackalloc byte[signer.SignatureLength]; // maximum is 64 bytes
        signer.Sign(id, key, signature);

        Span<byte> buffer = stackalloc byte[sizeOfAllInBytes];
        spanIdAsBytes.CopyTo(buffer);
        signature.CopyTo(buffer[spanIdAsBytes.Length..]);

        Span<byte> sessionKeySpan = stackalloc byte[16];
        sessionKey.TryWriteBytes(sessionKeySpan);
        // id -> (id,signature) -> id = id ^ signature ^ keyXor
        XorEncryptor.XorInline(buffer[..sizeOfIdInBytes], buffer[sizeOfIdInBytes..], keyXor, sessionKeySpan);

        return Base64UrlStringEncoder.Encode(buffer, encodedResult);
    }
}
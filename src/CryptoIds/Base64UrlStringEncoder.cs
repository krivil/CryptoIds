namespace CryptoIds;

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Helpers
public static partial class Base64
{
    // Original Base64 Encoding
    //private static readonly byte[] Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"u8.ToArray();

    // Modified Base64 Encoding, sorted by lexicographic order and URL friendly
    private static readonly byte[] Alphabet = "-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz"u8.ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetPaddingCharsToAddForEncode(int inputLength) =>
        (3 - (inputLength % 3)) % 3; // Modulo by 3 to handle cases where count is already a multiple of 3

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetPaddingCharsToAddForDecode(int inputLength) =>
        (4 - (inputLength % 4)) % 4; // Modulo by 4 to handle cases where length is already a multiple of 4

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetMaxByteCountForEncoding(int inputLength)
    {
        return ((inputLength + 2) / 3) * 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetMaxByteCountForEncoding<T>(int signatureLength = 0) where T : unmanaged
    {
        int typeSize = Unsafe.SizeOf<T>();
        int inputLength = typeSize + signatureLength;
        int resultMaxLength = GetMaxByteCountForEncoding(inputLength);
        return resultMaxLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryEncodeToUtf8(ReadOnlySpan<byte> value, ReadOnlySpan<byte> signature, Span<byte> buffer,
        out int bytesWritten)
    {
        int valueSize = value.Length;
        int bufferSize = valueSize + signature.Length;
        int resultMaxLength = GetMaxByteCountForEncoding(bufferSize);

        if (buffer.Length < resultMaxLength)
        {
            bytesWritten = resultMaxLength - buffer.Length;
            return false;
        }

        Span<byte> bytes = stackalloc byte[bufferSize];

        value.CopyTo(bytes[..valueSize]);
        signature.CopyTo(bytes[valueSize..]);

        Base64.EncodeToUtf8(bytes, buffer, out int consumed, out int written, isFinalBlock: true);

        int paddingCharsCount = GetPaddingCharsToAddForEncode(consumed);

        bytesWritten = written - paddingCharsCount;

        return true;
    }

    public static bool TryEncodeToUtf8<T>(T value, ReadOnlySpan<byte> signature, Span<byte> buffer,
        out int bytesWritten)
    {
        int valueSize = Unsafe.SizeOf<T>();

        Span<byte> valueBytes = stackalloc byte[valueSize];

        //BinaryPrimitives.WriteInt32BigEndian(bytes, value); // When type is known
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(valueBytes), value);

        // When running on little-endian systems, reverse the bytes to get the big-endian representation
        // This is necessary because encoded base64 strings are always big-endian and will be compared as such
        if (BitConverter.IsLittleEndian)
            valueBytes.Reverse();

        return TryEncodeToUtf8(valueBytes, signature, buffer, out bytesWritten);
    }

    public static bool TryEncodeToUtf8(int value, ReadOnlySpan<byte> signature, Span<byte> buffer,
        out int bytesWritten)
    {
        Span<byte> valueBytes = stackalloc byte[sizeof(int)];

        BinaryPrimitives.WriteInt32BigEndian(valueBytes, value);

        return TryEncodeToUtf8(valueBytes, signature, buffer, out bytesWritten);
    }

    public static bool TryEncodeToUtf8(long value, ReadOnlySpan<byte> signature, Span<byte> buffer,
        out int bytesWritten)
    {
        Span<byte> valueBytes = stackalloc byte[sizeof(long)];

        BinaryPrimitives.WriteInt64BigEndian(valueBytes, value);

        return TryEncodeToUtf8(valueBytes, signature, buffer, out bytesWritten);
    }

    public static byte[] EncodeToBytes<T>(T value, ReadOnlySpan<byte> signature = default) where T : unmanaged
    {
        int maxBufferBytes = GetMaxByteCountForEncoding<T>(signature.Length);
        Span<byte> buffer = stackalloc byte[maxBufferBytes];

        TryEncodeToUtf8(value, signature, buffer, out int bytesWritten);

        return buffer[..bytesWritten].ToArray();
    }

    public static string EncodeToString<T>(T value, ReadOnlySpan<byte> signature = default) where T : unmanaged
    {
        int maxBufferBytes = GetMaxByteCountForEncoding<T>(signature.Length);
        Span<byte> buffer = stackalloc byte[maxBufferBytes];

        TryEncodeToUtf8(value, signature, buffer, out int bytesWritten);

        return System.Text.Encoding.UTF8.GetString(buffer[..bytesWritten]);
    }

    public static string EncodeToString(int value, ReadOnlySpan<byte> signature = default)
    {
        int maxBufferBytes = GetMaxByteCountForEncoding<int>(signature.Length);
        Span<byte> buffer = stackalloc byte[maxBufferBytes];

        TryEncodeToUtf8(value, signature, buffer, out int bytesWritten);

        return System.Text.Encoding.UTF8.GetString(buffer[..bytesWritten]);
    }

    public static string EncodeToString(long value, ReadOnlySpan<byte> signature = default)
    {
        int maxBufferBytes = GetMaxByteCountForEncoding<int>(signature.Length);
        Span<byte> buffer = stackalloc byte[maxBufferBytes];

        TryEncodeToUtf8(value, signature, buffer, out int bytesWritten);

        return System.Text.Encoding.UTF8.GetString(buffer[..bytesWritten]);
    }




    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetMaxByteCountForDecoding(int inputLength)
    {
        int paddingChars = GetPaddingCharsToAddForDecode(inputLength);
        int encodedBytesLength = inputLength + paddingChars;
        int maxBufferSize = Base64.GetMaxDecodedFromUtf8Length(encodedBytesLength);
        return maxBufferSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryDecodeFromUtf8(string encoded, Span<byte> result, out int bytesWritten)
    {
        int inputLength = System.Text.Encoding.UTF8.GetByteCount(encoded);

        int paddingChars = GetPaddingCharsToAddForDecode(inputLength);

        int encodedBytesLength = inputLength + paddingChars;

        int maxBufferSize = Base64.GetMaxDecodedFromUtf8Length(encodedBytesLength);

        if (result.Length < maxBufferSize)
        {
            bytesWritten = maxBufferSize - result.Length;
            return false;
        }

        Span<byte> encodedBytes = stackalloc byte[encodedBytesLength];
        System.Text.Encoding.UTF8.GetBytes(encoded, encodedBytes);

        for (int i = encodedBytesLength - 1; i >= inputLength; i--)
        {
            encodedBytes[i] = 0x3D; // '='
        }

        Span<byte> buffer = stackalloc byte[maxBufferSize];
        bool retVal = OperationStatus.Done == Base64.DecodeFromUtf8(encodedBytes, buffer, out int bytesConsumed, out bytesWritten, isFinalBlock: true);
        if (retVal)
        {
            buffer[..bytesWritten].CopyTo(result);
        }
        return retVal;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryDecodeFromUtf8(ReadOnlySpan<byte> encoded, Span<byte> result, out int bytesWritten)
    {
        int inputLength = encoded.Length;

        int paddingChars = GetPaddingCharsToAddForDecode(inputLength);

        int encodedBytesLength = inputLength + paddingChars;

        int maxBufferSize = Base64.GetMaxDecodedFromUtf8Length(encodedBytesLength);

        if (result.Length < maxBufferSize)
        {
            bytesWritten = maxBufferSize - result.Length;
            return false;
        }

        Span<byte> encodedBytes = stackalloc byte[encodedBytesLength];
        encoded.CopyTo(encodedBytes);

        for (int i = encodedBytesLength - 1; i >= inputLength; i--)
        {
            encodedBytes[i] = 0x3D; // '='
        }

        Span<byte> buffer = stackalloc byte[maxBufferSize];
        bool retVal = OperationStatus.Done == Base64.DecodeFromUtf8(encodedBytes, buffer, out int bytesConsumed, out bytesWritten, isFinalBlock: true);
        if (retVal)
        {
            buffer[..bytesWritten].CopyTo(result);
        }
        return retVal;
    }

    public static bool TryDecodeFromUtf8<T>(string encoded, out T value, Span<byte> signature, out int bytesWritten) where T : unmanaged
    {
        int maxBufferSize = GetMaxByteCountForDecoding(encoded.Length);

        Span<byte> buffer = stackalloc byte[maxBufferSize];

        int typeSize = Unsafe.SizeOf<T>();

        if (TryDecodeFromUtf8(encoded, buffer, out int allBytesWritten))
        {
            Span<byte> valueSpan = buffer[..typeSize];
            valueSpan.Reverse();
            value = Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(valueSpan));
            buffer[typeSize..allBytesWritten].CopyTo(signature);
            bytesWritten = allBytesWritten - typeSize;
            return true;
        }

        value = default;
        bytesWritten = allBytesWritten - typeSize;
        return false;
    }

    public static bool TryDecodeFromUtf8<T>(ReadOnlySpan<byte> encoded, out T value, Span<byte> signature, out int bytesWritten) where T : unmanaged
    {
        int maxBufferSize = GetMaxByteCountForDecoding(encoded.Length);

        Span<byte> buffer = stackalloc byte[maxBufferSize];

        int typeSize = Unsafe.SizeOf<T>();

        if (TryDecodeFromUtf8(encoded, buffer, out int allBytesWritten))
        {
            Span<byte> valueSpan = buffer[..typeSize];
            valueSpan.Reverse();
            value = Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(valueSpan));
            buffer[typeSize..allBytesWritten].CopyTo(signature);
            bytesWritten = allBytesWritten - typeSize;
            return true;
        }

        value = default;
        bytesWritten = allBytesWritten - typeSize;
        return false;
    }

    public static bool TryDecodeFromUtf8(string encoded, out int value, Span<byte> signature, out int bytesWritten)
    {
        int maxBufferSize = GetMaxByteCountForDecoding(encoded.Length);

        Span<byte> buffer = stackalloc byte[maxBufferSize];

        if (TryDecodeFromUtf8(encoded, buffer, out int allBytesWritten))
        {
            value = BinaryPrimitives.ReadInt32BigEndian(buffer);
            buffer[sizeof(int)..allBytesWritten].CopyTo(signature);
            bytesWritten = allBytesWritten - sizeof(int);
            return true;
        }

        value = default;
        bytesWritten = allBytesWritten - sizeof(int);
        return false;
    }

    public static bool TryDecodeFromUtf8(ReadOnlySpan<byte> encoded, out int value, Span<byte> signature, out int bytesWritten)
    {
        int maxBufferSize = GetMaxByteCountForDecoding(encoded.Length);

        Span<byte> buffer = stackalloc byte[maxBufferSize];

        if (TryDecodeFromUtf8(encoded, buffer, out int allBytesWritten))
        {
            value = BinaryPrimitives.ReadInt32BigEndian(buffer);
            buffer[sizeof(int)..allBytesWritten].CopyTo(signature);
            bytesWritten = allBytesWritten - sizeof(int);
            return true;
        }

        value = default;
        bytesWritten = allBytesWritten - sizeof(int);
        return false;
    }

    public static bool TryDecodeFromUtf8(string encoded, out long value, Span<byte> signature, out int bytesWritten)
    {
        int maxBufferSize = GetMaxByteCountForDecoding(encoded.Length);

        Span<byte> buffer = stackalloc byte[maxBufferSize];

        if (TryDecodeFromUtf8(encoded, buffer, out int allBytesWritten))
        {
            value = BinaryPrimitives.ReadInt64BigEndian(buffer);
            buffer[sizeof(long)..allBytesWritten].CopyTo(signature);
            bytesWritten = allBytesWritten - sizeof(long);
            return true;
        }

        value = default;
        bytesWritten = allBytesWritten - sizeof(long);
        return false;
    }

    public static bool TryDecodeFromUtf8(ReadOnlySpan<byte> encoded, out long value, Span<byte> signature, out int bytesWritten)
    {
        int maxBufferSize = GetMaxByteCountForDecoding(encoded.Length);

        Span<byte> buffer = stackalloc byte[maxBufferSize];

        if (TryDecodeFromUtf8(encoded, buffer, out int allBytesWritten))
        {
            value = BinaryPrimitives.ReadInt64BigEndian(buffer);
            buffer[sizeof(long)..allBytesWritten].CopyTo(signature);
            bytesWritten = allBytesWritten - sizeof(long);
            return true;
        }

        value = default;
        bytesWritten = allBytesWritten - sizeof(long);
        return false;
    }
}

// Encoding
public static partial class Base64
{
    private static ReadOnlySpan<byte> EncodingMap => Alphabet;

    /// <summary>
    /// Encode the span of binary data into UTF-8 encoded text represented as base64.
    /// </summary>
    /// <param name="bytes">The input span which contains binary data that needs to be encoded.</param>
    /// <param name="utf8">The output span which contains the result of the operation, i.e. the UTF-8 encoded text in base64.</param>
    /// <param name="bytesConsumed">The number of input bytes consumed during the operation. This can be used to slice the input for subsequent calls, if necessary.</param>
    /// <param name="bytesWritten">The number of bytes written into the output span. This can be used to slice the output for subsequent calls, if necessary.</param>
    /// <param name="isFinalBlock"><see langword="true"/> (default) when the input span contains the entire data to encode.
    /// Set to <see langword="true"/> when the source buffer contains the entirety of the data to encode.
    /// Set to <see langword="false"/> if this method is being called in a loop and if more input data may follow.
    /// At the end of the loop, call this (potentially with an empty source buffer) passing <see langword="true"/>.</param>
    /// <returns>It returns the OperationStatus enum values:
    /// - Done - on successful processing of the entire input span
    /// - DestinationTooSmall - if there is not enough space in the output span to fit the encoded input
    /// - NeedMoreData - only if <paramref name="isFinalBlock"/> is <see langword="false"/>, otherwise the output is padded if the input is not a multiple of 3
    /// It does not return InvalidData since that is not possible for base64 encoding.
    /// </returns>
    public static unsafe OperationStatus EncodeToUtf8(ReadOnlySpan<byte> bytes, Span<byte> utf8, out int bytesConsumed, out int bytesWritten, bool isFinalBlock = true)
    {
        if (bytes.IsEmpty)
        {
            bytesConsumed = 0;
            bytesWritten = 0;
            return OperationStatus.Done;
        }

        fixed (byte* srcBytes = &MemoryMarshal.GetReference(bytes))
        fixed (byte* destBytes = &MemoryMarshal.GetReference(utf8))
        {
            int srcLength = bytes.Length;
            int destLength = utf8.Length;
            int maxSrcLength;

            if (srcLength <= MaximumEncodeLength && destLength >= GetMaxEncodedToUtf8Length(srcLength))
            {
                maxSrcLength = srcLength;
            }
            else
            {
                maxSrcLength = (destLength >> 2) * 3;
            }

            byte* src = srcBytes;
            byte* dest = destBytes;
            byte* srcEnd = srcBytes + (uint) srcLength;
            byte* srcMax = srcBytes + (uint) maxSrcLength;

            //if (maxSrcLength >= 16)
            //{
            //    byte* end = srcMax - 32;
            //    if (Avx2.IsSupported && (end >= src))
            //    {
            //        Avx2Encode(ref src, ref dest, end, maxSrcLength, destLength, srcBytes, destBytes);

            //        if (src == srcEnd)
            //            goto DoneExit;
            //    }

            //    end = srcMax - 16;
            //    if ((Ssse3.IsSupported || AdvSimd.Arm64.IsSupported) && BitConverter.IsLittleEndian && (end >= src))
            //    {
            //        Vector128Encode(ref src, ref dest, end, maxSrcLength, destLength, srcBytes, destBytes);

            //        if (src == srcEnd)
            //            goto DoneExit;
            //    }
            //}

            ref byte encodingMap = ref MemoryMarshal.GetReference(EncodingMap);
            uint result = 0;

            srcMax -= 2;
            while (src < srcMax)
            {
                result = Encode(src, ref encodingMap);
                Unsafe.WriteUnaligned(dest, result);
                src += 3;
                dest += 4;
            }

            if (srcMax + 2 != srcEnd)
                goto DestinationTooSmallExit;

            if (!isFinalBlock)
            {
                if (src == srcEnd)
                    goto DoneExit;

                goto NeedMoreData;
            }

            if (src + 1 == srcEnd)
            {
                result = EncodeAndPadTwo(src, ref encodingMap);
                Unsafe.WriteUnaligned(dest, result);
                src += 1;
                dest += 4;
            }
            else if (src + 2 == srcEnd)
            {
                result = EncodeAndPadOne(src, ref encodingMap);
                Unsafe.WriteUnaligned(dest, result);
                src += 2;
                dest += 4;
            }

DoneExit:
            bytesConsumed = (int) (src - srcBytes);
            bytesWritten = (int) (dest - destBytes);
            return OperationStatus.Done;

DestinationTooSmallExit:
            bytesConsumed = (int) (src - srcBytes);
            bytesWritten = (int) (dest - destBytes);
            return OperationStatus.DestinationTooSmall;

NeedMoreData:
            bytesConsumed = (int) (src - srcBytes);
            bytesWritten = (int) (dest - destBytes);
            return OperationStatus.NeedMoreData;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe uint Encode(byte* threeBytes, ref byte encodingMap)
    {
        uint t0 = threeBytes[0];
        uint t1 = threeBytes[1];
        uint t2 = threeBytes[2];

        uint i = (t0 << 16) | (t1 << 8) | t2;

        uint i0 = Unsafe.Add(ref encodingMap, (IntPtr) (i >> 18));
        uint i1 = Unsafe.Add(ref encodingMap, (IntPtr) ((i >> 12) & 0x3F));
        uint i2 = Unsafe.Add(ref encodingMap, (IntPtr) ((i >> 6) & 0x3F));
        uint i3 = Unsafe.Add(ref encodingMap, (IntPtr) (i & 0x3F));

        if (BitConverter.IsLittleEndian)
        {
            return i0 | (i1 << 8) | (i2 << 16) | (i3 << 24);
        }
        else
        {
            return (i0 << 24) | (i1 << 16) | (i2 << 8) | i3;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe uint EncodeAndPadOne(byte* twoBytes, ref byte encodingMap)
    {
        uint t0 = twoBytes[0];
        uint t1 = twoBytes[1];

        uint i = (t0 << 16) | (t1 << 8);

        uint i0 = Unsafe.Add(ref encodingMap, (IntPtr) (i >> 18));
        uint i1 = Unsafe.Add(ref encodingMap, (IntPtr) ((i >> 12) & 0x3F));
        uint i2 = Unsafe.Add(ref encodingMap, (IntPtr) ((i >> 6) & 0x3F));

        if (BitConverter.IsLittleEndian)
        {
            return i0 | (i1 << 8) | (i2 << 16) | (EncodingPad << 24);
        }
        else
        {
            return (i0 << 24) | (i1 << 16) | (i2 << 8) | EncodingPad;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe uint EncodeAndPadTwo(byte* oneByte, ref byte encodingMap)
    {
        uint t0 = oneByte[0];

        uint i = t0 << 8;

        uint i0 = Unsafe.Add(ref encodingMap, (IntPtr) (i >> 10));
        uint i1 = Unsafe.Add(ref encodingMap, (IntPtr) ((i >> 4) & 0x3F));

        if (BitConverter.IsLittleEndian)
        {
            return i0 | (i1 << 8) | (EncodingPad << 16) | (EncodingPad << 24);
        }
        else
        {
            return (i0 << 24) | (i1 << 16) | (EncodingPad << 8) | EncodingPad;
        }
    }

    internal const uint EncodingPad = '='; // '=', for padding

    private const int MaximumEncodeLength = (int.MaxValue / 4) * 3; // 1610612733

    /// <summary>
    /// Returns the maximum length (in bytes) of the result if you were to encode binary data within a byte span of size "length".
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified <paramref name="length"/> is less than 0 or larger than 1610612733 (since encode inflates the data by 4/3).
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetMaxEncodedToUtf8Length(int length) =>
        (uint) length > MaximumEncodeLength
            ? throw new ArgumentOutOfRangeException(nameof(length))
            : ((length + 2) / 3) * 4;
}

// Decoding
public static partial class Base64
{
    /// <summary>
    /// Decode the span of UTF-8 encoded text represented as base64 into binary data.
    /// If the input is not a multiple of 4, it will decode as much as it can, to the closest multiple of 4.
    /// </summary>
    /// <param name="utf8">The input span which contains UTF-8 encoded text in base64 that needs to be decoded.</param>
    /// <param name="bytes">The output span which contains the result of the operation, i.e. the decoded binary data.</param>
    /// <param name="bytesConsumed">The number of input bytes consumed during the operation. This can be used to slice the input for subsequent calls, if necessary.</param>
    /// <param name="bytesWritten">The number of bytes written into the output span. This can be used to slice the output for subsequent calls, if necessary.</param>
    /// <param name="isFinalBlock"><see langword="true"/> (default) when the input span contains the entire data to encode.
    /// Set to <see langword="true"/> when the source buffer contains the entirety of the data to encode.
    /// Set to <see langword="false"/> if this method is being called in a loop and if more input data may follow.
    /// At the end of the loop, call this (potentially with an empty source buffer) passing <see langword="true"/>.</param>
    /// <returns>It returns the OperationStatus enum values:
    /// - Done - on successful processing of the entire input span
    /// - DestinationTooSmall - if there is not enough space in the output span to fit the decoded input
    /// - NeedMoreData - only if <paramref name="isFinalBlock"/> is false and the input is not a multiple of 4, otherwise the partial input would be considered as InvalidData
    /// - InvalidData - if the input contains bytes outside of the expected base64 range, or if it contains invalid/more than two padding characters,
    ///   or if the input is incomplete (i.e. not a multiple of 4) and <paramref name="isFinalBlock"/> is <see langword="true"/>.
    /// </returns>
    public static OperationStatus DecodeFromUtf8(ReadOnlySpan<byte> utf8, Span<byte> bytes, out int bytesConsumed, out int bytesWritten, bool isFinalBlock = true) =>
        DecodeFromUtf8(utf8, bytes, out bytesConsumed, out bytesWritten, isFinalBlock, ignoreWhiteSpace: true);

    private static unsafe OperationStatus DecodeFromUtf8(ReadOnlySpan<byte> utf8, Span<byte> bytes, out int bytesConsumed, out int bytesWritten, bool isFinalBlock, bool ignoreWhiteSpace)
    {
        if (utf8.IsEmpty)
        {
            bytesConsumed = 0;
            bytesWritten = 0;
            return OperationStatus.Done;
        }

        fixed (byte* srcBytes = &MemoryMarshal.GetReference(utf8))
        fixed (byte* destBytes = &MemoryMarshal.GetReference(bytes))
        {
            int srcLength = utf8.Length & ~0x3;  // only decode input up to the closest multiple of 4.
            int destLength = bytes.Length;
            int maxSrcLength = srcLength;
            int decodedLength = GetMaxDecodedFromUtf8Length(srcLength);

            // max. 2 padding chars
            if (destLength < decodedLength - 2)
            {
                // For overflow see comment below
                maxSrcLength = destLength / 3 * 4;
            }

            byte* src = srcBytes;
            byte* dest = destBytes;
            byte* srcEnd = srcBytes + (uint) srcLength;
            byte* srcMax = srcBytes + (uint) maxSrcLength;

            //if (maxSrcLength >= 24)
            //{
            //    byte* end = srcMax - 45;
            //    if (Avx2.IsSupported && (end >= src))
            //    {
            //        Avx2Decode(ref src, ref dest, end, maxSrcLength, destLength, srcBytes, destBytes);

            //        if (src == srcEnd)
            //        {
            //            goto DoneExit;
            //        }
            //    }

            //    end = srcMax - 24;
            //    if ((Ssse3.IsSupported || AdvSimd.Arm64.IsSupported) && BitConverter.IsLittleEndian && (end >= src))
            //    {
            //        Vector128Decode(ref src, ref dest, end, maxSrcLength, destLength, srcBytes, destBytes);

            //        if (src == srcEnd)
            //        {
            //            goto DoneExit;
            //        }
            //    }
            //}

            // Last bytes could have padding characters, so process them separately and treat them as valid only if isFinalBlock is true
            // if isFinalBlock is false, padding characters are considered invalid
            int skipLastChunk = isFinalBlock ? 4 : 0;

            if (destLength >= decodedLength)
            {
                maxSrcLength = srcLength - skipLastChunk;
            }
            else
            {
                // This should never overflow since destLength here is less than int.MaxValue / 4 * 3 (i.e. 1610612733)
                // Therefore, (destLength / 3) * 4 will always be less than 2147483641
                Debug.Assert(destLength < (int.MaxValue / 4 * 3));
                maxSrcLength = (destLength / 3) * 4;
            }

            ref sbyte decodingMap = ref MemoryMarshal.GetReference((ReadOnlySpan<sbyte>) DecodingMap);
            srcMax = srcBytes + maxSrcLength;

            while (src < srcMax)
            {
                int result = Decode(src, ref decodingMap);

                if (result < 0)
                {
                    goto InvalidDataExit;
                }

                WriteThreeLowOrderBytes(dest, result);
                src += 4;
                dest += 3;
            }

            if (maxSrcLength != srcLength - skipLastChunk)
            {
                goto DestinationTooSmallExit;
            }

            // If input is less than 4 bytes, srcLength == sourceIndex == 0
            // If input is not a multiple of 4, sourceIndex == srcLength != 0
            if (src == srcEnd)
            {
                if (isFinalBlock)
                {
                    goto InvalidDataExit;
                }

                if (src == srcBytes + utf8.Length)
                {
                    goto DoneExit;
                }

                goto NeedMoreDataExit;
            }

            // if isFinalBlock is false, we will never reach this point

            // Handle last four bytes. There are 0, 1, 2 padding chars.
            uint t0 = srcEnd[-4];
            uint t1 = srcEnd[-3];
            uint t2 = srcEnd[-2];
            uint t3 = srcEnd[-1];

            int i0 = Unsafe.Add(ref decodingMap, (IntPtr) t0);
            int i1 = Unsafe.Add(ref decodingMap, (IntPtr) t1);

            i0 <<= 18;
            i1 <<= 12;

            i0 |= i1;

            byte* destMax = destBytes + (uint) destLength;

            if (t3 != EncodingPad)
            {
                int i2 = Unsafe.Add(ref decodingMap, (IntPtr) t2);
                int i3 = Unsafe.Add(ref decodingMap, (IntPtr) t3);

                i2 <<= 6;

                i0 |= i3;
                i0 |= i2;

                if (i0 < 0)
                {
                    goto InvalidDataExit;
                }
                if (dest + 3 > destMax)
                {
                    goto DestinationTooSmallExit;
                }

                WriteThreeLowOrderBytes(dest, i0);
                dest += 3;
            }
            else if (t2 != EncodingPad)
            {
                int i2 = Unsafe.Add(ref decodingMap, (IntPtr) t2);

                i2 <<= 6;

                i0 |= i2;

                if (i0 < 0)
                {
                    goto InvalidDataExit;
                }
                if (dest + 2 > destMax)
                {
                    goto DestinationTooSmallExit;
                }

                dest[0] = (byte) (i0 >> 16);
                dest[1] = (byte) (i0 >> 8);
                dest += 2;
            }
            else
            {
                if (i0 < 0)
                {
                    goto InvalidDataExit;
                }
                if (dest + 1 > destMax)
                {
                    goto DestinationTooSmallExit;
                }

                dest[0] = (byte) (i0 >> 16);
                dest += 1;
            }

            src += 4;

            if (srcLength != utf8.Length)
            {
                goto InvalidDataExit;
            }

DoneExit:
            bytesConsumed = (int) (src - srcBytes);
            bytesWritten = (int) (dest - destBytes);
            return OperationStatus.Done;

DestinationTooSmallExit:
            if (srcLength != utf8.Length && isFinalBlock)
            {
                goto InvalidDataExit; // if input is not a multiple of 4, and there is no more data, return invalid data instead
            }

            bytesConsumed = (int) (src - srcBytes);
            bytesWritten = (int) (dest - destBytes);
            return OperationStatus.DestinationTooSmall;

NeedMoreDataExit:
            bytesConsumed = (int) (src - srcBytes);
            bytesWritten = (int) (dest - destBytes);
            return OperationStatus.NeedMoreData;

InvalidDataExit:
            bytesConsumed = (int) (src - srcBytes);
            bytesWritten = (int) (dest - destBytes);
            return ignoreWhiteSpace ?
                InvalidDataFallback(utf8, bytes, ref bytesConsumed, ref bytesWritten, isFinalBlock) :
                OperationStatus.InvalidData;
        }

        static OperationStatus InvalidDataFallback(ReadOnlySpan<byte> utf8, Span<byte> bytes, ref int bytesConsumed, ref int bytesWritten, bool isFinalBlock)
        {
            utf8 = utf8.Slice(bytesConsumed);
            bytes = bytes.Slice(bytesWritten);

            OperationStatus status;
            do
            {
                int localConsumed = IndexOfAnyExceptWhiteSpace(utf8);
                if (localConsumed < 0)
                {
                    // The remainder of the input is all whitespace. Mark it all as having been consumed,
                    // and mark the operation as being done.
                    bytesConsumed += utf8.Length;
                    status = OperationStatus.Done;
                    break;
                }

                if (localConsumed == 0)
                {
                    // Non-whitespace was found at the beginning of the input. Since it wasn't consumed
                    // by the previous call to DecodeFromUtf8, it must be part of a Base64 sequence
                    // that was interrupted by whitespace or something else considered invalid.
                    // Fall back to block-wise decoding. This is very slow, but it's also very non-standard
                    // formatting of the input; whitespace is typically only found between blocks, such as
                    // when Convert.ToBase64String inserts a line break every 76 output characters.
                    return DecodeWithWhiteSpaceBlockwise(utf8, bytes, ref bytesConsumed, ref bytesWritten, isFinalBlock);
                }

                // Skip over the starting whitespace and continue.
                bytesConsumed += localConsumed;
                utf8 = utf8.Slice(localConsumed);

                // Try again after consumed whitespace
                status = DecodeFromUtf8(utf8, bytes, out localConsumed, out int localWritten, isFinalBlock, ignoreWhiteSpace: false);
                bytesConsumed += localConsumed;
                bytesWritten += localWritten;
                if (status is not OperationStatus.InvalidData)
                {
                    break;
                }

                utf8 = utf8.Slice(localConsumed);
                bytes = bytes.Slice(localWritten);
            }
            while (!utf8.IsEmpty);

            return status;
        }
    }

    private static OperationStatus DecodeWithWhiteSpaceBlockwise(ReadOnlySpan<byte> utf8, Span<byte> bytes, ref int bytesConsumed, ref int bytesWritten, bool isFinalBlock = true)
    {
        const int blockSize = 4;
        Span<byte> buffer = stackalloc byte[blockSize];
        OperationStatus status = OperationStatus.Done;

        while (!utf8.IsEmpty)
        {
            int encodedIdx = 0;
            int bufferIdx = 0;
            int skipped = 0;

            for (; encodedIdx < utf8.Length && (uint) bufferIdx < (uint) buffer.Length; ++encodedIdx)
            {
                if (IsWhiteSpace(utf8[encodedIdx]))
                {
                    skipped++;
                }
                else
                {
                    buffer[bufferIdx] = utf8[encodedIdx];
                    bufferIdx++;
                }
            }

            utf8 = utf8.Slice(encodedIdx);
            bytesConsumed += skipped;

            if (bufferIdx == 0)
            {
                continue;
            }

            bool hasAnotherBlock = utf8.Length >= blockSize && bufferIdx == blockSize;
            bool localIsFinalBlock = !hasAnotherBlock;

            // If this block contains padding and there's another block, then only whitespace may follow for being valid.
            if (hasAnotherBlock)
            {
                int paddingCount = GetPaddingCount(ref buffer[^1]);
                if (paddingCount > 0)
                {
                    hasAnotherBlock = false;
                    localIsFinalBlock = true;
                }
            }

            if (localIsFinalBlock && !isFinalBlock)
            {
                localIsFinalBlock = false;
            }

            status = DecodeFromUtf8(buffer.Slice(0, bufferIdx), bytes, out int localConsumed, out int localWritten, localIsFinalBlock, ignoreWhiteSpace: false);
            bytesConsumed += localConsumed;
            bytesWritten += localWritten;

            if (status != OperationStatus.Done)
            {
                return status;
            }

            // The remaining data must all be whitespace in order to be valid.
            if (!hasAnotherBlock)
            {
                for (int i = 0; i < utf8.Length; ++i)
                {
                    if (!IsWhiteSpace(utf8[i]))
                    {
                        // Revert previous dest increment, since an invalid state followed.
                        bytesConsumed -= localConsumed;
                        bytesWritten -= localWritten;

                        return OperationStatus.InvalidData;
                    }

                    bytesConsumed++;
                }

                break;
            }

            bytes = bytes.Slice(localWritten);
        }

        return status;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetPaddingCount(ref byte ptrToLastElement)
    {
        int padding = 0;

        if (ptrToLastElement == EncodingPad) padding++;
        if (Unsafe.Subtract(ref ptrToLastElement, 1) == EncodingPad) padding++;

        return padding;
    }

    /// <summary>
    /// Returns the maximum length (in bytes) of the result if you were to decode base 64 encoded text within a byte span of size "length".
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified <paramref name="length"/> is less than 0.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetMaxDecodedFromUtf8Length(int length) => length < 0 ? throw new ArgumentOutOfRangeException(nameof(length)) : (length >> 2) * 3;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe int Decode(byte* encodedBytes, ref sbyte decodingMap)
    {
        uint t0 = encodedBytes[0];
        uint t1 = encodedBytes[1];
        uint t2 = encodedBytes[2];
        uint t3 = encodedBytes[3];

        int i0 = Unsafe.Add(ref decodingMap, t0);
        int i1 = Unsafe.Add(ref decodingMap, t1);
        int i2 = Unsafe.Add(ref decodingMap, t2);
        int i3 = Unsafe.Add(ref decodingMap, t3);

        i0 <<= 18;
        i1 <<= 12;
        i2 <<= 6;

        i0 |= i3;
        i1 |= i2;

        i0 |= i1;
        return i0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void WriteThreeLowOrderBytes(byte* destination, int value)
    {
        destination[0] = (byte) (value >> 16);
        destination[1] = (byte) (value >> 8);
        destination[2] = (byte) value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOfAnyExceptWhiteSpace(ReadOnlySpan<byte> span)
    {
        for (int i = 0; i < span.Length; i++)
        {
            if (!IsWhiteSpace(span[i]))
            {
                return i;
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsWhiteSpace(int value)
    {
        if (Environment.Is64BitProcess)
        {
            // For description see https://github.com/dotnet/runtime/blob/48e74187cb15386c29eedaa046a5ee2c7ddef161/src/libraries/Common/src/System/HexConverter.cs#L314-L330
            // Lookup bit mask for "\t\n\r ".
            const ulong magicConstant = 0xC800010000000000UL;
            ulong i = (uint) value - '\t';
            ulong shift = magicConstant << (int) i;
            ulong mask = i - 64;
            return (long) (shift & mask) < 0;
        }

        if (value < 32)
        {
            const int BitMask = (1 << (int) '\t') | (1 << (int) '\n') | (1 << (int) '\r');
            return ((1 << value) & BitMask) != 0;
        }

        return value == 32;
    }

    private static readonly sbyte[] DecodingMap = GenerateDecodingMap();

    // Pre-computing this table using a custom string(EncodingMap) and GenerateDecodingMap (found in tests)
    //private static ReadOnlySpan<sbyte> DecodingMap =>
    //[
    //    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    //    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    //    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,  0, -1, -1,
    //     1,  2,  3,  4,  5,  6,  7,  8,  9, 10, -1, -1, -1, -1, -1, -1,
    //    -1, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
    //    26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, -1, -1, -1, -1, 37,
    //    -1, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52,
    //    53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, -1, -1, -1, -1, -1,
    //    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    //    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    //    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    //    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    //    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    //    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    //    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    //    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
    //];

    private const sbyte InvalidByte = -1; // Designating -1 for invalid bytes in the decoding map

    private static sbyte[] GenerateDecodingMap()
    {
        sbyte[] data = new sbyte[256]; // 0 to byte.MaxValue (255)
        var map = Alphabet;
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = InvalidByte;
        }
        for (int i = 0; i < map.Length; i++)
        {
            data[map[i]] = (sbyte) i;
        }
        //Assert.True(s_decodingMap.AsSpan().SequenceEqual(data));
        return data;
    }
}

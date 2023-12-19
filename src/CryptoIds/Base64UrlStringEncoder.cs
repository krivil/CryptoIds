namespace CryptoIds;

using System.Buffers;
using System.Diagnostics;


public static class Base64UrlStringEncoder
{
    private const int StackAllocThreshold = 128;

    public static int Decode(ReadOnlySpan<byte> input, Span<byte> output)
    {
        int inputLength = input.Length;

        if (inputLength == 0) return 0;

        // Assumption: input is base64url encoded without padding and contains no whitespace.

        int paddingCharsToAdd = GetPaddingCharsToAddForDecode(inputLength);
        int arraySizeRequired = checked(inputLength + paddingCharsToAdd);

        Debug.Assert(arraySizeRequired % 4 == 0, "Invariant: Array length must be a multiple of 4.");

        char[]? bufferToReturnToPool = null;

        Span<char> buffer = arraySizeRequired <= StackAllocThreshold
            ? stackalloc char[arraySizeRequired]
            : bufferToReturnToPool = ArrayPool<char>.Shared.Rent(arraySizeRequired);
        try
        {
            // Copy input into buffer, fixing up '-' -> '+' and '_' -> '/'.
            for (int i = 0; i < inputLength; i++)
            {
                byte ch = input[i];
                buffer[i] = ch switch
                {
                    (byte) '-' => '+',
                    (byte) '_' => '/',
                    _ => (char) ch
                };
            }

            // Add the padding characters back.
            for (int i = inputLength; paddingCharsToAdd > 0; i++, paddingCharsToAdd--)
            {
                buffer[i] = '=';
            }

            // Decode.
            // If the caller provided invalid base64 chars, they'll be caught here.
            _ = Convert.TryFromBase64Chars(buffer[..arraySizeRequired], output, out int bytesWritten);

            return bytesWritten;
        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<char>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static int Decode(ReadOnlySpan<char> input, Span<byte> output)
    {
        int inputLength = input.Length;

        if (inputLength == 0) return 0;

        // Assumption: input is base64url encoded without padding and contains no whitespace.

        int paddingCharsToAdd = GetPaddingCharsToAddForDecode(inputLength);
        int arraySizeRequired = checked(inputLength + paddingCharsToAdd);

        Debug.Assert(arraySizeRequired % 4 == 0, "Invariant: Array length must be a multiple of 4.");

        char[]? bufferToReturnToPool = null;

        Span<char> buffer = arraySizeRequired <= StackAllocThreshold
            ? stackalloc char[arraySizeRequired]
            : bufferToReturnToPool = ArrayPool<char>.Shared.Rent(arraySizeRequired);
        try
        {
            // Copy input into buffer, fixing up '-' -> '+' and '_' -> '/'.
            for (int i = 0; i < inputLength; i++)
            {
                char ch = input[i];
                buffer[i] = ch switch
                {
                    '-' => '+',
                    '_' => '/',
                    _ => ch
                };
            }

            // Add the padding characters back.
            for (int i = inputLength; paddingCharsToAdd > 0; i++, paddingCharsToAdd--)
            {
                buffer[i] = '=';
            }

            // Decode.
            // If the caller provided invalid base64 chars, they'll be caught here.
            _ = Convert.TryFromBase64Chars(buffer[..arraySizeRequired], output, out int bytesWritten);

            return bytesWritten;
        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<char>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static int Encode(ReadOnlySpan<byte> input, Span<byte> output)
    {
        int inputLength = input.Length;

        if (inputLength == 0) return 0;

        int bufferLength = GetArraySizeRequiredToEncodeWithPadding(inputLength);

        Debug.Assert(bufferLength % 4 == 0, "Invariant: Array length must be a multiple of 4.");

        int outputLength = bufferLength - GetPaddingCharsToAddForEncode(inputLength);

        if (output.Length < outputLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(output),
                $"Output buffer must be at least {bufferLength} characters long."
            );
        }

        char[]? bufferToReturnToPool = null;
        Span<char> buffer = bufferLength <= StackAllocThreshold
            ? stackalloc char[bufferLength]
            : bufferToReturnToPool = ArrayPool<char>.Shared.Rent(bufferLength);
        try
        {
            // Use base64url encoding with no padding characters. See RFC 4648, Sec. 5.

            _ = Convert.TryToBase64Chars(input, buffer, out int charsWritten);

            // Fix up '+' -> '-' and '/' -> '_'. Drop padding characters.
            for (int i = 0; i < charsWritten; i++)
            {
                char ch = buffer[i];
                switch (ch)
                {
                    case '+':
                        output[i] = (byte) '-';
                        break;
                    case '/':
                        output[i] = (byte) '_';
                        break;
                    case '=':
                        return i;
                    default:
                        output[i] = (byte) ch; // It's ASCII characters
                        break;
                }
            }

            return charsWritten;
        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<char>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static int Encode(ReadOnlySpan<byte> input, Span<char> output)
    {
        int inputLength = input.Length;

        if (inputLength == 0) return 0;

        int bufferLength = GetArraySizeRequiredToEncodeWithPadding(inputLength);

        Debug.Assert(bufferLength % 4 == 0, "Invariant: Array length must be a multiple of 4.");

        int outputLength = bufferLength - GetPaddingCharsToAddForEncode(inputLength);

        if (output.Length < outputLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(output),
                $"Output buffer must be at least {bufferLength} characters long."
            );
        }

        char[]? bufferToReturnToPool = null;
        Span<char> buffer = bufferLength <= StackAllocThreshold
            ? stackalloc char[bufferLength]
            : bufferToReturnToPool = ArrayPool<char>.Shared.Rent(bufferLength);
        try
        {
            // Use base64url encoding with no padding characters. See RFC 4648, Sec. 5.

            _ = Convert.TryToBase64Chars(input, buffer, out int charsWritten);

            // Fix up '+' -> '-' and '/' -> '_'. Drop padding characters.
            for (int i = 0; i < charsWritten; i++)
            {
                char ch = buffer[i];
                switch (ch)
                {
                    case '+':
                        output[i] = '-';
                        break;
                    case '/':
                        output[i] = '_';
                        break;
                    case '=':
                        return i;
                    default:
                        output[i] = ch;
                        break;
                }
            }

            return charsWritten;
        }
        finally
        {
            if (bufferToReturnToPool != null)
            {
                ArrayPool<char>.Shared.Return(bufferToReturnToPool);
            }
        }
    }

    public static int GetRequiredLengthForEncode(int length) =>
        GetArraySizeRequiredToEncodeWithoutPadding(length);

    public static int GetRequiredLengthForEncode(ReadOnlySpan<byte> input) =>
        GetArraySizeRequiredToEncodeWithoutPadding(input.Length);

    public static int GetMaximumRequiredLengthForDecode(ReadOnlySpan<char> input) =>
        GetOriginalByteCountFromBase64WithoutPadding(input.Length);

    public static int GetMaximumRequiredLengthForDecode(ReadOnlySpan<byte> input) =>
        GetOriginalByteCountFromBase64WithoutPadding(input.Length);


    private static int GetPaddingCharsToAddForDecode(int inputLength) =>
        (4 - (inputLength % 4)) % 4; // Modulo by 4 to handle cases where length is already a multiple of 4

    private static int GetOriginalByteCountFromBase64WithoutPadding(int base64StringLength)
    {
        // Calculate missing padding
        int paddingCount = (4 - (base64StringLength % 4)) % 4; // Modulo by 4 in case the length is already a multiple of 4

        // Calculate bytes
        int totalLengthWithPadding = checked(base64StringLength + paddingCount);
        return checked((totalLengthWithPadding / 4) * 3) - paddingCount;
    }

    private static int GetArraySizeRequiredToEncodeWithPadding(int count)
    {
        int numWholeOrPartialInputBlocks = checked(count + 2) / 3;
        return checked(numWholeOrPartialInputBlocks * 4);
    }

    private static int GetPaddingCharsToAddForEncode(int count) =>
        (3 - (count % 3)) % 3; // Modulo by 3 to handle cases where count is already a multiple of 3

    private static int GetArraySizeRequiredToEncodeWithoutPadding(int count) =>
        GetArraySizeRequiredToEncodeWithPadding(count) - GetPaddingCharsToAddForEncode(count);
}
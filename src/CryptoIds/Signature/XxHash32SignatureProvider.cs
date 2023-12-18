namespace CryptoIds.Signature;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public sealed class XxHash32SignatureProvider : ISignatureProvider
{
    private const int ConstSignatureLengthInBytes = sizeof(uint);
    // ReSharper disable StaticMemberInGenericType - it's fine
    private static readonly object HasherLock = new();
    private static readonly System.IO.Hashing.XxHash32 Hasher = new();
    // ReSharper restore StaticMemberInGenericType

    public static readonly XxHash32SignatureProvider Instance = new();

    public static int SignatureLengthInBytes => ConstSignatureLengthInBytes;

    public int SignatureLength => ConstSignatureLengthInBytes;

    public int Sign<T>(T id, ReadOnlySpan<byte> key, Span<byte> destination) where T : unmanaged
    {
        if (destination.Length < ConstSignatureLengthInBytes)
        {
            return 0;
        }

        Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<T>()];
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), id);

        int retVal;

        lock (HasherLock)
        {
            Hasher.Append(key);
            Hasher.Append(buffer);
            retVal = Hasher.GetHashAndReset(destination);
        }

        return retVal;
    }

    public bool Verify<T>(T id, ReadOnlySpan<byte> key, ReadOnlySpan<byte> signature) where T : unmanaged
    {
        Span<byte> destination = stackalloc byte[SignatureLength];
        return Sign(id, key, destination) == signature.Length && signature.SequenceEqual(destination);
    }
}
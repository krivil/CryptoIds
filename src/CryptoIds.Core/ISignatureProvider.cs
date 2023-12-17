namespace CryptoIds.Core;

using CryptoIds.Core.Signature;

public interface ISignatureProvider
{
    int SignatureLength { get; }

    int Sign<T>(T id, ReadOnlySpan<byte> key, Span<byte> destination) where T : unmanaged;

    bool Verify<T>(T id, ReadOnlySpan<byte> key, ReadOnlySpan<byte> signature) where T : unmanaged;
}

public static class SignatureProviderTypeExtensions
{
    public static ISignatureProvider ToSignatureProvider(this SignatureProviderType type) =>
        type switch
        {
            SignatureProviderType.Crc32 => Crc32SignatureProvider.Instance,
            SignatureProviderType.Crc64 => Crc64SignatureProvider.Instance,
            SignatureProviderType.XxHash32 => XxHash32SignatureProvider.Instance,
            SignatureProviderType.XxHash64 => XxHash64SignatureProvider.Instance,
            SignatureProviderType.Md5 => HmacMd5SignatureProvider.Instance,
            SignatureProviderType.Sha1 => HmacSha1SignatureProvider.Instance,
            SignatureProviderType.Sha256 => HmacSha256SignatureProvider.Instance,
            SignatureProviderType.Sha384 => HmacSha384SignatureProvider.Instance,
            SignatureProviderType.Sha512 => HmacSha512SignatureProvider.Instance,
            SignatureProviderType.Sha3_256 => HmacSha3_256SignatureProvider.Instance,
            SignatureProviderType.Sha3_384 => HmacSha3_384SignatureProvider.Instance,
            SignatureProviderType.Sha3_512 => HmacSha3_512SignatureProvider.Instance,
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unknown HMAC algorithm type: {type}")
        };
}
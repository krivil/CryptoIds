namespace CryptoIds.Benchmarks;

using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using CryptoIds.Signature;

[MemoryDiagnoser]
public class Benchmarks
{
    private static readonly int Id = 123456789;
    private static readonly byte[] Key = new byte[128];
    private static readonly byte[] KeyB = new byte[128];
    private static readonly Crc32SignatureProvider SignerCrc32 = Crc32SignatureProvider.Instance;
    private static readonly XxHash32SignatureProvider SignerXxHash32 = XxHash32SignatureProvider.Instance;
    private static readonly HmacMd5SignatureProvider SignerMd5 = HmacMd5SignatureProvider.Instance;
    private static readonly HmacSha1SignatureProvider SignerSha1 = HmacSha1SignatureProvider.Instance;
    private static readonly HmacSha256SignatureProvider SignerSha256 = HmacSha256SignatureProvider.Instance;
    private static readonly HmacSha3_256SignatureProvider SignerSha3256 = HmacSha3_256SignatureProvider.Instance;

    private static readonly CryptoIdContext ContextNotEncrypted = CryptoIdContext.CreateFromPassword("password", encrypt: false);
    private static readonly CryptoIdContext ContextEncrypted = CryptoIdContext.CreateFromPassword("password", encrypt: true);

    static Benchmarks()
    {
        RandomNumberGenerator.Fill(Key);
        RandomNumberGenerator.Fill(KeyB);
    }

    [Benchmark, BenchmarkCategory("CRC32", "IdOperations", "Bytes")]
    public void EncodeInt32Crc32_IdOperations_ToUtf8Bytes()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(SignerCrc32);

        Span<byte> encoded = stackalloc byte[stringLength];

        _ = IdOperations.TrySignAndEncode(Id, Key, SignerCrc32, encoded);
    }

    [Benchmark, BenchmarkCategory("CRC32", "IdOperations", "Chars")]
    public void EncodeInt32Crc32_IdOperations_ToChars()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(SignerCrc32);

        Span<char> encoded = stackalloc char[stringLength];

        _ = IdOperations.TrySignAndEncode(Id, Key, SignerCrc32, encoded);
    }

    [Benchmark, BenchmarkCategory("XXHash32", "Context", "Encrypted")]
    public void EncodeInt32XxHash32_WithContextEncrypted()
    {
        int stringLength = ContextEncrypted.GetRequiredLengthForEncode<int>();

        Span<byte> encoded = stackalloc byte[stringLength];

        _ = ContextEncrypted.TryEncode(Id, encoded);
    }

    [Benchmark, BenchmarkCategory("XXHash32", "Context", "NotEncrypted")]
    public void EncodeInt32XxHash32_WithContextNotEncrypted()
    {
        int stringLength = ContextNotEncrypted.GetRequiredLengthForEncode<int>();

        Span<byte> encoded = stackalloc byte[stringLength];

        _ = ContextNotEncrypted.TryEncode(Id, encoded);
    }

    [Benchmark, BenchmarkCategory("XXHash32", "IdOperations", "Bytes")]
    public void EncodeInt32XxHash32_IdOperations_ToUtf8Bytes()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(SignerXxHash32);

        Span<byte> encoded = stackalloc byte[stringLength];

        _ = IdOperations.TrySignAndEncode(Id, Key, SignerXxHash32, encoded);
    }

    [Benchmark, BenchmarkCategory("XXHash32", "IdOperations", "Chars")]
    public void EncodeInt32XxHash32_IdOperations_ToChars()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(SignerXxHash32);

        Span<char> encoded = stackalloc char[stringLength];

        _ = IdOperations.TrySignAndEncode(Id, Key, SignerXxHash32, encoded);
    }

    [Benchmark, BenchmarkCategory("MD5", "IdOperations", "Bytes")]
    public void EncodeInt32Md5_IdOperations_ToUtf8Bytes()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(SignerMd5);

        Span<byte> encoded = stackalloc byte[stringLength];

        _ = IdOperations.TrySignAndEncode(Id, Key, SignerMd5, encoded);
    }

    [Benchmark, BenchmarkCategory("MD5", "IdOperations", "Chars")]
    public void EncodeInt32Md5_IdOperations_ToChars()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(SignerMd5);

        Span<char> encoded = stackalloc char[stringLength];

        _ = IdOperations.TrySignAndEncode(Id, Key, SignerMd5, encoded);
    }

    [Benchmark, BenchmarkCategory("Sha1", "IdOperations", "Bytes")]
    public void EncodeInt32Sha1_IdOperations_ToUtf8Bytes()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(SignerSha1);

        Span<byte> encoded = stackalloc byte[stringLength];

        _ = IdOperations.TrySignAndEncode(Id, Key, SignerSha1, encoded);
    }

    [Benchmark, BenchmarkCategory("Sha1", "IdOperations", "Chars")]
    public void EncodeInt32Sha1_IdOperations_ToChars()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(SignerSha1);

        Span<char> encoded = stackalloc char[stringLength];

        _ = IdOperations.TrySignAndEncode(Id, Key, SignerSha1, encoded);
    }

    [Benchmark, BenchmarkCategory("Sha256", "IdOperations", "Bytes")]
    public void EncodeInt32Sha256_IdOperations_ToUtf8Bytes()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(SignerSha256);

        Span<byte> encoded = stackalloc byte[stringLength];

        _ = IdOperations.TrySignAndEncode(Id, Key, SignerSha256, encoded);
    }

    [Benchmark, BenchmarkCategory("Sha256", "IdOperations", "Chars")]
    public void EncodeInt32Sha256_IdOperations_ToChars()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(SignerSha256);

        Span<char> encoded = stackalloc char[stringLength];

        _ = IdOperations.TrySignAndEncode(Id, Key, SignerSha256, encoded);
    }

    [Benchmark, BenchmarkCategory("Sha3_256", "IdOperations", "Bytes")]
    public void EncodeInt32Sha3_256_IdOperations_ToUtf8Bytes()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(SignerSha3256);

        Span<byte> encoded = stackalloc byte[stringLength];

        _ = IdOperations.TrySignAndEncode(Id, Key, SignerSha3256, encoded);
    }

    [Benchmark, BenchmarkCategory("Sha3_256", "IdOperations", "Chars")]
    public void EncodeInt32Sha3_256_IdOperations_ToChars()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(SignerSha3256);

        Span<char> encoded = stackalloc char[stringLength];

        _ = IdOperations.TrySignAndEncode(Id, Key, SignerSha3256, encoded);
    }

    // [Benchmark]
    // public string EncodeInt32Crc32_newString()
    // {
    //     int id = 123456789;
    //     Crc32SignatureProvider signer = new();
    //
    //     int stringLength = SignedId.GetRequiredLengthForEncode<int>(signer);
    //     
    //     Span<char> encoded = stackalloc char[stringLength];
    //     
    //     SignedId.TrySignAndEncode(id, _key, signer, encoded);
    //     
    //     return new string(encoded);
    // }
    //
    // [Benchmark]
    // public string EncodeInt32Crc32_WithXor()
    // {
    //     int id = 123456789;
    //     Crc32SignatureProvider signer = new();
    //
    //     int stringLength = SignedId.GetRequiredLengthForEncode<int>(signer);
    //     
    //     Span<char> encoded = stackalloc char[stringLength];
    //     
    //     SignedId.TrySignAndXorAndEncode(id, _key, _keyB, signer, encoded);
    //     
    //     return encoded.ToString();
    // }
}
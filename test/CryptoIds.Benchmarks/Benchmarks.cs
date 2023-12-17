namespace CryptoIds.Benchmarks;

using BenchmarkDotNet.Attributes;
using CryptoIds.Core;
using CryptoIds.Core.Signature;
using System.Security.Cryptography;

[MemoryDiagnoser]
public class Benchmarks
{
    private static readonly int _id = 123456789;
    private static readonly byte[] _key = new byte[128];
    private static readonly byte[] _keyB = new byte[128];
    private static readonly Crc32SignatureProvider _signerCrc32 = Crc32SignatureProvider.Instance;
    private static readonly XxHash32SignatureProvider _signerXxHash32 = XxHash32SignatureProvider.Instance;
    private static readonly HmacMd5SignatureProvider _signerMd5 = HmacMd5SignatureProvider.Instance;
    private static readonly HmacSha1SignatureProvider _signerSha1 = HmacSha1SignatureProvider.Instance;
    private static readonly HmacSha256SignatureProvider _signerSha256 = HmacSha256SignatureProvider.Instance;
    private static readonly HmacSha3_256SignatureProvider _signerSha3_256 = HmacSha3_256SignatureProvider.Instance;

    private static readonly CryptoIdContext _contextNotEncrypted = CryptoIdContext.CreateFromPassword("password", encrypt: false);
    private static readonly CryptoIdContext _contextEncrypted = CryptoIdContext.CreateFromPassword("password", encrypt: true);

    static Benchmarks()
    {
        RandomNumberGenerator.Fill(_key);
        RandomNumberGenerator.Fill(_keyB);
    }

    [Benchmark, BenchmarkCategory("CRC32", "IdOperations", "Bytes")]
    public void EncodeInt32Crc32_IdOperations_ToUtf8Bytes()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(_signerCrc32);

        Span<byte> encoded = stackalloc byte[stringLength];

        IdOperations.TrySignAndEncode(_id, _key, _signerCrc32, encoded);
    }

    [Benchmark, BenchmarkCategory("CRC32", "IdOperations", "Chars")]
    public void EncodeInt32Crc32_IdOperations_ToChars()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(_signerCrc32);

        Span<char> encoded = stackalloc char[stringLength];

        IdOperations.TrySignAndEncode(_id, _key, _signerCrc32, encoded);
    }
    
    [Benchmark, BenchmarkCategory("XXHash32", "Context", "Encrypted")]
    public void EncodeInt32XxHash32_WithContextEncrypted()
    {
        int stringLength = _contextEncrypted.GetRequiredLengthForEncode<int>();

        Span<byte> encoded = stackalloc byte[stringLength];
        
        _contextEncrypted.TryEncode(_id, encoded);
    }
    
    [Benchmark, BenchmarkCategory("XXHash32", "Context", "NotEncrypted")]
    public void EncodeInt32XxHash32_WithContextNotEncrypted()
    {
        int stringLength = _contextNotEncrypted.GetRequiredLengthForEncode<int>();

        Span<byte> encoded = stackalloc byte[stringLength];
        
        _contextNotEncrypted.TryEncode(_id, encoded);
    }

    [Benchmark, BenchmarkCategory("XXHash32", "IdOperations", "Bytes")]
    public void EncodeInt32XxHash32_IdOperations_ToUtf8Bytes()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(_signerXxHash32);

        Span<byte> encoded = stackalloc byte[stringLength];

        IdOperations.TrySignAndEncode(_id, _key, _signerXxHash32, encoded);
    }

    [Benchmark, BenchmarkCategory("XXHash32", "IdOperations", "Chars")]
    public void EncodeInt32XxHash32_IdOperations_ToChars()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(_signerXxHash32);

        Span<char> encoded = stackalloc char[stringLength];

        IdOperations.TrySignAndEncode(_id, _key, _signerXxHash32, encoded);
    }

    [Benchmark, BenchmarkCategory("MD5", "IdOperations", "Bytes")]
    public void EncodeInt32Md5_IdOperations_ToUtf8Bytes()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(_signerMd5);

        Span<byte> encoded = stackalloc byte[stringLength];

        IdOperations.TrySignAndEncode(_id, _key, _signerMd5, encoded);
    }

    [Benchmark, BenchmarkCategory("MD5", "IdOperations", "Chars")]
    public void EncodeInt32Md5_IdOperations_ToChars()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(_signerMd5);

        Span<char> encoded = stackalloc char[stringLength];

        IdOperations.TrySignAndEncode(_id, _key, _signerMd5, encoded);
    }

    [Benchmark, BenchmarkCategory("Sha1", "IdOperations", "Bytes")]
    public void EncodeInt32Sha1_IdOperations_ToUtf8Bytes()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(_signerSha1);

        Span<byte> encoded = stackalloc byte[stringLength];

        IdOperations.TrySignAndEncode(_id, _key, _signerSha1, encoded);
    }

    [Benchmark, BenchmarkCategory("Sha1", "IdOperations", "Chars")]
    public void EncodeInt32Sha1_IdOperations_ToChars()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(_signerSha1);

        Span<char> encoded = stackalloc char[stringLength];

        IdOperations.TrySignAndEncode(_id, _key, _signerSha1, encoded);
    }

    [Benchmark, BenchmarkCategory("Sha256", "IdOperations", "Bytes")]
    public void EncodeInt32Sha256_IdOperations_ToUtf8Bytes()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(_signerSha256);

        Span<byte> encoded = stackalloc byte[stringLength];

        IdOperations.TrySignAndEncode(_id, _key, _signerSha256, encoded);
    }

    [Benchmark, BenchmarkCategory("Sha256", "IdOperations", "Chars")]
    public void EncodeInt32Sha256_IdOperations_ToChars()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(_signerSha256);

        Span<char> encoded = stackalloc char[stringLength];

        IdOperations.TrySignAndEncode(_id, _key, _signerSha256, encoded);
    }

    [Benchmark, BenchmarkCategory("Sha3_256", "IdOperations", "Bytes")]
    public void EncodeInt32Sha3_256_IdOperations_ToUtf8Bytes()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(_signerSha3_256);

        Span<byte> encoded = stackalloc byte[stringLength];

        IdOperations.TrySignAndEncode(_id, _key, _signerSha3_256, encoded);
    }

    [Benchmark, BenchmarkCategory("Sha3_256", "IdOperations", "Chars")]
    public void EncodeInt32Sha3_256_IdOperations_ToChars()
    {
        int stringLength = IdOperations.GetRequiredLengthForEncode<int>(_signerSha3_256);

        Span<char> encoded = stackalloc char[stringLength];

        IdOperations.TrySignAndEncode(_id, _key, _signerSha3_256, encoded);
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
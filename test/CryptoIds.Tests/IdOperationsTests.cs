namespace CryptoIds.Tests;

public class IdOperationsTests
{
    private static void TestSignAndVerify<T>(T id, byte[] key, ISignatureProvider provider)
        where T : unmanaged
    {
        Span<char> output = stackalloc char[IdOperations.GetRequiredLengthForEncode<T>(provider)];
        int bytesWritten = IdOperations.TrySignAndEncode(id, key, provider, output);
        Assert.True(bytesWritten > 0, "bytesWritten > 0");

        var encoded = output[..bytesWritten];
        bool success = IdOperations.TryDecodeAndValidate(encoded, key, provider, out T decoded);
        Assert.True(success, "success");
        Assert.Equal(id, decoded);
    }

    [Fact]
    public void TestSignAndVerifyCrc32()
    {
        var provider = SignatureProviderType.Crc32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifyCrc32MultiThreaded()
    {
        var provider = SignatureProviderType.Crc32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        });
    }

    [Fact]
    public void TestSignAndVerifyCrc64()
    {
        var provider = SignatureProviderType.Crc64.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifyCrc64MultiThreaded()
    {
        var provider = SignatureProviderType.Crc64.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        });
    }


    [Fact]
    public void TestSignAndVerifyXxHash32()
    {
        var provider = SignatureProviderType.XxHash32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifyXxHash32MultiThreaded()
    {
        var provider = SignatureProviderType.XxHash32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        });
    }

    [Fact]
    public void TestSignAndVerifyMd5()
    {
        var provider = SignatureProviderType.Md5.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifyMd5MultiThreaded()
    {
        var provider = SignatureProviderType.Md5.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        });
    }

    [Fact]
    public void TestSignAndVerifySha1()
    {
        var provider = SignatureProviderType.Sha1.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifySha1MultiThreaded()
    {
        var provider = SignatureProviderType.Sha1.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        });
    }

    [Fact]
    public void TestSignAndVerifySha256()
    {
        var provider = SignatureProviderType.Sha256.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifySha256MultiThreaded()
    {
        var provider = SignatureProviderType.Sha256.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        });
    }

    [Fact]
    public void TestSignAndVerifySha512()
    {
        var provider = SignatureProviderType.Sha512.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifySha512MultiThreaded()
    {
        var provider = SignatureProviderType.Sha512.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerify(id, key, provider);
        });
    }

    private static void TestSignAndVerifyAndEncrypt<T>(T id, byte[] key, byte[] key2, ISignatureProvider provider)
        where T : unmanaged
    {
        Span<char> output = stackalloc char[IdOperations.GetRequiredLengthForEncode<T>(provider)];
        int bytesWritten = IdOperations.TrySignAndXorAndEncode(id, key, key2, provider, output);
        Assert.True(bytesWritten > 0, "bytesWritten > 0");

        var encoded = output[..bytesWritten];
        bool success = IdOperations.TryDecodeAndXorAndValidate(encoded, key, key2, provider, out T decoded);
        Assert.True(success, "success");
        Assert.Equal(id, decoded);
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptCrc32()
    {
        var provider = SignatureProviderType.Crc32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptCrc32MultiThreaded()
    {
        var provider = SignatureProviderType.Crc32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        });
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptCrc64()
    {
        var provider = SignatureProviderType.Crc64.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptCrc64MultiThreaded()
    {
        var provider = SignatureProviderType.Crc64.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        });
    }


    [Fact]
    public void TestSignAndVerifyAndEncryptXxHash32()
    {
        var provider = SignatureProviderType.XxHash32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptXxHash32MultiThreaded()
    {
        var provider = SignatureProviderType.XxHash32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        });
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptMd5()
    {
        var provider = SignatureProviderType.Md5.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptMd5MultiThreaded()
    {
        var provider = SignatureProviderType.Md5.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        });
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptSha1()
    {
        var provider = SignatureProviderType.Sha1.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptSha1MultiThreaded()
    {
        var provider = SignatureProviderType.Sha1.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        });
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptSha256()
    {
        var provider = SignatureProviderType.Sha256.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptSha256MultiThreaded()
    {
        var provider = SignatureProviderType.Sha256.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        });
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptSha512()
    {
        var provider = SignatureProviderType.Sha512.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        }
    }

    [Fact]
    public void TestSignAndVerifyAndEncryptSha512MultiThreaded()
    {
        var provider = SignatureProviderType.Sha512.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] key2 = new byte[128];
        random.NextBytes(key2);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.Next();
            TestSignAndVerifyAndEncrypt(id, key, key2, provider);
        });
    }
}
namespace CryptoIds.Tests;

using CryptoIds;

public class SignatureProviderTests
{
    [Fact]
    public void TestCrc32()
    {
        var provider = SignatureProviderType.Crc32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestCrc32MultiThreaded()
    {
        var provider = SignatureProviderType.Crc32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        });
    }

    [Fact]
    public void TestCrc32_Int64()
    {
        var provider = SignatureProviderType.Crc32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestCrc32_Int64_MultiThreaded()
    {
        var provider = SignatureProviderType.Crc32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            long id = random.NextInt64();
            TestSignature(provider, id, key);
        });
    }

    [Fact]
    public void TestCrc64()
    {
        var provider = SignatureProviderType.Crc64.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestCrc64MultiThreaded()
    {
        var provider = SignatureProviderType.Crc64.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            int id = random.Next();
            TestSignature(provider, id, key); ;
        });
    }

    [Fact]
    public void TestCrc64_Int64()
    {
        var provider = SignatureProviderType.Crc64.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestCrc64_Int64_MultiThreaded()
    {
        var provider = SignatureProviderType.Crc64.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            long id = random.NextInt64();
            TestSignature(provider, id, key);
        });
    }

    [Fact]
    public void TestXxHash32()
    {
        var provider = SignatureProviderType.XxHash32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestXxHash32MultiThreaded()
    {
        var provider = SignatureProviderType.XxHash32.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        });
    }

    [Fact]
    public void TestXxHash64()
    {
        var provider = SignatureProviderType.XxHash64.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestXxHash64MultiThreaded()
    {
        var provider = SignatureProviderType.XxHash64.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        });
    }

    [Fact]
    public void TestMD5()
    {
        var provider = SignatureProviderType.Md5.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestMD5MultiThreaded()
    {
        var provider = SignatureProviderType.Md5.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        });
    }

    [Fact]
    public void TestSHA1()
    {
        var provider = SignatureProviderType.Sha1.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestSHA1MultiThreaded()
    {
        var provider = SignatureProviderType.Sha1.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        });
    }

    [Fact]
    public void TestSHA256()
    {
        var provider = SignatureProviderType.Sha256.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestSHA256MultiThreaded()
    {
        var provider = SignatureProviderType.Sha256.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            int id = random.Next();
            TestSignature(provider, id, key); ;
        });
    }

    [Fact]
    public void TestSHA384()
    {
        var provider = SignatureProviderType.Sha384.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestSHA384MultiThreaded()
    {
        var provider = SignatureProviderType.Sha384.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        });
    }

    [Fact]
    public void TestSHA512()
    {
        var provider = SignatureProviderType.Sha512.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestSHA512MultiThreaded()
    {
        var provider = SignatureProviderType.Sha512.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        });
    }

    [Fact]
    public void TestSHA3_256()
    {
        var provider = SignatureProviderType.Sha3_256.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestSHA3_256MultiThreaded()
    {
        var provider = SignatureProviderType.Sha3_256.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        });
    }

    [Fact]
    public void TestSHA3_384()
    {
        var provider = SignatureProviderType.Sha3_384.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestSHA3_384MultiThreaded()
    {
        var provider = SignatureProviderType.Sha3_384.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        });
    }

    [Fact]
    public void TestSHA3_512()
    {
        var provider = SignatureProviderType.Sha3_512.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        }
    }

    [Fact]
    public void TestSHA3_512MultiThreaded()
    {
        var provider = SignatureProviderType.Sha3_512.ToSignatureProvider();
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);

        Parallel.For(0, 1000, i =>
        {
            int id = random.Next();
            TestSignature(provider, id, key);
        });
    }

    private static void TestSignature<T>(ISignatureProvider provider, T id, byte[] key)
        where T : unmanaged
    {
        Span<byte> signature = stackalloc byte[provider.SignatureLength]; // maximum is 64 bytes
        int bytesWritten = provider.Sign(id, key, signature);
        Assert.Equal(provider.SignatureLength, bytesWritten);

        bool verify = provider.Verify(id, key, signature);
        Assert.True(verify);
    }
}
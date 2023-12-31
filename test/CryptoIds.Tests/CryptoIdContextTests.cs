namespace CryptoIds.Tests;

public class CryptoIdContextTests
{
    [Fact]
    public void TestCrc32()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Crc32, true);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestCrc32MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Crc32, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.NextInt64();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestCrc64()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Crc64, true);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestCrc64MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Crc64, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.NextInt64();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestXxHash32()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.XxHash32, true);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestXxHash32MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.XxHash32, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.NextInt64();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestXxHash64()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.XxHash64, true);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestXxHash64MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.XxHash64, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.NextInt64();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestHmacMd5()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Md5, true);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestHmacMd5MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Md5, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.NextInt64();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestHmacSha1()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Sha1, true);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestHmacSha1MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Sha1, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.NextInt64();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestHmacSha256()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Sha256, true);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestHmacSha256MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context =
            CryptoIdContext.Create(keys, SignatureProviderType.Sha256, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.NextInt64();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestHmacSha384()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Sha384, true);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestHmacSha384MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Sha384, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.NextInt64();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestHmacSha512()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Sha512, true);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestHmacSha512MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context =
            CryptoIdContext.Create(keys, SignatureProviderType.Sha512, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.NextInt64();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestHmacSha3_256()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Sha3_256, true);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestHmacSha3_256MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context =
            CryptoIdContext.Create(keys, SignatureProviderType.Sha3_256, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.NextInt64();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestHmacSha3_384()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Sha3_384, true);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestHmacSha3_384MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context =
            CryptoIdContext.Create(keys, SignatureProviderType.Sha3_384, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.NextInt64();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestHmacSha3_512()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Sha3_512, true);

        for (int i = 0; i < 1000; i++)
        {
            long id = random.NextInt64();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestHmacSha3_512MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.Sha3_512, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            long id = random.NextInt64();
            TestContext(context, id);
        });
    }

    private static void TestContext<T>(CryptoIdContext context, T id) where T : unmanaged
    {
        Span<char> chars = stackalloc char[context.GetRequiredLengthForEncode<T>()];

        int bytesWritten = context.TryEncode(id, chars);

        var encoded = chars[..bytesWritten];

        Assert.True(bytesWritten == chars.Length, "bytesWritten == chars.Length");

        bool verify = context.TryDecode(encoded, out T result);

        Assert.Equal(id, result);
        Assert.True(verify, "verify");
    }
}
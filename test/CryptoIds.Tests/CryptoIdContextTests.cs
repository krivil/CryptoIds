namespace CryptoIds.Tests;

using System.Globalization;

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
    public void TestInt32XxHash32()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.XxHash32, true);

        for (int i = 0; i < 1000; i++)
        {
            int id = random.Next();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestInt32XxHash32MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.XxHash32, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            int id = random.Next();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestInt64XxHash32()
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
    public void TestInt64XxHash32MultiThreaded()
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
    public void TestGuidXxHash32()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.XxHash32, true);

        for (int i = 0; i < 1000; i++)
        {
            Guid id = Guid.NewGuid();
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestGuidXxHash32MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.XxHash32, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            Guid id = Guid.NewGuid();
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestTuple2LongsXxHash32()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.XxHash32, true);

        for (int i = 0; i < 1000; i++)
        {
            (long, long) id = (random.NextInt64(), random.NextInt64());
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestTuple2LongsXxHash32MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.XxHash32, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            (long, long) id = (random.NextInt64(), random.NextInt64());
            TestContext(context, id);
        });
    }

    [Fact]
    public void TestTupleLongIntXxHash32()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.XxHash32, true);

        for (int i = 0; i < 1000; i++)
        {
            (long, int) id = (random.NextInt64(), random.Next());
            TestContext(context, id);
        }
    }

    [Fact]
    public void TestTupleLongIntXxHash32MultiThreaded()
    {
        Random random = new(681);
        Crypto2048BitKey keys = new();
        random.NextBytes(keys);
        var context = CryptoIdContext.Create(keys, SignatureProviderType.XxHash32, true);

        _ = Parallel.For(0, 1000, _ =>
        {
            (long, int) id = (random.NextInt64(), random.Next());
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

    [Fact]
    public void TestCustom()
    {
        //CryptoIdContext.Default =
        CryptoIdContext context = CryptoIdContext.CreateFromPassword("default_password");
        (long, long) id = (638422103460673600, 102);

        TestContext(context, id);

        string encoded = CryptoId.From(id).ToString(context);
        bool success = CryptoId<(long, long)>.TryParse(encoded, context, out CryptoId<(long, long)> cid);
        Assert.True(success);

        (long, long) result = cid.GetValue();

        Assert.Equal(id, result);
    }

    private static void TestContext<T>(CryptoIdContext context, T id) where T : unmanaged
    {
        string encoded = CryptoId<T>.From(id).ToString(context);
        T result = CryptoId<T>.Decode(encoded, context);

        Assert.Equal(id, result);
    }
}
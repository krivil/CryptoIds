namespace CryptoIds.Tests;

public class XorTests
{
    [Fact]
    public void TestXorEncryptAndDecryptWithKeyAndSeed()
    {
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] seed = new byte[128];
        random.NextBytes(seed);
        for (int i = 0; i < 1000; i++)
        {
            byte[] input = new byte[128];
            random.NextBytes(input);
            byte[] output = new byte[128];
            bool success = XorEncryptor.TryXor(input, seed, key, output);
            Assert.NotEqual(input, output);
            Assert.True(success);
            XorEncryptor.XorInline(output, seed, key);
            Assert.Equal(input, output);
        }
    }

    [Fact]
    public void TestXorEncryptAndDecryptWithKeyAndSeedMultiThreaded()
    {
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        byte[] seed = new byte[128];
        random.NextBytes(seed);
        _ = Parallel.For(0, 1000, _ =>
        {
            byte[] input = new byte[128];
            random.NextBytes(input);
            byte[] output = new byte[128];
            bool success = XorEncryptor.TryXor(input, seed, key, output);
            Assert.NotEqual(input, output);
            Assert.True(success);
            XorEncryptor.XorInline(output, seed, key);
            Assert.Equal(input, output);
        });
    }

    [Fact]
    public void TestXorEncryptAndDecryptWithKey()
    {
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        for (int i = 0; i < 1000; i++)
        {
            byte[] input = new byte[128];
            random.NextBytes(input);
            byte[] output = new byte[128];
            bool success = XorEncryptor.TryXor(input, key, output);
            Assert.NotEqual(input, output);
            Assert.True(success);
            XorEncryptor.XorInline(output, key);
            Assert.Equal(input, output);
        }
    }

    [Fact]
    public void TestXorEncryptAndDecryptWithKeyMultiThreaded()
    {
        Random random = new(681);
        byte[] key = new byte[128];
        random.NextBytes(key);
        _ = Parallel.For(0, 1000, _ =>
        {
            byte[] input = new byte[128];
            random.NextBytes(input);
            byte[] output = new byte[128];
            bool success = XorEncryptor.TryXor(input, key, output);
            Assert.NotEqual(input, output);
            Assert.True(success);
            XorEncryptor.XorInline(output, key);
            Assert.Equal(input, output);
        });
    }
}
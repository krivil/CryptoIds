namespace CryptoIds.Tests;

public class Base64EncoderTests
{
    [Fact]
    public void TestBase64EncodeAndDecode()
    {
        Random random = new(681);
        for (int i = 0; i < 1000; i++)
        {
            byte[] input = new byte[128];
            random.NextBytes(input);

            byte[] encodeBuffer = new byte[Base64.GetMaxByteCountForEncoding(input.Length)];
            _ = Base64.EncodeToUtf8(input, encodeBuffer, out _, out int bytesWrittenEncode);
            var encoded = encodeBuffer[..bytesWrittenEncode];

            byte[] decodeBuffer = new byte[Base64.GetMaxByteCountForDecoding(bytesWrittenEncode)];
            _ = Base64.DecodeFromUtf8(encoded, decodeBuffer, out _, out int bytesWrittenDecode);
            var decoded = decodeBuffer[..bytesWrittenDecode];
            
            Assert.Equal(input, decoded);
        }
    }

    [Fact]
    public void TestBase64EncodeAndDecodeInt32()
    {
        Random random = new(681);
        for (int i = 0; i < 1000; i++)
        {
            byte[] input = new byte[128];
            random.NextBytes(input);
            int id = random.Next();

            byte[] encodeBuffer = new byte[Base64.GetMaxByteCountForEncoding<int>(input.Length)];
            _ = Base64.TryEncodeToUtf8(id, input, encodeBuffer, out int bytesWrittenEncode);
            var encoded = encodeBuffer[..bytesWrittenEncode];

            byte[] decodeBuffer = new byte[Base64.GetMaxByteCountForDecoding(bytesWrittenEncode)];
            _ = Base64.TryDecodeFromUtf8(encoded, out int id2, decodeBuffer, out int bytesWrittenDecode);
            var decoded = decodeBuffer[..bytesWrittenDecode];

            Assert.Equal(id, id2);
            Assert.Equal(input, decoded);
        }
    }

    [Fact]
    public void TestBase64EncodeAndDecodeInt64()
    {
        Random random = new(681);
        for (int i = 0; i < 1000; i++)
        {
            byte[] input = new byte[128];
            random.NextBytes(input);
            long id = random.NextInt64();

            byte[] encodeBuffer = new byte[Base64.GetMaxByteCountForEncoding<long>(input.Length)];
            _ = Base64.TryEncodeToUtf8(id, input, encodeBuffer, out int bytesWrittenEncode);
            var encoded = encodeBuffer[..bytesWrittenEncode];

            byte[] decodeBuffer = new byte[Base64.GetMaxByteCountForDecoding(bytesWrittenEncode)];
            _ = Base64.TryDecodeFromUtf8(encoded, out long id2, decodeBuffer, out int bytesWrittenDecode);
            var decoded = decodeBuffer[..bytesWrittenDecode];

            Assert.Equal(id, id2);
            Assert.Equal(input, decoded);
        }
    }

    [Fact]
    public void TestBase64EncodeAndDecodeT()
    {
        Random random = new(681);
        for (int i = 0; i < 1000; i++)
        {
            byte[] input = new byte[128];
            random.NextBytes(input);
            ulong id = (ulong)Math.Abs(random.NextInt64());

            byte[] encodeBuffer = new byte[Base64.GetMaxByteCountForEncoding<ulong>(input.Length)];
            _ = Base64.TryEncodeToUtf8(id, input, encodeBuffer, out int bytesWrittenEncode);
            var encoded = encodeBuffer[..bytesWrittenEncode];

            byte[] decodeBuffer = new byte[Base64.GetMaxByteCountForDecoding(bytesWrittenEncode)];
            _ = Base64.TryDecodeFromUtf8(encoded, out ulong id2, decodeBuffer, out int bytesWrittenDecode);
            var decoded = decodeBuffer[..bytesWrittenDecode];

            Assert.Equal(id, id2);
            Assert.Equal(input, decoded);
        }
    }

    [Fact]
    public void TestBase64EncodeAndDecodeMultiThreaded()
    {
        Random random = new(681);
        _ = Parallel.For(0, 1000, _ =>
        {
            byte[] input = new byte[128];
            random.NextBytes(input);
            byte[] output = new byte[Base64.GetMaxByteCountForEncoding(input.Length)];
            Base64.EncodeToUtf8(input, output, out int consumedEncode, out int bytesWrittenEncode);
            var outputFinal = output[..bytesWrittenEncode];
            byte[] decoded = new byte[Base64.GetMaxByteCountForDecoding(bytesWrittenEncode)];
            Base64.DecodeFromUtf8(outputFinal, decoded, out int consumedDecode, out int bytesWrittenDecode);
            var decodedFinal = decoded[..bytesWrittenDecode];
            Assert.Equal(input, decodedFinal);
        });
    }
}
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
            byte[] output = new byte[Base64UrlStringEncoder.GetRequiredLengthForEncode(input.Length)];
            _ = Base64UrlStringEncoder.Encode(input, output);
            byte[] decoded = new byte[Base64UrlStringEncoder.GetMaximumRequiredLengthForDecode(output)];
            _ = Base64UrlStringEncoder.Decode(output, decoded);
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
            byte[] output = new byte[Base64UrlStringEncoder.GetRequiredLengthForEncode(input.Length)];
            _ = Base64UrlStringEncoder.Encode(input, output);
            byte[] decoded = new byte[Base64UrlStringEncoder.GetMaximumRequiredLengthForDecode(output)];
            _ = Base64UrlStringEncoder.Decode(output, decoded);
            Assert.Equal(input, decoded);
        });
    }
}
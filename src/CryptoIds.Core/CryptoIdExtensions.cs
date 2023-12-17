// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

using CryptoIds.Core;
using System.Security.Cryptography;

public static class CryptoIdExtensions
{
    public static IServiceProvider UseCryptoIds(this IServiceProvider services, string password, bool encrypt = false)
    {
        CryptoIdContext.Default = CryptoIdContext.CreateFromPassword(password);
        return services;
    }

    public static IServiceProvider UseCryptoIds(this IServiceProvider services, string password, CryptoIdConfigurationOptions options)
    {
        CryptoIdContext.Default = CryptoIdContext.CreateFromPassword(password,
            options.SignatureProviderType, options.Salt, options.KeyHashAlgorithm, options.KeyHashIterations, options.Encrypt);
        return services;
    }
}

public sealed class CryptoIdConfigurationOptions
{
    public bool Encrypt { get; set; } = false;
    public SignatureProviderType SignatureProviderType { get; set; } = SignatureProviderType.XxHash32;
    public byte[]? Salt { get; set; }
    public HashAlgorithmName KeyHashAlgorithm { get; set; } = HashAlgorithmName.SHA256;
    public int KeyHashIterations { get; set; } = 10_000;
}
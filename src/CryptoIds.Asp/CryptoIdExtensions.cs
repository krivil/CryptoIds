// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

using Microsoft.OpenApi.Models;
using CryptoIds.Core;

public static class CryptoIdExtensions
{
    public static void AddCryptoIds(
        this IServiceCollection services,
        string password,
        SignatureProviderType signature = SignatureProviderType.XxHash32,
        bool encrypt = false
    )
    {
        CryptoIdContext.Default = CryptoIdContext.CreateFromPassword(password, signature, encrypt: encrypt);
    }

    public static void AddCryptoIds(
        this IServiceCollection services,
        string password,
        CryptoIdConfigurationOptions options
    )
    {
        CryptoIdContext.Default = CryptoIdContext.CreateFromPassword(password,
            options.SignatureProviderType, options.Salt, options.KeyHashAlgorithm, options.KeyHashIterations, options.Encrypt);
    }
    
    public static void AddCryptoIdsWithSwagger(
        this IServiceCollection services,
        string password,
        SignatureProviderType signature = SignatureProviderType.XxHash32,
        bool encrypt = false
        )
    {
        CryptoIdContext.Default = CryptoIdContext.CreateFromPassword(password, signature, encrypt: encrypt);
        services.FixSwagger();
    }

    public static void AddCryptoIdsWithSwagger(
        this IServiceCollection services,
        string password,
        CryptoIdConfigurationOptions options
        )
    {
        CryptoIdContext.Default = CryptoIdContext.CreateFromPassword(password,
            options.SignatureProviderType, options.Salt, options.KeyHashAlgorithm, options.KeyHashIterations, options.Encrypt);
        
        services.FixSwagger();
    }

    private static void FixSwagger(this IServiceCollection services)
    {
        services.ConfigureSwaggerGen(options =>
        {
            options.MapType(typeof(CryptoId<>), () => new OpenApiSchema
            {
                Type = "string",
                Format = null,
                Pattern = "^[a-zA-Z0-9_-]+$",
                Nullable = false
            });
        });
    }
}
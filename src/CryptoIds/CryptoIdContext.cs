namespace CryptoIds;

using System.Security.Cryptography;


public sealed class CryptoIdContext
{
    private static volatile CryptoIdContext _default = CreateFromPassword(Environment.GetCommandLineArgs()[0]); // WARNING: DO NOT USE THIS IN PRODUCTION!

    public static CryptoIdContext Default
    {
        get => _default;
        set => _default = value;
    }

#if NET8_0_OR_GREATER

    public static CryptoIdContext Create(Crypto2048BitKey keys, ISignatureProvider signer, bool encrypt) =>
        new(keys, signer, encrypt);

    public static CryptoIdContext Create(Crypto2048BitKey keys, SignatureProviderType signatureProviderType, bool encrypt) =>
        new(keys, signatureProviderType.ToSignatureProvider(), encrypt);

#else

    public static CryptoIdContext Create(byte[] keys, ISignatureProvider signer, bool encrypt) =>
        new(keys, signer, encrypt);

    public static CryptoIdContext Create(byte[] keys, SignatureProviderType signatureProviderType, bool encrypt) =>
        new(keys, signatureProviderType.ToSignatureProvider(), encrypt);

#endif

    /// <summary>
    /// Creates a <see cref="CryptoIdContext"/> from a password.
    /// </summary>
    /// <param name="password">Password used to generate keys</param>
    /// <param name="signatureProvider">Hash algorithm for signature</param>
    /// <param name="salt">Randomness along with password to generate keys</param>
    /// <param name="keyHashAlgorithmName">Algorithm used for hashing in creating keys</param>
    /// <param name="keyHashIterations">Number of iterations used in creating keys</param>
    /// <param name="encrypt">Indicate whether to encrypt ids or just sign them. Signing is faster, encryption makes them look random</param>
    /// <returns></returns>
    public static CryptoIdContext CreateFromPassword(string password,
        SignatureProviderType signatureProvider = SignatureProviderType.XxHash32,
        Span<byte> salt = default,
        HashAlgorithmName keyHashAlgorithmName = default,
        int keyHashIterations = 10_000,
        bool encrypt = false)
    {
        salt = !salt.IsEmpty ? salt : new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 6, 10, 11, 12, 13, 14, 15, 16 };
        keyHashAlgorithmName = keyHashAlgorithmName.Name == default ? HashAlgorithmName.SHA256 : keyHashAlgorithmName;
        Crypto2048BitKey keys = new();
        Rfc2898DeriveBytes.Pbkdf2(password, salt, keys, keyHashIterations, keyHashAlgorithmName);

        return new CryptoIdContext(keys, signatureProvider.ToSignatureProvider(), encrypt);
    }

    private readonly bool _encrypt;
    private readonly ISignatureProvider _signer;
#if NET8_0_OR_GREATER
    private readonly Crypto2048BitKey _keys;
#else
    private readonly byte[] _keys;
#endif


#if NET8_0_OR_GREATER

    private CryptoIdContext(Crypto2048BitKey keys, ISignatureProvider signer, bool encrypt)
    {
        _encrypt = encrypt;
        _keys = keys;
        _signer = signer;
    }

#else

    private CryptoIdContext(byte[] keys, ISignatureProvider signer, bool encrypt)
    {
        if(keys.Length != 256) throw new ArgumentException("Keys must be 256 bytes", nameof(keys));
        _keys = keys;
        _signer = signer;
        _encrypt = encrypt;
    }

#endif

    public int GetRequiredLengthForEncode<T>() where T : unmanaged => IdOperations.GetRequiredLengthForEncode<T>(_signer);

    public bool TryDecode<T>(ReadOnlySpan<byte> encoded, out T result) where T : unmanaged =>
        _encrypt
            ? IdOperations.TryDecodeAndXorAndValidate(encoded, _keys.KeyA, _keys.KeyB, _signer, out result)
            : IdOperations.TryDecodeAndValidate(encoded, _keys.KeyA, _signer, out result);

    public bool TryDecode<T>(ReadOnlySpan<char> encoded, out T result) where T : unmanaged =>
        _encrypt
            ? IdOperations.TryDecodeAndXorAndValidate(encoded, _keys.KeyA, _keys.KeyB, _signer, out result)
            : IdOperations.TryDecodeAndValidate(encoded, _keys.KeyA, _signer, out result);

    public bool TryDecode<T>(ReadOnlySpan<byte> encoded, Guid userKey, out T result) where T : unmanaged =>
        _encrypt
            ? IdOperations.TryDecodeAndXorAndValidate(encoded, _keys.KeyA, _keys.KeyB, userKey, _signer, out result)
            : IdOperations.TryDecodeAndValidate(encoded, _keys.KeyA, _signer, out result);

    public bool TryDecode<T>(ReadOnlySpan<char> encoded, Guid userKey, out T result) where T : unmanaged =>
        _encrypt
            ? IdOperations.TryDecodeAndXorAndValidate(encoded, _keys.KeyA, _keys.KeyB, userKey, _signer, out result)
            : IdOperations.TryDecodeAndValidate(encoded, _keys.KeyA, _signer, out result);


    public int TryEncode<T>(T id, Span<byte> encodedResult) where T : unmanaged =>
        _encrypt
            ? IdOperations.TrySignAndXorAndEncode(id, _keys.KeyA, _keys.KeyB, _signer, encodedResult)
            : IdOperations.TrySignAndEncode(id, _keys.KeyA, _signer, encodedResult);

    public int TryEncode<T>(T id, Span<char> encodedResult) where T : unmanaged =>
        _encrypt
            ? IdOperations.TrySignAndXorAndEncode(id, _keys.KeyA, _keys.KeyB, _signer, encodedResult)
            : IdOperations.TrySignAndEncode(id, _keys.KeyA, _signer, encodedResult);

    public int TryEncode<T>(T id, Guid userKey, Span<byte> encodedResult) where T : unmanaged =>
        _encrypt
            ? IdOperations.TrySignAndXorAndEncode(id, _keys.KeyA, _keys.KeyB, userKey, _signer, encodedResult)
            : IdOperations.TrySignAndEncode(id, _keys.KeyA, _signer, encodedResult);

    public int TryEncode<T>(T id, Guid userKey, Span<char> encodedResult) where T : unmanaged =>
        _encrypt
            ? IdOperations.TrySignAndXorAndEncode(id, _keys.KeyA, _keys.KeyB, userKey, _signer, encodedResult)
            : IdOperations.TrySignAndEncode(id, _keys.KeyA, _signer, encodedResult);
}
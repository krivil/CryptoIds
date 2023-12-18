namespace CryptoIds.Websites.Api;

using CryptoIds;

public record IdDto(CryptoId<int> CryptoInt, CryptoId<long> CryptoLong, CryptoId<(int, long)> CryptoBoth, int Int, long Long)
{
    public static IdDto From(int id) => new(id, id, (id, id), id, id);
}

namespace CryptoIds.Websites.Api.Controllers;

using CryptoIds.Core;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CryptoIdController : ControllerBase
{
    private readonly ILogger<CryptoIdController> _logger;

    public CryptoIdController(ILogger<CryptoIdController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/", Name = "GetIds")]
    public IEnumerable<IdDto> Get([FromQuery] int from = 1, [FromQuery] int to = 100) =>
        Enumerable.Range(from, to).Select(IdDto.From);

    [HttpGet("/int/{id}", Name = "GetIdByInt")]
    public IdDto GetByInt([FromRoute] int id) => IdDto.From(id);

    [HttpGet("/crypto-int/{id}", Name = "GetIdByCryptoInt")]
    public IdDto GetByCryptoInt([FromRoute] CryptoId<int> id) => IdDto.From(id);
}

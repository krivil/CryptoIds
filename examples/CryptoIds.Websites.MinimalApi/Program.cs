using CryptoIds.Core;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCryptoIdsWithSwagger("Pa55W0rd!", encrypt: false);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("", ([FromQuery] int from = 1, [FromQuery] int to = 100) => Enumerable.Range(from, to).Select(IdDto.From))
    .WithName("ListIds")
    .WithOpenApi();

app.MapGet("/int/{id}", ([FromRoute] int id) => IdDto.From(id))
    .WithName("IdFromInt")
    .WithOpenApi();

app.MapGet("/crypto-int/{id}", ([FromRoute] CryptoId<int> id) => IdDto.From(id))
    .WithName("IdFromCryptoId")
    .WithOpenApi();

app.Run();

record IdDto(CryptoId<int> CryptoInt, CryptoId<long> CryptoLong, CryptoId<(int, long)> CryptoBoth, int Int, long Long)
{
    public static IdDto From(int id) => new(id, id, (id, id), id, id);
}

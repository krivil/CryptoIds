# CryptoIds

## Raison d'etre

Exposing database IDs in a REST API response is generally considered a bad practice. Best practices in API design and security suggest avoiding direct exposure of raw database IDs. Sometimes UUIDs are used instead of database IDs, but they are not always a good solution. UUIDs are hard to index and are not sortable.

## Overview

CryptoIds is a C# library designed for ASP.NET Core applications to securely and efficiently obfuscate database IDs. This library is essential for enhancing the security and integrity of APIs by preventing ID enumeration attacks and obscuring the structure of the database.

## Features
    
 - **Type Flexibility:** Supports any `unmanaged` type for ID signing and encoding in Base64.
    
    For more information about unmanaged types, see [this article](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types).
    - sbyte, byte, short, ushort, int, uint, long, ulong, nint, nuint, char, float, double, decimal, or bool
    - Any enum type
    - Any pointer type
    - Any user-defined struct type that contains fields of unmanaged types only, such as tuples.

 - **Optional Encryption:** Provides the option to encrypt IDs. Encrypted IDs appear random, enhancing security.
 - **Sortable IDs:** When not encrypted, the library ensures that the resulting strings are sortable, maintaining order consistency.
 - **ASP.NET Core Integration:** Seamless integration with ASP.NET Core for obfuscating IDs in JSON responses and route parameters.
 - **Swagger Support:** Integrates with Swagger for API documentation, ensuring that ID obfuscation is consistently represented.
 - **Multiple Hash Functions:** Supports various hash functions for signatures, offering a balance between cryptographic strength and signature length.
 - **User-Specific Encryption:** Allows encryption based on specific user GUIDs or a shared key, providing flexibility in access control.

## Getting Started

### Installation

Install the [CryptoIds NuGet package](https://www.nuget.org/packages/CryptoIds/) from the package manager console:

    PM> Install-Package CryptoIds

or using the .NET Core CLI:

    > dotnet add package CryptoIds

### Usage

#### 1. Configure CryptoIds

```csharp

CryptoIdContext.Default = CryptoIdContext.CreateFromPassword("Pa55W0rd!", SignatureProviderType.Md5, encrypt: true);

```

or if you are using ASP.NET Core with Swagger, you can use the following extension method:


```csharp

builder.Services.AddCryptoIdsWithSwagger("Pa55W0rd!", SignatureProviderType.Md5, encrypt: true);

```

#### 2. Use it in your controllers:

```csharp

public class WeatherForecast
{
    public CryptoId<int> Id { get; set; }
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string Summary { get; set; }
}

```

```csharp
public Task<> GetWeatherForecastAsync(int id) 
{
    WeatherForecast result = new WeatherForecast();
    
    // ...
    
    result.Id = id;
    return result;
}

```

```csharp

[HttpGet("{id}")]
public async Task<ActionResult<WeatherForecast>> Get(CryptoId<int> id)
{
    var weatherForecast = await _weatherForecastService.GetWeatherForecastAsync(id);

    if (weatherForecast == null)
    {
        return NotFound();
    }

    return weatherForecast;
}

```

#### 3. Enjoy the benefits of obfuscated IDs:

```json

{
    "id": "AqQAAgAAABQAAQAAAAEAAQAAAAI",
    "date": "2021-01-01T00:00:00.0000000Z",
    "temperatureC": -17,
    "temperatureF": 1,
    "summary": "Freezing"
}
    
```

To summarize, the library lets you replace your Ids of type `int` with `CryptoId<int>` and the library will take care of the rest.
`int` and `CryptoId<int>` are implicitly convertible to each other, so you can use them interchangeably, as long as `CryptoIdContext.Default` is set.
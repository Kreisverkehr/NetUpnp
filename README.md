# NetUpnp

NetUpnp is a small .NET library for discovering and reading Universal Plug and Play (UPnP) devices.

> [!WARNING]
> This project is a work in progress. The API, behavior, and package contents may change without notice. It is not yet recommended for production use.

## Requirements

- .NET 10
- A network interface that can reach the UPnP devices being discovered

## Installation

```sh
dotnet add package Kreisverkehr.NetUpnp
```

## Quick start

Register NetUpnp with the Microsoft dependency injection container and enumerate discovered devices:

```csharp
using Kreisverkehr.NetUpnp;
using Microsoft.Extensions.DependencyInjection;

IServiceProvider services = new ServiceCollection()
    .AddUpnp()
    .BuildServiceProvider();

IUpnpClient upnpClient = services.GetRequiredService<IUpnpClient>();

await foreach (var device in upnpClient.DiscoverDevicesAsync("ssdp:all"))
{
    Console.WriteLine($"{device.FriendlyName} ({device.DeviceType})");
}
```

`DiscoverDevicesAsync` uses SSDP discovery and yields devices as their descriptions are retrieved. The `IUpnpClient` also exposes `RunDiscoverDevicesAsync` for event-based discovery through the `DeviceDiscovered` event.

## Custom descriptions

UPnP devices can expose vendor-specific XML elements. Pass a custom description type to `AddUpnp` when those elements need to be deserialized into custom model classes:

```csharp
services.AddUpnp(options =>
{
    options.DescriptionType = typeof(MyUpnpDescription);
});
```

The custom description and device types should derive from `UpnpDescription` and `UpnpDevice`, and use `XmlSerializer` attributes for their XML elements. See the sample in [`Samples/CustomizeUpnpDescription.cs`](Samples/CustomizeUpnpDescription.cs).

## Samples

The runnable examples are documented in [`Samples/README.md`](Samples/README.md).

## License

NetUpnp is available under the MIT license. See [LICENSE](LICENSE).

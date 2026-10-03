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

Register NetUpnp with the Microsoft dependency injection container and enumerate discovered devices as typed XML descriptions:

```csharp
using Kreisverkehr.NetUpnp;
using Kreisverkehr.NetUpnp.Model;
using Microsoft.Extensions.DependencyInjection;

IServiceProvider services = new ServiceCollection()
    .AddUpnp()
    .BuildServiceProvider();

IUpnpClient<UpnpDescription> upnpClient = services.GetRequiredService<IUpnpClient<UpnpDescription>>();

await foreach ((Uri location, UpnpDescription description) in upnpClient.DiscoverDevicesAsync("ssdp:all"))
{
    Console.WriteLine($"{description.Device.FriendlyName} ({description.Device.DeviceType})");
    Console.WriteLine($"Description at {location}");
}
```

`DiscoverDevicesAsync` uses SSDP discovery and yields the service description location together with the parsed description. The raw `IUpnpClient` interface also exposes a `XDocument`-based flow when you want to inspect the untyped XML. `RunDiscoverDevicesAsync` remains available for event-driven discovery through the `DeviceDiscovered` event.

## Custom descriptions

UPnP devices can expose vendor-specific XML elements. Derive your own description and device model classes from `UpnpDescription` and `UpnpDevice`, then resolve the generic `IUpnpClient<TDescription>` for the specific type you want to deserialize:

```csharp
using Kreisverkehr.NetUpnp;
using Microsoft.Extensions.DependencyInjection;

IServiceProvider services = new ServiceCollection()
    .AddUpnp()
    .BuildServiceProvider();

IUpnpClient<MyUpnpDescription> upnpClient = services.GetRequiredService<IUpnpClient<MyUpnpDescription>>();

await foreach ((Uri location, MyUpnpDescription description) in upnpClient.DiscoverDevicesAsync("urn:example:device:MyDevice:1"))
{
    MyUpnpDevice device = (MyUpnpDevice)description.Device;
    Console.WriteLine($"{device.FriendlyName}: {device.CustomValue}");
    Console.WriteLine($"Description at {location}");
}
```

The custom description and device types should use `XmlSerializer` attributes for their XML elements. See the sample in [`Samples/CustomizeUpnpDescription.cs`](Samples/CustomizeUpnpDescription.cs).

## Samples

The runnable examples are documented in [`Samples/README.md`](Samples/README.md).

## License

NetUpnp is available under the MIT license. See [LICENSE](LICENSE).

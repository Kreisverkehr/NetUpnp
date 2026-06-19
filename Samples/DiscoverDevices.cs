#!/usr/bin/env dotnet

#:project ../src/NetUpnp/NetUpnp.csproj
#:package ConsoleTableExt@3.3.0
#:package Microsoft.Extensions.DependencyInjection@10.0.*
#:package Microsoft.Extensions.Logging@10.0.*
#:package Microsoft.Extensions.Logging.Console@10.0.*

using Kreisverkehr.NetUpnp;
using Kreisverkehr.NetUpnp.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

IServiceProvider services = new ServiceCollection()
    .AddLogging(b => b
        .AddConsole()
        .SetMinimumLevel(LogLevel.Warning)
    )
    .AddUpnp()
    .BuildServiceProvider()
;

IUpnpClient upnpClient = services.GetRequiredService<IUpnpClient>();
// upnpClient.DeviceDiscovered += (sender, e) =>
// {
//     Console.WriteLine($"Discovered device: {e.Device.FriendlyName} ({e.Device.DeviceType})");
// };

await foreach (UpnpDevice device in upnpClient.DiscoverDevicesAsync())
{
    Console.WriteLine($"Discovered device: {device.FriendlyName} ({device.DeviceType})");
}

ConsoleKeyInfo key;
while((key = Console.ReadKey()).Key != ConsoleKey.Enter)
{
}
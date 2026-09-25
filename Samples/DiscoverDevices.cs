#!/usr/bin/env dotnet

#:project ../src/NetUpnp/NetUpnp.csproj
#:package ConsoleTableExt@3.3.0
#:package Microsoft.Extensions.DependencyInjection@10.0.*
#:package Microsoft.Extensions.Logging@10.0.*
#:package Microsoft.Extensions.Logging.Console@10.0.*

using ConsoleTableExt;
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
IUpnpDeviceCollection deviceCollection = services.GetRequiredService<IUpnpDeviceCollection>();
upnpClient.DeviceDiscovered += (sender, e) =>
{
    Console.WriteLine($"Discovered device: {e.Device.FriendlyName} ({e.Device.DeviceType})");
};

await upnpClient.RunDiscoverDevicesAsync("ssdp:all", 3, true);

ConsoleTableBuilder
    .From(deviceCollection
        .Select(d => new DeviceInfo(
            FriendlyName: d.FriendlyName,
            DeviceType: d.DeviceType,
            Manufacturer: d.Manufacturer,
            ModelName: d.ModelName,
            UniqueDeviceName: d.UniqueDeviceName
        ))
        .ToList())
    .WithColumn("Friendly Name", "Device Type", "Manufacturer", "Model Name", "Unique Device Name")
    .WithTitle("Discovered UPnP Devices")
    .ExportAndWriteLine();

ConsoleKeyInfo key;
while((key = Console.ReadKey()).Key != ConsoleKey.Enter)
{
}

record DeviceInfo(string FriendlyName, string DeviceType, string Manufacturer, string ModelName, string UniqueDeviceName);
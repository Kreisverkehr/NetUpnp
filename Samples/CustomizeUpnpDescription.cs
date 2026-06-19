#!/usr/bin/env dotnet

#:project ../src/NetUpnp/NetUpnp.csproj
#:package ConsoleTableExt@3.3.0
#:package Microsoft.Extensions.DependencyInjection@10.0.*
#:package Microsoft.Extensions.Logging@10.0.*
#:package Microsoft.Extensions.Logging.Console@10.0.*

using System.Xml.Serialization;
using Kreisverkehr.NetUpnp;
using Kreisverkehr.NetUpnp.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

IServiceProvider services = new ServiceCollection()
    .AddLogging(b => b
        .AddConsole()
        .SetMinimumLevel(LogLevel.Information)
    )
    .AddUpnp( options =>
    {
        options.DescriptionType = typeof(SapIpServerDescription);
    })
    .BuildServiceProvider()
;

IUpnpClient upnpClient = services.GetRequiredService<IUpnpClient>();

await foreach (UpnpDevice device in upnpClient.DiscoverDevicesAsync("urn:ses-com:device:SatIPServer:1"))
{
    Console.WriteLine($"Discovered device: {device.FriendlyName} ({device.DeviceType})");
    if(device is SapIpServerDevice sapIpServerDevice)
    {
        Console.WriteLine($"SAT>IP Capabilities: {sapIpServerDevice.SatIpCapabilities}");
        Console.WriteLine($"SAT>IP M3U: {sapIpServerDevice.SatIpM3U}");
    }
}

[XmlRoot("root", Namespace = "urn:schemas-upnp-org:device-1-0")]
public class SapIpServerDescription : UpnpDescription
{
    [XmlElement("device", Namespace = "urn:schemas-upnp-org:device-1-0", Type = typeof(SapIpServerDevice))]
    public override required UpnpDevice Device { get => base.Device; set => base.Device = value; }
}

public class SapIpServerDevice : UpnpDevice
{
    [XmlElement("X_SATIPCAP", Namespace = "urn:ses-com:satip")]
    public string? SatIpCapabilities { get; set; }

    [XmlElement("X_SATIPM3U", Namespace = "urn:ses-com:satip")]
    public string? SatIpM3U { get; set; }
}
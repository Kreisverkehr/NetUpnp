using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Xml.Serialization;
using Kreisverkehr.NetSsdp;
using Kreisverkehr.NetUpnp.Model;
using Kreisverkehr.NetUpnp.Options;
using Microsoft.Extensions.Options;

namespace Kreisverkehr.NetUpnp;

public interface IUpnpClient
{
    event EventHandler<UpnpDeviceDiscoveredEventArgs>? DeviceDiscovered;
    Task RunDiscoverDevicesAsync(string searchTarget = "ssdp:all", int maxWaitSeconds = 1, bool waitForResponses = false, CancellationToken cancellationToken = default);
    IAsyncEnumerable<UpnpDevice> DiscoverDevicesAsync(string searchTarget = "ssdp:all", int maxWaitSeconds = 1, CancellationToken cancellationToken = default);
}

public class UpnpDeviceDiscoveredEventArgs : EventArgs
{
    public UpnpDevice Device { get; }

    public UpnpDeviceDiscoveredEventArgs(UpnpDevice device)
    {
        Device = device;
    }
}

public class UpnpClient : IUpnpClient
{
    private readonly ISsdpServiceCollection _ssdpServiceCollection;
    private readonly ISsdpClient _ssdpClient;
    private readonly HttpClient _httpClient;
    private readonly UpnpOptions _options;
    private readonly XmlSerializer _descriptionSerializer;

    public UpnpClient(ISsdpServiceCollection ssdpServiceCollection, ISsdpClient ssdpClient, HttpClient httpClient, IOptions<UpnpOptions> options)
    {
        _ssdpServiceCollection = ssdpServiceCollection;
        _ssdpServiceCollection.ServiceDiscovered += ServiceDiscovered;
        _ssdpClient = ssdpClient;
        _httpClient = httpClient;
        _options = options.Value;
        _descriptionSerializer = new XmlSerializer(_options.DescriptionType);
    }

    public event EventHandler<UpnpDeviceDiscoveredEventArgs>? DeviceDiscovered;

    public async Task RunDiscoverDevicesAsync(string searchTarget = "ssdp:all", int maxWaitSeconds = 1, bool waitForResponses = false, CancellationToken cancellationToken = default)
    {
        await _ssdpClient.RunDiscoveryAsync(searchTarget, maxWaitSeconds, waitForResponses, cancellationToken);
    }

    public async IAsyncEnumerable<UpnpDevice> DiscoverDevicesAsync(string searchTarget = "ssdp:all", int maxWaitSeconds = 1, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateUnbounded<UpnpDevice>();
        DeviceDiscovered += Handler;

        _ = RunDiscoverDevicesAsync(searchTarget, maxWaitSeconds, true, cancellationToken)
            .ContinueWith(_ => Cleanup(), CancellationToken.None);
        
        while(await channel.Reader.WaitToReadAsync(cancellationToken))
            yield return await channel.Reader.ReadAsync(cancellationToken);

        async void Handler(object? sender, UpnpDeviceDiscoveredEventArgs e)
        {
            await channel.Writer.WriteAsync(e.Device, cancellationToken);
        }
        
        void Cleanup()
        {
            DeviceDiscovered -= Handler;
            channel.Writer.Complete();
        }
    }

    private async void ServiceDiscovered(object? sender, SsdpServiceDiscoveredEventArgs e)
    {
        var descStream = await _httpClient.GetStreamAsync(e.Service.ServiceDescriptionLocation);
        if(descStream is null) return;

        var description = _descriptionSerializer.Deserialize(descStream) as UpnpDescription;
        if(description is null) return;

        DeviceDiscovered?.Invoke(this, new UpnpDeviceDiscoveredEventArgs(description.Device));
    }
}
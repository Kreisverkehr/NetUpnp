using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Xml.Linq;
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
    IAsyncEnumerable<Tuple<Uri, XDocument>> DiscoverDevicesAsync(string searchTarget = "ssdp:all", int maxWaitSeconds = 1, CancellationToken cancellationToken = default);
}

public interface IUpnpClient<TDescription> where TDescription : UpnpDescription
{
    event EventHandler<UpnpDeviceDiscoveredEventArgs<TDescription>>? DeviceDiscovered;
    Task RunDiscoverDevicesAsync(string searchTarget = "ssdp:all", int maxWaitSeconds = 1, bool waitForResponses = false, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Tuple<Uri, TDescription>> DiscoverDevicesAsync(string searchTarget = "ssdp:all", int maxWaitSeconds = 1, CancellationToken cancellationToken = default);
}

public class UpnpDeviceDiscoveredEventArgs(Uri serviceDescriptionLocation, XDocument serviceDescription) : EventArgs
{
    public Uri ServiceDescriptionLocation { get; } = serviceDescriptionLocation;
    public XDocument ServiceDescription { get; } = serviceDescription;
}

public class UpnpDeviceDiscoveredEventArgs<TDescription>(Uri serviceDescriptionLocation, TDescription serviceDescription) where TDescription : UpnpDescription
{
    public Uri ServiceDescriptionLocation { get; } = serviceDescriptionLocation;
    public TDescription ServiceDescription { get; } = serviceDescription;
}

public class UpnpClient : IUpnpClient
{
    private readonly ISsdpServiceCollection _ssdpServiceCollection;
    private readonly ISsdpClient _ssdpClient;
    private readonly HttpClient _httpClient;
    private readonly UpnpOptions _options;

    public UpnpClient(ISsdpServiceCollection ssdpServiceCollection, ISsdpClient ssdpClient, HttpClient httpClient, IOptions<UpnpOptions> options)
    {
        _ssdpServiceCollection = ssdpServiceCollection;
        _ssdpServiceCollection.ServiceDiscovered += ServiceDiscovered;
        _ssdpClient = ssdpClient;
        _httpClient = httpClient;
        _options = options.Value;
    }

    public event EventHandler<UpnpDeviceDiscoveredEventArgs>? DeviceDiscovered;

    public async Task RunDiscoverDevicesAsync(string searchTarget = "ssdp:all", int maxWaitSeconds = 1, bool waitForResponses = false, CancellationToken cancellationToken = default)
    {
        await _ssdpClient.RunDiscoveryAsync(searchTarget, maxWaitSeconds, waitForResponses, cancellationToken);
    }

    public async IAsyncEnumerable<Tuple<Uri, XDocument>> DiscoverDevicesAsync(string searchTarget = "ssdp:all", int maxWaitSeconds = 1, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateUnbounded<Tuple<Uri, XDocument>>();
        DeviceDiscovered += Handler;

        _ = RunDiscoverDevicesAsync(searchTarget, maxWaitSeconds, true, cancellationToken)
            .ContinueWith(_ => Cleanup(), CancellationToken.None);
        
        while(await channel.Reader.WaitToReadAsync(cancellationToken))
            yield return await channel.Reader.ReadAsync(cancellationToken);

        async void Handler(object? sender, UpnpDeviceDiscoveredEventArgs e)
        {
            await channel.Writer.WriteAsync(new Tuple<Uri, XDocument>(e.ServiceDescriptionLocation, e.ServiceDescription), cancellationToken);
        }
        
        void Cleanup()
        {
            DeviceDiscovered -= Handler;
            channel.Writer.Complete();
        }
    }

    private async void ServiceDiscovered(object? sender, SsdpServiceDiscoveredEventArgs e)
    {
        if(e.Service.ServiceDescriptionLocation is null) return;

        var descStream = await _httpClient.GetStreamAsync(e.Service.ServiceDescriptionLocation);
        if(descStream is null) return;

        var description = XDocument.Load(descStream);
        if(description is null) return;

        DeviceDiscovered?.Invoke(this, new UpnpDeviceDiscoveredEventArgs(e.Service.ServiceDescriptionLocation, description));
    }
}

public sealed class UpnpClient<TDescription> : IUpnpClient<TDescription>, IDisposable where TDescription : UpnpDescription
{
    private readonly IUpnpClient _upnpClient;
    private readonly UpnpOptions _options;
    private readonly XmlSerializer _descriptionSerializer;

    public UpnpClient(IUpnpClient upnpClient, IOptions<UpnpOptions> options)
    {
        _upnpClient = upnpClient;
        _options = options.Value;
        _descriptionSerializer = new XmlSerializer(typeof(TDescription));

        _upnpClient.DeviceDiscovered += ServiceDiscovered;
    }

    public event EventHandler<UpnpDeviceDiscoveredEventArgs<TDescription>>? DeviceDiscovered;

    public async IAsyncEnumerable<Tuple<Uri, TDescription>> DiscoverDevicesAsync(string searchTarget = "ssdp:all", int maxWaitSeconds = 1, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach(var (uri, description) in _upnpClient.DiscoverDevicesAsync(searchTarget, maxWaitSeconds, cancellationToken))
        {
            if(description is null) continue;

            if(_descriptionSerializer.Deserialize(description.CreateReader()) is not TDescription typedDescription) continue;

            DeviceDiscovered?.Invoke(this, new UpnpDeviceDiscoveredEventArgs<TDescription>(uri, typedDescription));
            yield return new Tuple<Uri, TDescription>(uri, typedDescription);
        }
    }

    public void Dispose()
    {
        _upnpClient.DeviceDiscovered -= ServiceDiscovered;
    }

    public async Task RunDiscoverDevicesAsync(string searchTarget = "ssdp:all", int maxWaitSeconds = 1, bool waitForResponses = false, CancellationToken cancellationToken = default)
    {
        await _upnpClient.RunDiscoverDevicesAsync(searchTarget, maxWaitSeconds, waitForResponses, cancellationToken);
    }

    private async void ServiceDiscovered(object? sender, UpnpDeviceDiscoveredEventArgs e)
    {
        if(e.ServiceDescription is null) return;

        if(_descriptionSerializer.Deserialize(e.ServiceDescription.CreateReader()) is not TDescription typedDescription) return;

        DeviceDiscovered?.Invoke(this, new UpnpDeviceDiscoveredEventArgs<TDescription>(e.ServiceDescriptionLocation, typedDescription));
    }
}
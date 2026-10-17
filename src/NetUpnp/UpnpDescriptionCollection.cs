using System.Collections;
using System.Collections.Concurrent;
using System.Xml.Linq;
using Kreisverkehr.NetUpnp.Extensions;
using Microsoft.Extensions.Logging;

namespace Kreisverkehr.NetUpnp;

public interface IUpnpDescriptionCollection : IReadOnlyCollection<IReadOnlyDictionary<Uri, XDocument>> { }

public class UpnpDescriptionCollection : IUpnpDescriptionCollection
{
    private readonly XNamespace _upnpNamespace = "urn:schemas-upnp-org:device-1-0";
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<Uri, XDocument>> _descriptions = new ConcurrentDictionary<string, ConcurrentDictionary<Uri, XDocument>>();
    private readonly ILogger<UpnpDescriptionCollection> _logger;

    public int Count => _descriptions.Values.Sum(dict => dict.Count);

    public UpnpDescriptionCollection(IUpnpClient client, ILogger<UpnpDescriptionCollection> logger)
    {
        client.DeviceDiscovered += (sender, args) => AddDescription(args.ServiceDescriptionLocation, args.ServiceDescription);
        _logger = logger;
    }

    private void AddDescription(Uri serviceDescriptionLocation, XDocument description)
    {
        string? uniqueDeviceName = description.Root?.Element(_upnpNamespace + "device")?.Element(_upnpNamespace + "UDN")?.Value;
        if (string.IsNullOrEmpty(uniqueDeviceName))
            return;

        _descriptions.AddOrUpdate(uniqueDeviceName, AddDescription, UpdateDescription);

        ConcurrentDictionary<Uri, XDocument> AddDescription(string uniqueDeviceName)
        {
            _logger?.LogDescriptionLoaded(serviceDescriptionLocation, uniqueDeviceName);
            return new ConcurrentDictionary<Uri, XDocument>([new KeyValuePair<Uri, XDocument>(serviceDescriptionLocation, description)]);
        }

        ConcurrentDictionary<Uri, XDocument> UpdateDescription(string uniqueDeviceName, ConcurrentDictionary<Uri, XDocument> existingDict)
        {
            _logger?.LogDescriptionUpdated(serviceDescriptionLocation, uniqueDeviceName);
            existingDict.AddOrUpdate(serviceDescriptionLocation, description, (_, _) => description);
            return existingDict;
        }
    }

    public IEnumerator<IReadOnlyDictionary<Uri, XDocument>> GetEnumerator()
    {
        return _descriptions.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
using System.Collections;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Kreisverkehr.NetUpnp;

public interface IUpnpDescriptionCollection : IReadOnlyCollection<IReadOnlyDictionary<Uri, XDocument>> { }

public class UpnpDescriptionCollection : IUpnpDescriptionCollection
{
    private readonly XNamespace _upnpNamespace = "urn:schemas-upnp-org:device-1-0";
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<Uri, XDocument>> _descriptions = new ConcurrentDictionary<string, ConcurrentDictionary<Uri, XDocument>>();

    public int Count => _descriptions.Values.Sum(dict => dict.Count);

    public UpnpDescriptionCollection(IUpnpClient client)
    {
        client.DeviceDiscovered += (sender, args) => AddDescription(args.ServiceDescriptionLocation, args.ServiceDescription);
    }

    private void AddDescription(Uri serviceDescriptionLocation, XDocument description)
    {
        string? uniqueDeviceName = description.Root?.Element(_upnpNamespace + "device")?.Element(_upnpNamespace + "UDN")?.Value;
        if(string.IsNullOrEmpty(uniqueDeviceName))
            return;

        _descriptions.AddOrUpdate(
            uniqueDeviceName,
            _ => new ConcurrentDictionary<Uri, XDocument>([new KeyValuePair<Uri, XDocument>(serviceDescriptionLocation, description)]),
            (_, existingDict) =>
            {
                existingDict.AddOrUpdate(serviceDescriptionLocation, description, (_, _) => description);
                return existingDict;
            });
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
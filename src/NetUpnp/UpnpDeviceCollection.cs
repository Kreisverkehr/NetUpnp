using System.Collections;
using System.Collections.Concurrent;
using System.Xml.Serialization;
using Kreisverkehr.NetUpnp.Model;

namespace Kreisverkehr.NetUpnp;

public interface IUpnpDeviceCollection : IUpnpDeviceCollection<UpnpDescription>;
public interface IUpnpDeviceCollection<T> : IReadOnlyCollection<Tuple<Uri, T>> where T : UpnpDescription { }
public class UpnpDeviceCollection(IUpnpDescriptionCollection descriptionCollection) 
    : UpnpDeviceCollection<UpnpDescription>(descriptionCollection), IUpnpDeviceCollection { }

public class UpnpDeviceCollection<T> : IUpnpDeviceCollection<T> where T : UpnpDescription
{
    private readonly IUpnpDescriptionCollection _descriptionCollection;
    private readonly XmlSerializer _descriptionSerializer = new(typeof(T));

    public UpnpDeviceCollection(IUpnpDescriptionCollection descriptionCollection)
    {
        _descriptionCollection = descriptionCollection;
    }

    public int Count => _descriptionCollection.Count;

    public IEnumerator<Tuple<Uri, T>> GetEnumerator()
    {
        var descriptions = 
            from descriptionDict in _descriptionCollection
            from kvp in descriptionDict
            let description = (T)_descriptionSerializer.Deserialize(kvp.Value.CreateReader())!
            select new Tuple<Uri, T>(kvp.Key, description);

        return descriptions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
using System.Xml.Serialization;

namespace Kreisverkehr.NetUpnp.Model;

public class UpnpSpecVersion
{
    [XmlElement("major", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string Major { get; set; }

    [XmlElement("minor", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string Minor { get; set; }
}
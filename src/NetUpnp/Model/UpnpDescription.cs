using System.Xml.Serialization;

namespace Kreisverkehr.NetUpnp.Model;

[XmlRoot("root", Namespace = "urn:schemas-upnp-org:device-1-0")]
public class UpnpDescription
{
    [XmlElement("configId", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string ConfigId { get; set; }

    [XmlElement("specVersion", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required UpnpSpecVersion SpecVersion { get; set; }

    [XmlElement("URLBase", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual string? UrlBase { get; set; }

    [XmlElement("device", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required UpnpDevice Device { get; set; }
}
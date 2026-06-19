using System.Xml.Serialization;

namespace Kreisverkehr.NetUpnp.Model;
    
public class UpnpService
{
    [XmlElement("serviceType", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string ServiceType { get; set; }

    [XmlElement("serviceId", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string ServiceId { get; set; }

    [XmlElement("SCPDURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string ServiceDescriptionLocation { get; set; }

    [XmlElement("controlURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string ControlUrl { get; set; }

    [XmlElement("eventSubURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string EventSubUrl { get; set; }
}
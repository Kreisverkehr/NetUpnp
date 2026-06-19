using System.Xml.Serialization;

namespace Kreisverkehr.NetUpnp.Model;

public class UpnpIcon
{
    [XmlElement("mimetype", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string MimeType { get; set; }
    
    [XmlElement("width", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required int Width { get; set; }
    
    [XmlElement("height", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required int Height { get; set; }
    
    [XmlElement("depth", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required int Depth { get; set; }
    
    [XmlElement("url", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string Url { get; set; }
}
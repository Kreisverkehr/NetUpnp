using System.Xml.Serialization;

namespace Kreisverkehr.NetUpnp.Model;

public class UpnpDevice : IEquatable<UpnpDevice>
{
    [XmlElement("deviceType", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string DeviceType { get; set; }
    
    [XmlElement("friendlyName", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string FriendlyName { get; set; }
    
    [XmlElement("manufacturer", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string Manufacturer { get; set; }
    
    [XmlElement("manufacturerURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual string? ManufacturerUrl { get; set; }
    
    [XmlElement("modelDescription", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual string? ModelDescription { get; set; }
    
    [XmlElement("modelName", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string ModelName { get; set; }
    
    [XmlElement("modelNumber", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual string? ModelNumber { get; set; }
    
    [XmlElement("modelURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual string? ModelUrl { get; set; }
    
    [XmlElement("serialNumber", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual string? SerialNumber { get; set; }
    
    [XmlElement("UDN", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual required string UniqueDeviceName { get; set; }
    
    [XmlElement("UPC", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual string? UniversalProductCode { get; set; }

    [XmlArray("iconList", Namespace = "urn:schemas-upnp-org:device-1-0")]
    [XmlArrayItem("icon", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual List<UpnpIcon> Icons {get;set;} = new List<UpnpIcon>();

    [XmlArray("serviceList", Namespace = "urn:schemas-upnp-org:device-1-0")]
    [XmlArrayItem("service", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual List<UpnpService> Services {get;set;} = new List<UpnpService>();

    [XmlArray("deviceList", Namespace = "urn:schemas-upnp-org:device-1-0")]
    [XmlArrayItem("device", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual List<UpnpDevice> Devices {get;set;} = new List<UpnpDevice>();

    [XmlElement("presentationURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public virtual string? PresentationUrl { get; set; }

    internal UpnpDevice UpdateDevice(UpnpDevice newDevice)
    {
        DeviceType = newDevice.DeviceType;
        FriendlyName = newDevice.FriendlyName;
        Manufacturer = newDevice.Manufacturer;
        ManufacturerUrl = newDevice.ManufacturerUrl;
        ModelDescription = newDevice.ModelDescription;
        ModelName = newDevice.ModelName;
        ModelNumber = newDevice.ModelNumber;
        ModelUrl = newDevice.ModelUrl;
        SerialNumber = newDevice.SerialNumber;
        UniqueDeviceName = newDevice.UniqueDeviceName;
        UniversalProductCode = newDevice.UniversalProductCode;
        Icons = newDevice.Icons;
        Services = newDevice.Services;
        Devices = newDevice.Devices;
        PresentationUrl = newDevice.PresentationUrl;
        return this;
    }

    public bool Equals(UpnpDevice? other) 
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return UniqueDeviceName == other.UniqueDeviceName;
    }

    public override bool Equals(object? obj)
    {
        if(obj is UpnpDevice other)
        {
            return Equals(other);
        }
        return false;
    }

    override public int GetHashCode() => UniqueDeviceName.GetHashCode();
}
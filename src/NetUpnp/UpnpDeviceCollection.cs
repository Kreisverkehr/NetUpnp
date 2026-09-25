using System.Collections;
using System.Collections.Concurrent;
using Kreisverkehr.NetUpnp.Model;

namespace Kreisverkehr.NetUpnp;

public interface IUpnpDeviceCollection : IReadOnlyCollection<UpnpDevice>
{
    void AddDevice(UpnpDevice device);
}

public class UpnpDeviceCollection : IUpnpDeviceCollection
{
    private readonly ConcurrentDictionary<string, UpnpDevice> _devices = new ConcurrentDictionary<string, UpnpDevice>();
    private readonly IUpnpClient _client;

    public int Count => _devices.Count;

    public UpnpDeviceCollection(IUpnpClient client)
    {
        _client = client;
        _client.DeviceDiscovered += (sender, args) => AddDevice(args.Device);
    }

    public void AddDevice(UpnpDevice device)
    {
        _devices.AddOrUpdate(device.UniqueDeviceName, device, (key, oldValue) => oldValue.UpdateDevice(device));
    }

    public IEnumerator<UpnpDevice> GetEnumerator()
    {
        return _devices.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
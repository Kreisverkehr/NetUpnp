# Samples

These samples are .NET file-based apps. Run them from the repository root with .NET 10:

```sh
dotnet run Samples/DiscoverDevices.cs
dotnet run Samples/CustomizeUpnpDescription.cs
```

Both samples perform network discovery, so they must be run on a network where UPnP devices are reachable.

## DiscoverDevices.cs

Discovers all available UPnP devices (`ssdp:all`), prints each device as it is found, and displays a table with basic device metadata. The sample resolves `IUpnpClient<UpnpDescription>` and reads the returned `(Uri, UpnpDescription)` tuples so the service description URL remains available alongside the parsed XML model.

## CustomizeUpnpDescription.cs

Discovers SAT>IP server devices and demonstrates mapping vendor-specific XML elements into custom `UpnpDescription` and `UpnpDevice` types via `IUpnpClient<SapIpServerDescription>`. It prints the SAT>IP capabilities and M3U URL when those values are present.

> [!WARNING]
> NetUpnp is a work in progress. The samples and API may change as the project develops.

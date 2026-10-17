using Microsoft.Extensions.Logging;

namespace Kreisverkehr.NetUpnp.Extensions;

internal static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 1000,
        EventName = "UPNP Startup",
        Level = LogLevel.Information,
        Message = "UPNP Client started"
        )]
    internal static partial void LogStart(this ILogger logger);

    [LoggerMessage(
        EventId = 2000,
        EventName = "UPNP description loaded",
        Level = LogLevel.Information,
        Message = "A new UPNP device description has been loaded from location {uri} for device {udn}"
        )]
    internal static partial void LogDescriptionLoaded(this ILogger logger, Uri uri, string udn);

    [LoggerMessage(
        EventId = 2010,
        EventName = "UPNP description updated",
        Level = LogLevel.Debug,
        Message = "A new UPNP device description has been updated from location {uri} for device {udn}"
        )]
    internal static partial void LogDescriptionUpdated(this ILogger logger, Uri uri, string udn);

    [LoggerMessage(
        EventId = 2100,
        EventName = "UPNP description load failed",
        Level = LogLevel.Warning,
        Message = "A new UPNP device description could not be loaded from {uri}."
        )]
    internal static partial void LogDescriptionLoadFailed(this ILogger logger, Uri uri, Exception ex);


}
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
        Message = "A new UPNP device description has been loaded from location {uri}"
        )]
    internal static partial void LogDescriptionLoaded(this ILogger logger, Uri uri);

    [LoggerMessage(
        EventId = 2100,
        EventName = "UPNP description load failed",
        Level = LogLevel.Warning,
        Message = "A new UPNP device description could not be loaded from {uri}. Exception: {ex}"
        )]
    internal static partial void LogDescriptionLoadFailed(this ILogger logger, Uri uri, Exception ex);


}
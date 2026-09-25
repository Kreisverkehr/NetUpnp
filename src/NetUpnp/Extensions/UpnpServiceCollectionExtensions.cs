using Kreisverkehr.NetUpnp;
using Kreisverkehr.NetUpnp.Options;
using Microsoft.Extensions.Configuration;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class UpnpServiceCollectionExtensions
{
    public static IServiceCollection AddUpnp(this IServiceCollection services) => services
        .AddSsdp()
        .AddSingleton<IUpnpClient, UpnpClient>()
        .AddSingleton<IUpnpDeviceCollection, UpnpDeviceCollection>()
        .AddHttpClient()
    ;

    public static IServiceCollection AddUpnp(this IServiceCollection services, Action<UpnpOptions>? configure = null) => services
        .AddUpnp()
        .AddOptions<UpnpOptions>()
            .Configure(configure ?? (_ => { })).Services
    ;

    public static IServiceCollection AddUpnp(this IServiceCollection services, Action<UpnpOptions, IServiceProvider>? configure = null) => services
        .AddUpnp()
        .AddOptions<UpnpOptions>()
            .Configure(configure ?? ((_, _) => { })).Services
    ;

    public static IServiceCollection AddUpnp(this IServiceCollection services, IConfiguration configuration) => services
        .AddUpnp()
        .AddOptions<UpnpOptions>()
            .Bind(configuration)
            .ValidateOnStart()
            .Services
    ;
}

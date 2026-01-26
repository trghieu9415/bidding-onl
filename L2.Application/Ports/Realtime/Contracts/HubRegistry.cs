using Microsoft.Extensions.DependencyInjection;

namespace L2.Application.Ports.Realtime.Contracts;

public class HubRegistry {
  private readonly Dictionary<string, Type> _hubs = new();

  public void Register<THub>(string key) where THub : class {
    _hubs[key] = typeof(THub);
  }

  public bool TryGetHubType(string key, out Type? hubType) {
    return _hubs.TryGetValue(key, out hubType);
  }
}

public static class HubRegistryExtensions {
  public static IServiceCollection AddHubRegistry(this IServiceCollection services, Action<HubRegistry> configure) {
    var registry = new HubRegistry();
    configure(registry);
    services.AddSingleton(registry);
    return services;
  }
}

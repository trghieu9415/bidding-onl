using L2.Application.Ports.Logging;
using Microsoft.Extensions.Logging;

namespace L3.Infrastructure.Adapters.Logging;

public class SystemLogger<T>(ILogger<T> logger) : ISystemLogger<T> {
  public void LogWarning(string message, params object[] args) {
    using (logger.BeginScope(new Dictionary<string, object> { { "LogType", "SystemWarning" } })) {
      logger.LogWarning(message, args);
    }
  }

  public void LogError(Exception ex, string message, params object[] args) {
    logger.LogError(ex, message, args);
  }
}

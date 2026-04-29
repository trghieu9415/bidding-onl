using L2.Application.Ports.Logging;
using Microsoft.Extensions.Logging;

namespace L3.Infrastructure.Adapters.Logging;

public class BusinessLogger<T>(ILogger<T> logger) : IBusinessLogger<T> {
  public void LogInformation(string message, params object[] args) {
    using (logger.BeginScope(new Dictionary<string, object> { { "LogType", "BusinessInfo" } })) {
      logger.LogInformation(message, args);
    }
  }

  public void LogError(Exception ex, string message, params object[] args) {
    using (logger.BeginScope(new Dictionary<string, object> { { "LogType", "BusinessError" } })) {
      logger.LogWarning(ex, message, args);
    }
  }
}

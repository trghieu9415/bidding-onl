namespace L2.Application.Ports.Logging;

public interface ISystemLogger<T> {
  void LogWarning(string message, params object[] args);
  void LogError(Exception ex, string message, params object[] args);
}

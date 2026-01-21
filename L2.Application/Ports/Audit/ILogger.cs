namespace L2.Application.Ports.Logging;

public interface ILogger {
  void LogInformation(string message, params object[] args);
  void LogError(System.Exception? exception, string message, params object[] args);
}

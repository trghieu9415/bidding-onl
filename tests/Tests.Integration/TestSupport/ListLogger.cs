using Microsoft.Extensions.Logging;

namespace Tests.Integration.TestSupport;

internal sealed class ListLogger<T> : ILogger<T> {
  public List<LogRecord> Records { get; } = [];
  public List<object> Scopes { get; } = [];

  public IDisposable? BeginScope<TState>(TState state) where TState : notnull {
    Scopes.Add(state);
    return new ScopeHandle();
  }

  public bool IsEnabled(LogLevel logLevel) {
    return true;
  }

  public void Log<TState>(
    LogLevel logLevel,
    EventId eventId,
    TState state,
    Exception? exception,
    Func<TState, Exception?, string> formatter
  ) {
    Records.Add(new LogRecord(logLevel, formatter(state, exception), exception, state));
  }

  private sealed class ScopeHandle : IDisposable {
    public void Dispose() {
    }
  }
}

internal sealed record LogRecord(LogLevel Level, string Message, Exception? Exception, object State);

using L3.Infrastructure.Adapters.Logging;
using Microsoft.Extensions.Logging;
using Tests.Integration.TestSupport;
using Xunit;

namespace Tests.Integration.L3.Infrastructure.Adapters.Logging;

public class LoggerAdapterTests {
  [Fact]
  public void BusinessLogger_writes_information_with_business_scope() {
    var logger = new ListLogger<LoggerAdapterTests>();
    var adapter = new BusinessLogger<LoggerAdapterTests>(logger);

    adapter.LogInformation("Order {OrderId} confirmed", 123);

    Assert.Equal(LogLevel.Information, logger.Records[0].Level);
    Assert.Contains("Order 123 confirmed", logger.Records[0].Message);
    Assert.Equal("BusinessInfo", GetScopeValue(logger.Scopes[0], "LogType"));
  }

  [Fact]
  public void BusinessLogger_logs_error_using_warning_level_and_business_error_scope() {
    var logger = new ListLogger<LoggerAdapterTests>();
    var adapter = new BusinessLogger<LoggerAdapterTests>(logger);

    adapter.LogError(new InvalidOperationException("boom"), "Could not process {OrderId}", 123);

    Assert.Equal(LogLevel.Warning, logger.Records[0].Level);
    Assert.Equal("BusinessError", GetScopeValue(logger.Scopes[0], "LogType"));
  }

  [Fact]
  public void SystemLogger_writes_warning_scope_and_error_level() {
    var logger = new ListLogger<LoggerAdapterTests>();
    var adapter = new SystemLogger<LoggerAdapterTests>(logger);

    adapter.LogWarning("Slow query {Query}", "search");
    adapter.LogError(new InvalidOperationException("boom"), "Failure {Id}", 99);

    Assert.Equal(LogLevel.Warning, logger.Records[0].Level);
    Assert.Equal("SystemWarning", GetScopeValue(logger.Scopes[0], "LogType"));
    Assert.Equal(LogLevel.Error, logger.Records[1].Level);
  }

  private static string? GetScopeValue(object scope, string key) {
    return scope is IEnumerable<KeyValuePair<string, object>> pairs
      ? pairs.FirstOrDefault(x => x.Key == key).Value?.ToString()
      : null;
  }
}

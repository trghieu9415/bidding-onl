using Serilog;
using Serilog.Events;

namespace L0.API.Extensions;

public static class SerilogExtension {
  public static void AddSerilogCustom(this WebApplicationBuilder builder) {
    var logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
    Directory.CreateDirectory(logDir);

    const string sysTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";
    const string bizTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}";

    Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Information()
      .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
      .Enrich.FromLogContext()
      .WriteTo.Console(outputTemplate: sysTemplate)
      .WriteTo.Map(
        evt => (IsBusiness: IsBusinessLog(evt), evt.Level),
        (key, wt) => {
          var (isBiz, level) = key;
          var fileName = (isBiz, level) switch {
            (true, >= LogEventLevel.Warning) => "biz-error-.log",
            (true, _) => "biz-info-.log",
            (false, >= LogEventLevel.Error) => "sys-error-.log",
            (false, LogEventLevel.Warning) => "sys-warn-.log",
            _ => null
          };

          if (fileName != null) {
            wt.File(Path.Combine(logDir, fileName),
              rollingInterval: RollingInterval.Day,
              outputTemplate: isBiz ? bizTemplate : sysTemplate
            );
          }
        }).CreateLogger();

    builder.Host.UseSerilog();
  }

  private static bool IsBusinessLog(LogEvent logEvent) {
    return
      logEvent.Properties.TryGetValue("LogType", out var value) &&
      value is ScalarValue { Value: string str } &&
      str.StartsWith("Business");
  }
}

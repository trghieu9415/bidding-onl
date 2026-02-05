using Serilog;
using Serilog.Events;

namespace L0.API.Extensions;

public static class SerilogExtension {
  public static void AddSerilogCustom(this WebApplicationBuilder builder) {
    var logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "error-.log");

    Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Information()
      .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
      .Enrich.FromLogContext()
      // .WriteTo.File(
      //   logPath,
      //   LogEventLevel.Error,
      //   "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}",
      //   rollingInterval: RollingInterval.Minute
      // )
      .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
      )
      .CreateLogger();

    builder.Host.UseSerilog();
  }
}

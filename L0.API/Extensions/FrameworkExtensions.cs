using System.Text.Json;
using System.Text.Json.Serialization;

namespace L0.API.Extensions;

public static class FrameworkExtensions {
  public static IServiceCollection AddWebFramework(this IServiceCollection services) {
    // --- Controller & JSON Config ---
    services
      .AddControllers()
      .ConfigureApiBehaviorOptions(options => {
        options.SuppressModelStateInvalidFilter = true;
      })
      .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
      });

    // --- Routing Configuration ---
    services.AddRouting(options => {
      options.LowercaseUrls = true;
      options.LowercaseQueryStrings = true;
    });

    return services;
  }
}

using System.Text.Json;

namespace L3.Infrastructure.Adapters.Gateway;

public static class GatewayPayloadExtensions {
  public static Dictionary<string, string> ExtractProperties(this JsonElement payload, params string[] propertyNames) {
    var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    var missingProps = new List<string>();

    foreach (var prop in propertyNames) {
      if (payload.TryGetProperty(prop, out var element)) {
        var value = element.ToString();

        if (string.IsNullOrWhiteSpace(value)) {
          missingProps.Add(prop);
        } else {
          result[prop] = value;
        }
      } else {
        missingProps.Add(prop);
      }
    }

    if (missingProps.Count != 0) {
      throw new ArgumentException(
        $"Payload JSON không hợp lệ! Thiếu hoặc rỗng các thuộc tính bắt buộc: {string.Join(", ", missingProps)}.");
    }

    return result;
  }


  public static Dictionary<string, string> ExtractProperties(this string rawJson, params string[] propertyNames) {
    ArgumentException.ThrowIfNullOrWhiteSpace(rawJson);

    using var document = JsonDocument.Parse(rawJson);
    return document.RootElement.ExtractProperties(propertyNames);
  }
}

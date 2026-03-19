using System.Text.Json;

namespace L3.Infrastructure.Adapters.Gateway;

public static class GatewayPayloadExtensions {
  /// <summary>
  ///   Hàm phân tích object để lấy ra danh sách các thuộc tính mong muốn.
  /// </summary>
  public static Dictionary<string, string> ExtractProperties(this object data, params string[] propertyNames) {
    if (data == null) {
      throw new ArgumentNullException(nameof(data), "Payload data không được để trống.");
    }

    var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    switch (data) {
      case JsonElement jsonElement: {
        foreach (var prop in propertyNames) {
          result[prop] = jsonElement.TryGetProperty(prop, out var el) ? el.GetString() ?? "" : "";
        }

        break;
      }
      case IDictionary<string, object> dict: {
        foreach (var prop in propertyNames) {
          result[prop] = dict.TryGetValue(prop, out var val) ? val?.ToString() ?? "" : "";
        }

        break;
      }
      default: {
        var type = data.GetType();
        foreach (var prop in propertyNames) {
          var propInfo = type.GetProperty(prop);
          result[prop] = propInfo?.GetValue(data)?.ToString() ?? "";
        }

        break;
      }
    }

    var missingProps = propertyNames.Where(p => string.IsNullOrWhiteSpace(result[p])).ToList();
    if (missingProps.Count != 0) {
      throw new ArgumentException(
        $"Dữ liệu payload không hợp lệ! Thiếu các thuộc tính bắt buộc: {string.Join(", ", missingProps)}.",
        nameof(data));
    }

    return result;
  }
}

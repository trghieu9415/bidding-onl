using System.Reflection;
using System.Text.Json;

namespace Tests.Unit.L2.Application.TestDoubles;

public static class TestJson {
  public static JsonElement CreateObject(string json = "{}") {
    using var doc = JsonDocument.Parse(json);
    return doc.RootElement.Clone();
  }
}

public static class ReflectionTestHelper {
  public static void SetProperty<TValue>(object target, string propertyName, TValue value) {
    var property = target.GetType().GetProperty(
      propertyName,
      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
    );
    property!.SetValue(target, value);
  }
}

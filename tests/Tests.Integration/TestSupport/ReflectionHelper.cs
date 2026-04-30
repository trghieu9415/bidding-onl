using System.Reflection;

namespace Tests.Integration.TestSupport;

internal static class ReflectionHelper {
  public static void SetField<TTarget>(TTarget target, string fieldName, object? value) {
    var field = typeof(TTarget).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

    if (field == null) {
      throw new InvalidOperationException($"Field '{fieldName}' was not found on '{typeof(TTarget).Name}'.");
    }

    field.SetValue(target, value);
  }
}

using System.Reflection;

namespace Tests.Integration.TestSupport;

internal static class AsyncReturn {
  public static object? For(MethodInfo method, object? value = null) {
    return For(method.ReturnType, value);
  }

  public static object? For(Type returnType, object? value = null) {
    if (returnType == typeof(void)) {
      return null;
    }

    if (returnType == typeof(Task)) {
      return Task.CompletedTask;
    }

    if (returnType == typeof(ValueTask)) {
      return ValueTask.CompletedTask;
    }

    if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)) {
      var resultType = returnType.GetGenericArguments()[0];
      var fromResult = typeof(Task)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Single(x => x.Name == nameof(Task.FromResult))
        .MakeGenericMethod(resultType);

      return fromResult.Invoke(null, [value]);
    }

    if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>)) {
      return Activator.CreateInstance(returnType, value);
    }

    return value;
  }
}

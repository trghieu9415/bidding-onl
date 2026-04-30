using System.Reflection;

namespace Tests.Integration.TestSupport;

internal sealed class DynamicProxy : DispatchProxy {
  public Func<MethodInfo, object?[]?, object?> Invoker { get; set; } = null!;

  protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) {
    if (targetMethod == null) {
      throw new InvalidOperationException("Proxy target method was not provided.");
    }

    return Invoker(targetMethod, args);
  }
}

internal static class DynamicProxyFactory {
  public static T Create<T>(Func<MethodInfo, object?[]?, object?> invoker) where T : class {
    var proxy = DispatchProxy.Create<T, DynamicProxy>();
    ((DynamicProxy)(object)proxy).Invoker = invoker;
    return proxy;
  }
}

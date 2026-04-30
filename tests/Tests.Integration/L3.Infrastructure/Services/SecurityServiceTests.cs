using System.Reflection;
using L3.Infrastructure.Services;
using L3.Infrastructure.Services.Abstractions;
using StackExchange.Redis;
using Tests.Integration.TestSupport;
using Xunit;

namespace Tests.Integration.L3.Infrastructure.Services;

public class SecurityServiceTests {
  [Fact]
  public async Task BlacklistAsync_and_IsBlacklistedAsync_share_the_same_redis_keys() {
    var redisStore = new Dictionary<string, string>();
    var service = new SecurityService(CreateRedis(redisStore));

    await service.BlacklistAsync("jti-1", TimeSpan.FromMinutes(10));
    var isBlacklisted = await service.IsBlacklistedAsync("jti-1");

    Assert.True(isBlacklisted);
    Assert.Equal("true", redisStore[SecurityKeys.BlackList("jti-1")]);
  }

  [Fact]
  public async Task SyncSecurityStampAsync_and_GetSecurityStampAsync_round_trip_value() {
    var redisStore = new Dictionary<string, string>();
    var service = new SecurityService(CreateRedis(redisStore));
    var userId = Guid.NewGuid();

    await service.SyncSecurityStampAsync(userId, "stamp-xyz");
    var stamp = await service.GetSecurityStampAsync(userId);

    Assert.Equal("stamp-xyz", stamp);
    Assert.Equal("stamp-xyz", redisStore[SecurityKeys.UserStamp(userId)]);
  }

  [Fact]
  public async Task GetSecurityStampAsync_returns_null_when_key_is_missing() {
    var service = new SecurityService(CreateRedis(new Dictionary<string, string>()));

    var stamp = await service.GetSecurityStampAsync(Guid.NewGuid());

    Assert.Null(stamp);
  }

  private static IConnectionMultiplexer CreateRedis(Dictionary<string, string> store) {
    var database = DynamicProxyFactory.Create<IDatabase>((method, args) => method.Name switch {
      "StringSetAsync" => HandleStringSet(method, args, store),
      "KeyExistsAsync" => AsyncReturn.For(method, store.ContainsKey(args![0]!.ToString()!)),
      "StringGetAsync" => AsyncReturn.For(method, CreateRedisValue(method.ReturnType.GenericTypeArguments[0], store, args![0]!.ToString()!)),
      _ => throw new NotSupportedException($"IDatabase.{method.Name} is not configured for this test.")
    });

    return DynamicProxyFactory.Create<IConnectionMultiplexer>((method, _) => method.Name switch {
      "GetDatabase" => database,
      _ => throw new NotSupportedException($"IConnectionMultiplexer.{method.Name} is not configured for this test.")
    });
  }

  private static object? HandleStringSet(MethodInfo method, object?[]? args, IDictionary<string, string> store) {
    store[args![0]!.ToString()!] = args[1]!.ToString()!;
    return AsyncReturn.For(method, true);
  }

  private static object CreateRedisValue(Type resultType, IReadOnlyDictionary<string, string> store, string key) {
    if (!store.TryGetValue(key, out var value)) {
      return Activator.CreateInstance(resultType)!;
    }

    var implicitOp = resultType.GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, [typeof(string)]);
    if (implicitOp == null) {
      throw new InvalidOperationException($"Could not create redis value for '{resultType.Name}'.");
    }

    return implicitOp.Invoke(null, [value])!;
  }
}

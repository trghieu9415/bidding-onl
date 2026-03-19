namespace L2.Application.Constants;

public static class CacheTags {
  // NOTE: ========== [BUSINESS] ==========
  public static string CurrentSession => "current-session";

  // NOTE: ========== [SYSTEM] ==========
  public static string BlackList(string jti) {
    return $"blacklist:{jti}";
  }

  public static string UserStamp(Guid id) {
    return $"user:{id}:stamp";
  }
}

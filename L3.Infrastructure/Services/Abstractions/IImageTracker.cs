namespace L3.Infrastructure.Services.Abstractions;

public interface IImageTracker {
  public Task<HashSet<string>> GetInUseImageUrlsAsync(CancellationToken ct = default);
}

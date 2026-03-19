namespace L3.Infrastructure.Services.Abstractions;

public interface IStorageService {
  Task<string> UploadAsync(string fileName, Stream content, string ext, string folder, CancellationToken ct);
  Task DeleteAsync(string fileName, string folder, CancellationToken ct);
  Task<List<string>> ListFilesAsync(string folder, CancellationToken ct = default);
}

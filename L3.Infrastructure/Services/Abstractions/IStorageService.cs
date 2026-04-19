namespace L3.Infrastructure.Services.Abstractions;

public interface IStorageService {
  Task<string> UploadAsync(string fileName, Stream content, string ext, string folder, CancellationToken ct);

  Task<(List<string> Urls, List<string> Errors)> UploadBatchAsync(
    IEnumerable<(string FileName, Stream Content, string Ext, string Folder)> files,
    CancellationToken ct = default
  );

  Task<bool> DeleteAsync(string fileName, string folder, CancellationToken ct);
  Task<List<string>> ListFilesAsync(string folder, CancellationToken ct = default);
}

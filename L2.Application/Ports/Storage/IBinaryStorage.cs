namespace L2.Application.Ports.Storage;

public interface IBinaryStorage {
  Task<string> UploadAsync(string fileName, Stream content, string folder, string ext, CancellationToken ct = default);
  Task DeleteAsync(string fileName, string folder, CancellationToken ct = default);
}
using L2.Application.Ports.Storage;

namespace L3.Infrastructure.Adapters.Storage;

public class LocalBinaryStorage : IBinaryStorage {
  private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

  public Task DeleteAsync(string fileName, string folder, CancellationToken ct = default) {
    var filePath = Path.Combine(_basePath, folder, fileName);

    if (File.Exists(filePath)) {
      File.Delete(filePath);
    }

    return Task.CompletedTask;
  }

  public async Task<string> UploadAsync(
    string fileName,
    Stream content,
    string ext,
    string folder,
    CancellationToken ct = default
  ) {
    var targetDirectory = Path.Combine(_basePath, folder);

    if (!Directory.Exists(targetDirectory)) {
      Directory.CreateDirectory(targetDirectory);
    }

    var uniqueFileName = $"{Guid.NewGuid()}_{fileName}.{ext}";
    var filePath = Path.Combine(targetDirectory, uniqueFileName);

    await using var fileStream = new FileStream(filePath, FileMode.Create);
    await content.CopyToAsync(fileStream, ct);

    return $"/uploads/{folder}/{uniqueFileName}";
  }
}
using L3.Infrastructure.Services.Abstractions;

namespace L3.Infrastructure.Services;

public class LocalStorageService : IStorageService {
  private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

  public Task DeleteAsync(string fileName, string folder, CancellationToken ct) {
    var filePath = Path.Combine(_basePath, folder, fileName);

    if (File.Exists(filePath)) {
      File.Delete(filePath);
    }

    return Task.CompletedTask;
  }

  public Task<List<string>> ListFilesAsync(string folder, CancellationToken ct = default) {
    throw new NotImplementedException();
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

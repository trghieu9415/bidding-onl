using Amazon.S3;
using Amazon.S3.Model;
using L3.Infrastructure.Options;
using L3.Infrastructure.Services.Abstractions;

namespace L3.Infrastructure.Services;

public class S3StorageService : IStorageService {
  private readonly string _bucketName;
  private readonly IAmazonS3 _s3Client;

  public S3StorageService(S3Settings s3Settings) {
    var s3Config = new AmazonS3Config {
      ServiceURL = s3Settings.ServiceUrl,
      ForcePathStyle = s3Settings.ForcePathStyle,
      MaxErrorRetry = s3Settings.Retry,
      Timeout = TimeSpan.FromSeconds(s3Settings.TimeOut)
    };

    _bucketName = s3Settings.BucketName;
    _s3Client = new AmazonS3Client(s3Settings.AccessKey, s3Settings.SecretKey, s3Config);
  }


  public async Task<string> UploadAsync(string fileName, Stream content, string ext, string folder,
    CancellationToken ct) {
    var key = string.IsNullOrEmpty(folder) ? $"{fileName}{ext}" : $"{folder}/{fileName}{ext}";

    var putRequest = new PutObjectRequest {
      BucketName = _bucketName,
      Key = key,
      InputStream = content,
      AutoCloseStream = true,
      ContentType = GetContentType(ext),
      CannedACL = S3CannedACL.PublicRead
    };
    await _s3Client.PutObjectAsync(putRequest, ct);

    var publicUrl = $"{_s3Client.Config.ServiceURL.TrimEnd('/')}/{_bucketName}/{key}";
    return publicUrl;
  }

  public async Task<bool> DeleteAsync(string fileName, string folder, CancellationToken ct) {
    var key = string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";

    var deleteRequest = new DeleteObjectRequest {
      BucketName = _bucketName,
      Key = key
    };

    await _s3Client.DeleteObjectAsync(deleteRequest, ct);
    return true;
  }

  public async Task<(List<string> Urls, List<string> Errors)> UploadBatchAsync(
    IEnumerable<(string FileName, Stream Content, string Ext, string Folder)> files,
    CancellationToken ct = default) {
    var urls = new List<string>();
    var errors = new List<string>();

    var uploadTasks = files.Select(async file => {
      try {
        var url = await UploadAsync(file.FileName, file.Content, file.Ext, file.Folder, ct);
        return new { Success = true, Url = url, Error = string.Empty };
      } catch (Exception ex) {
        return new { Success = false, Url = string.Empty, Error = $"'{file.FileName}': {ex.Message}" };
      }
    });

    var results = await Task.WhenAll(uploadTasks);

    urls.AddRange(results.Where(r => r.Success).Select(r => r.Url));
    errors.AddRange(results.Where(r => !r.Success).Select(r => r.Error));
    return (urls, errors);
  }

  public async Task<List<string>> ListFilesAsync(string folder, CancellationToken ct = default) {
    var request = new ListObjectsV2Request {
      BucketName = _bucketName,
      Prefix = string.IsNullOrEmpty(folder) ? "" : $"{folder}/"
    };

    var response = await _s3Client.ListObjectsV2Async(request, ct);

    var baseUrl = $"{_s3Client.Config.ServiceURL.TrimEnd('/')}/{_bucketName}/";
    return response.S3Objects
      .Where(x => !x.Key.EndsWith('/'))
      .Select(x => $"{baseUrl}{x.Key}")
      .ToList();
  }

  // NOTE: ========== [Private Helper] ==========
  private string GetContentType(string ext) {
    return ext.ToLower() switch {
      ".jpg" or ".jpeg" => "image/jpeg",
      ".png" => "image/png",
      ".pdf" => "application/pdf",
      _ => "application/octet-stream"
    };
  }
}

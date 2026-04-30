using Amazon.S3;
using Amazon.S3.Model;
using L3.Infrastructure.Options;
using L3.Infrastructure.Services;
using Tests.Integration.TestSupport;
using Xunit;

namespace Tests.Integration.L3.Infrastructure.Services;

public class S3StorageServiceTests {
  [Fact]
  public async Task UploadAsync_builds_expected_key_and_public_url() {
    PutObjectRequest? captured = null;
    var fakeS3 = DynamicProxyFactory.Create<IAmazonS3>((method, args) => method.Name switch {
      "PutObjectAsync" => CapturePutObject(method, args, request => captured = request),
      _ => throw new NotSupportedException($"IAmazonS3.{method.Name} is not configured for this test.")
    });

    var service = CreateService(fakeS3);
    await using var stream = new MemoryStream([1, 2, 3]);

    var url = await service.UploadAsync("poster", stream, ".png", "catalog", CancellationToken.None);

    Assert.NotNull(captured);
    Assert.Equal("bucket-test", captured!.BucketName);
    Assert.Equal("catalog/poster.png", captured.Key);
    Assert.Equal("image/png", captured.ContentType);
    Assert.Equal("https://storage.test/bucket-test/catalog/poster.png", url);
  }

  [Fact]
  public async Task DeleteAsync_sends_expected_key() {
    DeleteObjectRequest? captured = null;
    var fakeS3 = DynamicProxyFactory.Create<IAmazonS3>((method, args) => method.Name switch {
      "DeleteObjectAsync" => CaptureDeleteObject(method, args, request => captured = request),
      _ => throw new NotSupportedException($"IAmazonS3.{method.Name} is not configured for this test.")
    });

    var service = CreateService(fakeS3);
    var result = await service.DeleteAsync("poster.png", "catalog", CancellationToken.None);

    Assert.True(result);
    Assert.NotNull(captured);
    Assert.Equal("bucket-test", captured!.BucketName);
    Assert.Equal("catalog/poster.png", captured.Key);
  }

  [Fact]
  public async Task ListFilesAsync_returns_public_urls_for_non_folder_objects() {
    var fakeS3 = DynamicProxyFactory.Create<IAmazonS3>((method, _) => method.Name switch {
      "ListObjectsV2Async" => AsyncReturn.For(method, new ListObjectsV2Response {
        S3Objects = {
          new S3Object { Key = "catalog/a.jpg" },
          new S3Object { Key = "catalog/folder/" },
          new S3Object { Key = "catalog/b.jpg" }
        }
      }),
      _ => throw new NotSupportedException($"IAmazonS3.{method.Name} is not configured for this test.")
    });

    var service = CreateService(fakeS3);
    var result = await service.ListFilesAsync("catalog");

    Assert.Equal([
      "https://storage.test/bucket-test/catalog/a.jpg",
      "https://storage.test/bucket-test/catalog/b.jpg"
    ], result);
  }

  [Fact]
  public async Task UploadBatchAsync_collects_successes_and_failures() {
    var fakeS3 = DynamicProxyFactory.Create<IAmazonS3>((method, args) => method.Name switch {
      "PutObjectAsync" => HandleBatchPut(method, args),
      _ => throw new NotSupportedException($"IAmazonS3.{method.Name} is not configured for this test.")
    });

    var service = CreateService(fakeS3);
    var files = new List<(string FileName, Stream Content, string Ext, string Folder)> {
      ("ok", new MemoryStream([1]), ".jpg", "catalog"),
      ("fail", new MemoryStream([2]), ".jpg", "catalog")
    };

    var (urls, errors) = await service.UploadBatchAsync(files);

    Assert.Single(urls);
    Assert.Equal("https://storage.test/bucket-test/catalog/ok.jpg", urls[0]);
    Assert.Single(errors);
    Assert.Contains("'fail': upload failed", errors[0]);
  }

  private static S3StorageService CreateService(IAmazonS3 s3Client) {
    var service = new S3StorageService(new S3Settings {
      ServiceUrl = "https://storage.test/",
      BucketName = "bucket-test",
      AccessKey = "ak",
      SecretKey = "sk",
      ForcePathStyle = true,
      Retry = 1,
      TimeOut = 5
    });

    ReflectionHelper.SetField(service, "_s3Client", s3Client);
    return service;
  }

  private static object CapturePutObject(
    System.Reflection.MethodInfo method,
    object?[]? args,
    Action<PutObjectRequest> onCapture
  ) {
    onCapture((PutObjectRequest)args![0]!);
    return AsyncReturn.For(method, new PutObjectResponse());
  }

  private static object CaptureDeleteObject(
    System.Reflection.MethodInfo method,
    object?[]? args,
    Action<DeleteObjectRequest> onCapture
  ) {
    onCapture((DeleteObjectRequest)args![0]!);
    return AsyncReturn.For(method, new DeleteObjectResponse());
  }

  private static object HandleBatchPut(System.Reflection.MethodInfo method, object?[]? args) {
    var request = (PutObjectRequest)args![0]!;
    if (request.Key.Contains("fail", StringComparison.Ordinal)) {
      throw new InvalidOperationException("upload failed");
    }

    return AsyncReturn.For(method, new PutObjectResponse());
  }
}

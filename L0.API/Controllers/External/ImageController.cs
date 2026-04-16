using L0.API.Response;
using L3.Infrastructure.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers.External;

// TODO: Sử dụng SixLabors.ImageSharp để xử lý hình ảnh tránh mã độc
[Authorize]
public class ImageController(IStorageService storage) : ExternalController {
  [HttpPost("upload")]
  [RequestSizeLimit(5 * 1024 * 1024)]
  [ProducesSuccess<UrlResponse>]
  public async Task<IActionResult> Upload(IFormFile? file, CancellationToken ct) {
    if (file == null || file.Length == 0) {
      return ApiResponse.Fail("Vui lòng chọn ảnh để tải lên", 400);
    }

    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    var extension = Path.GetExtension(file.FileName).ToLower();

    if (!allowedExtensions.Contains(extension)) {
      return ApiResponse.Fail("Định dạng ảnh không được hỗ trợ", 400);
    }

    if (file.Length > 5 * 1024 * 1024) {
      return ApiResponse.Fail("Dung lượng ảnh không được vượt quá 5MB", 400);
    }

    try {
      await using var stream = file.OpenReadStream();
      var fileName = Path.GetFileNameWithoutExtension(file.FileName);

      var url = await storage.UploadAsync(
        fileName,
        stream,
        "catalog",
        extension.TrimStart('.'),
        ct
      );

      return ApiResponse.Success(new { Url = url }, "Tải ảnh lên thành công");
    } catch (Exception ex) {
      return ApiResponse.Fail($"Lỗi khi lưu tệp: {ex.Message}", 500);
    }
  }

  [HttpDelete("remove")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> Delete([FromQuery] string fileName, CancellationToken ct) {
    if (string.IsNullOrEmpty(fileName)) {
      return ApiResponse.Fail("Thiếu tên tệp", 400);
    }

    try {
      await storage.DeleteAsync(fileName, "catalog", ct);
      return ApiResponse.Success("Đã xóa ảnh thành công");
    } catch (Exception) {
      return ApiResponse.Fail("Không thể xóa ảnh", 400);
    }
  }
}

public record UrlResponse(string Url);

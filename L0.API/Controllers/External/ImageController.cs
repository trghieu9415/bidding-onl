// FILE: L0.API/Controllers/External/ImageController.cs

using L0.API.Response;
using L2.Application.Ports.Storage;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers.External;

public class ImageController(IBinaryStorage storage) : ExternalController {
  [HttpPost("upload")]
  public async Task<IActionResult> Upload(IFormFile? file, CancellationToken ct = default) {
    if (file == null || file.Length == 0) {
      return AppResponse.Fail("Vui lòng chọn ảnh để tải lên", 400);
    }

    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    var extension = Path.GetExtension(file.FileName).ToLower();

    if (!allowedExtensions.Contains(extension)) {
      return AppResponse.Fail("Định dạng ảnh không được hỗ trợ", 400);
    }

    if (file.Length > 5 * 1024 * 1024) {
      return AppResponse.Fail("Dung lượng ảnh không được vượt quá 5MB", 400);
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

      return AppResponse.Success(new { Url = url }, "Tải ảnh lên thành công");
    } catch (Exception ex) {
      return AppResponse.Fail($"Lỗi khi lưu tệp: {ex.Message}", 500);
    }
  }

  [HttpDelete("remove")]
  public async Task<IActionResult> Delete([FromQuery] string fileName, CancellationToken ct = default) {
    if (string.IsNullOrEmpty(fileName)) {
      return AppResponse.Fail("Thiếu tên tệp", 400);
    }

    try {
      await storage.DeleteAsync(fileName, "catalog", ct);
      return AppResponse.Success("Đã xóa ảnh thành công");
    } catch (Exception ex) {
      return AppResponse.Fail("Không thể xóa ảnh", 400);
    }
  }
}

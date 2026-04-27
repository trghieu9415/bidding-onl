using FluentValidation;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.RegisterItem;

public record RegisterItemCommand(
  Guid UserId,
  RegisterItemRequest Data
) : IRequest<Guid>, ITransactional;

public record RegisterItemRequest(
  string Name,
  string Description,
  decimal StartingPrice,
  ItemCondition Condition,
  List<Guid> CategoryIds,
  string? MainImageUrl,
  List<string> SubImageUrls
);

public sealed class RegisterItemValidator : AbstractValidator<RegisterItemRequest> {
  public RegisterItemValidator() {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Tên sản phẩm không được để trống.")
      .MaximumLength(200)
      .WithMessage("Tên sản phẩm không được vượt quá 200 ký tự.");

    RuleFor(x => x.Description)
      .NotEmpty()
      .WithMessage("Mô tả sản phẩm không được để trống.")
      .MaximumLength(2000)
      .WithMessage("Mô tả sản phẩm không được vượt quá 2000 ký tự.");

    RuleFor(x => x.StartingPrice)
      .GreaterThan(0)
      .WithMessage("Giá khởi điểm phải lớn hơn 0.");

    RuleFor(x => x.CategoryIds)
      .NotNull()
      .WithMessage("Danh sách danh mục không được để trống.")
      .Must(x => x.Count > 0)
      .WithMessage("Phải chọn ít nhất 1 danh mục.");

    RuleForEach(x => x.CategoryIds)
      .NotEmpty()
      .WithMessage("Id danh mục không hợp lệ.");

    RuleFor(x => x.MainImageUrl)
      .MaximumLength(1000)
      .WithMessage("Đường dẫn ảnh chính không được vượt quá 1000 ký tự.")
      .When(x => !string.IsNullOrWhiteSpace(x.MainImageUrl));

    RuleFor(x => x.SubImageUrls)
      .NotNull()
      .WithMessage("Danh sách ảnh phụ không được để trống.");

    RuleForEach(x => x.SubImageUrls)
      .NotEmpty()
      .WithMessage("Đường dẫn ảnh phụ không được để trống.")
      .MaximumLength(1000)
      .WithMessage("Đường dẫn ảnh phụ không được vượt quá 1000 ký tự.");
  }
}

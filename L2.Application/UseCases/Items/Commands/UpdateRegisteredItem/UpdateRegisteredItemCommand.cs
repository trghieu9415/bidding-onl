using FluentValidation;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.UpdateRegisteredItem;

public record UpdateRegisteredItemCommand(
  Guid Id,
  Guid UserId,
  UpdateRegisteredItemRequest Data
) : IRequest<bool>, ITransactional;

public record UpdateRegisteredItemRequest(
  string? Name,
  string? Description,
  decimal? StartingPrice,
  ItemCondition? Condition,
  List<Guid>? CategoryIds,
  string? MainImageUrl,
  List<string>? SubImageUrls
);

public class UpdateRegisteredItemValidator : AbstractValidator<UpdateRegisteredItemCommand> {
  public UpdateRegisteredItemValidator() {
    RuleFor(x => x.Id)
      .NotEmpty().WithMessage("Id không được rỗng.");

    RuleFor(x => x.Data)
      .NotNull().WithMessage("Dữ liệu cập nhật không được null.");

    RuleFor(x => x.Data.Name)
      .MaximumLength(200).WithMessage("Tên không được vượt quá 200 ký tự.");

    RuleFor(x => x.Data.Description)
      .MaximumLength(2000).WithMessage("Mô tả không được vượt quá 2000 ký tự.");

    RuleFor(x => x.Data.StartingPrice)
      .GreaterThan(0).When(x => x.Data.StartingPrice.HasValue)
      .WithMessage("Giá khởi điểm phải lớn hơn 0.");

    RuleFor(x => x.Data.Condition)
      .IsInEnum().When(x => x.Data.Condition.HasValue)
      .WithMessage("Tình trạng sản phẩm không hợp lệ.");

    RuleFor(x => x.Data.CategoryIds)
      .Must(ids => ids == null || ids.Count > 0)
      .WithMessage("Danh mục không được là danh sách rỗng.");

    RuleForEach(x => x.Data.CategoryIds!)
      .NotEmpty().WithMessage("CategoryId không hợp lệ.");

    RuleFor(x => x.Data.MainImageUrl)
      .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.Data.MainImageUrl))
      .WithMessage("MainImageUrl không hợp lệ.");

    RuleForEach(x => x.Data.SubImageUrls!)
      .Must(BeAValidUrl).When(x => x.Data.SubImageUrls != null)
      .WithMessage("Một trong các SubImageUrl không hợp lệ.");
  }

  private bool BeAValidUrl(string? url) {
    return Uri.TryCreate(url, UriKind.Absolute, out _);
  }
}

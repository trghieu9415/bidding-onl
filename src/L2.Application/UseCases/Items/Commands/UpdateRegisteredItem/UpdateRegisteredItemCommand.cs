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
    RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
    RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId không được để trống.");
    RuleFor(x => x.Data).NotNull().ChildRules(data => {
      data.RuleFor(x => x.Name)
        .MaximumLength(200).WithMessage("Tên không quá 200 ký tự.");

      data.RuleFor(x => x.Description)
        .MaximumLength(2000).WithMessage("Mô tả không quá 2000 ký tự.");

      data.RuleFor(x => x.StartingPrice)
        .GreaterThan(0).When(x => x.StartingPrice.HasValue)
        .WithMessage("Giá khởi điểm phải lớn hơn 0.");

      data.RuleFor(x => x.Condition)
        .IsInEnum().When(x => x.Condition.HasValue)
        .WithMessage("Tình trạng không hợp lệ.");

      data.RuleFor(x => x.CategoryIds)
        .Must(ids => ids == null || ids.Count > 0)
        .WithMessage("Danh mục không được rỗng.");

      data.RuleForEach(x => x.CategoryIds)
        .NotEmpty().WithMessage("CategoryId không hợp lệ.");

      data.RuleFor(x => x.MainImageUrl)
        .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.MainImageUrl))
        .WithMessage("MainImageUrl không hợp lệ.");

      data.RuleForEach(x => x.SubImageUrls)
        .Must(BeAValidUrl).When(x => x.SubImageUrls != null)
        .WithMessage("SubImageUrl không hợp lệ.");
    });
  }

  private bool BeAValidUrl(string? url) {
    return Uri.TryCreate(url, UriKind.Absolute, out _);
  }
}

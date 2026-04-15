using FluentValidation;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.RegisterItem;

public record RegisterItemCommand(
  string Name,
  string Description,
  decimal StartingPrice,
  ItemCondition Condition,
  List<Guid> CategoryIds,
  string? MainImageUrl,
  List<string> SubImageUrls
) : IRequest<Guid>, ITransactional;

public class RegisterItemValidator : AbstractValidator<RegisterItemCommand> {
  public RegisterItemValidator() {
    RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    RuleFor(x => x.Description).NotEmpty();
    RuleFor(x => x.StartingPrice).GreaterThan(0).WithMessage("Giá khởi điểm phải lớn hơn 0");
    RuleFor(x => x.CategoryIds).NotEmpty().WithMessage("Phải chọn ít nhất một danh mục");
  }
}

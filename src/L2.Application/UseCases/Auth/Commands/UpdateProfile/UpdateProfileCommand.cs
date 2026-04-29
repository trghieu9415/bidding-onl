using FluentValidation;
using L2.Application.Abstractions;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.UpdateProfile;

public record UpdateProfileCommand(
  Guid UserId,
  UserRole Role,
  UpdateProfileRequest Data
) : IRequest<bool>, ITransactional;

public sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand> {
  public UpdateProfileValidator() {
    RuleFor(x => x.UserId)
      .NotEmpty()
      .WithMessage("Id người dùng không được để trống.");

    RuleFor(x => x.Data)
      .NotNull()
      .WithMessage("Thông tin cập nhật không được để trống.");
  }
}

public record UpdateProfileRequest(string FullName, string? PhoneNumber, string? Url);

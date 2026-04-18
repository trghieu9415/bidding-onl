using L2.Application.Abstractions;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.UpdateProfile;

public record UpdateProfileCommand(
  Guid UserId,
  UserRole Role,
  UpdateProfileRequest Data
) : IRequest<bool>, ITransactional;

public record UpdateProfileRequest(string FullName, string? PhoneNumber, string? Url);

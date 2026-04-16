using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.UpdateProfile;

public record UpdateProfileCommand(string FullName, string? PhoneNumber, string? Url) : IRequest<bool>, ITransactional;

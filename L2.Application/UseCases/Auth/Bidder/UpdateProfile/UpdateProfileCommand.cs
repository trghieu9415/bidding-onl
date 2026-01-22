using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Bidder.UpdateProfile;

public record UpdateProfileCommand(string FullName, string? PhoneNumber, string? Url) : ICommand<Unit>;
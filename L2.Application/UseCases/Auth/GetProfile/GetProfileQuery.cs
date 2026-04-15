using L2.Application.Abstractions;
using L2.Application.Models;

namespace L2.Application.UseCases.Auth.GetProfile;

public record GetProfileQuery : IQuery<GetProfileResult>;

public record GetProfileResult(User Profile);

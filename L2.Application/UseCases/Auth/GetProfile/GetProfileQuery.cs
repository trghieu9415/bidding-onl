using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auth.GetProfile;

public record GetProfileQuery : IRequest<GetProfileResult>;

public record GetProfileResult(User Profile);

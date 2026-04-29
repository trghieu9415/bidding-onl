using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auth.Queries.GetProfile;

public record GetProfileQuery(Guid Id, UserRole Role) : IRequest<GetProfileResult>;

public record GetProfileResult(User User);

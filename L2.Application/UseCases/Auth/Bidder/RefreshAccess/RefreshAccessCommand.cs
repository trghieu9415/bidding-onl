using L2.Application.Abstractions;

namespace L2.Application.UseCases.Auth.Bidder.RefreshAccess;

public record RefreshAccessCommand(string RefreshToken) : ICommand<RefreshAccessResult>;

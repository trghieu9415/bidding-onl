using MediatR;

namespace L2.Application.Abstractions;

public interface ICommand<out TResponse> : IRequest<TResponse>, ITransactional;

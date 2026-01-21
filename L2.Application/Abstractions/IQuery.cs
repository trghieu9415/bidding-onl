using MediatR;

namespace L2.Application.Abstractions;

public interface IQuery<out TResponse> : IRequest<TResponse>;

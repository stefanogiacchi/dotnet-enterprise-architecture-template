using MediatR;

namespace ArcAI.Application.Common.Interfaces;

/// <summary>
/// Marker interface for commands (write operations).
/// Commands represent state-changing operations and return a result.
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the command.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Marker interface for commands without a return value.
/// Used for fire-and-forget operations.
/// </summary>
public interface ICommand : IRequest
{
}

using MediatR;

namespace ArcAI.Application.Common.Interfaces;

/// <summary>
/// Marker interface for queries (read operations).
/// Queries represent read-only operations that return data without changing state.
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the query.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
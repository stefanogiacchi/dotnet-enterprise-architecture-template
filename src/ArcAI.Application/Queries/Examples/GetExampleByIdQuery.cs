using MediatR;

namespace Application.Queries.Examples;

public sealed record GetExampleByIdQuery(Guid Id) : IRequest<ExampleDto>;

public sealed record ExampleDto(Guid Id, string Name);

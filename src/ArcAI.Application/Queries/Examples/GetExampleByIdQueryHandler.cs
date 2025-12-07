using Application.Queries.Examples;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Queries.Examples;

public class GetExampleByIdQueryHandler : IRequestHandler<GetExampleByIdQuery, ExampleDto>
{
    private readonly ILogger<GetExampleByIdQueryHandler> _logger;

    public GetExampleByIdQueryHandler(ILogger<GetExampleByIdQueryHandler> logger)
    {
        _logger = logger;
    }

    public Task<ExampleDto> Handle(GetExampleByIdQuery request, CancellationToken cancellationToken)
    {
        // Dummy data for template purposes
        var dto = new ExampleDto(request.Id, "Sample entity from template");

        _logger.LogInformation("Returning ExampleDto with Id {Id}", dto.Id);

        return Task.FromResult(dto);
    }
}

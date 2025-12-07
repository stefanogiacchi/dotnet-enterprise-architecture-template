using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArcAI.Api.Controllers;

/// <summary>
/// Products catalog endpoints (v1).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/catalog/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetProducts(
        CancellationToken cancellationToken)
    {
        var items = await Task.FromResult(new List<ProductResponse>
        {
            new ProductResponse
            {
                Id = Guid.NewGuid(),
                Sku = "SKU-EXAMPLE",
                Name = "Sample product",
                Description = "Sample description",
                Price = 99.90m,
                IsActive = true
            }
        } as IReadOnlyList<ProductResponse>);

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> GetProductById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var product = await Task.FromResult<ProductResponse?>(null);

        if (product is null)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Product not found",
                Detail = $"No product found with id '{id}'.",
                Instance = HttpContext.Request.Path
            };

            return NotFound(problem);
        }

        return Ok(product);
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var skuAlreadyExists = false; // TODO: verifica reale tramite servizio/domain

        if (skuAlreadyExists)
        {
            var conflict = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Product SKU already exists",
                Detail = $"A product with SKU '{request.Sku}' already exists.",
                Instance = HttpContext.Request.Path
            };

            return Conflict(conflict);
        }

        var created = new ProductResponse
        {
            Id = Guid.NewGuid(),
            Sku = request.Sku,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            IsActive = true
        };

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = created.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0" },
            created);
    }
}

public sealed class ProductResponse
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}

public sealed class CreateProductRequest
{
    public string Sku { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}

internal static class HttpContextApiVersionExtensions
{
    public static ApiVersion? GetRequestedApiVersion(this HttpContext httpContext)
    {
        return httpContext.Features.Get<IApiVersioningFeature>()?.RequestedApiVersion;
    }
}

using Asp.Versioning;
using ArcAI.Api.Contracts;
using ArcAI.Application.Commands.Products.CreateProduct;
using ArcAI.Application.Commands.Products.DeleteProduct;
using ArcAI.Application.Commands.Products.UpdateProduct;
using ArcAI.Application.DTOs;
using ArcAI.Application.Queries.Products.GetProductById;
using ArcAI.Application.Queries.Products.SearchProducts;
using ArcAI.Domain.Enums;
using MediatR;
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
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Search products with pagination and filtering.
    /// </summary>
    /// <param name="request">Search parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResponse<ProductResponse>>> SearchProducts(
        [FromQuery] SearchProductsRequest request,
        CancellationToken cancellationToken)
    {
        // Parse status if provided
        ProductStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<ProductStatus>(request.Status, true, out var parsedStatus))
        {
            status = parsedStatus;
        }

        var query = new SearchProductsQuery
        {
            SearchTerm = request.SearchTerm,
            Status = status,
            CategoryId = request.CategoryId,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        var response = new PaginatedResponse<ProductResponse>
        {
            Items = result.Items.Select(MapToResponse).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            HasPreviousPage = result.HasPreviousPage,
            HasNextPage = result.HasNextPage
        };

        return Ok(response);
    }

    /// <summary>
    /// Get a product by its identifier.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> GetProductById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery { Id = id };
        var product = await _mediator.Send(query, cancellationToken);

        if (product is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Product not found",
                Detail = $"No product found with id '{id}'.",
                Instance = HttpContext.Request.Path
            });
        }

        return Ok(MapToResponse(product));
    }

    /// <summary>
    /// Create a new product.
    /// </summary>
    /// <param name="request">The product creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created product.</returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand
        {
            Sku = request.Sku,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Currency = request.Currency,
            CategoryId = request.CategoryId
        };

        var product = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = product.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0" },
            MapToResponse(product));
    }

    /// <summary>
    /// Update an existing product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="request">The product update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated product.</returns>
    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> UpdateProduct(
        [FromRoute] Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Currency = request.Currency,
            CategoryId = request.CategoryId
        };

        var product = await _mediator.Send(command, cancellationToken);

        return Ok(MapToResponse(product));
    }

    /// <summary>
    /// Delete a product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand { Id = id };
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    private static ProductResponse MapToResponse(ProductDto dto) => new()
    {
        Id = dto.Id,
        Sku = dto.Sku,
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.PriceAmount,
        Currency = dto.PriceCurrency,
        Status = dto.Status.ToString(),
        CategoryId = dto.CategoryId,
        IsAvailable = dto.IsAvailable,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };
}

internal static class HttpContextApiVersionExtensions
{
    public static ApiVersion? GetRequestedApiVersion(this HttpContext httpContext)
    {
        return httpContext.Features.Get<IApiVersioningFeature>()?.RequestedApiVersion;
    }
}
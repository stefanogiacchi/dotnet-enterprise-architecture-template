using ArcAI.Application.Common.Interfaces;
using System.Windows.Input;

namespace ArcAI.Application.Commands.Products.DeleteProduct;

/// <summary>
/// Command for deleting a product.
/// </summary>
public sealed record DeleteProductCommand : ICommand
{
    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public Guid Id { get; init; }
}
namespace ArcAI.Application.DTOs;

public sealed class ProductDto
{
    public Guid Id { get; set; }
    public required string Sku { get; set; }          
    public required string Name { get; set; }          
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public required string Currency { get; set; }      
    public required string Status { get; set; }         
    public Guid? CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public int Version { get; set; }
}
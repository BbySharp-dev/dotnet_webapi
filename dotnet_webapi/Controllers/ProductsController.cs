using Microsoft.AspNetCore.Mvc;

namespace dotnet_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> Products = new()
    {
        new Product(1, "Laptop", "High-performance laptop", 999.99m),
        new Product(2, "Mouse", "Wireless mouse", 29.99m),
        new Product(3, "Keyboard", "Mechanical keyboard", 89.99m)
    };

    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ILogger<ProductsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    /// <returns>List of products</returns>
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetProducts()
    {
        _logger.LogInformation("Getting all products");
        return Ok(Products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);

        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", id);
            return NotFound();
        }

        return Ok(product);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="createProductRequest">Product creation request</param>
    /// <returns>Created product</returns>
    [HttpPost]
    public ActionResult<Product> CreateProduct(CreateProductRequest createProductRequest)
    {
        _logger.LogInformation("Creating new product: {ProductName}", createProductRequest.Name);

        var newId = Products.Max(p => p.Id) + 1;
        var product = new Product(newId, createProductRequest.Name, createProductRequest.Description, createProductRequest.Price);

        Products.Add(product);

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="updateProductRequest">Product update request</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id}")]
    public ActionResult<Product> UpdateProduct(int id, UpdateProductRequest updateProductRequest)
    {
        _logger.LogInformation("Updating product with ID: {ProductId}", id);

        var productIndex = Products.FindIndex(p => p.Id == id);
        if (productIndex == -1)
        {
            _logger.LogWarning("Product with ID {ProductId} not found for update", id);
            return NotFound();
        }

        var updatedProduct = new Product(id, updateProductRequest.Name, updateProductRequest.Description, updateProductRequest.Price);
        Products[productIndex] = updatedProduct;

        return Ok(updatedProduct);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    public ActionResult DeleteProduct(int id)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", id);

        var productIndex = Products.FindIndex(p => p.Id == id);
        if (productIndex == -1)
        {
            _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
            return NotFound();
        }

        Products.RemoveAt(productIndex);
        return NoContent();
    }
}

public record Product(int Id, string Name, string Description, decimal Price);

public record CreateProductRequest(string Name, string Description, decimal Price);

public record UpdateProductRequest(string Name, string Description, decimal Price);

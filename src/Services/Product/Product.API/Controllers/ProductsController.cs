using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Application.Commands;
using Product.Application.DTOs;
using Product.Application.Queries;
using System.Security.Claims;

namespace Product.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = new GetAllProductsQuery();
                var products = await _mediator.Send(query);

                _logger.LogInformation("Retrieved {Count} products", products.Count());
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return StatusCode(500, new { message = "An error occurred while retrieving products" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var query = new GetProductByIdQuery(id);
                var product = await _mediator.Send(query);

                if (product == null)
                {
                    _logger.LogWarning("Product not found: {ProductId}", id);
                    return NotFound(new { message = "Product not found" });
                }

                _logger.LogInformation("Retrieved product: {ProductId}", id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product {ProductId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the product" });
            }
        }

        [HttpPost]
        [Authorize(Policy = "UserOrAdmin")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            try
            {
                var userId = GetUserId();
                var command = new CreateProductCommand(
                    request.Name,
                    request.Description,
                    request.Price,
                    request.Stock,
                    request.Category,
                    userId
                );

                var product = await _mediator.Send(command);

                _logger.LogInformation("Product created: {ProductId} by user {UserId}", product.Id, userId);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, new { message = "An error occurred while creating the product" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UserOrAdmin")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
        {
            try
            {
                var userId = GetUserId();
                var command = new UpdateProductCommand(
                    id,
                    request.Name,
                    request.Description,
                    request.Price,
                    request.Stock,
                    request.Category,
                    userId
                );

                var product = await _mediator.Send(command);

                _logger.LogInformation("Product updated: {ProductId} by user {UserId}", id, userId);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Product not found: {ProductId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the product" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var command = new DeleteProductCommand(id);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    _logger.LogWarning("Product not found for deletion: {ProductId}", id);
                    return NotFound(new { message = "Product not found" });
                }

                _logger.LogInformation("Product deleted: {ProductId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the product" });
            }
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}

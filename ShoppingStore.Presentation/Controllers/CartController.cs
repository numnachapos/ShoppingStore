using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Interfaces;
using Serilog;

namespace ShoppingStore.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(IShoppingCartManager shoppingCartManager, Serilog.ILogger logger) : Controller
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateCart()
        {
            try
            {
                var cart = await shoppingCartManager.CreateCartAsync();
                logger.Information($"Cart created with {cart.Id}");
                return Ok(cart);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to create cart.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartRequest request)
        {
            if (!IsValidRequest(request, out var validationMessage))
            {
                logger.Warning(validationMessage);
                return BadRequest(validationMessage);
            }

            try
            {
                var article = await shoppingCartManager.GetArticleByIdAsync(request.Item.ArticleId);
                if (article == null)
                {
                    logger.Warning($"Article with ID {request.Item.ArticleId} not found.");
                    return NotFound($"Article with ID {request.Item.ArticleId} not found.");
                }

                var cart = await shoppingCartManager.GetCartByIdAsync(request.Item.ShoppingCartId);
                if (cart == null)
                {
                    logger.Warning($"Cart with ID {request.Item.ShoppingCartId} not found.");
                    return NotFound($"Cart with ID {request.Item.ShoppingCartId} not found.");
                }

                // Use the existing article and shopping cart entities
                request.Item.Article = article;
                request.Item.ShoppingCart = cart;

                var response = await shoppingCartManager.AddToCartAsync(request);
                if (response == null)
                {
                    logger.Warning("Failed to add item to cart.");
                    return BadRequest("Failed to add item to cart.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to add item to cart.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartByIdAsync(Guid id)
        {
            try
            {
                var cart = await shoppingCartManager.GetCartByIdAsync(id);
                if (cart == null)
                {
                    return NotFound($"Cart with ID {id} not found.");
                }
                return Ok(cart);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to get cart by ID.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("remove/{articleId}/{cartId}")]
        public async Task<IActionResult> RemoveFromCart(Guid articleId, Guid cartId)
        {
            try
            {
                await shoppingCartManager.RemoveFromCartAsync(articleId, cartId);
                return Ok("Item removed from cart.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to remove item from cart.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItemQuantity([FromBody] CartUpdateRequest request)
        {
            try
            {
                await shoppingCartManager.UpdateCartItemQuantityAsync(request.ArticleId, request.CartId, request.Quantity);
                return Ok("Item quantity updated in cart.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to update item quantity in cart.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("clear/{cartId}")]
        public async Task<IActionResult> ClearCart(Guid cartId)
        {
            try
            {
                await shoppingCartManager.ClearCartAsync(cartId);
                return Ok("Cart cleared.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to clear cart.");
                return StatusCode(500, "Internal server error");
            }
        }

        private static bool IsValidRequest(CartRequest request, out string validationMessage)
        {
            validationMessage = string.Empty;

            if (request == null)
            {
                validationMessage = "Request cannot be null.";
                return false;
            }

            if (request.Item.ArticleId == Guid.Empty)
            {
                validationMessage = "Article ID cannot be empty.";
                return false;
            }

            if (request.Item.Quantity <= 0)
            {
                validationMessage = "Quantity must be greater than zero.";
                return false;
            }

            return true;
        }
    }
}

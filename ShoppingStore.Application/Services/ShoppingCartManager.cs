using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Interfaces;

namespace ShoppingStore.Application.Services
{
    public class ShoppingCartManager(IArticleRepository articleRepository) : IShoppingCartManager
    {
        private readonly List<ShoppingCart> _shoppingCarts = new List<ShoppingCart>();

        public async Task<ShoppingCart> CreateCartAsync()
        {
            var cart = new ShoppingCart();
            _shoppingCarts.Add(cart);
            return await Task.FromResult(cart);
        }

        public async Task<CartResponse> AddToCartAsync(CartRequest request)
        {
            var cart = _shoppingCarts.FirstOrDefault(c => c.Id == request.Item.ShoppingCartId);
            if (cart == null)
            {
                cart = new ShoppingCart { Id = request.Item.ShoppingCartId };
                _shoppingCarts.Add(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ArticleId == request.Item.ArticleId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Item.Quantity;
            }
            else
            {
                cart.Items.Add(request.Item);
            }

            return await Task.FromResult(new CartResponse
            {
                Items = cart.Items.ToArray()
            });
        }

        public async Task<ShoppingCart> GetCartByIdAsync(Guid id)
        {
            var cart = _shoppingCarts.FirstOrDefault(c => c.Id == id);
            return await Task.FromResult(cart) ?? throw new KeyNotFoundException($"Cart with ID {id} not found.");
        }

        public async Task RemoveFromCartAsync(Guid articleId, Guid cartId)
        {
            var cart = _shoppingCarts.FirstOrDefault(c => c.Id == cartId);
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ArticleId == articleId);
                if (item != null)
                {
                    cart.Items.Remove(item);
                }
            }
            await Task.CompletedTask;
        }

        public async Task<CartResponse> UpdateCartItemQuantityAsync(Guid articleId, Guid cartId, int quantity)
        {
            var cart = _shoppingCarts.FirstOrDefault(c => c.Id == cartId);
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ArticleId == articleId);
                if (item != null)
                {
                    item.Quantity = quantity;
                }
            }
            return await Task.FromResult(new CartResponse
            {
                Items = cart?.Items.ToArray() ?? Array.Empty<CartItem>()
            });
        }

        public async Task ClearCartAsync(Guid cartId)
        {
            var cart = _shoppingCarts.FirstOrDefault(c => c.Id == cartId);
            cart?.Items.Clear();
            await Task.CompletedTask;
        }

        public async Task<Article?> GetArticleByIdAsync(Guid articleId)
        {
            return await articleRepository.GetArticleByIdAsync(articleId);
        }
    }
}
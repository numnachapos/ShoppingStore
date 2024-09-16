using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Interfaces;

namespace ShoppingStore.Infrastructure.Data
{
    public class DbShoppingCartManager(IShoppingCartRepository shoppingCartRepository, IArticleRepository articleRepository) : IShoppingCartManager
    {
        public async Task<ShoppingCart> CreateCartAsync()
        {
            var cart = new ShoppingCart();
            await shoppingCartRepository.AddCartAsync(cart);
            return cart;
        }

        public async Task<CartResponse> AddToCartAsync(CartRequest request)
        {
            var existingItem = await shoppingCartRepository.GetCartItemAsync(request.Item.ArticleId, request.Item.ShoppingCartId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Item.Quantity;
                await shoppingCartRepository.UpdateCartItemAsync(existingItem);
            }
            else
            {
                await shoppingCartRepository.AddCartItemAsync(request.Item);
            }

            var items = await shoppingCartRepository.GetItemsByCartIdAsync(request.Item.ShoppingCartId);
            return new CartResponse { Items = items.ToArray() };
        }

        public async Task<ShoppingCart> GetCartByIdAsync(Guid id)
        {
            return await shoppingCartRepository.GetCartByIdAsync(id);
        }

        public async Task RemoveFromCartAsync(Guid articleId, Guid cartId)
        {
            var item = await shoppingCartRepository.GetCartItemAsync(articleId, cartId);
            if (item != null)
            {
                await shoppingCartRepository.RemoveCartItemAsync(item.Id);
            }
        }

        public async Task<CartResponse> UpdateCartItemQuantityAsync(Guid articleId, Guid cartId, int quantity)
        {
            var item = await shoppingCartRepository.GetCartItemAsync(articleId, cartId);
            if (item != null)
            {
                item.Quantity = quantity;
                await shoppingCartRepository.UpdateCartItemAsync(item);
            }

            var items = await shoppingCartRepository.GetItemsByCartIdAsync(cartId);
            return new CartResponse { Items = items.ToArray() };
        }

        public async Task ClearCartAsync(Guid cartId)
        {
            await shoppingCartRepository.ClearCartAsync(cartId);
        }
        public async Task<Article?> GetArticleByIdAsync(Guid articleId)
        {
            return await articleRepository.GetArticleByIdAsync(articleId);
        }
    }
}

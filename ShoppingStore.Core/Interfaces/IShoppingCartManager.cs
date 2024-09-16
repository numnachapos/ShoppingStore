using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Domain.Interfaces
{
    public interface IShoppingCartManager
    {
        Task<ShoppingCart> CreateCartAsync();
        Task<CartResponse> AddToCartAsync(CartRequest request);
        Task<ShoppingCart> GetCartByIdAsync(Guid id);
        Task RemoveFromCartAsync(Guid articleId, Guid cartId);
        Task<CartResponse> UpdateCartItemQuantityAsync(Guid articleId, Guid cartId, int quantity);
        Task ClearCartAsync(Guid cartId);
        Task<Article?> GetArticleByIdAsync(Guid articleId);
    }
}

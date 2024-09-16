using ShoppingStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Domain.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart> GetCartByIdAsync(Guid id);
        Task AddCartItemAsync(CartItem cartItem);
        Task UpdateCartItemAsync(CartItem cartItem);
        Task RemoveCartItemAsync(Guid cartItemId);
        Task<CartItem?> GetCartItemAsync(Guid articleId, Guid cartId);
        Task<IEnumerable<CartItem>> GetItemsByCartIdAsync(Guid cartId);
        Task ClearCartAsync(Guid cartId);
        Task AddCartAsync(ShoppingCart cart);
    }
}

using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Interfaces;
using ShoppingStore.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Infrastructure.Repositories
{
    public class ShoppingCartRepository(ShoppingStoreContext context) : IShoppingCartRepository
    {
        public async Task<ShoppingCart> GetCartByIdAsync(Guid id) =>
            await context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Article)
            .FirstOrDefaultAsync(c => c.Id == id) ?? throw new KeyNotFoundException($"Cart with ID {id} not found.");

        public async Task AddCartItemAsync(CartItem cartItem)
        {
            await context.Items.AddAsync(cartItem);
            await context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            context.Update(cartItem);
            await context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(Guid cartItemId)
        {
            var cartItem = await context.Items.FirstOrDefaultAsync(i => i.Id == cartItemId) ?? throw new KeyNotFoundException($"Cart item with ID {cartItemId} not found.");
            context.Items.Remove(cartItem);
            await context.SaveChangesAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(Guid articleId, Guid cartId)
        {
            return await context.Items.FirstOrDefaultAsync(i => i.ArticleId == articleId && i.ShoppingCartId == cartId);
        }

        public async Task<IEnumerable<CartItem>> GetItemsByCartIdAsync(Guid cartId)
        {
            return await context.Items.Where(i => i.ShoppingCartId == cartId).ToListAsync();
        }

        public async Task ClearCartAsync(Guid cartId)
        {
            var items = await context.Items.Where(i => i.ShoppingCartId == cartId).ToListAsync();
            if (items.Count != 0)
            {
                context.Items.RemoveRange(items);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddCartAsync(ShoppingCart cart)
        {
            await context.Carts.AddAsync(cart);
            await context.SaveChangesAsync();
        }
    }
}

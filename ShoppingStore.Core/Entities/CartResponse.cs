using System.Collections;

namespace ShoppingStore.Domain.Entities
{
    public class CartResponse
    {
        public CartItem[] Items { get; set; } = Array.Empty<CartItem>();
    }
}
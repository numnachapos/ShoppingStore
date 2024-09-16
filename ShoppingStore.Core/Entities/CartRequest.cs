namespace ShoppingStore.Domain.Entities
{
    public class CartRequest
    {
        public CartRequest() { }

        public CartItem Item { get; set; } = new CartItem();
    }
}
namespace ShoppingStore.Domain.Entities
{
    public class CartUpdateRequest
    {
        public Guid ArticleId { get; set; }
        public Guid CartId { get; set; }
        public int Quantity { get; set; }
    }
}
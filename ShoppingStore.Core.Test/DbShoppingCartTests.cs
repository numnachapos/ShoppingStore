using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using ShoppingStore.Application.Services;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Interfaces;
using ShoppingStore.Infrastructure.Data;
using Xunit;

namespace ShoppingStore.Tests
{
    public class DbShoppingCartTests : IDisposable
    {
        private Guid _cartId;
        private DbShoppingCartManager? _manager;
        private ShoppingStoreContext? _context;
        private Mock<IShoppingCartRepository>? _mockCartRepository;
        private Mock<IArticleRepository>? _mockArticleRepository;

        public DbShoppingCartTests()
        {
            SetUp();
        }

        private void SetUp()
        {
            _cartId = Guid.NewGuid();
            var options = new DbContextOptionsBuilder<ShoppingStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ShoppingStoreContext(options);

            // Seed the in-memory database with initial data
            var initialArticle = new Article
            {
                Id = Guid.NewGuid(),
                SKU = "SKU12345",
                Name = "Initial Article",
                Price = 10.0
            };

            var initialItem = new CartItem
            {
                ArticleId = initialArticle.Id,
                Article = initialArticle,
                Quantity = 5,
                ShoppingCartId = _cartId
            };

            var initialCart = new ShoppingCart
            {
                Id = _cartId,
                Items = new List<CartItem> { initialItem }
            };

            _context.Carts.Add(initialCart);
            _context.SaveChanges();

            _mockCartRepository = new Mock<IShoppingCartRepository>();
            _mockArticleRepository = new Mock<IArticleRepository>();

            _manager = new DbShoppingCartManager(_mockCartRepository.Object, _mockArticleRepository.Object);
        }

        public void Dispose()
        {
            _manager = null;
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        private static ShoppingStoreContext CreateNewContext()
        {
            var options = new DbContextOptionsBuilder<ShoppingStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            return new ShoppingStoreContext(options);
        }

        private void InitializeManagerWithContext()
        {
            using var context = CreateNewContext();
            _manager = new DbShoppingCartManager(_mockCartRepository!.Object, _mockArticleRepository!.Object);

            _mockCartRepository.Setup(repo => repo.GetCartByIdAsync(_cartId))
                .ReturnsAsync(context.Carts.Include(c => c.Items).FirstOrDefault(c => c.Id == _cartId)!);
        }

        [Fact]
        public async Task ShouldReturnArticleInCart()
        {
            InitializeManagerWithContext();

            var cart = await _manager!.GetCartByIdAsync(_cartId);

            Assert.NotNull(cart);
            Assert.Single(cart!.Items);
            var cartItem = cart.Items.First();
            Assert.Equal(cart.Items.First().ArticleId, cartItem.ArticleId);
            Assert.Equal(cart.Items.First().Quantity, cartItem.Quantity);
            Assert.Equal(cart.Items.First().ShoppingCartId, cartItem.ShoppingCartId);
            Assert.Equal(cart.Items.First().Article.SKU, cartItem.Article.SKU);
        }

        [Fact]
        public async Task ShouldRemoveArticleFromCart()
        {
            InitializeManagerWithContext();

            _mockCartRepository!.Setup(repo => repo.GetCartItemAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Guid articleId, Guid cartId) =>
                    _context!.Items.FirstOrDefault(i => i.ArticleId == articleId && i.ShoppingCartId == cartId)!);

            _mockCartRepository.Setup(repo => repo.RemoveCartItemAsync(It.IsAny<Guid>()))
                .Callback<Guid>(async (cartItemId) =>
                {
                    var cartItem = await _context!.Items.FirstOrDefaultAsync(i => i.Id == cartItemId);
                    if (cartItem != null)
                    {
                        _context.Items.Remove(cartItem);
                        await _context.SaveChangesAsync();
                    }
                });

            var cart = await _manager!.GetCartByIdAsync(_cartId);
            var cartItem = cart!.Items.First();

            await _manager.RemoveFromCartAsync(cartItem.ArticleId, _cartId);

            _context!.Entry(cart).State = EntityState.Detached;
            cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == _cartId);

            Assert.NotNull(cart);
            Assert.Empty(cart!.Items);
        }

        [Fact]
        public async Task ShouldUpdateArticleQuantityInCart()
        {
            InitializeManagerWithContext();

            _mockCartRepository!.Setup(repo => repo.GetCartItemAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Guid articleId, Guid cartId) =>
                    _context!.Items.FirstOrDefault(i => i.ArticleId == articleId && i.ShoppingCartId == cartId)!);

            _mockCartRepository.Setup(repo => repo.UpdateCartItemAsync(It.IsAny<CartItem>()))
                .Callback<CartItem>(async (cartItem) =>
                {
                    var existingItem = await _context!.Items.FirstOrDefaultAsync(i => i.Id == cartItem.Id);
                    if (existingItem != null)
                    {
                        existingItem.Quantity = cartItem.Quantity;
                        _context.Items.Update(existingItem);
                        await _context.SaveChangesAsync();
                    }
                });

            var cart = await _manager!.GetCartByIdAsync(_cartId);
            var cartItem = cart!.Items.First();

            await _manager.UpdateCartItemQuantityAsync(cartItem.ArticleId, _cartId, 10);

            _context!.Entry(cart).State = EntityState.Detached;
            cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == _cartId);
            cartItem = cart?.Items.FirstOrDefault();

            Assert.NotNull(cartItem);
            Assert.Equal(10, cartItem!.Quantity);
        }

        [Fact]
        public async Task ShouldClearCart()
        {
            InitializeManagerWithContext();

            _mockCartRepository!.Setup(repo => repo.ClearCartAsync(It.IsAny<Guid>()))
                .Callback<Guid>(async (cartId) =>
                {
                    var items = await _context!.Items.Where(i => i.ShoppingCartId == cartId).ToListAsync();
                    if (items.Count != 0)
                    {
                        _context.Items.RemoveRange(items);
                        await _context.SaveChangesAsync();
                    }
                });

            var cart = await _context!.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == _cartId);
            Assert.NotNull(cart);

            await _manager!.ClearCartAsync(_cartId);

            cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == _cartId);
            Assert.NotNull(cart);
            Assert.Empty(cart!.Items);
        }

        [Fact]
        public async Task ShouldReturnEmptyCart()
        {
            InitializeManagerWithContext();

            _mockCartRepository!.Setup(repo => repo.GetCartItemAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Guid articleId, Guid cartId) =>
                    _context!.Items.FirstOrDefault(i => i.ArticleId == articleId && i.ShoppingCartId == cartId)!);

            _mockCartRepository.Setup(repo => repo.RemoveCartItemAsync(It.IsAny<Guid>()))
                .Callback<Guid>(async (cartItemId) =>
                {
                    var cartItem = await _context!.Items.FirstOrDefaultAsync(i => i.Id == cartItemId);
                    if (cartItem != null)
                    {
                        _context.Items.Remove(cartItem);
                        await _context.SaveChangesAsync();
                    }
                });

            var cart = await _manager!.GetCartByIdAsync(_cartId);
            var cartItem = cart!.Items.First();

            await _manager.RemoveFromCartAsync(cartItem.ArticleId, _cartId);

            _context!.Entry(cart).State = EntityState.Detached;
            cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == _cartId);

            Assert.NotNull(cart);
            Assert.Empty(cart!.Items);
        }

        [Fact]
        public async Task ShouldHandleNonExistentCart()
        {
            var nonExistentCartId = Guid.NewGuid();

            using var context = CreateNewContext();
            _mockCartRepository!.Setup(repo => repo.GetCartByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new KeyNotFoundException($"Cart with ID {nonExistentCartId} not found."));

            _manager = new DbShoppingCartManager(_mockCartRepository.Object, _mockArticleRepository!.Object);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _manager.GetCartByIdAsync(nonExistentCartId));
            Assert.Equal($"Cart with ID {nonExistentCartId} not found.", ex.Message);
        }
    }
}

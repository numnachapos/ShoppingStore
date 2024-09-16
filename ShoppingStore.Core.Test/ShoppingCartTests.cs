using Moq;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Services;
using ShoppingStore.Domain.Interfaces;
using Xunit;

namespace ShoppingStore.Tests
{
    public class ShoppingCartTests : IDisposable
    {
        private readonly Guid _cartId;
        private ShoppingCartManager? _manager;
        private readonly Mock<IArticleRepository>? _mockArticleRepository;

        public ShoppingCartTests()
        {
            _cartId = Guid.NewGuid();
            _mockArticleRepository = new Mock<IArticleRepository>();
            _manager = new ShoppingCartManager(_mockArticleRepository.Object);
        }

        public void Dispose()
        {
            _manager = null;
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task ShouldReturnArticleAddedToCart()
        {
            var article = new Article
            {
                Id = Guid.NewGuid(),
                SKU = "SKU12345",
                Name = "Test Article",
                Price = 10.0
            };

            var item = new CartItem
            {
                ArticleId = article.Id,
                Article = article,
                Quantity = 5,
                ShoppingCartId = _cartId
            };

            var request = new CartRequest
            {
                Item = item
            };

            CartResponse response = await _manager!.AddToCartAsync(request);

            Assert.NotNull(response);
            Assert.Contains(item, response.Items);
            Assert.Equal("SKU12345", response.Items.First().Article.SKU);
        }

        [Fact]
        public async Task ShouldVerifyAddToCartCalledOnce()
        {
            var article = new Article
            {
                Id = Guid.NewGuid(),
                SKU = "SKU12345",
                Name = "Test Article",
                Price = 10.0
            };

            var item = new CartItem
            {
                ArticleId = article.Id,
                Article = article,
                Quantity = 5,
                ShoppingCartId = _cartId
            };

            var request = new CartRequest
            {
                Item = item
            };

            var mockManager = new Mock<IShoppingCartManager>();

            mockManager.Setup(manager => manager.AddToCartAsync(It.IsAny<CartRequest>()))
                       .ReturnsAsync((CartRequest request) => new CartResponse
                       {
                           Items = new[] { request.Item }
                       });

            CartResponse response = await mockManager.Object.AddToCartAsync(request);

            // Verify that AddToCartAsync was called exactly once
            mockManager.Verify(manager => manager.AddToCartAsync(It.IsAny<CartRequest>()), Times.Once);
        }

        [Fact]
        public async Task ShouldVerifyAddToCartCalledMultipleTimes()
        {
            var article = new Article
            {
                Id = Guid.NewGuid(),
                SKU = "SKU12345",
                Name = "Test Article",
                Price = 10.0
            };

            var item1 = new CartItem
            {
                ArticleId = article.Id,
                Article = article,
                Quantity = 5,
                ShoppingCartId = _cartId
            };

            var request1 = new CartRequest
            {
                Item = item1
            };

            var item2 = new CartItem
            {
                ArticleId = article.Id,
                Article = article,
                Quantity = 10,
                ShoppingCartId = _cartId
            };

            var request2 = new CartRequest
            {
                Item = item2
            };

            var mockManager = new Mock<IShoppingCartManager>();

            mockManager.Setup(manager => manager.AddToCartAsync(It.IsAny<CartRequest>()))
                       .ReturnsAsync((CartRequest request) => new CartResponse
                       {
                           Items = new[] { request.Item }
                       });

            // Call AddToCartAsync multiple times
            CartResponse response1 = await mockManager.Object.AddToCartAsync(request1);
            CartResponse response2 = await mockManager.Object.AddToCartAsync(request2);

            // Verify that AddToCartAsync was called exactly twice
            mockManager.Verify(manager => manager.AddToCartAsync(It.IsAny<CartRequest>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ShouldThrowExceptionWhenAddToCartFails()
        {
            var article = new Article
            {
                Id = Guid.NewGuid(),
                SKU = "SKU12345",
                Name = "Test Article",
                Price = 10.0
            };

            var item = new CartItem
            {
                ArticleId = article.Id,
                Article = article,
                Quantity = 5,
                ShoppingCartId = _cartId
            };

            var request = new CartRequest
            {
                Item = item
            };

            var mockManager = new Mock<IShoppingCartManager>();

            // Set up the mock to throw an exception when AddToCartAsync is called
            mockManager.Setup(manager => manager.AddToCartAsync(It.IsAny<CartRequest>()))
                       .ThrowsAsync(new InvalidOperationException("AddToCart failed"));

            // Act and Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await mockManager.Object.AddToCartAsync(request));
            Assert.Equal("AddToCart failed", ex.Message);

            // Verify that AddToCartAsync was called exactly once
            mockManager.Verify(manager => manager.AddToCartAsync(It.IsAny<CartRequest>()), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnArticlesAddedToCart()
        {
            var article1 = new Article
            {
                Id = Guid.NewGuid(),
                SKU = "SKU12345",
                Name = "Test Article 1",
                Price = 10.0
            };

            var item1 = new CartItem
            {
                ArticleId = article1.Id,
                Article = article1,
                Quantity = 5,
                ShoppingCartId = _cartId
            };

            var request = new CartRequest
            {
                Item = item1
            };

            await _manager!.AddToCartAsync(request);

            var article2 = new Article
            {
                Id = Guid.NewGuid(),
                SKU = "SKU67890",
                Name = "Test Article 2",
                Price = 20.0
            };

            var item2 = new CartItem
            {
                ArticleId = article2.Id,
                Article = article2,
                Quantity = 10,
                ShoppingCartId = _cartId
            };

            request = new CartRequest
            {
                Item = item2
            };

            var response = await _manager.AddToCartAsync(request);

            Assert.NotNull(response);
            Assert.Contains(item1, response.Items);
            Assert.Contains(item2, response.Items);
        }

        [Fact]
        public async Task ShouldReturnCombinedArticlesAddedToCart()
        {
            var article = new Article
            {
                Id = Guid.NewGuid(),
                SKU = "SKU12345",
                Name = "Test Article",
                Price = 10.0
            };

            var item1 = new CartItem
            {
                ArticleId = article.Id,
                Article = article,
                Quantity = 5,
                ShoppingCartId = _cartId
            };

            var request = new CartRequest
            {
                Item = item1
            };

            CartResponse response = await _manager!.AddToCartAsync(request);

            var item2 = new CartItem
            {
                ArticleId = article.Id,
                Article = article,
                Quantity = 10,
                ShoppingCartId = _cartId
            };

            request = new CartRequest
            {
                Item = item2
            };

            response = await _manager.AddToCartAsync(request);

            Assert.NotNull(response);
            Assert.Contains(response.Items, item => item.ArticleId == item1.ArticleId && item.Quantity == 15);
        }
    }
}

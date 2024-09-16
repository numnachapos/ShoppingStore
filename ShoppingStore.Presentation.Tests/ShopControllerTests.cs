using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Core;
using ShoppingStore.Presentation.Controllers;
using System.Diagnostics.CodeAnalysis;

namespace ShoppingStore.Presentation.Tests
{
    public class ShopControllerTests : IAsyncDisposable
    {
        private ShopController _controller;

        public ShopControllerTests()
        {
            _controller = new ShopController();
        }

        private async ValueTask DisposeAsyncCore()
        {
            if (_controller != null)
            {
                await _controller.DisposeAsync();
            }
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await DisposeAsyncCore();
            GC.SuppressFinalize(this);
        }

        [SetUp]
        public void Setup()
        {
            _controller = new ShopController();
        }

        [Test]
        public void Index_ReturnsAllArticles_WhenQueryIsNullOrEmpty()
        {
            // Act
            var result = _controller.Index(string.Empty) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var articles = result.Value as IEnumerable<Article>;
            Assert.That(articles, Is.Not.Null);
            Assert.That(articles.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Index_FiltersArticles_WhenQueryIsProvided()
        {
            // Act
            var result = _controller.Index("T-Shirt") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var articles = result.Value as List<Article>;
            Assert.That(articles, Has.Count.EqualTo(2));
        }

        [Test]
        public void Index_FiltersArticlesCaseInsensitive_WhenQueryIsProvided()
        {
            // Act
            var result = _controller.Index("t-shirt") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var articles = result.Value as List<Article>;
            Assert.That(articles, Has.Count.EqualTo(2));
        }

        [Test]
        public void Index_ReturnsEmptyList_WhenNoArticlesMatchQuery()
        {
            // Act
            var result = _controller.Index("Pants") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var articles = result.Value as List<Article>;
            Assert.That(articles, Has.Count.EqualTo(0));
        }

        [Test]
        public void Index_ReturnsAllArticles_WhenQueryIsWhitespace()
        {
            // Act
            var result = _controller.Index(" ") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var articles = result.Value as List<Article>;
            Assert.That(articles, Has.Count.EqualTo(3));
        }

        [Test]
        public void Index_ReturnsBlousonArticle_WhenQueryIsBlouson()
        {
            // Act
            var result = _controller.Index("Blouson") as OkObjectResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                if (result != null)
                {
                    var articles = result.Value as List<Article>;
                    Assert.That(articles, Is.Not.Null);
                    if (articles != null)
                    {
                        Assert.That(articles, Has.Count.EqualTo(1));
                        Assert.That(articles[0], Is.Not.Null);
                        Assert.That(articles[0].Name, Is.EqualTo("Green Blouson"));
                    }
                }
            });
        }

        [Test]
        public void GetAll_ShouldReturnAllArticles()
        {
            // Act
            var result = _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var articles = result.Value as List<Article>;
            Assert.That(articles, Is.Not.Null);
            Assert.That(articles, Has.Count.EqualTo(3));
        }

        [Test]
        public void GetSingle_ShouldReturnArticle_WhenIdIsValid()
        {
            // Arrange
            var articleId = _controller.Articles.First().Id.ToString();

            // Act
            var result = _controller.GetSingle(articleId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var article = result.Value as Article;
            Assert.That(article, Is.Not.Null);
            Assert.That(article.Id.ToString(), Is.EqualTo(articleId));
        }

        [Test]
        public void GetSingle_ShouldReturnNotFound_WhenIdIsInvalid()
        {
            // Act
            var result = _controller.GetSingle(Guid.NewGuid().ToString());

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public void GetSingle_ShouldReturnBadRequest_WhenIdIsMalformed()
        {
            // Act
            var result = _controller.GetSingle("invalid-guid");

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoppingStore.Application.Services;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Interfaces;
using ShoppingStore.Presentation.Controllers;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ShoppingStore.Tests
{
    public class ArticleControllerTests
    {
        private readonly ArticleController _controller;
        private readonly Mock<IArticleService> _articleServiceMock;
        private readonly Mock<Serilog.ILogger> _loggerMock;

        public ArticleControllerTests()
        {
            _articleServiceMock = new Mock<IArticleService>();
            _loggerMock = new Mock<Serilog.ILogger>();
            _controller = new ArticleController(_articleServiceMock.Object, _loggerMock.Object);

            // Setup mock data
            var articles = new List<Article>
                {
                    new Article { Id = Guid.NewGuid(), Name = "Red T-Shirt", Price = 9.99 },
                    new Article { Id = Guid.NewGuid(), Name = "Blue T-Shirt", Price = 11.99 },
                    new Article { Id = Guid.NewGuid(), Name = "Green Blouson", Price = 99.99 }
                };

            _articleServiceMock.Setup(service => service.GetAllArticles()).ReturnsAsync(articles);
            _articleServiceMock.Setup(service => service.GetArticleById(It.IsAny<Guid>())).ReturnsAsync((Guid id) => articles.FirstOrDefault(a => a.Id == id) ?? throw new KeyNotFoundException($"Article with ID {id} not found."));
        }

        [Fact]
        public async Task GetAllArticles_ShouldReturnAllArticles()
        {
            // Act
            var result = await _controller.GetAllArticles() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var articles = result.Value as List<Article>;
            Assert.NotNull(articles);
            Assert.Equal(3, articles.Count);
        }

        [Fact]
        public async Task GetArticleById_ShouldReturnArticle_WhenIdIsValid()
        {
            // Arrange
            var articleId = (await _articleServiceMock.Object.GetAllArticles()).First().Id;

            // Act
            var result = await _controller.GetArticleById(articleId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var article = result.Value as Article;
            Assert.NotNull(article);
            Assert.Equal(articleId, article.Id);
        }

        [Fact]
        public async Task GetArticleById_ShouldReturnNotFound_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            _articleServiceMock.Setup(service => service.GetArticleById(invalidId))
                               .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetArticleById(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateArticle_ShouldReturnCreatedArticle()
        {
            // Arrange
            var newArticle = new Article { Id = Guid.NewGuid(), Name = "Yellow Hat", Price = 19.99 };
            _articleServiceMock.Setup(service => service.CreateArticle(newArticle)).ReturnsAsync(newArticle);

            // Act
            var result = await _controller.CreateArticle(newArticle) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            var createdArticle = result.Value as Article;
            Assert.NotNull(createdArticle);
            Assert.Equal(newArticle.Id, createdArticle.Id);
        }

        [Fact]
        public async Task UpdateArticle_ShouldReturnNoContent_WhenArticleIsUpdated()
        {
            // Arrange
            var existingArticle = (await _articleServiceMock.Object.GetAllArticles()).First();
            _articleServiceMock.Setup(service => service.UpdateArticle(existingArticle)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateArticle(existingArticle.Id, existingArticle);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateArticle_ShouldReturnNotFound_WhenArticleDoesNotExist()
        {
            // Arrange
            var nonExistentArticle = new Article { Id = Guid.NewGuid(), Name = "Non-Existent", Price = 0 };
            _articleServiceMock.Setup(service => service.UpdateArticle(nonExistentArticle)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.UpdateArticle(nonExistentArticle.Id, nonExistentArticle);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteArticle_ShouldReturnNoContent_WhenArticleIsDeleted()
        {
            // Arrange
            var articleId = (await _articleServiceMock.Object.GetAllArticles()).First().Id;
            _articleServiceMock.Setup(service => service.DeleteArticle(articleId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteArticle(articleId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteArticle_ShouldReturnNotFound_WhenArticleDoesNotExist()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            _articleServiceMock.Setup(service => service.DeleteArticle(invalidId)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteArticle(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
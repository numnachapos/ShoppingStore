using Xunit;
using Moq;
using ShoppingStore.Domain.Interfaces;
using ShoppingStore.Application.Services;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Tests
{
    public class ArticleServiceTests
    {
        private readonly Mock<IArticleRepository> _articleRepositoryMock;
        private readonly ArticleService _articleService;

        public ArticleServiceTests()
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _articleService = new ArticleService(_articleRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllArticles_ShouldReturnAllArticles()
        {
            // Arrange
            var articles = new List<Article>
                {
                    new Article { Id = Guid.NewGuid(), Name = "Red T-Shirt", Price = 9.99 },
                    new Article { Id = Guid.NewGuid(), Name = "Blue T-Shirt", Price = 11.99 },
                    new Article { Id = Guid.NewGuid(), Name = "Green Blouson", Price = 99.99 }
                };

            _articleRepositoryMock.Setup(repo => repo.GetAllArticlesAsync()).ReturnsAsync(articles);

            // Act
            var result = await _articleService.GetAllArticles();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetArticleById_ShouldReturnArticle_WhenIdIsValid()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            var article = new Article { Id = articleId, Name = "Red T-Shirt", Price = 9.99 };

            _articleRepositoryMock.Setup(repo => repo.GetArticleByIdAsync(articleId)).ReturnsAsync(article);

            // Act
            var result = await _articleService.GetArticleById(articleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(articleId, result.Id);
        }

        [Fact]
        public async Task GetArticleById_ShouldThrowException_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            _articleRepositoryMock.Setup(repo => repo.GetArticleByIdAsync(invalidId)).ThrowsAsync(new KeyNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _articleService.GetArticleById(invalidId));
        }

        [Fact]
        public async Task CreateArticle_ShouldReturnCreatedArticle()
        {
            // Arrange
            var newArticle = new Article { Id = Guid.NewGuid(), Name = "Yellow Hat", Price = 19.99 };
            _articleRepositoryMock.Setup(repo => repo.CreateArticleAsync(newArticle)).ReturnsAsync(newArticle);

            // Act
            var result = await _articleService.CreateArticle(newArticle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newArticle.Id, result.Id);
        }

        [Fact]
        public async Task UpdateArticle_ShouldCompleteSuccessfully()
        {
            // Arrange
            var existingArticle = new Article { Id = Guid.NewGuid(), Name = "Red T-Shirt", Price = 9.99 };
            _articleRepositoryMock.Setup(repo => repo.UpdateArticleAsync(existingArticle)).Returns(Task.CompletedTask);

            // Act
            await _articleService.UpdateArticle(existingArticle);

            // Assert
            _articleRepositoryMock.Verify(repo => repo.UpdateArticleAsync(existingArticle), Times.Once);
        }

        [Fact]
        public async Task DeleteArticle_ShouldCompleteSuccessfully()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            _articleRepositoryMock.Setup(repo => repo.DeleteArticleAsync(articleId)).Returns(Task.CompletedTask);

            // Act
            await _articleService.DeleteArticle(articleId);

            // Assert
            _articleRepositoryMock.Verify(repo => repo.DeleteArticleAsync(articleId), Times.Once);
        }
    }
}

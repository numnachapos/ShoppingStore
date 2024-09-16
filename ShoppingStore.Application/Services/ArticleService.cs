using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Interfaces;

namespace ShoppingStore.Application.Services
{
    public class ArticleService(IArticleRepository articleRepository) : IArticleService
    {
        public async Task<IEnumerable<Article>> GetAllArticles()
        {
            return await articleRepository.GetAllArticlesAsync();
        }

        public async Task<Article> GetArticleById(Guid id)
        {
            var article = await articleRepository.GetArticleByIdAsync(id) ?? throw new KeyNotFoundException($"Article with ID {id} not found.");
            return article;
        }

        public async Task<Article> CreateArticle(Article article)
        {
            return await articleRepository.CreateArticleAsync(article);
        }

        public async Task UpdateArticle(Article article)
        {
            await articleRepository.UpdateArticleAsync(article);
        }

        public async Task DeleteArticle(Guid id)
        {
            await articleRepository.DeleteArticleAsync(id);
        }
    }
}

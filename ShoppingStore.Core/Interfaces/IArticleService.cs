using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Domain.Interfaces
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetAllArticles();
        Task<Article> GetArticleById(Guid id);
        Task<Article> CreateArticle(Article article);
        Task UpdateArticle(Article article);
        Task DeleteArticle(Guid id);
    }
}

using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Interfaces;
using ShoppingStore.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Infrastructure.Repositories
{
    public class ArticleRepository(ShoppingStoreContext context) : IArticleRepository
    {
        public async Task<IEnumerable<Article>> GetAllArticlesAsync() => await context.Articles.ToListAsync();

        public async Task<Article> GetArticleByIdAsync(Guid id) => await context.Articles.FindAsync(id) ?? throw new KeyNotFoundException($"Article with ID {id} not found.");

        public async Task<Article> CreateArticleAsync(Article article)
        {
            context.Articles.Add(article);
            await context.SaveChangesAsync();
            return article;
        }

        public async Task UpdateArticleAsync(Article article)
        {
            context.Articles.Update(article);
            await context.SaveChangesAsync();
        }

        public async Task DeleteArticleAsync(Guid id)
        {
            var article = await context.Articles.FindAsync(id) ?? throw new KeyNotFoundException($"Article with ID {id} not found.");
            context.Articles.Remove(article);
            await context.SaveChangesAsync();
        }
    }
}

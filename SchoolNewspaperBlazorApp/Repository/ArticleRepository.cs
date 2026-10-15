using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        public ArticleRepository(IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddArticleAsync(Article article)
        {
            await _contextFactory.CreateDbContext().Articles.AddAsync(article);
            await _contextFactory.CreateDbContext().SaveChangesAsync();
        }
        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await _contextFactory.CreateDbContext().Articles.ToListAsync();
           
        }
        public async Task<Article> GetArticleByIdAsync(int id)
        {
            var article = await _contextFactory.CreateDbContext().Articles.FirstOrDefaultAsync(a => a.Id == id);
            if (article == null)
            {
                throw new Exception($"Article with ID {id} not found.");
            }
            return article;
        }
        public async Task RemoveArticleByIdAsync(int id)
        {
            var article = await _contextFactory.CreateDbContext().Articles.FirstOrDefaultAsync(a => a.Id == id);
            if (article == null) {
                throw new Exception($"Article with ID {id} not found.");
            }
            else
            {
                _contextFactory.CreateDbContext().Articles.Remove(article);
                await _contextFactory.CreateDbContext().SaveChangesAsync();
            }
                

        }
    }
}

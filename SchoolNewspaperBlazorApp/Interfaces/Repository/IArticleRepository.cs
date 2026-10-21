using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface IArticleRepository
    {
        Task<bool> AddArticleAsync(Article article);
        Task<List<Article>> GetAllArticlesAsync();
        Task<Article> GetArticleByIdAsync(int id);
        Task RemoveArticleByIdAsync(int id);
        Task UpdateArticleByIdAsync(Article article);
    }
}

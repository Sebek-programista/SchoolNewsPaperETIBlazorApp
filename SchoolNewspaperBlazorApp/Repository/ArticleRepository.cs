
using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;

        public ArticleRepository(
            IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<int> AddArticleAsync(Article article)
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync();

            if (string.IsNullOrWhiteSpace(article.Title) ||
                string.IsNullOrWhiteSpace(article.Text) ||
                string.IsNullOrWhiteSpace(article.Author))
            {
                return 0;
            }

            try
            {
                await context.Articles.AddAsync(article);
                await context.SaveChangesAsync();

                return article.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateArticleByIdAsync(Article article)
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync();

            if (article.Id <= 0 ||
                string.IsNullOrWhiteSpace(article.Title) ||
                string.IsNullOrWhiteSpace(article.Text) ||
                string.IsNullOrWhiteSpace(article.Author))
            {
                return 0;
            }

            try
            {
                var existingArticle = await context.Articles
                    .FirstOrDefaultAsync(a => a.Id == article.Id);

                if (existingArticle == null)
                {
                    return 0;
                }

                // Podstawowe dane artykułu
                existingArticle.Title = article.Title;
                existingArticle.Text = article.Text;
                existingArticle.Author = article.Author;
                existingArticle.Category = article.Category;

                // Zachowujemy poprzednie zdjęcie, jeśli nie wybrano nowego
                if (article.MainImageId > 0)
                {
                    existingArticle.MainImageId = article.MainImageId;
                }

                // Ogłoszenia
                existingArticle.AnnouncementDeadline =
                    article.AnnouncementDeadline;

                // Zastępstwa
                existingArticle.SubstituteDate =
                    article.SubstituteDate;
                existingArticle.SchoolClass =
                    article.SchoolClass;
                existingArticle.Teacher =
                    article.Teacher;

                // Wydarzenia
                existingArticle.EventDate =
                    article.EventDate;
                existingArticle.EventTime =
                    article.EventTime;
                existingArticle.EventLocation =
                    article.EventLocation;

                // Zawody
                existingArticle.CompetitionDate =
                    article.CompetitionDate;
                existingArticle.CompetitionDiscipline =
                    article.CompetitionDiscipline;
                existingArticle.CompetitionLocation =
                    article.CompetitionLocation;

                // Rekrutacja
                existingArticle.AdmissionsStart =
                    article.AdmissionsStart;
                existingArticle.AdmissionsEnd =
                    article.AdmissionsEnd;

                // Egzaminy
                existingArticle.ExamDate =
                    article.ExamDate;
                existingArticle.ExamSubject =
                    article.ExamSubject;
                existingArticle.ExamClass =
                    article.ExamClass;

                // Samorząd uczniowski
                existingArticle.CouncilInitiative =
                    article.CouncilInitiative;

                await context.SaveChangesAsync();

                return existingArticle.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync();

            return await context.Articles
                .AsNoTracking()
                .OrderByDescending(a => a.PublishDate)
                .ToListAsync();
        }

        public async Task<Article> GetArticleByIdAsync(int id)
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync();

            var article = await context.Articles
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                throw new Exception(
                    $"Nie znaleziono artykułu o ID {id}.");
            }

            return article;
        }

        public async Task<bool> RemoveArticleByIdAsync(int id)
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync();

            try
            {
                var article = await context.Articles
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (article == null)
                {
                    return false;
                }

                context.Articles.Remove(article);
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

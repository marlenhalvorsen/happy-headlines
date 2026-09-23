using ArticleService.Models;

namespace ArticleService.Repositories;

public interface IArticleRepository
{
    Task<Article> CreateArticle(string region, Article article);
    Task<Article?> FetchArticle(string region, int id);
    Task<Article?> UpdateArticle(string region, int id, Article article);
    Task<bool> DeleteArticle(string region, int id);
}

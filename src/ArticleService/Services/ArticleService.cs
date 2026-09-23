using ArticleService.Models;
using ArticleService.Repositories;

namespace ArticleService.Services;

public class ArticleService
{
    private readonly IArticleRepository _repository;

    public ArticleService(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Article> CreateArticle(string region, Article article)
    {
        return await _repository.CreateArticle(region, article);
    }

    public async Task<Article?> FetchArticle(string region, int id)
    {
        return await _repository.FetchArticle(region, id);
    }

    public async Task<Article?> UpdateArticle(string region, int id, Article article)
    {
        return await _repository.UpdateArticle(region, id, article);
    }

    public async Task<bool> DeleteArticle(string region, int id)
    {
        return await _repository.DeleteArticle(region, id);
    }
}

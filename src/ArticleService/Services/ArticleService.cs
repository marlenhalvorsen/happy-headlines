using ArticleService.Data;
using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Services;

public class ArticleService
{
    private readonly ArticlePartitioner _partitioner;

    public ArticleService(ArticlePartitioner partitioner)
    {
        _partitioner = partitioner;
    }

    public async Task<Article> CreateArticle(string region, Article article)
    {
        var connectionString = _partitioner.GetConnectionString(region);

        await using var db = new ArticleDbContext(connectionString);

        db.Articles.Add(article);
        await db.SaveChangesAsync();

        return article;
    }
    
    public async Task<Article?> FetchArticle(string region, int id)
    {
        var connectionString = _partitioner.GetConnectionString(region);
        await using var db = new ArticleDbContext(connectionString);

        return await db.Articles.FindAsync(id);
    }

    public async Task<Article?> UpdateArticle(string region, int id, Article article)
    {
        var connectionString = _partitioner.GetConnectionString(region);
        await using var db = new ArticleDbContext(connectionString);

        var existingArticle = await db.Articles.FindAsync(id);

        if (existingArticle == null)
            return null;

        existingArticle.Title = article.Title;
        existingArticle.Content = article.Content;

        await db.SaveChangesAsync();

        return existingArticle;
    }

    public async Task<bool> DeleteArticle(string region, int id)
    {
        var connectionString = _partitioner.GetConnectionString(region);
        await using var db = new ArticleDbContext(connectionString);

        var article = await db.Articles.FindAsync(id);

        if (article == null)
            return false;

        db.Articles.Remove(article);
        await db.SaveChangesAsync();

        return true;
    }
}
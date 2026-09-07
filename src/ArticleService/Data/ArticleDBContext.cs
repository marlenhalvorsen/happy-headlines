using Microsoft.EntityFrameworkCore;
using ArticleService.Models;

namespace ArticleService.Data;

public class ArticleDbContext : DbContext
{
    public ArticleDbContext(string connectionString)
        : base(new DbContextOptionsBuilder<ArticleDbContext>()
            .UseNpgsql(connectionString)
            .Options)
    {
    }

    public DbSet<Article> Articles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>().ToTable("articles");

        modelBuilder.Entity<Article>().Property(a => a.Id).HasColumnName("id");
        modelBuilder.Entity<Article>().Property(a => a.Title).HasColumnName("title");
        modelBuilder.Entity<Article>().Property(a => a.Content).HasColumnName("content");
    }
}

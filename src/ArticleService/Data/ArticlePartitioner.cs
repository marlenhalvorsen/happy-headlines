using Microsoft.EntityFrameworkCore;

namespace ArticleService.Services;

public class ArticlePartitioner
{
    private readonly IConfiguration _configuration;

    public ArticlePartitioner(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetConnectionString(string region)
    {
        var connectionString = _configuration.GetConnectionString(region);

        if (connectionString == null)
        {
            throw new ArgumentException($"Unknown region: {region}");
        }

        return connectionString;
    }
}

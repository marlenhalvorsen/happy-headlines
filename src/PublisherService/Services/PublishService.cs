using PublisherService.DTOs;
using PublisherService.Messaging;

namespace PublisherService.Services;

public class PublishService : IPublishService
{
    private readonly IArticlePublisher _articlePublisher;

    public PublishService(IArticlePublisher articlePublisher)
    {
        _articlePublisher = articlePublisher;
    }

    public async Task PublishArticleAsync(PublishArticleDto dto)
    {
        await _articlePublisher.PublishAsync(dto);
    }
}

using PublisherService.DTOs;

namespace PublisherService.Messaging;

public interface IArticlePublisher
{
    Task PublishAsync(PublishArticleDto article);
}

using PublisherService.DTOs;

namespace PublisherService.Services;

public interface IPublishService
{
    Task PublishArticleAsync(PublishArticleDto dto);
}

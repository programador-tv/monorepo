using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Logic;

public sealed class TagBusinessLogic(ITagRepository _repository) : ITagBusinessLogic
{
    public async Task CreateTagsForLiveAndFreeTime(
        List<CreateTagForLiveAndFreeTimeRequest> tagsForLiveAndFreeTimeRequest
    )
    {
        foreach (var tag in tagsForLiveAndFreeTimeRequest)
        {
            var newTag = Tag.AddForLiveAndFreeTime(tag.Titulo, tag.LiveId, tag.FreeTimeId);
            await _repository.CreateTagForLiveAndFreeTime(newTag);
        }
    }
}

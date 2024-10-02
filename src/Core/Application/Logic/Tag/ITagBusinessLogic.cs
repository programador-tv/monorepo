using Domain.Contracts;
using Domain.Entities;

namespace Application.Logic;

public interface ITagBusinessLogic
{
    Task CreateTagsForLiveAndFreeTime(
        List<CreateTagForLiveAndFreeTimeRequest> tagsForLiveAndFreeTimeRequest
    );
}

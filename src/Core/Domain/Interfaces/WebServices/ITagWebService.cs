using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Domain.WebServices;

public interface ITagWebService
{
    Task CreateTagsForLiveAndFreeTime(
        List<CreateTagForLiveAndFreeTimeRequest> tagsForLiveAndFreeTimeRequest
    );
}

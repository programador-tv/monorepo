using Domain.Entities;

namespace Application.Logic
{
    public interface IFeedbackJoinTimeBusinessLogic
    {
        Task<FeedbackJoinTime> CreateFeedbackJoinTime(Guid joinTimeId);
    }
}

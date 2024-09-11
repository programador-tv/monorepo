using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IFeedbackJoinTimeRepository
    {
        Task<FeedbackJoinTime> AddFeedbackJoinTime(FeedbackJoinTime feedbackJoinTime);
    }
}

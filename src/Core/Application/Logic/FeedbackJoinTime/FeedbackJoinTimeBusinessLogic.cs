using Domain.Entities;
using Domain.Interfaces;

namespace Application.Logic
{
    public sealed class FeedbackJoinTimeBusinessLogic(IFeedbackJoinTimeRepository _repository)
        : IFeedbackJoinTimeBusinessLogic
    {
        public async Task<FeedbackJoinTime> CreateFeedbackJoinTime(Guid joinTimeId)
        {
            var feedback = FeedbackJoinTime.Create(
                joinTimeId,
                DateTime.Now,
                false,
                false,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );
            var addFeedback = await _repository.AddFeedbackJoinTime(feedback);
            return addFeedback;
        }
    }
}

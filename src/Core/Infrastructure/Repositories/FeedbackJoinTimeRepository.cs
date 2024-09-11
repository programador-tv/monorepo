using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories
{
    public sealed class FeedbackJoinTimeRepository(ApplicationDbContext context)
        : GenericRepository<FeedbackJoinTime>(context),
            IFeedbackJoinTimeRepository
    {
        public async Task<FeedbackJoinTime> AddFeedbackJoinTime(FeedbackJoinTime feedbackJoinTime)
        {
            var operation = await DbContext.FeedbackJoinTimes.AddAsync(feedbackJoinTime);
            await DbContext.SaveChangesAsync();
            return operation.Entity;
        }
    }
}

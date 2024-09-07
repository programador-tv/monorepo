using Domain.Contracts;
using Domain.Entities;

namespace Domain.Interfaces.WebServices
{
    public interface IFollowWebService
    {
        Task<bool> IsFollowing(Guid followerId, Guid followingId);
    }
}

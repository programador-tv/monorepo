using Domain.Contracts;
using Domain.Interfaces.WebServices;


namespace Infrastructure.WebServices.Core
{
    public sealed class FollowWebService(CoreClient client) : IFollowWebService
    {
        private const string baseRoute = "api/follow/isFollowing";
        public async Task<bool> IsFollowing(Guid followerId, Guid followingId)
        {
            var route = $"{baseRoute}/{followerId}/{followingId}";
            return await client.GetAsync<bool>(route);
        }
    }
}

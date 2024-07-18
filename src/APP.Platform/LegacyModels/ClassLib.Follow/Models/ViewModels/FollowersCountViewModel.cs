using MassTransit.SagaStateMachine;

namespace ClassLib.Follow.Models.ViewModels
{
    public sealed class FollowersCountViewModel
    {
        public Guid UserId { get; set; }
        public int Followers { get; set; }
    }
}

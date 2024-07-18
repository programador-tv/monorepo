using Domain.Enums;

namespace Domain.Entities
{
    public sealed class FeedbackJoinTime : _FeedbackBase
    {
        public Guid JoinTimeId { get; set; }
        public DateTime? DataTentativaMarcacao { get; set; }
    }
}

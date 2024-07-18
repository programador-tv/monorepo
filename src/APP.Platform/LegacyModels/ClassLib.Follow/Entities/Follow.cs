using System;

namespace Domain.Entities
{
    public sealed class Follow : _BaseEntity
    {
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }
        public Boolean Active { get; set; }
    }
}

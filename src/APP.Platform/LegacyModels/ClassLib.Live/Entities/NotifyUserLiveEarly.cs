namespace Domain.Entities
{
    public sealed class NotifyUserLiveEarly : _BaseEntity
    {
        public Guid LiveId { get; set; }
        public Guid PerfilId { get; set; }
        public bool Active { get; set; }
        public bool hasNotificated { get; set; }
    }
}

namespace Domain.Entities
{
    public sealed class FreeTimeBackstage : _BaseEntity
    {
        public Guid TimeSelectionId { get; set; }
        public int MaxParticipants { get; set; }
        public bool Ilimitado { get; set; }
        public bool AutoAccept { get; set; }
    }
}

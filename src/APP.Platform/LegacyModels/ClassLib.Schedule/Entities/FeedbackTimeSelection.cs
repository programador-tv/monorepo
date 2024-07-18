using Domain.Enums;

namespace Domain.Entities
{
    public sealed class FeedbackTimeSelection : _FeedbackBase
    {
        public Guid TimeSelectionId { get; set; }
        public DateTime? DataDeclaracao { get; set; }
        public DateTime? Aceite { get; set; }
    }
}

namespace Domain.Entities
{
    public sealed class HelpBackstage : _BaseEntity
    {
        public Guid TimeSelectionId { get; set; }
        public string Descricao { get; set; } = "";
        public string? ImagePath { get; set; }
    }
}

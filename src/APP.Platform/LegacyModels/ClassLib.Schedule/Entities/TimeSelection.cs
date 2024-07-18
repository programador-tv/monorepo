using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    public sealed class TimeSelection : _BaseEntity
    {
        public Guid? PerfilId { get; set; }
        public Guid? RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? TituloTemporario { get; set; }
        public EnumTipoTimeSelection Tipo { get; set; }
        public StatusTimeSelection Status { get; set; }
        public bool NotifiedMentoriaProxima { get; set; }
        public string? PreviewImage { get; set; }
        public TipoAction TipoAction { get; set; }
        public Variacao Variacao { get; set; }

        [NotMapped]
        public List<Tag>? Tags { get; set; }

        [NotMapped]
        public Perfil? Perfil { get; set; }

        [NotMapped]
        public string? TempoRestante { get; set; }

        [NotMapped]
        public string? LinkSala { get; set; }

        [NotMapped]
        public bool ActionNeeded { get; set; }

        [NotMapped]
        public int CountInteressados { get; set; }

        [NotMapped]
        public string? Descricao { get; set; }

        [NotMapped]
        public bool ShowSelectRandomStudents { get; set; }

        [NotMapped]
        public string? RequestedHelpImagePath { get; set; }

        [NotMapped]
        public int MaxParticipants { get; set; }
        [NotMapped]
        public bool Ilimitado { get; set; }
    }
}

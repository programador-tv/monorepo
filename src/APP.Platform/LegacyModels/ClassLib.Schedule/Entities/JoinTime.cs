using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    public sealed class JoinTime : _BaseEntity
    {
        public Guid? PerfilId { get; set; }
        public Guid? TimeSelectionId { get; set; }
        public StatusJoinTime StatusJoinTime { get; set; }
        public bool NotifiedMentoriaProxima { get; set; }

        public TipoAction TipoAction { get; set; } = TipoAction.Aprender;
    }
}

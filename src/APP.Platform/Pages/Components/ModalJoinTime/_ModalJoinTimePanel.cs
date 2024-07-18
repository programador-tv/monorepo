using Domain.Entities;

namespace APP.Platform.Pages.Components.ModalJoinTime
{
    public class ModalJoinTimePanel
    {
        public Dictionary<JoinTime, TimeSelection> JoinEvent { get; set; } = new();
    }
}

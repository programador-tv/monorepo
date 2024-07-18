using Domain.Enums;

public class ModalRepeatEvent
{
    public EnumWeekPattern WeekPattern { get; set; }
    public EnumRepeatWeekParttern? RepeatWeekParttern { get; set; }
    public bool NeedAddActualMonth { get; set; }
}

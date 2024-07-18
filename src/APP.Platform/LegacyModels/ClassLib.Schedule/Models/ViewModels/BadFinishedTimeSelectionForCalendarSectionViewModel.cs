namespace Domain.Models.ViewModels;

public sealed class BadFinishedTimeSelectionForCalendarSectionViewModel
{
    public Guid Id { get; set; }
    public string? Title { get; set; } = "";
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

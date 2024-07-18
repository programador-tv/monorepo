using Microsoft.AspNetCore.Http;

namespace Domain.RequestModels;

public class HelpBackstageRequestModel
{
    public Guid TimeSelectionId { get; set; }
    public string Description { get; set; } = string.Empty;
    public IFormFile? ImageFile { get; set; }
}

using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Logic;

public interface IHelpBackstageBusinessLogic
{
    public Task<HelpBackstage> ScheduleResquestedHelp(CreateHelpBackstageRequest request);
    public Task SaveImageFile(Guid timeSelectionId, IFormFile img);
    public Task<List<HelpBackstage>> GetByIds(List<Guid> ids);
}

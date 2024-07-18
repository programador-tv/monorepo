using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.FileHandling;
using Microsoft.AspNetCore.Http;

namespace Application.Logic;

public class HelpBackstageBusinessLogic(IHelpBackstageRepository _repository, ISaveFile _saveFile)
    : IHelpBackstageBusinessLogic
{
    public async Task<HelpBackstage> ScheduleResquestedHelp(CreateHelpBackstageRequest request)
    {
        var backstage = HelpBackstage.Create(request);
        await _repository.AddAsync(backstage);
        return backstage;
    }

    public async Task SaveImageFile(Guid timeSelectionId, IFormFile img)
    {
        var backstage =
            await _repository.GetByTimeSelectionIdAsync(timeSelectionId)
            ?? throw new KeyNotFoundException("HelpBackstage não encontrado");
        var extension = Path.GetExtension(img.FileName);
        var relativeImgPath = $"{timeSelectionId}_{Guid.NewGuid()}{extension}";
        var imgPath = _saveFile.BuildPathFileSave(relativeImgPath, "RequestHelp");
        await _saveFile.SaveImageFile(img, imgPath);
        backstage.AddImagePath(imgPath);
        await _repository.UpdateAsync(backstage);
    }

    public async Task<List<HelpBackstage>> GetByIds(List<Guid> ids) =>
        await _repository.GetByIdsAsync(ids)
        ?? throw new KeyNotFoundException("Help backstage não encontrado");
}

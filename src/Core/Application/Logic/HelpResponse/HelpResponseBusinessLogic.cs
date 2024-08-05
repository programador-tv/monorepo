using Domain.Contracts;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Repositories;

namespace Application.Logic;

public sealed class HelpResponseBusinessLogic(IHelpResponseRepository _repository)
    : IHelpResponseBusinessLogic
{
    public async Task Add(CreateHelpResponse requestHelpResponse)
    {
        var helpResponse = HelpResponse.Create(
            requestHelpResponse.timeSelectionId,
            requestHelpResponse.perfilId,
            requestHelpResponse.Conteudo
        );
        await _repository.AddAsync(helpResponse);
    }

    public async Task Delete(Guid helpResponseId)
    {
        var helpResponse = await _repository.GetById(helpResponseId);
        helpResponse.DeleteResponse();
        await _repository.UpdateAsync(helpResponse);
    }

    public async Task<List<HelpResponse>> GetAll(Guid timeSelectionId)
    {
        return await _repository.GetAllAsync(timeSelectionId);
    }
}

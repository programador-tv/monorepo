using Domain.Contracts;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Repositories;

namespace Application.Logic;

public sealed class HelpResponseBusinessLogic(IHelpResponseRepository _repository)
    : IHelpResponseBusinessLogic
{
    public async Task Add(CreateHelpResponse helpResponse)
    {
        var createdHelpResponse = HelpResponse.Create(
            helpResponse.timeSelectionId,
            helpResponse.perfilId,
            helpResponse.Conteudo
        );
        await _repository.AddAsync(createdHelpResponse);
    }

    public async Task Delete(Guid helpResponseId)
    {
        var helpResponse =
            await _repository.GetById(helpResponseId)
            ?? throw new KeyNotFoundException("Coment�rio n�o encontrado.");
        helpResponse.DeleteResponse();
        await _repository.UpdateAsync(helpResponse);
    }

    public async Task<List<HelpResponse>> GetAll(Guid timeSelectionId)
    {
        return await _repository.GetAllAsync(timeSelectionId);
    }
}

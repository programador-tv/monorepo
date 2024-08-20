using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Logic;

public interface IHelpResponseBusinessLogic
{
    Task<List<HelpResponse>> GetAll(Guid timeSelectionId);
    Task<HelpResponse> Add(CreateHelpResponse helpResponse);
    Task Delete(Guid helpResponseId);
}

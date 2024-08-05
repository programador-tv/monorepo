using Domain.Contracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebServices;

public interface IHelpResponseWebService
{
    Task Add(CreateHelpResponse helpResponse);
    Task Update(Guid helpResponseId);
    Task<List<HelpResponse>> GetAll(Guid timeSelectionId);
    Task<HelpResponse> GetById(Guid id);
}

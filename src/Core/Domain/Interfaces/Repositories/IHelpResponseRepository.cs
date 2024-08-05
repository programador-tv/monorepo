﻿using Domain.Contracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IHelpResponseRepository
    {
        Task<List<HelpResponse>> GetAllAsync(Guid timeSelectionId);
        Task<HelpResponse> GetById(Guid id);
        Task AddAsync(HelpResponse helpResponse);
        Task UpdateAsync(HelpResponse helpResponse);
    }
}

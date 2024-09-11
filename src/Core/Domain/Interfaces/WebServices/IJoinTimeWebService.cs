using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Domain.WebServices;

public interface IJoinTimeWebService
{
    Task UpdateOldJoinTimes();
    Task<List<JoinTime>> GetJoinTimesAtivos(Guid timeId);
}

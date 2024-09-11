using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class JoinTimeRepository(ApplicationDbContext context)
    : GenericRepository<Notification>(context),
        IJoinTimeRepository
{
    public async Task<Dictionary<Guid, Guid>> GetJoinTimePerfilIdsByTimeSelectionIds(
        List<Guid> ids
    ) =>
        await DbContext
            .JoinTimes.Where(e => ids.Contains(e.TimeSelectionId))
            .Select(e => new { e.PerfilId, e.TimeSelectionId })
            .ToDictionaryAsync(e => e.TimeSelectionId, e => e.PerfilId);

    public async Task<Dictionary<JoinTime, TimeSelection>> GetFreeTimeMarcadosAntigos()
    {
        var markedJts = await DbContext
            .JoinTimes.Where(e => e.StatusJoinTime == StatusJoinTime.Marcado)
            .ToListAsync();

        var tsIds = markedJts.Select(e => e.TimeSelectionId).ToList();

        var OldTimeSelections = await DbContext
            .TimeSelections.Where(e => tsIds.Contains(e.Id))
            .ToListAsync();

        var OldTimeSelectionsIds = OldTimeSelections
            .Where(e => e.EndTime < DateTime.Now)
            .Select(e => e.Id)
            .ToList();

        var oldMarkedJts = markedJts
            .Where(e => OldTimeSelectionsIds.Contains(e.TimeSelectionId))
            .ToList();

        var pairJtAndTs = new Dictionary<JoinTime, TimeSelection>();
        foreach (var jt in oldMarkedJts)
        {
            var ts = OldTimeSelections.First(e => e.Id == jt.TimeSelectionId);
            pairJtAndTs[jt] = ts;
        }

        return pairJtAndTs;
    }

    public async Task UpdateRange(List<JoinTime> jts)
    {
        DbContext.JoinTimes.UpdateRange(jts);
        await DbContext.SaveChangesAsync();
    }

    public async Task<List<JoinTime>> GetJoinTimesAtivos(Guid timeId)
    {
        return await DbContext
            .JoinTimes.Where(j =>
                j.TimeSelectionId == timeId
                && (
                    j.StatusJoinTime == StatusJoinTime.Marcado
                    || j.StatusJoinTime == StatusJoinTime.Pendente
                )
            )
            .ToListAsync();
    }
}

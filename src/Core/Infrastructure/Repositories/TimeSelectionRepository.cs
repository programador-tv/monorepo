using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class TimeSelectionRepository(ApplicationDbContext context)
    : GenericRepository<Notification>(context),
        ITimeSelectionRepository
{
    public async Task<TimeSelection> GetById(Guid id) =>
        await DbContext.TimeSelections.FirstOrDefaultAsync(e => e.Id == id)
        ?? throw new KeyNotFoundException("TimeSelection não encontrado.");

    public async Task<List<TimeSelection>> GetFreeTimeMarcadosAntigos()
    {
        var tss = await DbContext
            .TimeSelections.Where(e =>
                e.Tipo == EnumTipoTimeSelection.FreeTime && e.Status == StatusTimeSelection.Marcado
            )
            .ToListAsync();
        return tss.Where(e => e.EndTime < DateTime.Now).ToList();
    }

    public async Task<Dictionary<Guid, Guid>> GetTimeSelectionPerfilIdsByJoinTimeIds(List<Guid> ids)
    {
        var tsIds = await DbContext
            .JoinTimes.Where(e => ids.Contains(e.Id))
            .Select(e => e.TimeSelectionId)
            .ToListAsync();

        return await DbContext
            .TimeSelections.Where(e => tsIds.Contains(e.Id))
            .Select(e => new { e.PerfilId, e.Id })
            .ToDictionaryAsync(e => e.Id, e => e.PerfilId);
    }

    public async Task UpdateRange(List<TimeSelection> tss)
    {
        DbContext.TimeSelections.UpdateRange(tss);
        await DbContext.SaveChangesAsync();
    }

    public async Task<
        Dictionary<TimeSelection, List<JoinTime>>
    > GetUpcomingTimeSelectionAndJoinTime()
    {
        var dict = new Dictionary<TimeSelection, List<JoinTime>> { };

        var tss = await DbContext
            .TimeSelections.Where(e =>
                e.Tipo == EnumTipoTimeSelection.FreeTime
                && e.Status == StatusTimeSelection.Marcado
                && !e.NotifiedMentoriaProxima
            )
            .ToListAsync();
        #warning deveria estar na busca do banco e não depois 
        var tsIds = tss.Where(e => e.StartTime > DateTime.Now.AddMinutes(-30))
            .Select(e => e.Id)
            .ToList();

        var jts = await DbContext
            .JoinTimes.Where(e =>
                e.StatusJoinTime == StatusJoinTime.Marcado
                && !e.NotifiedMentoriaProxima
                && tsIds.Contains(e.TimeSelectionId)
            )
            .ToListAsync();

        foreach (var ts in tss)
        {
            var jtsByTs = jts.Where(e => e.TimeSelectionId == ts.Id).ToList();
            dict[ts] = jtsByTs;
        }

        return dict;
    }
}

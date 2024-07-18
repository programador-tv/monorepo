using System;
using System.Net.WebSockets;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Infrastructure.Contexts;
using Infrastructure.WebServices;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class LiveRepository(ApplicationDbContext context)
    : GenericRepository<Live>(context),
        ILiveRepository
{
    public async Task UpdateAsync(Live live)
    {
        DbContext.Lives.Update(live);
        await DbContext.SaveChangesAsync();
    }

    public async Task<Live> AddAsync(Live live)
    {
        var result = await DbContext.Lives.AddAsync(live);
        await DbContext.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Live> GetByIdAsync(Guid id)
    {
        return await DbContext.Lives.FindAsync(id)
            ?? throw new KeyNotFoundException("Live não encontrada");
    }

    public async Task<Dictionary<Live, List<NotifyUserLiveEarly>>> GetUpcomingLives()
    {
        var dict = new Dictionary<Live, List<NotifyUserLiveEarly>>();

        var notifyUsers = await DbContext
            .NotifyUserLiveEarlies.Where(n => n.Active && !n.HasNotificated)
            .ToListAsync();

        if (notifyUsers.Count == 0)
        {
            return dict;
        }

        var liveIds = notifyUsers.Select(n => n.LiveId).Distinct().ToList();

        var liveBackstage = await DbContext
            .LiveBackstages.Where(l => liveIds.Contains(l.LiveId))
            .ToListAsync();

        var tsIds = liveBackstage.Select(l => l.TimeSelectionId).Distinct().ToList();

        var tsIdsThatShouldNotify = await DbContext
            .TimeSelections.Where(t =>
                tsIds.Contains(t.Id) && t.StartTime <= DateTime.Now.AddMinutes(30)
            )
            .Select(e => e.Id)
            .ToListAsync();

        var liveBackstageThatShouldNotify = liveBackstage
            .Where(l => tsIdsThatShouldNotify.Contains(l.TimeSelectionId))
            .ToList();

        var notifyUsersThatShouldNotify = notifyUsers
            .Where(n => liveBackstageThatShouldNotify.Select(l => l.LiveId).Contains(n.LiveId))
            .ToList();

        var lives = await DbContext.Lives.Where(l => liveIds.Contains(l.Id)).ToListAsync();

        dict = lives.ToDictionary(
            l => l,
            l => notifyUsersThatShouldNotify.Where(n => n.LiveId == l.Id).ToList()
        );

        return dict;
    }

    public async Task UpdateRangeNotifyUserLiveEarly(List<NotifyUserLiveEarly> notifyUsers)
    {
        DbContext.NotifyUserLiveEarlies.UpdateRange(notifyUsers);
        await DbContext.SaveChangesAsync();
    }

    public async Task<Live> GetByUrlAsync(string url)
    {
        return await DbContext.Lives.FirstOrDefaultAsync(e => e.UrlAlias == url)
            ?? throw new KeyNotFoundException("Live não encontrada");
    }

    public async Task UpdateRangeAsync(List<Live> lives)
    {
        DbContext.Lives.UpdateRange(lives);
        await DbContext.SaveChangesAsync();
    }

    public async Task<Guid> GetKeyByStreamId(string streamId)
    {
        var result =
            await DbContext
                .Lives.Select(e => new { e.StreamId, e.Id })
                .FirstOrDefaultAsync(e => e.StreamId == streamId)
            ?? throw new KeyNotFoundException("Live não encontrada");

        return result.Id;
    }

    public async Task<List<Live>> SearchBySpecificTitle(string keyword)
    {
        var result = await DbContext
            .Lives.Where(live =>
                (live.Titulo ?? "").ToUpper() == keyword.ToUpper() && live.Visibility
            )
            .ToListAsync();

        return result;
    }

    public async Task<List<Live>> SearchByTitleContaining(string keyword)
    {
        var result = await DbContext
            .Lives.Where(live =>
                (live.Titulo ?? "").ToUpper().Contains(keyword.ToUpper()) && live.Visibility
            )
            .ToListAsync();

        return result;
    }

    public async Task<List<Live>> SearchByDescriptionContaining(string keyword)
    {
        var result = await DbContext
            .Lives.Where(live =>
                (live.Descricao ?? "").ToUpper().Contains(keyword.ToUpper()) && live.Visibility
            )
            .ToListAsync();

        return result;
    }

    public async Task<List<Live>> SearchByTagContaining(List<Tag> tags)
    {
        var responseLives = new List<Live>();

        foreach (var tag in tags)
        {
            var relatedLives = await DbContext
                .Lives.Where(live => live.Id.ToString() == tag.LiveRelacao && live.Visibility)
                .ToListAsync();

            if (relatedLives.Count > 0 && relatedLives != null)
            {
                responseLives.AddRange(relatedLives);
            }
        }

        return responseLives;
    }

    public async Task<List<Live>> SearchByListPerfilId(List<Perfil> perfils)
    {
        var responseLives = new List<Live>();

        foreach (var perfil in perfils)
        {
            var relatedLives = await DbContext
                .Lives.Where(live => live.PerfilId == perfil.Id && live.Visibility)
                .ToListAsync();

            if (relatedLives.Count > 0 && relatedLives != null)
            {
                responseLives.AddRange(relatedLives);
            }
        }

        return responseLives;
    }

    public async Task<List<Guid>> CloseNonUpdatedLiveRangeAsync()
    {
        var lives = await DbContext.Lives.Where(e =>
            e.StatusLive == StatusLive.Iniciada
            && e.UltimaAtualizacao != null
            && e.UltimaAtualizacao.Value.AddMinutes(5) < DateTime.Now
        ).ToListAsync();

        lives.ForEach(e => e.Encerra());
        DbContext.UpdateRange(lives);
        await DbContext.SaveChangesAsync();

        return lives.Select(e => e.Id).ToList();
    }
}

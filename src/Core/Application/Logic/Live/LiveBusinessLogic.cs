using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Domain.WebServices;
using Queue;

namespace Application.Logic;

public sealed class LiveBusinessLogic(
    ILiveRepository _repository,
    IMessagePublisher _messagePublisher,
    IOpenaiWebService _openaiWebService,
    IPerfilRepository _perfilRepository,
    ITagRepository _tagRepository
) : ILiveBusinessLogic
{
    public async Task<CreateLiveResponse> AddLive(CreateLiveRequest createLiveRequest)
    {
        var live = Live.Create(createLiveRequest);
        await _repository.AddAsync(live);
        return new CreateLiveResponse(live.Id);
    }

    public async Task<Live> GetLiveById(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task NotifyUpcomingLives()
    {
        var livesAndUsersForNotify = await _repository.GetUpcomingLives();

        foreach (var liveAndUsersNotify in livesAndUsersForNotify)
        {
            foreach (var userNotify in liveAndUsersNotify.Value)
            {
                userNotify.MarkAsNotificated();
                var notification = userNotify.BuildUpcomingNotification(
                    liveAndUsersNotify.Key.PerfilId,
                    liveAndUsersNotify.Key.Id,
                    liveAndUsersNotify.Key.Titulo ?? string.Empty
                );
                await _messagePublisher.PublishAsync("NotificationsQueue", notification);
            }
        }

        var allUserNotify = livesAndUsersForNotify.SelectMany(l => l.Value).ToList();
        await _repository.UpdateRangeNotifyUserLiveEarly(allUserNotify);
    }

    public Task<Live> GetLiveByUrl(string url)
    {
        return _repository.GetByUrlAsync(url);
    }

    public async Task UpdateThumbnail(UpdateLiveThumbnailRequest updateLiveThumbnailRequest)
    {
        var live =
            await _repository.GetByIdAsync(updateLiveThumbnailRequest.Id)
            ?? throw new KeyNotFoundException();
        live.AtualizaThumbnail(updateLiveThumbnailRequest.ImageBase64);
        await _repository.UpdateAsync(live);
    }

    public async Task KeepOn(Guid id)
    {
        var live = await _repository.GetByIdAsync(id);
        live!.MantemAberta();
        await _repository.UpdateAsync(live);
    }

    public async Task FinishWithDuration(Guid id, string duration)
    {
        var live = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        live.Finaliza();
        live.AtualizaDuracao(duration);
        await _repository.UpdateAsync(live);
    }

    public async Task Close()
    {
        var idsToClose = await _repository.CloseNonUpdatedLiveRangeAsync();

        foreach (var id in idsToClose)
        {
            var message = new StopLiveProcessMessage(id);
            await _messagePublisher.PublishAsync("LiveCloseQueue", message);
        }
    }

    public async Task<Guid> GetKeyByStreamId(string streamId) =>
        await _repository.GetKeyByStreamId(streamId);

    public async Task<string> GetTitleAndDescriptionSugestion(string tags)
    {
        string template =
            "Olá, especialista em geração de textos atrativos e criativos. Estou procurando criar um título e uma descrição para uma live de um site de programação de software, com base nas seguintes palavras-chave: {0}. Por favor, utilize sua expertise para criar um título e uma descrição impactantes, que atraiam a atenção do público. Separe o título e a descrição pelo conjunto de caracteres [$$]. Exemplo:Título: título. [$$] Descrição:descricao";
        var prompt = string.Format(template, tags);

        return await _openaiWebService.GetCompletationResponse(prompt);
    }

    public async Task<List<Live>> SearchLives(string keyword)
    {
        var lives = new List<Live>();

        foreach (var word in keyword.Trim().Split(' '))
        {
            var livesByTitle = await _repository.SearchBySpecificTitle(word);
            lives.AddRange(livesByTitle);

            var livesByTitleContaining = await _repository.SearchByTitleContaining(word);
            lives.AddRange(livesByTitleContaining);

            var livesByDescriptionContaining = await _repository.SearchByDescriptionContaining(
                word
            );
            lives.AddRange(livesByDescriptionContaining);

            var perfis = await _perfilRepository.GetWhenContainsAsync(word);
            var livesByPerfilId = await _repository.SearchByListPerfilId(perfis);
            lives.AddRange(livesByPerfilId);

            var tags = await _tagRepository.GetTagRelationByKeyword(word);
            var livesByTag = await _repository.SearchByTagContaining(tags);
            lives.AddRange(livesByTag);
        }

        return lives.Distinct().ToList();
    }
}

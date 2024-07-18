using System.Diagnostics;
using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.EndPoints;

namespace tests;

public class LiveEndPointsTests
{
    private readonly Mock<ILiveBusinessLogic> mockLogic;

    public LiveEndPointsTests()
    {
        mockLogic = new Mock<ILiveBusinessLogic>();
    }

    [Fact]
    public async Task GetLiveByUrl_ShouldReturnOk_WithLive()
    {
        var url = "Test-efb58h";
        var live = Live.Create(
            new CreateLiveRequest(
                PerfilId: Guid.NewGuid(),
                Titulo: "Test",
                Descricao: "Teste de descrição",
                Thumbnail: "https://i.ytimg.com/vi/9XzDuhgJhKs/maxresdefault.jpg",
                IsUsingObs: false,
                StreamId: Guid.NewGuid().ToString(),
                UrlAlias: "Test-efb58h"
            )
        );

        mockLogic.Setup(logic => logic.GetLiveByUrl(url)).ReturnsAsync(live);

        var result = await LiveEndPoints.GetLiveByUrl(mockLogic.Object, url);

        var Ok = Assert.IsType<Ok<Live>>(result);
        Assert.Equal(live, Ok.Value);
    }

    [Fact]
    public async Task GetLiveByUrl_ShouldReturnBadRequest_WithMessage_OnException()
    {
        var url = "qwerty";
        mockLogic
            .Setup(logic => logic.GetLiveByUrl(url))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await LiveEndPoints.GetLiveByUrl(mockLogic.Object, url);

        var badRequestResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Erro de teste", badRequestResult.Value);
    }

    [Fact]
    public async Task Add_ShouldReturnOk_WhenLiveIsAdded()
    {
        var liveId = Guid.NewGuid();
        var live = Live.Create(
            new CreateLiveRequest(
                PerfilId: Guid.NewGuid(),
                Titulo: "Test",
                Descricao: "Teste de descrição",
                Thumbnail: "https://i.ytimg.com/vi/9XzDuhgJhKs/maxresdefault.jpg",
                IsUsingObs: false,
                StreamId: Guid.NewGuid().ToString(),
                UrlAlias: "Test-efb58h"
            )
        );

        mockLogic.Setup(logic => logic.GetLiveById(liveId)).ReturnsAsync(live);

        var result = await LiveEndPoints.GetLiveById(mockLogic.Object, liveId);

        Assert.IsType<Ok<Live>>(result);
        var Ok = result as Ok<Live>;
        Assert.Equal(live, Ok?.Value);
    }

    [Fact]
    public async Task Add_ShouldReturnBadRequest_WithMessage_OnException()
    {
        var request = new CreateLiveRequest(
            PerfilId: Guid.NewGuid(),
            Titulo: "Test",
            Descricao: "Teste de descrição",
            Thumbnail: "https://i.ytimg.com/vi/9XzDuhgJhKs/maxresdefault.jpg",
            IsUsingObs: false,
            StreamId: Guid.NewGuid().ToString(),
            UrlAlias: "Test-efb58h"
        );
        mockLogic
            .Setup(logic => logic.AddLive(request))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await LiveEndPoints.Add(mockLogic.Object, request);

        var badRequestResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Erro de teste", badRequestResult.Value);
    }

    [Fact]
    public async Task GetLiveById_ShouldReturnOk_WithLive()
    {
        var liveId = Guid.NewGuid();
        var live = Live.Create(
            new CreateLiveRequest(
                PerfilId: Guid.NewGuid(),
                Titulo: "Test",
                Descricao: "Teste de descrição",
                Thumbnail: "https://i.ytimg.com/vi/9XzDuhgJhKs/maxresdefault.jpg",
                IsUsingObs: false,
                StreamId: Guid.NewGuid().ToString(),
                UrlAlias: "Test-efb58h"
            )
        );

        mockLogic.Setup(logic => logic.GetLiveById(liveId)).ReturnsAsync(live);

        var result = await LiveEndPoints.GetLiveById(mockLogic.Object, liveId);

        var Ok = Assert.IsType<Ok<Live>>(result);
        Assert.Equal(live, Ok.Value);
    }

    [Fact]
    public async Task GetLiveById_ShouldReturnBadRequest_WithMessage_OnException()
    {
        var liveId = Guid.NewGuid();
        mockLogic
            .Setup(logic => logic.GetLiveById(liveId))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await LiveEndPoints.GetLiveById(mockLogic.Object, liveId);

        var badRequestResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Erro de teste", badRequestResult.Value);
    }

    [Fact]
    public async Task NotifyUpcomingLives_ShouldReturnOk()
    {
        mockLogic.Setup(logic => logic.NotifyUpcomingLives()).Returns(Task.CompletedTask);

        var result = await LiveEndPoints.NotifyUpcomingLives(mockLogic.Object);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public async Task NotifyUpcomingLives_ShouldReturnBadRequest_WithMessage_OnException()
    {
        mockLogic
            .Setup(logic => logic.NotifyUpcomingLives())
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await LiveEndPoints.NotifyUpcomingLives(mockLogic.Object);

        var badRequestResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Erro de teste", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateThumbnail_ShouldReturnOk()
    {
        var guidLiveId = Guid.NewGuid();
        var newImageString = "newThumbnail";
        var updateRequest = new UpdateLiveThumbnailRequest(guidLiveId, newImageString);

        mockLogic.Setup(logic => logic.UpdateThumbnail(updateRequest)).Returns(Task.CompletedTask);

        var result = await LiveEndPoints.UpdateThumbnail(mockLogic.Object, updateRequest);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public async Task UpdateThumbnail_ShouldReturnBadRequest_WithMessage_OnException()
    {
        var guidLiveId = Guid.NewGuid();
        var newImageString = "newThumbnail";
        var updateRequest = new UpdateLiveThumbnailRequest(guidLiveId, newImageString);

        mockLogic
            .Setup(logic => logic.UpdateThumbnail(updateRequest))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await LiveEndPoints.UpdateThumbnail(mockLogic.Object, updateRequest);

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task KeepLiveOn_ShouldReturnOk()
    {
        var guidLiveId = Guid.NewGuid();

        mockLogic.Setup(logic => logic.KeepOn(guidLiveId)).Returns(Task.CompletedTask);

        var result = await LiveEndPoints.KeepLiveOn(mockLogic.Object, guidLiveId.ToString());

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public async Task KeepLiveOn_ShouldReturnBadRequest_WithMessage_OnException()
    {
        var guidLiveId = Guid.NewGuid();

        mockLogic
            .Setup(logic => logic.KeepOn(guidLiveId))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await LiveEndPoints.KeepLiveOn(mockLogic.Object, guidLiveId.ToString());

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task FinishWithDuration_ShouldReturnOk()
    {
        var guidLiveId = Guid.NewGuid();
        var formatedDate = "17/04/2024";
        var liveThumbnailRequest = new LiveThumbnailRequest(guidLiveId, formatedDate);

        mockLogic
            .Setup(logic => logic.FinishWithDuration(guidLiveId, formatedDate))
            .Returns(Task.CompletedTask);

        var result = await LiveEndPoints.FinishWithDuration(mockLogic.Object, liveThumbnailRequest);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public async Task FinishWithDuration_ShouldReturnBadRequest_WithMessage_OnException()
    {
        var guidLiveId = Guid.NewGuid();
        var formatedDate = "17/04/2024";
        var liveThumbnailRequest = new LiveThumbnailRequest(guidLiveId, formatedDate);

        mockLogic
            .Setup(logic => logic.FinishWithDuration(guidLiveId, formatedDate))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await LiveEndPoints.FinishWithDuration(mockLogic.Object, liveThumbnailRequest);

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task Close_ShouldReturnOk()
    {
        mockLogic.Setup(logic => logic.Close()).Returns(Task.CompletedTask);

        var result = await LiveEndPoints.Close(mockLogic.Object);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public async Task Close_ShouldReturnBadRequest_WithMessage_OnException()
    {
        mockLogic.Setup(logic => logic.Close()).ThrowsAsync(new Exception("Erro de teste"));

        var result = await LiveEndPoints.Close(mockLogic.Object);

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task GetKeyByStreamId_ShouldReturnOk()
    {
        var streamId = "streamId";
        var expectedGuid = Guid.NewGuid();

        mockLogic.Setup(logic => logic.GetKeyByStreamId(streamId)).ReturnsAsync(expectedGuid);

        var result = await LiveEndPoints.GetKeyByStreamId(mockLogic.Object, streamId);

        Assert.IsType<Ok<Guid>>(result);
        Assert.Equal(expectedGuid, ((Ok<Guid>)result).Value);
    }

    [Fact]
    public async Task GetKeyByStreamId_ShouldReturnBadRequest_WithMessage_OnException()
    {
        var streamId = "streamId";

        mockLogic
            .Setup(logic => logic.GetKeyByStreamId(streamId))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await LiveEndPoints.GetKeyByStreamId(mockLogic.Object, streamId);

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task SearchLives_ShouldReturnOk()
    {
        var keyword = "Business";

        var live1 = new Live(
            id: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: Guid.NewGuid().ToString(),
            liveEstaAberta: true,
            titulo: "Business",
            descricao: "",
            thumbnail: "Thumbnail",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        var live2 = new Live(
            id: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: Guid.NewGuid().ToString(),
            liveEstaAberta: true,
            titulo: "Business Logic",
            descricao: "",
            thumbnail: "Thumbnail",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        var live3 = new Live(
            id: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: Guid.NewGuid().ToString(),
            liveEstaAberta: true,
            titulo: "",
            descricao: "Business",
            thumbnail: "Thumbnail",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        var live4 = new Live(
            id: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: Guid.NewGuid().ToString(),
            liveEstaAberta: true,
            titulo: "",
            descricao: "",
            thumbnail: "Thumbnail",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        var live5 = new Live(
            id: Guid.NewGuid(),
            perfilId: Guid.NewGuid(),
            dataCriacao: DateTime.Now,
            ultimaAtualizacao: DateTime.Now,
            formatedDuration: "",
            codigoLive: "",
            recordedUrl: "",
            streamId: Guid.NewGuid().ToString(),
            liveEstaAberta: true,
            titulo: "",
            descricao: "",
            thumbnail: "Thumbnail",
            visibility: true,
            tentativasDeObterUrl: 0,
            statusLive: StatusLive.Preparando,
            isUsingObs: false,
            urlAlias: "Test-efb58h"
        );

        var lives = new List<Live> { live1, live2, live3, live4, live5 };

        mockLogic.Setup(logic => logic.SearchLives(keyword)).ReturnsAsync(lives);

        var result = await LiveEndPoints.SearchLives(mockLogic.Object, keyword);

        Assert.IsType<Ok<List<Live>>>(result);
        var okResult = result as Ok<List<Live>>;
        Assert.Equal(lives, okResult?.Value);
    }

    [Fact]
    public async Task SearchLives_ShouldReturnBadRequest_WithMessage_OnException()
    {
        var keyword = "Business";

        mockLogic
            .Setup(logic => logic.SearchLives(keyword))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await LiveEndPoints.SearchLives(mockLogic.Object, keyword);

        Assert.IsType<BadRequest<string>>(result);
    }
}

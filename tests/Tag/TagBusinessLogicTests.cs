using System.Collections.Generic;
using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;

namespace tests;

public class TagBusinessLogicTests
{
    private readonly TagBusinessLogic _businessLogic;
    private readonly Mock<ITagRepository> _mockRepository;

    public TagBusinessLogicTests()
    {
        _mockRepository = new Mock<ITagRepository>();
        _businessLogic = new TagBusinessLogic(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateTagsForLiveAndFreeTime_ShouldInvokeCreateTagsForLiveAndFreeTime()
    {
        var list = new List<CreateTagForLiveAndFreeTimeRequest>();
        var request = new CreateTagForLiveAndFreeTimeRequest(
            Titulo: "Teste",
            LiveId: Guid.NewGuid().ToString(),
            FreeTimeId: Guid.NewGuid().ToString()
        );
        list.Add(request);

        _mockRepository
            .Setup(repo => repo.CreateTagForLiveAndFreeTime(It.IsAny<Tag>()))
            .Returns(Task.CompletedTask);

        await _businessLogic.CreateTagsForLiveAndFreeTime(list);

        _mockRepository.Verify(
            repo => repo.CreateTagForLiveAndFreeTime(It.IsAny<Tag>()),
            Times.Once
        );
    }
}

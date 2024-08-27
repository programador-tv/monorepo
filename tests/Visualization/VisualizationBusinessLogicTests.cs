using Application.Logic;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace tests
{
	public class VisualizationBusinessLogicTests
	{
		private readonly VisualizationBusinessLogic mockLogic;
		private readonly Mock<IVisualizationsRepository> mockRepository;

		public VisualizationBusinessLogicTests()
		{
			mockRepository = new Mock<IVisualizationsRepository>();
			mockLogic = new VisualizationBusinessLogic(mockRepository.Object);
		}

		[Fact]
		public async Task GetVisualizationsByLiveIds_ShouldReturnVisualizationList()
		{
			var visualizations = new List<Visualization> {
				Visualization.Create(Guid.NewGuid(), Guid.NewGuid(), ""),
				Visualization.Create(Guid.NewGuid(), Guid.NewGuid(), "")
			};

			mockRepository.Setup(repo => repo.GetVisualizationsByLiveIds(It.IsAny<List<Guid>>())).ReturnsAsync(visualizations);

			var result = await mockLogic.GetLiveVisualizations([Guid.NewGuid(), Guid.NewGuid()]);

			Assert.Equal(visualizations, result);
			mockRepository.Verify(repo => repo.GetVisualizationsByLiveIds(It.IsAny<List<Guid>>()), Times.Once());
		}
	}
}
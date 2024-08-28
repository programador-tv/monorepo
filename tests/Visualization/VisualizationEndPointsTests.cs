using System;
using Application.Logic;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.EndPoints;

namespace tests
{
    public class VisualizationEndPointsTests
    {
        private readonly Mock<IVisualizationBusinessLogic> mockLogic;

        public VisualizationEndPointsTests()
        {
            mockLogic = new Mock<IVisualizationBusinessLogic>();
        }

        [Fact]
        public async Task GetLiveVisualizationEndpoint_ShouldCallLogic()
        {
            var visualizations = new List<Visualization>
            {
                Visualization.Create(Guid.NewGuid(), Guid.NewGuid(), ""),
                Visualization.Create(Guid.NewGuid(), Guid.NewGuid(), "")
            };

            mockLogic
                .Setup(logic => logic.GetLiveVisualizations(It.IsAny<List<Guid>>()))
                .ReturnsAsync(visualizations);

            var result = await VisualizationEndPoints.GetLiveVisualization(
                mockLogic.Object,
                [Guid.NewGuid(), Guid.NewGuid()]
            );

            mockLogic.Verify(
                logic => logic.GetLiveVisualizations(It.IsAny<List<Guid>>()),
                Times.Once()
            );
        }
    }
}

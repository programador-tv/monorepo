using Domain.Entities;

namespace tests;

public class FreeTimeBackstageTest
{
    [Fact]
    public void CreateFreeTimeBackstage_ReturnsCorrectValues()
    {
        // Arrange
        Guid expectedTimeSelectionId = Guid.NewGuid();
        int expectedMaxParticipants = 8;
        bool expectedIlimitado = true;

        // Act
        var result = FreeTimeBackstage.Create(expectedTimeSelectionId, expectedMaxParticipants, expectedIlimitado);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTimeSelectionId,result.TimeSelectionId);
        Assert.Equal(expectedMaxParticipants,result.MaxParticipants);
        Assert.Equal(expectedIlimitado,result.Ilimitado);
    }
}

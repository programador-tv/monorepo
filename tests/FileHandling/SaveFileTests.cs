using Infrastructure.FileHandling;
using Microsoft.AspNetCore.Http;
using Moq;

namespace tests;

public class SaveFileTests
{
    [Fact]
    public void BuildPathFileSave_shouldReturnPathForImage()
    {
        var saveFiles = new SaveFile();
        var formFile = new FormFile(Stream.Null, 0, 0, "name", "filename");

        var result = saveFiles.BuildPathFileSave(formFile.FileName, "RequestHelp");
        var expectedResult = Path.Combine("shared", "RequestHelp", formFile.FileName);

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task SaveImageFile_ShouldSaveImageCorrectly()
    {
        var fileMock = new Mock<IFormFile>();
        var tempDirectory = Path.Combine(Path.GetTempPath(), "TestImages");
        var saveFiles = new SaveFile();

        Directory.CreateDirectory(tempDirectory);

        var outputPath = Path.Combine(tempDirectory, "arquivo.jpg");

        fileMock
            .Setup(x => x.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>(
                (stream, cancellationToken) =>
                {
                    using var writer = new StreamWriter(stream);
                    writer.Write("Conteúdo do arquivo de teste");
                }
            )
            .Returns(Task.CompletedTask);

        await saveFiles.SaveImageFile(fileMock.Object, outputPath);

        fileMock.Verify(
            x => x.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()),
            Times.Once
        );

        Directory.Delete(tempDirectory, true);
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Infrastructure.FileHandling;

namespace Infrastructure.Tests
{
    public class MP4ProcessManagerTests : IDisposable
    {
        private readonly Guid _testId = Guid.Parse("5e680523-bc9e-47f3-8670-6a87bfac849f");
        private readonly string _baseDirectory;
        private readonly string _inputFile;
        private readonly string _outputFile;
        private MP4ProcessManager _processManager;
        private bool _progressReceived = false;

        

        public MP4ProcessManagerTests()
        {
             string currentDirectory = Directory.GetCurrentDirectory();

             string projectRoot = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", "..", ".."));

             _baseDirectory = Path.Combine(projectRoot,"tests","Live");



             Environment.CurrentDirectory = _baseDirectory;


            _inputFile = Path.Combine(_baseDirectory, "Assets", "Lives", $"{_testId}.m3u8");
            _outputFile = Path.Combine(_baseDirectory, "Assets", "Lives", $"{_testId}.mp4");

            if (File.Exists(_outputFile))
            {
                File.Delete(_outputFile);
            }

            _processManager = new MP4ProcessManager(_testId);
            _processManager.ProgressUpdated += (sender, message) =>
            {
                Console.WriteLine($"Progress: {message}");
                _progressReceived = true;
            };

        }


        [Fact]
        public async Task Run_ShouldStartProcessAndCreateMP4File()
        {
            try
            {
                Assert.True(IsFFmpegInstalled(), "FFmpeg is not installed or not accessible in the system PATH");
                Assert.True(File.Exists(_inputFile), $"The .m3u8 file should exist at {_inputFile}");


                Assert.Equal(_inputFile, Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Lives", $"{_testId}.m3u8"));


                _processManager.Run();

                Assert.False(_processManager.IsDied());

                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(120));
                var completionTask = Task.Run(async () =>
                {
                    while (!_processManager.IsDied() && !File.Exists(_outputFile))
                    {
                        await Task.Delay(1000);
                        Console.WriteLine($"Waiting for MP4 file... Exists: {File.Exists(_outputFile)}");
                    }
                });

                var completedTask = await Task.WhenAny(completionTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    Console.WriteLine("Process timed out. Dumping diagnostic information:");
                    Console.WriteLine($"Input file exists: {File.Exists(_inputFile)}");
                    Console.WriteLine($"Output file exists: {File.Exists(_outputFile)}");
                    Console.WriteLine($"Process is dead: {_processManager.IsDied()}");
                    Console.WriteLine($"Progress received: {_progressReceived}");
                    throw new TimeoutException("The process did not complete within the expected time.");
                }

                Assert.True(_progressReceived, "No progress updates were received from the process");
                Assert.True(File.Exists(_outputFile), $"The MP4 file should have been created at {_outputFile}");

                var fileInfo = new FileInfo(_outputFile);
                Assert.True(fileInfo.Length > 0, "The MP4 file should not be empty.");
            }
            finally
            {
                // Delete the output MP4 file after the test is complete
                
                    try
                    {
                        File.Delete(_outputFile);
                        Console.WriteLine($"Deleted output MP4 file: {_outputFile}");
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Warning: Could not delete output MP4 file. {ex.Message}");
                    }
                
            }
        }

        private static bool IsFFmpegInstalled()
        {
            try
            {
                using (var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = "-version",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    Console.WriteLine($"FFmpeg version info: {output}");
                    return !string.IsNullOrEmpty(output) && output.Contains("ffmpeg version");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking FFmpeg installation: {ex.Message}");
                return false;
            }
        }

        public void Dispose()
        {
            _processManager.StopAsync().Wait();

            // The deletion of the output file is now handled in the test method
        }
    }
}
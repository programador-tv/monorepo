using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.FileHandling
{
    public sealed class MP4ProcessManager
    {
        private Process _process;
        private bool _isStopped = false;
        public Guid Id { get; }
        public event EventHandler<string> ProgressUpdated;

        public MP4ProcessManager(Guid id)
        {
            Id = id;
            


             var baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Lives");
             if (!Directory.Exists(baseDirectory))
             {
                 Directory.CreateDirectory(baseDirectory);
             }


            string inputFile = Path.Combine(baseDirectory, $"{id}.m3u8");
            string outputFile = Path.Combine(baseDirectory, $"{id}.mp4");

            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-i {inputFile} -c copy -bsf:a aac_adtstoasc -progress pipe:1 -stats " +
                            $"-metadata comment=\"Processed file ID: {id}\" {outputFile}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true,
            };

            _process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    ProgressUpdated?.Invoke(this, e.Data);
                }
            };
            _process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    ProgressUpdated?.Invoke(this, $"Error: {e.Data}");
                }
            };
        }

        public void Run()
        {
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            _isStopped = false;
        }

        public bool IsDied()
        {
            return _isStopped || _process == null || _process.HasExited;
        }

        public async Task StopAsync()
        {
            if (_isStopped || _process == null || _process.HasExited)
            {
                return;
            }

            try
            {
                await _process.StandardInput.WriteAsync("q");
                await _process.StandardInput.FlushAsync();

                if (!await WaitForExitAsync(TimeSpan.FromSeconds(5)))
                {
                    _process.Kill(true);
                }
            }
            catch (Exception ex)
            {
                ProgressUpdated?.Invoke(this, $"Error stopping process: {ex.Message}");
            }
            finally
            {
                _process.Dispose();
                _process = null;
                _isStopped = true;
            }
        }

        private async Task<bool> WaitForExitAsync(TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<bool>();
            _process.Exited += (sender, args) => tcs.TrySetResult(true);

            if (_process.HasExited)
            {
                return true;
            }

            using var cts = new CancellationTokenSource(timeout);
            cts.Token.Register(() => tcs.TrySetResult(false));

            return await tcs.Task;
        }
    }
}
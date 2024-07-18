using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.FileHandling;

public sealed class HLSProcessManager
{
    private Process _process;
    public Guid Id;

    public HLSProcessManager(Guid id)
    {
        Id = id;
        var baseDirectory = Directory.GetCurrentDirectory() + "/Assets/Lives/";
        if (!Directory.Exists(baseDirectory))
        {
            Directory.CreateDirectory(baseDirectory);
        }
        string hlsSegmentPath = Path.Combine(baseDirectory, $"{id}-%d.ts");
        string hlsPlaylistPath = Path.Combine(baseDirectory, $"{id}.m3u8");

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",

                Arguments =
                    "-fflags +genpts -i pipe:0 -err_detect ignore_err -vsync cfr"
                    + " -c:v libx264 -r 30 -g 5 -crf 15 -s 1920x1080 -preset ultrafast -tune zerolatency -pix_fmt yuv420p"
                    + " -c:a aac -b:a 128k "
                    + $" -hls_flags append_list+omit_endlist -f hls -hls_playlist_type event -hls_time 3 -hls_list_size 0 -hls_segment_filename {hlsSegmentPath} {hlsPlaylistPath} ",
                RedirectStandardOutput = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };
    }

    public void Run()
    {
        _process.Start();
    }

    public async Task WriteToStandardInput(byte[] input)
    {
        await _process.StandardInput.BaseStream.WriteAsync(input);
    }

    public bool IsDied()
    {
        return _process.HasExited;
    }

    public async Task StopAsync()
    {
        if (!_process.HasExited)
        {
            _process.StandardInput.Close();
            await _process.WaitForExitAsync();
        }
        _process.Dispose();
    }

    public async Task LogOutputAsync()
    {
        string line;
        while (_process.StandardInput.BaseStream.CanWrite)
        {
            line = await _process.StandardError.ReadLineAsync() ?? string.Empty;
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }
            Console.WriteLine("------: " + line);
        }
    }
}

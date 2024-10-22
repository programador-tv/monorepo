﻿using System.Globalization;
using Domain.Utils;

namespace Infrastructure.FileHandling;

public sealed class VideoHandlingSingleton : IVideoHandling
{
    private readonly List<HLSProcessManager> processManagers = [];

    public async Task ProcessChunkAsync(Guid id, byte[] chunk, int tentative)
    {
        var process = processManagers.FirstOrDefault(e => e.Id == id);
        if (process?.IsDied() ?? false)
        {
            await StopAsync(process.Id);
        }
        if (process == null)
        {
            process = new HLSProcessManager(id);
            processManagers.Add(process);
            process.Run();
            _ = process.LogOutputAsync();
        }
        try
        {
            await process.WriteToStandardInput(chunk);
        }
        catch
        {
            if (tentative == 3)
            {
                throw;
            }
            await StopAsync(process.Id);
            await ProcessChunkAsync(id, chunk, ++tentative);
        }
    }

    public async Task StopAsync(Guid id)
    {
        var process = processManagers.FirstOrDefault(e => e.Id == id);
        if (process != null)
        {
            await process.StopAsync();
            processManagers.Remove(process);
        }
    }

    public static string GetM3U8DurationById(Guid id)
    {
        var baseDirectory = Directory.GetCurrentDirectory() + "/Assets/Lives/";
        string m3u8FilePath = Path.Combine(baseDirectory, $"{id}.m3u8");

        double totalDuration = 0;
        try
        {
            using var sr = new StreamReader(m3u8FilePath);

            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("#EXTINF:"))
                {
                    totalDuration += double.Parse(
                        line.Split(':')[1].Split(',')[0],
                        CultureInfo.InvariantCulture
                    );
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("O arquivo não pôde ser lido:");
            Console.WriteLine(e.Message);
        }

        var formatedTime = TimeUtils.FormatTimeToDuration(totalDuration);
        return formatedTime;
    }
}

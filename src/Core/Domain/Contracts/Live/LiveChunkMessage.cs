namespace Domain.Contracts;

public record LiveChunkMessage(string Id, byte[] Chunk);

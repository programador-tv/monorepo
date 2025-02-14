﻿using Domain.Primitives;

namespace Domain.Entities
{
    public sealed class Visualization(
        Guid id,
        Guid liveId,
        Guid perfilId,
        string? IPV4,
        DateTime dataEntrou
    ) : Entity(id)
    {
        public Guid LiveId { get; private set; } = liveId;
        public Guid PerfilId { get; private set; } = perfilId;
        public string? IPV4 { get; private set; } = IPV4;
        public DateTime DataEntrou { get; private set; } = dataEntrou;

        public static Visualization Create(Guid liveId, Guid perfilId, string? ipv4)
        {
            return new Visualization(Guid.NewGuid(), liveId, perfilId, ipv4, DateTime.Now);
        }
    }
}

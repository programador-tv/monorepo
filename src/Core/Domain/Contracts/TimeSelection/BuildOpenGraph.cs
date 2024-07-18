using Domain.Entities;

namespace Domain.Contracts;

public record BuildOpenGraphImage(
    Guid PerfilId,
    string UserName,
    string UserNickName,
    string UserPhoto,
    string TempTitle,
    List<Tag> Tags,
    DateTime StartTime,
    DateTime EndTime,
    string TempoTotal
);

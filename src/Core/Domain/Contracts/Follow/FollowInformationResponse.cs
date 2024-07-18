using System.Globalization;
using Domain.Enumerables;
using Microsoft.AspNetCore.Http;

namespace Domain.Contracts;

public record FollowInformationResponse(int Followers, int Following);

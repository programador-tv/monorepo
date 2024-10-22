﻿using Domain.Entities;
using Domain.Enums;
using Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models;

namespace Platform.Services;

public interface ILiveService
{
    public LiveViewModel BuildLiveViewModels(Live live, Perfil perfil, int views);
    public Task<List<PrivateLiveViewModel>> RenderPrivateLives(
        Perfil perfilOwner,
        Guid perfilLogInId,
        bool isPrivate,
        int pageNumber,
        int pageSize
    );
    public List<LiveSchedulePreviewViewModel> RenderPreviewLiveSchedule(
        Perfil perfilOwner,
        Guid perfilLogInId
    );
}

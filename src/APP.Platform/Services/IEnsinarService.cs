using Domain.Entities;
using Domain.Enums;
using Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models;

namespace Platform.Services;

public interface IEnsinarService
{
    public void AddTagsToRoom(Room Room, List<string> TagsSelected);
    public string GenerateNewRoomCode();
    public void CreateAndAddRoomToContext(Room Room);
    public void RemoveRoomProfileIdFromModelState(ModelStateDictionary ModelState);
    public void SetRoomProfileIdFromUserProfile(Room Room, Perfil UserProfile);

    public List<TimeSelectionForCalendarSectionViewModel> CastTimeSelectionIntoCalendarSectionViewModel(
        List<TimeSelection> timeSeletions
    );

    public List<BadFinishedTimeSelectionForCalendarSectionViewModel> CastTimeSelectionIntoBadFinishedCalendarSectionViewModel(
        List<TimeSelection> timeSeletions
    );
    public void GetTimeSelectionItem(
        Guid timeSelectionId,
        Perfil userProfile,
        string _meetUrl,
        Dictionary<TimeSelection, List<Perfil>> TimeSelectionsCheckedUsers,
        List<TimeSelection> oldTimeSelectionList,
        Dictionary<TimeSelection, List<JoinTimeViewModel>> timeSelectionDictionary
    );
    public void GetTimeSelectionList(
        Perfil userProfile,
        string _meetUrl,
        Dictionary<TimeSelection, List<Perfil>> TimeSelectionsCheckedUsers,
        List<TimeSelection> oldTimeSelectionList,
        Dictionary<TimeSelection, List<JoinTimeViewModel>> timeSelectionDictionary
    );
    public List<TimeSelection> GetTimeSelections(Perfil userProfile);
    public void HandleTimeSelection(
        TimeSelection e,
        Perfil userProfile,
        string _meetUrl,
        Dictionary<TimeSelection, List<Perfil>> TimeSelectionsCheckedUsers,
        List<TimeSelection> oldTimeSelectionList,
        Dictionary<TimeSelection, List<JoinTimeViewModel>> timeSelectionDictionary
    );
    public bool ShouldAddToOldList(TimeSelection e);
    public void UpdateTimeSelection(TimeSelection e, Perfil userProfile, string _meetUrl);
    public List<JoinTimeViewModel> CreateJoinTimeViewModels(TimeSelection e);
    public bool SetActionNeeded(TimeSelection e);
    public void AddToSelectionList(
        TimeSelection e,
        List<JoinTimeViewModel> viewmodels,
        Dictionary<TimeSelection, List<Perfil>> TimeSelectionsCheckedUsers,
        Dictionary<TimeSelection, List<JoinTimeViewModel>> timeSelectionDictionary
    );
    public JoinTimeViewModel CreateJoinTimeViewModel(
        Domain.Entities.Perfil perfil,
        Guid joinTimeId,
        StatusJoinTime statusJoinTime
    );
    public void CheckActionNeedAndUpdateTime(
        KeyValuePair<TimeSelection, List<JoinTimeViewModel>> timeSelectionAndJoinTimes
    );
}

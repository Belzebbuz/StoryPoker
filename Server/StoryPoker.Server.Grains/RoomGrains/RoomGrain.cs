using Microsoft.Extensions.Logging;
using Orleans.Core;
using Orleans.Runtime;
using StoryPoker.Server.Abstractions;
using StoryPoker.Server.Abstractions.Room;
using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Grains.Base;
using StoryPoker.Server.Grains.Constants;

namespace StoryPoker.Server.Grains.RoomGrains;

internal sealed class RoomGrain(
    [PersistentState(stateName: "room-state", storageName: StorageConstants.PersistenceStorage)]
    IStorage<RoomGrainState> storageState,
    ILogger<ObservableGrain<IRoomGrainObserver>> logger) : ObservableGrain<IRoomGrainObserver>(logger), IRoomGrain
{
    public Task<RoomGrainState?> GetAsync()
    {
        RoomGrainState? state = null;
        if (storageState.State.Instantiated)
            state = storageState.State;
        return Task.FromResult(state);
    }

    public async Task<ResponseState> InitStateAsync(InitStateRequest request)
    {
        var stateScreen = storageState.State with { };
        if (storageState.State.Instantiated)
            return ResponseState.Fail("Комната уже создана");
        storageState.State = RoomGrainState.Init(request);
        var response = await SaveStateAsync(stateScreen);
        await NotifyAsync();
        return response;
    }

    public async Task<ResponseState> AddPlayerAsync(AddPlayerRequest request)
    {
        var stateScreen = storageState.State with { };
        var result = storageState.State.AddPlayer(request);
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        await NotifyAsync();
        return response;
    }

    public async Task<ResponseState> AddIssueAsync(AddIssueRequest request)
    {
        var stateScreen = storageState.State with { };
        storageState.State.AddIssue(request);
        var response = await SaveStateAsync(stateScreen);
        await NotifyAsync();
        return response;
    }

    public async Task<ResponseState> RemoveIssueAsync(Guid issueId)
    {
        var stateScreen = storageState.State with { };
        storageState.State.RemoveIssue(issueId);
        var response = await SaveStateAsync(stateScreen);
        await NotifyAsync();
        return response;
    }

    public async Task<ResponseState> SetCurrentIssueAsync(Guid issueId)
    {
        var stateScreen = storageState.State with { };
        var result = storageState.State.SetCurrentIssue(issueId);
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        await NotifyAsync();
        return response;
    }

    public async Task<ResponseState> SetPlayerIssueStoryPointAsync(SetStoryPointRequest request)
    {
        var stateScreen = storageState.State with { };
        var result = storageState.State.SetStoryPoint(request);
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        await NotifyAsync();
        return response;
    }

    public async Task<ResponseState> StartVoteAsync()
    {
        var stateScreen = storageState.State with { };
        var result = storageState.State.StartVote();
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        await NotifyAsync();
        return response;
    }

    public async Task<ResponseState> StopVoteAsync()
    {
        var stateScreen = storageState.State with { };
        var result = storageState.State.StopVote();
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        await NotifyAsync();
        return response;
    }

    private async Task<ResponseState> SaveStateAsync(RoomGrainState stateScreen)
    {
        try
        {
            await storageState.WriteStateAsync();
            return ResponseState.Success();
        }
        catch (Exception e)
        {
            storageState.State = stateScreen;
            return ResponseState.Fail(e.Message);
        }
    }

    private ValueTask NotifyAsync()
    {
        ObserverManager.Notify(ob => ob.RoomStateChangedAsync());
        return ValueTask.CompletedTask;
    }
}

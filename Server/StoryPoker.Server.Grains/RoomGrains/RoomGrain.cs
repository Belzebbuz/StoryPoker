using Microsoft.Extensions.Logging;
using Orleans.Core;
using Orleans.Runtime;
using StoryPoker.Server.Abstractions;
using StoryPoker.Server.Abstractions.Notifications;
using StoryPoker.Server.Abstractions.Room;
using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Grains.Constants;

namespace StoryPoker.Server.Grains.RoomGrains;

internal sealed class RoomGrain(
    [PersistentState(stateName: "room-state", storageName: StorageConstants.PersistenceStorage)]
    IStorage<RoomGrainState> storageState,
    ILogger<RoomGrain> logger) : Grain, IRoomGrain
{
    public Task<RoomGrainState> GetAsync()
    {
        return Task.FromResult(storageState.State);
    }

    public async Task<ResponseState> InitStateAsync(RoomRequest.CreateRoom request)
    {
        var stateScreen = storageState.State with { };
        if (storageState.State.Instantiated)
            return ResponseState.Fail("Комната уже создана");
        storageState.State = RoomGrainState.Init(request);
        var response = await SaveStateAsync(stateScreen);
        logger.LogInformation($"Комната №'{this.GetPrimaryKey()}' -> успешно создана.");
        return response;
    }

    public async Task<ResponseState> AddPlayerAsync(RoomRequest.AddPlayer request)
    {
        var stateScreen = storageState.State with { };
        var result = storageState.State.AddNewPlayer(request);
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        logger.LogInformation($"Комната № '{this.GetPrimaryKey()}' -> добавлен игрок Id: {request.Id}");
        return response;
    }
    public async Task<ResponseState> RemovePlayerAsync(Guid playerId)
    {
        var stateScreen = storageState.State with { };
        storageState.State.RemovePlayer(playerId);
        var response = await SaveStateAsync(stateScreen);
        logger.LogInformation($"Комната № '{this.GetPrimaryKey()}' -> удален игрок Id: {playerId}");
        return response;
    }

    public async Task<ResponseState> AddIssueAsync(RoomRequest.AddIssue request)
    {
        var stateScreen = storageState.State with { };
        storageState.State.AddIssue(request);
        var response = await SaveStateAsync(stateScreen);
        logger.LogInformation($"Комната № '{this.GetPrimaryKey()}' -> добавлена задача.");
        return response;
    }

    public async Task<ResponseState> RemoveIssueAsync(Guid issueId)
    {
        var stateScreen = storageState.State with { };
        var result =  storageState.State.RemoveIssue(issueId);
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        logger.LogInformation($"Комната № '{this.GetPrimaryKey()}' -> удалена задача.");
        return response;
    }

    public async Task<ResponseState> SetCurrentIssueAsync(Guid issueId)
    {
        var stateScreen = storageState.State with { };
        var result = storageState.State.SetCurrentIssue(issueId);
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        logger.LogInformation($"Комната № '{this.GetPrimaryKey()}' -> установлена новая текущая задача Id: {issueId}.");
        return response;
    }

    public async Task<ResponseState> SetPlayerIssueStoryPointAsync(RoomRequest.SetStoryPoint request)
    {
        var stateScreen = storageState.State with { };
        var result = storageState.State.SetStoryPoint(request);
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        logger.LogInformation($"Комната № '{this.GetPrimaryKey()}' -> " +
                              $"игрок Id: {request.PlayerId} поставил оценку текущей задаче.");
        if(storageState.State.Issues[storageState.State.VotingIssueId!.Value].Stage == VotingStage.VoteEnded)
            logger.LogInformation($"Комната № '{this.GetPrimaryKey()}' -> остановлено голосование по текущей задаче.");
        return response;
    }

    public async Task<ResponseState> SetNewSpectatorAsync(Guid playerId)
    {
        var stateScreen = storageState.State with { };
        var result = storageState.State.SetNewSpectator(playerId);
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        logger.LogInformation($"Комната № '{this.GetPrimaryKey()}' -> " +
                              $"игрок Id: {playerId} установлен новым наблюдателем.");
        return response;
    }

    public async Task<ResponseState> SetIssueListOrderAsync(IssueOrder order)
    {
        var stateScreen = storageState.State with { };
        storageState.State.SetIssueOrderBy(order);
        var response = await SaveStateAsync(stateScreen);
        return response;
    }

    public async Task<ResponseState> SetIssueOrderAsync(Guid issueId, int newOrder)
    {
        var stateScreen = storageState.State with { };
        var result = storageState.State.SetIssuesNewOrder(issueId,newOrder);
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        logger.LogInformation($"Комната № '{this.GetPrimaryKey()}' -> Задаче {issueId} установлен новый порядковый номер {newOrder}.");
        return response;
    }

    public async Task<ResponseState> StartVoteAsync()
    {
        var stateScreen = storageState.State with { };
        var result = storageState.State.StartVote();
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        logger.LogInformation($"Комната № '{this.GetPrimaryKey()}' -> началось голосование по текущей задаче.");
        return response;
    }

    public async Task<ResponseState> StopVoteAsync()
    {
        var stateScreen = storageState.State with { };
        var result = storageState.State.StopVote();
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var response = await SaveStateAsync(stateScreen);
        logger.LogInformation($"Комната № '{this.GetPrimaryKey()}' -> остановлено голосование по текущей задаче.");
        return response;
    }

    private async Task<ResponseState> SaveStateAsync(RoomGrainState stateScreen)
    {
        try
        {
            await storageState.WriteStateAsync();
            await NotifyAsync();
            return ResponseState.Success();
        }
        catch (Exception e)
        {
            storageState.State = stateScreen;
            logger.LogError($"Комната Id: '{this.GetPrimaryKey()}' произошла ошибка при сохранении состояния.\n{e}");
            return ResponseState.Fail(e.Message);
        }
    }

    private async Task NotifyAsync()
    {
        var notificator = GrainFactory.GetGrain<IRoomNotificationGrain>(this.GetPrimaryKey());
        await notificator.NotifyAsync(
            new RoomStateChangedNotification(this.GetPrimaryKey(),
            storageState.State.Players.Any()));
    }
}

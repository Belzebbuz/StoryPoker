using Microsoft.Extensions.Logging;
using Orleans.Core;
using Orleans.Runtime;
using StoryPoker.Server.Abstractions;
using StoryPoker.Server.Abstractions.Common;
using StoryPoker.Server.Abstractions.GroupedRoom;
using StoryPoker.Server.Abstractions.GroupedRoom.Commands;
using StoryPoker.Server.Abstractions.GroupedRoom.Models;
using StoryPoker.Server.Abstractions.Metadata.Models;
using StoryPoker.Server.Abstractions.Notifications;
using StoryPoker.Server.Grains.Abstractions;
using StoryPoker.Server.Grains.Constants;
using StoryPoker.Server.Grains.Extensions;
using StoryPoker.Server.Grains.GroupedRoom.Models;

namespace StoryPoker.Server.Grains.GroupedRoom;

public class GroupedRoomGrain(
    [PersistentState(stateName: "grouped-room-state", storageName: StorageConstants.PersistenceStorage)]
    IStorage<InternalGroupedRoom> room,
    ILogger<GroupedRoomGrain> logger) : Grain, IGroupedRoomGrain
{
    public Task<GroupedRoomResponse> GetAsync(Guid playerId)
    {
        var response = room.State.ToResponse(playerId);
        return Task.FromResult(response);
    }

    public async Task<ResponseState> InitStateAsync(CreateRoomRequest request)
    {
        var stateScreen = room.State with { };
        if (room.State.Instantiated)
            return ResponseState.Fail("Комната уже создана");
        room.State = InternalGroupedRoom.Init(request);
        var response = await SaveStateAsync(stateScreen, "Init");
        logger.LogInformation($"Комната №'{this.GetPrimaryKey()}' -> успешно создана.");
        return response;
    }

    public async Task<ResponseState> ExecuteCommandAsync(GroupedRoomCommand command)
    {
        var stateScreen = room.State with { };
        var result = command.Execute(room.State);
        if (result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var saveResult = await SaveStateAsync(stateScreen, command.GetType().Name);
        if (!saveResult.IsSuccess)
            return saveResult;
        foreach (var @event in room.State.PopEvents())
        {
            HandleEvent(@event);
        }

        return ResponseState.Success();
    }

    public Task<GetGroupsResponse> GetGroupsAsync()
    {
        var response = room.State.Groups
            .ToDictionary(x => x.Key, x => x.Value.Name);
        return Task.FromResult(new GetGroupsResponse(response));
    }

    public Task<GetIssueResponse> GetIssueAsync(Guid issueId)
    {
        var issue = room.State.Issues[issueId];
        var response = new GetIssueResponse(
            issue.Name,
            issue.CalculatedGroupPoints.Select(x => x.Key).ToArray());
        return Task.FromResult(response);
    }

    private void HandleEvent(IDomainEvent @event)
    {
        var runnerType = typeof(IRoomEventRunnerGrain<>).MakeGenericType([@event.GetType()]);
        var eventRunner = (IRoomEventRunnerGrain)GrainFactory.GetGrain(runnerType, this.GetPrimaryKey());
        if (eventRunner is not null)
        {
            var _ = eventRunner.RunAsync(@event);
        }
    }

    private async Task<ResponseState> SaveStateAsync(InternalGroupedRoom stateScreen, string source)
    {
        try
        {
            await room.WriteStateAsync();
            logger.LogInformation($"Комната Id:{this.GetPrimaryKey()} состояние изменилось. -> {source}");
            await NotifyChangedAsync();
            return ResponseState.Success();
        }
        catch (Exception e)
        {
            room.State = stateScreen;
            logger.LogError($"Комната Id: '{this.GetPrimaryKey()}' произошла ошибка при сохранении состояния.\n{e}");
            return ResponseState.Fail(e.Message);
        }
    }

    private async Task NotifyChangedAsync()
    {
        var notificator = GrainFactory.GetGrain<IRoomNotificationGrain>(this.GetPrimaryKey());
        var spectatorExist = room.State.Spectator.InRoom;

        await notificator.NotifyAsync(
            new RoomStateChangedNotification(this.GetPrimaryKey(),
                spectatorExist || room.State.Players.Any()));
    }
}

using Microsoft.Extensions.Logging;
using Orleans.Core;
using Orleans.Runtime;
using StoryPoker.Server.Abstractions;
using StoryPoker.Server.Abstractions.Common;
using StoryPoker.Server.Abstractions.DefaultRoom;
using StoryPoker.Server.Abstractions.DefaultRoom.Commands;
using StoryPoker.Server.Abstractions.DefaultRoom.Models;
using StoryPoker.Server.Abstractions.Metadata.Models;
using StoryPoker.Server.Abstractions.Notifications;
using StoryPoker.Server.Grains.Abstractions;
using StoryPoker.Server.Grains.Constants;
using StoryPoker.Server.Grains.DefaultRoom.Models;
using StoryPoker.Server.Grains.Extensions;

namespace StoryPoker.Server.Grains.DefaultRoom;

internal sealed class DefaultRoomGrain(
    [PersistentState(stateName: "room-state", storageName: StorageConstants.PersistenceStorage)]
    IStorage<InternalDefaultRoom> storageState,
    ILogger<DefaultRoomGrain> logger) : Grain, IDefaultRoomGrain
{
    public Task<DefaultRoomStateResponse> GetAsync(Guid playerId)
    {
        var response = storageState.State.ToResponse(playerId);
        return Task.FromResult(response);
    }

    public async Task<ResponseState> InitStateAsync(CreateRoomRequest request)
    {
        var stateScreen = storageState.State with { };
        if (storageState.State.Instantiated)
            return ResponseState.Fail("Комната уже создана");
        storageState.State = InternalDefaultRoom.Init(request);
        var response = await SaveStateAsync(stateScreen, "Init");
        logger.LogInformation($"Комната №'{this.GetPrimaryKey()}' -> успешно создана.");
        return response;
    }

    public async Task<ResponseState> ExecuteCommandAsync(DefaultRoomCommand command)
    {
        var stateScreen = storageState.State with { };
        var result = command.Execute(storageState.State);
        if(result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        var saveResult = await SaveStateAsync(stateScreen, command.GetType().Name);
        if (!saveResult.IsSuccess)
            return saveResult;
        foreach (var @event in storageState.State.PopEvents())
        {
            HandleEvent(@event);
        }
        return ResponseState.Success();
    }

    private void HandleEvent(IDomainEvent @event)
    {
        var runnerType = typeof(IRoomEventRunnerGrain<>).MakeGenericType([@event.GetType()]);
        var eventRunner = (IRoomEventRunnerGrain)GrainFactory.GetGrain(runnerType,this.GetPrimaryKey());
        if (eventRunner is not null)
        {
            var _ = eventRunner.RunAsync(@event);
        }
    }

    private async Task<ResponseState> SaveStateAsync(InternalDefaultRoom stateScreen, string source)
    {
        try
        {
            await storageState.WriteStateAsync();
            logger.LogInformation($"Комната Id:{this.GetPrimaryKey()} состояние изменилось. -> {source}");
            await NotifyChangedAsync();
            return ResponseState.Success();
        }
        catch (Exception e)
        {
            storageState.State = stateScreen;
            logger.LogError($"Комната Id: '{this.GetPrimaryKey()}' произошла ошибка при сохранении состояния.\n{e}");
            return ResponseState.Fail(e.Message);
        }
    }

    private async Task NotifyChangedAsync()
    {
        var notificator = GrainFactory.GetGrain<IRoomNotificationGrain>(this.GetPrimaryKey());
        await notificator.NotifyAsync(
            new RoomStateChangedNotification(this.GetPrimaryKey(),
            storageState.State.Players.Any()));
    }
}

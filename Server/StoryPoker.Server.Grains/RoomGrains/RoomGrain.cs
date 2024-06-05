using System.Text.Json;
using Microsoft.Extensions.Logging;
using Orleans.Core;
using Orleans.Runtime;
using StoryPoker.Server.Abstractions;
using StoryPoker.Server.Abstractions.Notifications;
using StoryPoker.Server.Abstractions.Room;
using StoryPoker.Server.Abstractions.Room.Commands;
using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Abstractions.Room.Models.Enums;
using StoryPoker.Server.Grains.Abstractions;
using StoryPoker.Server.Grains.Constants;
using StoryPoker.Server.Grains.RoomGrains.Models;
using StoryPoker.Server.Grains.RoomGrains.Models.DomainEvents;

namespace StoryPoker.Server.Grains.RoomGrains;

internal sealed class RoomGrain(
    [PersistentState(stateName: "room-state", storageName: StorageConstants.PersistenceStorage)]
    IStorage<InternalRoom> storageState,
    ILogger<RoomGrain> logger,
    IRoomStateResponseFactory responseFactory) : Grain, IRoomGrain
{
    public Task<RoomStateResponse> GetAsync(Guid playerId)
    {
        var response = responseFactory.ToPlayerResponse(playerId, storageState.State);
        return Task.FromResult(response);
    }

    public async Task<ResponseState> InitStateAsync(RoomRequest.CreateRoom request)
    {
        var stateScreen = storageState.State with { };
        if (storageState.State.Instantiated)
            return ResponseState.Fail("Комната уже создана");
        storageState.State = InternalRoom.Init(request);
        var response = await SaveStateAsync(stateScreen, "Init");
        logger.LogInformation($"Комната №'{this.GetPrimaryKey()}' -> успешно создана.");
        return response;
    }

    public async Task<ResponseState> ExecuteCommandAsync(RoomCommand command)
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

    private async Task<ResponseState> SaveStateAsync(InternalRoom stateScreen, string source)
    {
        try
        {
            await storageState.WriteStateAsync();
            logger.LogInformation($"Комната Id:{this.GetPrimaryKey()} состояние изменилось. -> {source}");
            return ResponseState.Success();
        }
        catch (Exception e)
        {
            storageState.State = stateScreen;
            logger.LogError($"Комната Id: '{this.GetPrimaryKey()}' произошла ошибка при сохранении состояния.\n{e}");
            return ResponseState.Fail(e.Message);
        }
        finally
        {
            await NotifyChangedAsync();
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

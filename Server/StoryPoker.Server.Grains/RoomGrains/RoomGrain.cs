using Microsoft.Extensions.Logging;
using Orleans.Core;
using Orleans.Runtime;
using StoryPoker.Server.Abstractions;
using StoryPoker.Server.Abstractions.Notifications;
using StoryPoker.Server.Abstractions.Room;
using StoryPoker.Server.Abstractions.Room.Commands;
using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Grains.Abstractions;
using StoryPoker.Server.Grains.Constants;
using StoryPoker.Server.Grains.RoomGrains.Models;

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
        var response = await SaveStateAsync(stateScreen);
        logger.LogInformation($"Комната №'{this.GetPrimaryKey()}' -> успешно создана.");
        return response;
    }

    public async Task<ResponseState> ExecuteCommandAsync(RoomCommand command)
    {
        var stateScreen = storageState.State with { };
        var result = command.Execute(storageState.State);
        if(result.IsError)
            return ResponseState.Fail(result.FirstError.Description);
        return await SaveStateAsync(stateScreen);
    }

    private async Task<ResponseState> SaveStateAsync(InternalRoom stateScreen)
    {
        try
        {
            await storageState.WriteStateAsync();
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
            await NotifyAsync();
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

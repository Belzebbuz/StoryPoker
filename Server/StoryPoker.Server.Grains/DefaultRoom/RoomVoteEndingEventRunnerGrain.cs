using Orleans.Concurrency;
using StoryPoker.Server.Abstractions.DefaultRoom;
using StoryPoker.Server.Abstractions.DefaultRoom.Commands;
using StoryPoker.Server.Abstractions.Notifications;
using StoryPoker.Server.Grains.Abstractions;
using StoryPoker.Server.Grains.DefaultRoom.Models.DomainEvents;

namespace StoryPoker.Server.Grains.DefaultRoom;

[StatelessWorker]
public class RoomVoteEndingEventRunnerGrain : Grain, IRoomEventRunnerGrain<VoteEndingTimerEvent>
{
    private short _timeLeft = 5;
    private IDisposable? _timer;
    public Task RunAsync(VoteEndingTimerEvent @event)
    {
        _timer = RegisterTimer(async _ =>
        {
            await NotifyTimerAsync(_timeLeft);
            _timeLeft--;
            if (_timeLeft < 0)
            {
                var roomGrain = GrainFactory.GetGrain<IDefaultRoomGrain>(this.GetPrimaryKey());
                await roomGrain.ExecuteCommandAsync(new ChangeVotingStageCommand(VoteStageChangeType.Stop));
                _timer?.Dispose();
                _timeLeft = 5;
            }

        },null,TimeSpan.Zero, TimeSpan.FromSeconds(1));
        return Task.CompletedTask;
    }

    Task IRoomEventRunnerGrain.RunAsync(IDomainEvent @event)
    {
        return @event is not VoteEndingTimerEvent value ? Task.CompletedTask : RunAsync(value);
    }
    private async Task NotifyTimerAsync(short secondsLeft)
    {
        var notificator = GrainFactory.GetGrain<IRoomNotificationGrain>(this.GetPrimaryKey());
        await notificator.NotifyAsync(
            new RoomVoteEndingTimerNotification(this.GetPrimaryKey(),secondsLeft));
    }
}

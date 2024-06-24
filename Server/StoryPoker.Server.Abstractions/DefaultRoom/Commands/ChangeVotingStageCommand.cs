using ErrorOr;
using StoryPoker.Server.Abstractions.DefaultRoom.State;

namespace StoryPoker.Server.Abstractions.DefaultRoom.Commands;

[GenerateSerializer,Immutable]
public sealed record ChangeVotingStageCommand(
    [property: Id(0)] VoteStageChangeType Stage) : DefaultRoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        return Stage switch
        {
            VoteStageChangeType.Start => roomState.StartVote(),
            VoteStageChangeType.Stop => roomState.StopVote(),
            VoteStageChangeType.StartEndTimer => roomState.SetEndingTimerVote(),
            _ => Error.Failure("Команда не поддерживается.")
        };
    }
}

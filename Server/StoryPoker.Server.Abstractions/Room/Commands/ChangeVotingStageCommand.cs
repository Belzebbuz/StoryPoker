using ErrorOr;
using StoryPoker.Server.Abstractions.Room.StateAbstractions;

namespace StoryPoker.Server.Abstractions.Room.Commands;

[GenerateSerializer,Immutable]
public sealed record ChangeVotingStageCommand(
    [property: Id(0)] VoteStageChangeType Stage) : RoomCommand
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

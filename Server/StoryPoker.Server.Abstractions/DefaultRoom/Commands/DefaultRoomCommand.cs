using ErrorOr;
using StoryPoker.Server.Abstractions.Common;
using IRoomState = StoryPoker.Server.Abstractions.DefaultRoom.State.IRoomState;

namespace StoryPoker.Server.Abstractions.DefaultRoom.Commands;

[GenerateSerializer,Immutable]
public abstract record DefaultRoomCommand : IRoomCommand
{
    public abstract ErrorOr<Success> Execute(IRoomState roomState);
}

using ErrorOr;
using StoryPoker.Server.Abstractions.Room.StateAbstractions;

namespace StoryPoker.Server.Abstractions.Room.Commands;

[GenerateSerializer,Immutable]
public abstract record RoomCommand
{
    public abstract ErrorOr<Success> Execute(IRoomState roomState);
}

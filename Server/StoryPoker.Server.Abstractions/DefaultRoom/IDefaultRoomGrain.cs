using StoryPoker.Server.Abstractions.Common;
using StoryPoker.Server.Abstractions.DefaultRoom.Commands;
using StoryPoker.Server.Abstractions.DefaultRoom.Models;

namespace StoryPoker.Server.Abstractions.DefaultRoom;

public interface IDefaultRoomGrain : IRoomGrain<DefaultRoomCommand, DefaultRoomStateResponse>
{

}

using System.Collections.Concurrent;
using StoryPoker.Client.Web.Api.Abstractions.Forms;

namespace StoryPoker.Client.Web.Api.Infrastructure.Forms;

internal static class InMemoryFormCollection
{
    public static IDictionary<string, IEnumerable<InputBase>> FormGroups => _formGroups;
    private static readonly ConcurrentDictionary<string, IEnumerable<InputBase>> _formGroups = Create();

    private static ConcurrentDictionary<string, IEnumerable<InputBase>> Create()
    {
        var result = new ConcurrentDictionary<string, IEnumerable<InputBase>>();
        result.TryAdd(FormKeys.CreatePokerRoom, GetCreatePokerRoomForm());
        result.TryAdd(FormKeys.AddDefaultPlayer, GetAddPlayerForm());
        result.TryAdd(FormKeys.AddGroupedPlayer, GetAddGroupedPlayerForm());
        result.TryAdd(FormKeys.AddDefaultIssue, GetAddIssueForm());
        result.TryAdd(FormKeys.AddGroups, GetAddGroupsForm());
        result.TryAdd(FormKeys.AddGroupedIssue, GetAddGroupedIssueForm());
        result.TryAdd(FormKeys.UpdateGroupedIssue, GetUpdateGroupedIssueForm());
        return result;
    }

    private static IEnumerable<InputBase> GetUpdateGroupedIssueForm()
    {
        var title = new UpdateGroupedIssueTitleInputForm()
        {
            Key = "title",
            ControlType = "textbox",
            Label = "Заголовок",
            Required = true,
            Order = 1,
            Type = "text",
        };
        var groupNames = new UpdateIssueGroupsInputForm()
        {
            Key = "groupNames",
            ControlType = "multiselect",
            Label = "Команды",
            Required = true,
            Order = 1,
            Type = "text",
        };
        return [title, groupNames];
    }

    private static IEnumerable<InputBase> GetAddGroupedIssueForm()
    {
        var title = new DefaultInputForm
        {
            Key = "title",
            ControlType = "textbox",
            Label = "Заголовок",
            Required = true,
            Order = 1,
            Type = "text",
        };
        var groupNames = new MultiSelectGroupInputForm
        {
            Key = "groupNames",
            ControlType = "multiselect",
            Label = "Команды",
            Required = true,
            Order = 1,
            Type = "text",
        };
        return [title, groupNames];
    }

    private static IEnumerable<InputBase> GetAddGroupsForm()
    {
        var groupNames = new DefaultInputForm
        {
            Key = "groupNames",
            ControlType = "multitext",
            Label = "Команды",
            Required = true,
            Order = 1,
            Type = "text",
        };
        return [groupNames];
    }

    private static IEnumerable<InputBase> GetAddGroupedPlayerForm()
    {
        var playerNameInput = new PlayerNameInputForm
        {
            Key = "playerName",
            ControlType = "textbox",
            Label = "Имя игрока",
            Required = true,
            Order = 1,
            Type = "text"
        };
        var groupNameInput = new SelectGroupInputForm()
        {
            Key = InputKeys.GroupName,
            ControlType = "dropdown",
            Label = "Команда",
            Required = true,
            Order = 1,
            Type = "text"
        };
        var result = new InputBase[] { playerNameInput, groupNameInput };
        return result;
    }

    private static IEnumerable<InputBase> GetAddIssueForm()
    {
        var playerNameInput = new DefaultInputForm
        {
            Key = "title",
            ControlType = "textbox",
            Label = "Заголовок",
            Required = true,
            Order = 1,
            Type = "text"
        };
        var result = new InputBase[] { playerNameInput };
        return result;
    }

    private static IEnumerable<InputBase> GetAddPlayerForm()
    {
        var playerNameInput = new PlayerNameInputForm
        {
            Key = "playerName",
            ControlType = "textbox",
            Label = "Имя игрока",
            Required = true,
            Order = 1,
            Type = "text"
        };
        var result = new InputBase[] { playerNameInput };
        return result;
    }

    private static IEnumerable<InputBase> GetCreatePokerRoomForm()
    {
        var roomNameInput = new RoomNameInputForm()
        {
            Key = "roomName",
            ControlType = "textbox",
            Label = "Название комнаты",
            Required = true,
            Order = 1,
            Type = "text",
        };
        var playerNameInput = new PlayerNameInputForm
        {
            Key = "playerName",
            ControlType = "textbox",
            Label = "Имя игрока",
            Required = true,
            Order = 2,
            Type = "text",
        };
        var roomType = new DefaultInputForm
        {
            Key = "roomType",
            ControlType = "dropdown",
            Label = "Тип комнаты",
            Required = true,
            Order = 3,
            Type = "text",
            Options = new Dictionary<string, OptionValue>()
            {
                { "default", new("Обычная", []) },
                {
                    "grouped", new("Командная", [
                        new DefaultInputForm
                        {
                            Key = "groupNames",
                            ControlType = "multitext",
                            Label = "Команды",
                            Required = true,
                            Order = 4,
                            Type = "text",
                        }
                    ])
                },
            },
        };
        var result = new InputBase[] { roomNameInput, playerNameInput, roomType };
        return result;
    }
}

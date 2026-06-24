using Content.Client.UserInterface.Controls;
using Content.Shared.Administration;

namespace Content.Client.Administration.UI.Tabs.PlayerTab;

public record PlayerListData(PlayerInfo Info, string FilteringString) : ListData;

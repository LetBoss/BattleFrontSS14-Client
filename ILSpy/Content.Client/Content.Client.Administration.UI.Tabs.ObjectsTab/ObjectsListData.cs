using Content.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.Tabs.ObjectsTab;

public record ObjectsListData((string Name, NetEntity Entity) Info, string FilteringString, Color BackgroundColor) : ListData;

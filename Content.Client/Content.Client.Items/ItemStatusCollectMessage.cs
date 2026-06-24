using System.Collections.Generic;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Items;

public sealed class ItemStatusCollectMessage : EntityEventArgs
{
	public List<Control> Controls = new List<Control>();
}

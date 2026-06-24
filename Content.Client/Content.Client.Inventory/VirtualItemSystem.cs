using System;
using Content.Client.Hands.UI;
using Content.Client.Items;
using Content.Shared.Inventory.VirtualItem;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Inventory;

public sealed class VirtualItemSystem : SharedVirtualItemSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).Subs.ItemStatus<VirtualItemComponent>((Func<Entity<VirtualItemComponent>, Control?>)((Entity<VirtualItemComponent> _) => (Control?)(object)new HandVirtualItemStatus()));
	}
}

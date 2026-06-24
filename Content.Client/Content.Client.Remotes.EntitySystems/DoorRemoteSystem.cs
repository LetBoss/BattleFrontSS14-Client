using System;
using Content.Client.Items;
using Content.Client.Remote.UI;
using Content.Shared.Remotes.Components;
using Content.Shared.Remotes.EntitySystems;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Remotes.EntitySystems;

public sealed class DoorRemoteSystem : SharedDoorRemoteSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).Subs.ItemStatus<DoorRemoteComponent>((Func<Entity<DoorRemoteComponent>, Control?>)((Entity<DoorRemoteComponent> ent) => (Control?)(object)new DoorRemoteStatusControl(ent)));
	}
}

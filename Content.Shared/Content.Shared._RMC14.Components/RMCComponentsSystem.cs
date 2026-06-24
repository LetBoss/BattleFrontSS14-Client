using System;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Components;

public sealed class RMCComponentsSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RemoveComponentsComponent, ComponentInit>((EntityEventRefHandler<RemoveComponentsComponent, ComponentInit>)OnRemoveComponentsInit, (Type[])null, (Type[])null);
	}

	private void OnRemoveComponentsInit(Entity<RemoveComponentsComponent> ent, ref ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.EntityManager.RemoveComponents(Entity<RemoveComponentsComponent>.op_Implicit(ent), ent.Comp.Components);
	}
}

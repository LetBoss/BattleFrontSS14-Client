using System;
using Content.Shared._CIV14merka.Rangefinder;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._CIV14merka.Rangefinder;

public sealed class CivRangefinderSystem : SharedCivRangefinderSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CivRangefinderComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<CivRangefinderComponent, AfterAutoHandleStateEvent>)OnRangefinderState, (Type[])null, (Type[])null);
	}

	private void OnRangefinderState(Entity<CivRangefinderComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UserInterfaceComponent val = default(UserInterfaceComponent);
		if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<CivRangefinderComponent>.op_Implicit(ent), ref val))
		{
			return;
		}
		foreach (BoundUserInterface value in val.ClientOpenInterfaces.Values)
		{
			if (value is CivRangefinderBui civRangefinderBui)
			{
				civRangefinderBui.Refresh();
			}
		}
	}
}

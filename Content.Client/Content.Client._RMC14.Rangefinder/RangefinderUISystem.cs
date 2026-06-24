using System;
using Content.Shared._RMC14.Rangefinder;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Rangefinder;

public sealed class RangefinderUISystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RangefinderComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<RangefinderComponent, AfterAutoHandleStateEvent>)OnRangefinderState, (Type[])null, (Type[])null);
	}

	private void OnRangefinderState(Entity<RangefinderComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<RangefinderComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is RangefinderBui rangefinderBui)
				{
					rangefinderBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"RangefinderBui"}:\n{value}");
		}
	}
}

using System;
using Content.Shared._RMC14.Intel;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Intel;

public sealed class IntelUISystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<ViewIntelObjectivesComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<ViewIntelObjectivesComponent, AfterAutoHandleStateEvent>)OnViewIntelObjectivesAfterState, (Type[])null, (Type[])null);
	}

	private void OnViewIntelObjectivesAfterState(Entity<ViewIntelObjectivesComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<ViewIntelObjectivesComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is ViewIntelObjectivesBui viewIntelObjectivesBui)
				{
					viewIntelObjectivesBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"ViewIntelObjectivesBui"}\n{value}");
		}
	}
}

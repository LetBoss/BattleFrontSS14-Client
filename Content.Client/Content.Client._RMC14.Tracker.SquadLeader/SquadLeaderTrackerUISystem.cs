using System;
using Content.Shared._RMC14.Tracker.SquadLeader;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Tracker.SquadLeader;

public sealed class SquadLeaderTrackerUISystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SquadLeaderTrackerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<SquadLeaderTrackerComponent, AfterAutoHandleStateEvent>)OnOverwatchAfterState, (Type[])null, (Type[])null);
	}

	private void OnOverwatchAfterState(Entity<SquadLeaderTrackerComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<SquadLeaderTrackerComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is SquadInfoBui squadInfoBui)
				{
					squadInfoBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"SquadInfoBui"}\n{value}");
		}
	}
}

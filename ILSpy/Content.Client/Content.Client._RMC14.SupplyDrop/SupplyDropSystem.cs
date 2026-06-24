using System;
using Content.Client._RMC14.Overwatch;
using Content.Shared._RMC14.SupplyDrop;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.SupplyDrop;

public sealed class SupplyDropSystem : SharedSupplyDropSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SupplyDropComputerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<SupplyDropComputerComponent, AfterAutoHandleStateEvent>)OnSupplyDropComputerState, (Type[])null, (Type[])null);
	}

	private void OnSupplyDropComputerState(Entity<SupplyDropComputerComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<SupplyDropComputerComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is SupplyDropComputerBui supplyDropComputerBui)
				{
					supplyDropComputerBui.Refresh();
				}
				if (value2 is OverwatchConsoleBui overwatchConsoleBui)
				{
					overwatchConsoleBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"SupplyDropComputerBui"}\n{value}");
		}
	}
}

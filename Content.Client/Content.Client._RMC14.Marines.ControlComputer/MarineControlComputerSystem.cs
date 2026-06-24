using System;
using Content.Shared._RMC14.Marines.ControlComputer;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Marines.ControlComputer;

public sealed class MarineControlComputerSystem : SharedMarineControlComputerSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MarineControlComputerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<MarineControlComputerComponent, AfterAutoHandleStateEvent>)OnComputerState, (Type[])null, (Type[])null);
	}

	private void OnComputerState(Entity<MarineControlComputerComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<MarineControlComputerComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is MarineControlComputerBui marineControlComputerBui)
				{
					marineControlComputerBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"MarineControlComputerBui"}:\n{value}");
		}
	}
}

using System;
using Content.Shared._RMC14.OrbitalCannon;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.OrbitalCannon;

public sealed class OrbitalCannonUISystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<OrbitalCannonComputerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<OrbitalCannonComputerComponent, AfterAutoHandleStateEvent>)OnState, (Type[])null, (Type[])null);
	}

	private void OnState(Entity<OrbitalCannonComputerComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<OrbitalCannonComputerComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is OrbitalCannonComputerBui orbitalCannonComputerBui)
				{
					orbitalCannonComputerBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"OrbitalCannonComputerBui"}\n{value}");
		}
	}
}

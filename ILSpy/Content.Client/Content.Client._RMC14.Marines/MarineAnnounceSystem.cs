using System;
using Content.Client._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.Announce;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Marines;

public sealed class MarineAnnounceSystem : SharedMarineAnnounceSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MarineCommunicationsComputerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<MarineCommunicationsComputerComponent, AfterAutoHandleStateEvent>)OnCommunicationsComputerState, (Type[])null, (Type[])null);
	}

	private void OnCommunicationsComputerState(Entity<MarineCommunicationsComputerComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<MarineCommunicationsComputerComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is MarineCommunicationsComputerBui marineCommunicationsComputerBui)
				{
					marineCommunicationsComputerBui.OnStateUpdate();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"MarineCommunicationsComputerBui"}\n{value}");
		}
	}
}

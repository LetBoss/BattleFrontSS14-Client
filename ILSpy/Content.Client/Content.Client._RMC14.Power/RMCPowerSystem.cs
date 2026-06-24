using System;
using Content.Client.Power.Components;
using Content.Shared._RMC14.Power;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Power;

public sealed class RMCPowerSystem : SharedRMCPowerSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCApcComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<RMCApcComponent, AfterAutoHandleStateEvent>)OnApcState, (Type[])null, (Type[])null);
	}

	public override bool IsPowered(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ApcPowerReceiverComponent apcPowerReceiverComponent = default(ApcPowerReceiverComponent);
		if (((EntitySystem)this).TryComp<ApcPowerReceiverComponent>(ent, ref apcPowerReceiverComponent))
		{
			return apcPowerReceiverComponent.Powered;
		}
		return false;
	}

	private void OnApcState(Entity<RMCApcComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<RMCApcComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is RMCApcBui rMCApcBui)
				{
					rMCApcBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"RMCApcBui"}\n{value}");
		}
	}
}

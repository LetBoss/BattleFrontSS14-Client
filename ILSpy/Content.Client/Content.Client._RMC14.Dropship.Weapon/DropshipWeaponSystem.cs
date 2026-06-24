using System;
using Content.Shared._RMC14.Dropship.Weapon;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Dropship.Weapon;

public sealed class DropshipWeaponSystem : SharedDropshipWeaponSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DropshipTerminalWeaponsComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<DropshipTerminalWeaponsComponent, AfterAutoHandleStateEvent>)OnWeaponsState, (Type[])null, (Type[])null);
	}

	private void OnWeaponsState(Entity<DropshipTerminalWeaponsComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshWeaponsUI(ent);
	}

	protected override void RefreshWeaponsUI(Entity<DropshipTerminalWeaponsComponent> terminal)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			base.RefreshWeaponsUI(terminal);
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<DropshipTerminalWeaponsComponent>.op_Implicit(terminal), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is DropshipWeaponsBui dropshipWeaponsBui)
				{
					dropshipWeaponsBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"DropshipWeaponsBui"}:\n{value}");
		}
	}
}

using System;
using Content.Client._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.TacticalMap;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._RMC14.TacticalMap;

public sealed class TacticalMapSystem : SharedTacticalMapSystem
{
	[Dependency]
	private IPlayerManager _player;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TacticalMapUserComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<TacticalMapUserComponent, AfterAutoHandleStateEvent>)OnUserState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TacticalMapComputerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<TacticalMapComputerComponent, AfterAutoHandleStateEvent>)OnComputerState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TacticalMapLinesComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<TacticalMapLinesComponent, AfterAutoHandleStateEvent>)OnLinesState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TacticalMapLabelsComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<TacticalMapLabelsComponent, AfterAutoHandleStateEvent>)OnLabelsState, (Type[])null, (Type[])null);
	}

	private void RefreshUser(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(ent, ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is TacticalMapUserBui tacticalMapUserBui)
				{
					tacticalMapUserBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"TacticalMapUserBui"}\n{value}");
		}
	}

	private void RefreshComputer(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(ent, ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is TacticalMapComputerBui tacticalMapComputerBui)
				{
					tacticalMapComputerBui.Refresh();
				}
				else if (value2 is DropshipWeaponsBui dropshipWeaponsBui)
				{
					dropshipWeaponsBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"TacticalMapComputerBui"}\n{value}");
		}
	}

	private void OnUserState(Entity<TacticalMapUserComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid val = Entity<TacticalMapUserComponent>.op_Implicit(ent);
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == val)
		{
			RefreshUser(Entity<TacticalMapUserComponent>.op_Implicit(ent));
		}
	}

	private void OnComputerState(Entity<TacticalMapComputerComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		RefreshComputer(Entity<TacticalMapComputerComponent>.op_Implicit(ent));
	}

	private void OnLinesState(Entity<TacticalMapLinesComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<TacticalMapUserComponent>(Entity<TacticalMapLinesComponent>.op_Implicit(ent)))
		{
			RefreshUser(Entity<TacticalMapLinesComponent>.op_Implicit(ent));
		}
		if (((EntitySystem)this).HasComp<TacticalMapComputerComponent>(Entity<TacticalMapLinesComponent>.op_Implicit(ent)))
		{
			RefreshComputer(Entity<TacticalMapLinesComponent>.op_Implicit(ent));
		}
	}

	private void OnLabelsState(Entity<TacticalMapLabelsComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<TacticalMapUserComponent>(Entity<TacticalMapLabelsComponent>.op_Implicit(ent)))
		{
			RefreshUser(Entity<TacticalMapLabelsComponent>.op_Implicit(ent));
		}
		if (((EntitySystem)this).HasComp<TacticalMapComputerComponent>(Entity<TacticalMapLabelsComponent>.op_Implicit(ent)))
		{
			RefreshComputer(Entity<TacticalMapLabelsComponent>.op_Implicit(ent));
		}
	}
}

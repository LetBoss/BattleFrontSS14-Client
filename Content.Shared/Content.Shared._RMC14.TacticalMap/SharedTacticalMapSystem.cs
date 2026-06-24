using System;
using System.Collections.Generic;
using Content.Shared._RMC14.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.TacticalMap;

public abstract class SharedTacticalMapSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	public int LineLimit { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<TacticalMapUserComponent, OpenTacticalMapActionEvent>((EntityEventRefHandler<TacticalMapUserComponent, OpenTacticalMapActionEvent>)OnUserOpenAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TacticalMapUserComponent, OpenTacMapAlertEvent>((EntityEventRefHandler<TacticalMapUserComponent, OpenTacMapAlertEvent>)OnUserOpenAlert, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCTacticalMapLineLimit, (Action<int>)delegate(int v)
		{
			LineLimit = v;
		}, true);
	}

	private void OnUserOpenAction(Entity<TacticalMapUserComponent> ent, ref OpenTacticalMapActionEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetTacticalMap(out Entity<TacticalMapComponent> map))
		{
			UpdateUserData(ent, Entity<TacticalMapComponent>.op_Implicit(map));
		}
		ToggleMapUI(ent);
	}

	private void OnUserOpenAlert(Entity<TacticalMapUserComponent> ent, ref OpenTacMapAlertEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetTacticalMap(out Entity<TacticalMapComponent> map))
		{
			UpdateUserData(ent, Entity<TacticalMapComponent>.op_Implicit(map));
		}
		ToggleMapUI(ent);
	}

	public bool TryGetTacticalMap(out Entity<TacticalMapComponent> map)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = default(EntityUid);
		TacticalMapComponent mapComp = default(TacticalMapComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<TacticalMapComponent>().MoveNext(ref uid, ref mapComp))
		{
			map = Entity<TacticalMapComponent>.op_Implicit((uid, mapComp));
			return true;
		}
		map = default(Entity<TacticalMapComponent>);
		return false;
	}

	protected void UpdateMapData(Entity<TacticalMapComputerComponent> computer)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetTacticalMap(out Entity<TacticalMapComponent> map))
		{
			UpdateMapData(computer, Entity<TacticalMapComponent>.op_Implicit(map));
		}
	}

	protected virtual void UpdateMapData(Entity<TacticalMapComputerComponent> computer, TacticalMapComponent map)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		TacticalMapIncludeXenosEvent ev = default(TacticalMapIncludeXenosEvent);
		((EntitySystem)this).RaiseLocalEvent<TacticalMapIncludeXenosEvent>(ref ev);
		if (ev.Include)
		{
			computer.Comp.Blips = new Dictionary<int, TacticalMapBlip>(map.MarineBlips);
			foreach (KeyValuePair<int, TacticalMapBlip> blip in map.XenoBlips)
			{
				computer.Comp.Blips.TryAdd(blip.Key, blip.Value);
			}
		}
		else
		{
			computer.Comp.Blips = map.MarineBlips;
		}
		((EntitySystem)this).Dirty<TacticalMapComputerComponent>(computer, (MetaDataComponent)null);
		TacticalMapLinesComponent lines = ((EntitySystem)this).EnsureComp<TacticalMapLinesComponent>(Entity<TacticalMapComputerComponent>.op_Implicit(computer));
		lines.MarineLines = map.MarineLines;
		((EntitySystem)this).Dirty(Entity<TacticalMapComputerComponent>.op_Implicit(computer), (IComponent)(object)lines, (MetaDataComponent)null);
	}

	public virtual void OpenComputerMap(Entity<TacticalMapComputerComponent?> computer, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<TacticalMapComputerComponent>(Entity<TacticalMapComputerComponent>.op_Implicit(computer), ref computer.Comp, false))
		{
			_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(computer.Owner), (Enum)TacticalMapComputerUi.Key, user, false);
			UpdateMapData(Entity<TacticalMapComputerComponent>.op_Implicit((Entity<TacticalMapComputerComponent>.op_Implicit(computer), computer.Comp)));
		}
	}

	public virtual void UpdateUserData(Entity<TacticalMapUserComponent> user, TacticalMapComponent map)
	{
	}

	private void ToggleMapUI(Entity<TacticalMapUserComponent> user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (_ui.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(user.Owner), (Enum)TacticalMapUserUi.Key, Entity<TacticalMapUserComponent>.op_Implicit(user)))
		{
			_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(user.Owner), (Enum)TacticalMapUserUi.Key, (EntityUid?)Entity<TacticalMapUserComponent>.op_Implicit(user), false);
		}
		else
		{
			_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(user.Owner), (Enum)TacticalMapUserUi.Key, Entity<TacticalMapUserComponent>.op_Implicit(user), false);
		}
	}
}

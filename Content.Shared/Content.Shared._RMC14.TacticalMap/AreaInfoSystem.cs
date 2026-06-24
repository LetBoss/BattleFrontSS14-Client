using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CCVar;
using Content.Shared.Alert;
using Content.Shared.Coordinates;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.TacticalMap;

public sealed class AreaInfoSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private InventorySystem _inv;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private SharedTransformSystem _transform;

	private readonly Queue<Entity<AreaInfoComponent>> _marineAlertCopyQueue = new Queue<Entity<AreaInfoComponent>>();

	private TimeSpan _maxProcessTime;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GrantAreaInfoComponent, GotEquippedEvent>((EntityEventRefHandler<GrantAreaInfoComponent, GotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrantAreaInfoComponent, GotUnequippedEvent>((EntityEventRefHandler<GrantAreaInfoComponent, GotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AreaInfoComponent, MapInitEvent>((EntityEventRefHandler<AreaInfoComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AreaInfoComponent, ComponentRemove>((EntityEventRefHandler<AreaInfoComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AreaInfoComponent, MoveEvent>((EntityEventRefHandler<AreaInfoComponent, MoveEvent>)OnMoveEvent, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCMaxTacmapAlertProcessTimeMilliseconds, (Action<float>)delegate(float v)
		{
			_maxProcessTime = TimeSpan.FromMilliseconds(v);
		}, true);
	}

	private void OnGotEquipped(Entity<GrantAreaInfoComponent> ent, ref GotEquippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (ent.Comp.Slots & args.SlotFlags) != SlotFlags.NONE)
		{
			((EntitySystem)this).EnsureComp<AreaInfoComponent>(args.Equipee);
		}
	}

	private void OnGotUnequipped(Entity<GrantAreaInfoComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (ent.Comp.Slots & args.SlotFlags) != SlotFlags.NONE && !_inv.TryGetInventoryEntity<GrantAreaInfoComponent>(Entity<InventoryComponent>.op_Implicit(args.Equipee), out Entity<GrantAreaInfoComponent> _))
		{
			((EntitySystem)this).RemCompDeferred<AreaInfoComponent>(args.Equipee);
		}
	}

	private void OnMapInit(Entity<AreaInfoComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		(string areaName, short ceilingLevel, string restrictions) areaInfo = GetAreaInfo(Entity<AreaInfoComponent>.op_Implicit(ent));
		string areaName = areaInfo.areaName;
		short ceilingLevel = areaInfo.ceilingLevel;
		string restrictions = areaInfo.restrictions;
		AlertsSystem alerts = _alerts;
		EntityUid euid = Entity<AreaInfoComponent>.op_Implicit(ent);
		ProtoId<AlertPrototype> alert = ent.Comp.Alert;
		short? severity = ceilingLevel;
		string dynamicMessage = base.Loc.GetString("rmc-area-info", new(string, object)[3]
		{
			("area", areaName),
			("ceilingLevel", ceilingLevel),
			("restrictions", restrictions)
		});
		alerts.ShowAlert(euid, alert, severity, null, autoRemove: false, showCooldown: true, dynamicMessage);
	}

	private void OnRemove(Entity<AreaInfoComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ClearAlert(Entity<AreaInfoComponent>.op_Implicit(ent), ent.Comp.Alert);
	}

	private void OnMoveEvent(Entity<AreaInfoComponent> ent, ref MoveEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			(string areaName, short ceilingLevel, string restrictions) areaInfo = GetAreaInfo(Entity<AreaInfoComponent>.op_Implicit(ent));
			string areaName = areaInfo.areaName;
			short ceilingLevel = areaInfo.ceilingLevel;
			string restrictions = areaInfo.restrictions;
			AlertsSystem alerts = _alerts;
			EntityUid euid = Entity<AreaInfoComponent>.op_Implicit(ent);
			ProtoId<AlertPrototype> alert = ent.Comp.Alert;
			short? severity = ceilingLevel;
			string dynamicMessage = base.Loc.GetString("rmc-area-info", new(string, object)[3]
			{
				("area", areaName),
				("ceilingLevel", ceilingLevel),
				("restrictions", restrictions)
			});
			alerts.ShowAlert(euid, alert, severity, null, autoRemove: false, showCooldown: true, dynamicMessage);
		}
	}

	private (string areaName, short ceilingLevel, string restrictions) GetAreaInfo(EntityUid ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = ent.ToCoordinates();
		if (!_area.TryGetArea(coordinates, out Entity<AreaComponent>? area, out EntityPrototype areaProto))
		{
			return (areaName: base.Loc.GetString("rmc-tacmap-alert-no-area"), ceilingLevel: 0, restrictions: string.Empty);
		}
		short ceilingLevel = 0;
		short severityToUse = 0;
		bool hasHiveCoreProtection = IsProtectedByRoofing(coordinates, (Entity<RoofingEntityComponent> r) => !r.Comp.CanOrbitalBombard && r.Comp.Range > 10f);
		bool hasPylonProtection = IsProtectedByRoofing(coordinates, (Entity<RoofingEntityComponent> r) => r.Comp.CanOrbitalBombard && !r.Comp.CanCAS && r.Comp.Range < 10f);
		if (!_area.CanOrbitalBombard(coordinates, out var _))
		{
			ceilingLevel = 4;
			severityToUse = (short)(hasHiveCoreProtection ? 7 : 5);
		}
		else if (!_area.CanCAS(coordinates))
		{
			ceilingLevel = 3;
			severityToUse = (short)(hasPylonProtection ? 6 : 4);
		}
		else if (!_area.CanSupplyDrop(_transform.ToMapCoordinates(coordinates, true)) || !_area.CanMortarFire(coordinates))
		{
			ceilingLevel = 2;
			severityToUse = 3;
		}
		else if (!_area.CanMortarPlacement(coordinates) || !_area.CanLase(coordinates) || !_area.CanMedevac(coordinates) || !_area.CanParadrop(coordinates))
		{
			ceilingLevel = 1;
			severityToUse = 2;
		}
		else
		{
			ceilingLevel = 0;
			severityToUse = 1;
		}
		List<string> allowedActions = new List<string>();
		List<string> restrictedActions = new List<string>();
		if (_area.CanOrbitalBombard(coordinates, out var _))
		{
			allowedActions.Add("Orbital Strike");
		}
		else
		{
			restrictedActions.Add("Orbital Strike");
		}
		if (_area.CanCAS(coordinates))
		{
			allowedActions.Add("Close Air Support");
		}
		else
		{
			restrictedActions.Add("Close Air Support");
		}
		if (_area.CanSupplyDrop(_transform.ToMapCoordinates(coordinates, true)))
		{
			allowedActions.Add("Supply Drops");
		}
		else
		{
			restrictedActions.Add("Supply Drops");
		}
		if (_area.CanMortarFire(coordinates))
		{
			allowedActions.Add("Mortar Fire");
		}
		else
		{
			restrictedActions.Add("Mortar Fire");
		}
		if (_area.CanMortarPlacement(coordinates))
		{
			allowedActions.Add("Mortar Placement");
		}
		else
		{
			restrictedActions.Add("Mortar Placement");
		}
		if (_area.CanLase(coordinates))
		{
			allowedActions.Add("Laser Designation");
		}
		else
		{
			restrictedActions.Add("Laser Designation");
		}
		if (area.Value.Comp.Medevac)
		{
			allowedActions.Add("Casualty Evacuation");
		}
		else
		{
			restrictedActions.Add("Casualty Evacuation");
		}
		if (area.Value.Comp.Paradropping)
		{
			allowedActions.Add("Paradropping");
		}
		else
		{
			restrictedActions.Add("Paradropping");
		}
		if (area.Value.Comp.NoTunnel)
		{
			restrictedActions.Add("Tunneling");
		}
		if (area.Value.Comp.Unweedable)
		{
			restrictedActions.Add("Weed Placement");
		}
		else if (!area.Value.Comp.ResinAllowed)
		{
			restrictedActions.Add("Resin Structures");
		}
		string protectionSource = "";
		if (hasHiveCoreProtection)
		{
			protectionSource = "\nProtection: Hive Core";
		}
		else if (hasPylonProtection)
		{
			protectionSource = "\nProtection: Hive Pylon";
		}
		string restrictionsStr = $"\nCeiling level: {ceilingLevel}{protectionSource}";
		if (allowedActions.Count > 0)
		{
			restrictionsStr += "\n\nAllowed:";
			restrictionsStr = restrictionsStr + "\n• " + string.Join("\n• ", allowedActions);
		}
		if (restrictedActions.Count > 0)
		{
			restrictionsStr += "\n\nBlocked:";
			restrictionsStr = restrictionsStr + "\n• " + string.Join("\n• ", restrictedActions);
		}
		return (areaName: areaProto.Name, ceilingLevel: severityToUse, restrictions: restrictionsStr);
	}

	private bool IsProtectedByRoofing(EntityCoordinates coordinates, Predicate<Entity<RoofingEntityComponent>> predicate)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RoofingEntityComponent> roofs = ((EntitySystem)this).EntityQueryEnumerator<RoofingEntityComponent>();
		EntityUid uid = default(EntityUid);
		RoofingEntityComponent roof = default(RoofingEntityComponent);
		float distance = default(float);
		while (roofs.MoveNext(ref uid, ref roof))
		{
			if (predicate(Entity<RoofingEntityComponent>.op_Implicit((uid, roof))) && ((EntityCoordinates)(ref coordinates)).TryDistance(_entityManager, uid.ToCoordinates(), ref distance) && distance <= roof.Range)
			{
				return true;
			}
		}
		return false;
	}

	public override void Update(float frameTime)
	{
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		if (_marineAlertCopyQueue.Count > 0)
		{
			Entity<AreaInfoComponent> ent;
			while (_marineAlertCopyQueue.TryDequeue(out ent))
			{
				if (_timing.CurTime >= time + _maxProcessTime)
				{
					return;
				}
				if (!((EntitySystem)this).TerminatingOrDeleted(Entity<AreaInfoComponent>.op_Implicit(ent), (MetaDataComponent)null))
				{
					(string areaName, short ceilingLevel, string restrictions) areaInfo = GetAreaInfo(Entity<AreaInfoComponent>.op_Implicit(ent));
					string areaName = areaInfo.areaName;
					short ceilingLevel = areaInfo.ceilingLevel;
					string restrictions = areaInfo.restrictions;
					AlertsSystem alerts = _alerts;
					EntityUid euid = Entity<AreaInfoComponent>.op_Implicit(ent);
					ProtoId<AlertPrototype> alert = ent.Comp.Alert;
					short? severity = ceilingLevel;
					string dynamicMessage = base.Loc.GetString("rmc-area-info", new(string, object)[3]
					{
						("area", areaName),
						("ceilingLevel", ceilingLevel),
						("restrictions", restrictions)
					});
					alerts.ShowAlert(euid, alert, severity, null, autoRemove: false, showCooldown: true, dynamicMessage);
				}
			}
		}
		EntityQueryEnumerator<AreaInfoComponent> tacMapQuery = ((EntitySystem)this).EntityQueryEnumerator<AreaInfoComponent>();
		EntityUid uid = default(EntityUid);
		AreaInfoComponent alert2 = default(AreaInfoComponent);
		while (tacMapQuery.MoveNext(ref uid, ref alert2))
		{
			if (!(time < alert2.NextUpdateTime))
			{
				_marineAlertCopyQueue.Enqueue(Entity<AreaInfoComponent>.op_Implicit((uid, alert2)));
				alert2.NextUpdateTime = time + alert2.UpdateInterval;
			}
		}
	}
}

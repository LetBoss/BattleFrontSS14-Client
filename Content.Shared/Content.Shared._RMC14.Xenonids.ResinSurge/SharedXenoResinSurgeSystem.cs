using System;
using System.Numerics;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Construction.FloorResin;
using Content.Shared._RMC14.Xenonids.Fruit;
using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.ResinSurge;

public sealed class SharedXenoResinSurgeSystem : EntitySystem
{
	[Dependency]
	private IMapManager _map;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedXenoConstructReinforceSystem _xenoReinforce;

	[Dependency]
	private SharedXenoFruitSystem _xenoFruit;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private SharedMapSystem _sharedMap;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private SharedXenoWeedsSystem _weeds;

	[Dependency]
	private ExamineSystemShared _examine;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoResinSurgeComponent, XenoResinSurgeActionEvent>((EntityEventRefHandler<XenoResinSurgeComponent, XenoResinSurgeActionEvent>)OnXenoResinSurgeAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoResinSurgeComponent, ResinSurgeStickyResinDoafter>((EntityEventRefHandler<XenoResinSurgeComponent, ResinSurgeStickyResinDoafter>)OnResinSurgeDoAfter, (Type[])null, (Type[])null);
	}

	private void SurgeUnstableWall(Entity<XenoResinSurgeComponent> xeno, EntityCoordinates target)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityCoordinates)(ref target)).IsValid((IEntityManager)(object)base.EntityManager) && _net.IsServer)
		{
			EntityUid wall = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(xeno.Comp.UnstableWallId), target);
			_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(wall));
		}
	}

	private void SurgeStickyResin(Entity<XenoResinSurgeComponent> xeno, EntityCoordinates target)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityCoordinates)(ref target)).IsValid((IEntityManager)(object)base.EntityManager) && _net.IsServer)
		{
			EntityUid resin = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(xeno.Comp.StickyResinId), target, (ComponentRegistry)null);
			_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(resin));
		}
	}

	private void ReduceSurgeCooldown(Entity<XenoResinSurgeComponent> xeno, double? cooldownMult = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		XenoResinSurgeActionComponent actionComp = default(XenoResinSurgeActionComponent);
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<XenoResinSurgeComponent>.op_Implicit(xeno)))
		{
			if (((EntitySystem)this).TryComp<XenoResinSurgeActionComponent>(Entity<ActionComponent>.op_Implicit(action), ref actionComp))
			{
				_actions.SetCooldown(action.AsNullable(), actionComp.SuccessCooldown * (cooldownMult ?? ((double)actionComp.FailCooldownMult)));
				break;
			}
		}
	}

	private void SetSurgeCooldown(Entity<XenoResinSurgeComponent> xeno, TimeSpan? cooldown = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		XenoResinSurgeActionComponent actionComp = default(XenoResinSurgeActionComponent);
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<XenoResinSurgeComponent>.op_Implicit(xeno)))
		{
			if (((EntitySystem)this).TryComp<XenoResinSurgeActionComponent>(Entity<ActionComponent>.op_Implicit(action), ref actionComp))
			{
				_actions.SetCooldown(action.AsNullable(), cooldown ?? actionComp.SuccessCooldown);
				break;
			}
		}
	}

	private void OnXenoResinSurgeAction(Entity<XenoResinSurgeComponent> xeno, ref XenoResinSurgeActionEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? grid = _transform.GetGrid(args.Target);
		if (!grid.HasValue)
		{
			return;
		}
		EntityUid gridId = grid.GetValueOrDefault();
		MapGridComponent grid2 = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
		{
			return;
		}
		if (!_examine.InRangeUnOccluded(xeno.Owner, args.Target, xeno.Comp.Range))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-resin-surge-see-fail"), Entity<XenoResinSurgeComponent>.op_Implicit(xeno), Entity<XenoResinSurgeComponent>.op_Implicit(xeno));
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityCoordinates target = args.Target.SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
		if (xeno.Comp.ResinDoafter.HasValue || !_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit((ValueTuple<EntityUid, XenoPlasmaComponent>)(xeno.Owner, null)), args.PlasmaCost))
		{
			return;
		}
		grid = args.Entity;
		if (grid.HasValue)
		{
			EntityUid entity = grid.GetValueOrDefault();
			ResinSurgeReinforcableComponent construct = default(ResinSurgeReinforcableComponent);
			if (((EntitySystem)this).TryComp<ResinSurgeReinforcableComponent>(entity, ref construct) && _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(entity)))
			{
				if (((EntitySystem)this).HasComp<XenoConstructReinforceComponent>(entity))
				{
					_popup.PopupClient(base.Loc.GetString("rmc-xeno-resin-surge-shield-fail", (ValueTuple<string, object>)("target", entity)), Entity<XenoResinSurgeComponent>.op_Implicit(xeno), Entity<XenoResinSurgeComponent>.op_Implicit(xeno));
					ReduceSurgeCooldown(xeno);
					((HandledEntityEventArgs)args).Handled = false;
					return;
				}
				string popupSelf = base.Loc.GetString("rmc-xeno-resin-surge-shield-self", (ValueTuple<string, object>)("target", entity));
				string popupOthers = base.Loc.GetString("rmc-xeno-resin-surge-shield-others", (ValueTuple<string, object>)("xeno", xeno), (ValueTuple<string, object>)("target", entity));
				_popup.PopupPredicted(popupSelf, popupOthers, Entity<XenoResinSurgeComponent>.op_Implicit(xeno), Entity<XenoResinSurgeComponent>.op_Implicit(xeno));
				_xenoReinforce.Reinforce(entity, xeno.Comp.ReinforceAmount, xeno.Comp.ReinforceDuration);
				if (_net.IsServer)
				{
					if (((EntitySystem)this).HasComp<DoorComponent>(entity))
					{
						((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.SurgeDoorEffect), entity.ToCoordinates(), (ComponentRegistry)null, default(Angle));
					}
					else
					{
						((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.SurgeWallEffect), entity.ToCoordinates(), (ComponentRegistry)null, default(Angle));
					}
				}
				return;
			}
			XenoFruitComponent fruit = default(XenoFruitComponent);
			if (((EntitySystem)this).TryComp<XenoFruitComponent>(entity, ref fruit) && _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(entity)))
			{
				if (!_xenoFruit.TrySpeedupGrowth(Entity<XenoFruitComponent>.op_Implicit((entity, fruit)), xeno.Comp.FruitGrowth))
				{
					_popup.PopupClient(base.Loc.GetString("rmc-xeno-resin-surge-fruit-fail", (ValueTuple<string, object>)("target", entity)), Entity<XenoResinSurgeComponent>.op_Implicit(xeno), Entity<XenoResinSurgeComponent>.op_Implicit(xeno));
					ReduceSurgeCooldown(xeno);
					((HandledEntityEventArgs)args).Handled = false;
				}
				else
				{
					_popup.PopupClient(base.Loc.GetString("rmc-xeno-resin-surge-fruit", (ValueTuple<string, object>)("target", entity)), Entity<XenoResinSurgeComponent>.op_Implicit(xeno), Entity<XenoResinSurgeComponent>.op_Implicit(xeno));
					((HandledEntityEventArgs)args).Handled = false;
					double cooldownTimeMult = (fruit.GrowTime.TotalSeconds - fruit.GrowTime / xeno.Comp.FruitCooldownDivisor) * 0.1;
					ReduceSurgeCooldown(xeno, cooldownTimeMult);
				}
				return;
			}
			XenoWeedsComponent weeds = default(XenoWeedsComponent);
			if (((EntitySystem)this).TryComp<XenoWeedsComponent>(entity, ref weeds) || _weeds.IsOnFriendlyWeeds(Entity<TransformComponent>.op_Implicit(entity)))
			{
				EntityUid weedEnt = entity;
				if (weeds == null)
				{
					Entity<XenoWeedsComponent>? weedTempEnt = _weeds.GetWeedsOnFloor(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), entity.ToCoordinates());
					if (weedTempEnt.HasValue)
					{
						weedEnt = Entity<XenoWeedsComponent>.op_Implicit(weedTempEnt.Value);
						((EntitySystem)this).TryComp<XenoWeedsComponent>(weedEnt, ref weeds);
					}
				}
				if (weeds != null && _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(weedEnt)))
				{
					string popupSelf2 = base.Loc.GetString("rmc-xeno-resin-surge-wall-self");
					string popupOthers2 = base.Loc.GetString("rmc-xeno-resin-surge-wall-others", (ValueTuple<string, object>)("xeno", xeno));
					_popup.PopupPredicted(popupSelf2, popupOthers2, Entity<XenoResinSurgeComponent>.op_Implicit(xeno), Entity<XenoResinSurgeComponent>.op_Implicit(xeno));
					SurgeUnstableWall(xeno, target);
					return;
				}
			}
		}
		ResinSurgeStickyResinDoafter ev = new ResinSurgeStickyResinDoafter(((EntitySystem)this).GetNetCoordinates(target, (MetaDataComponent)null));
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoResinSurgeComponent>.op_Implicit(xeno), xeno.Comp.StickyResinDoAfterPeriod, ev, Entity<XenoResinSurgeComponent>.op_Implicit(xeno))
		{
			BreakOnMove = true,
			DuplicateCondition = DuplicateConditions.SameEvent
		};
		if (_doAfter.TryStartDoAfter(doAfter, out var id))
		{
			xeno.Comp.ResinDoafter = id;
		}
		else
		{
			ReduceSurgeCooldown(xeno);
		}
		((HandledEntityEventArgs)args).Handled = false;
	}

	private void OnResinSurgeDoAfter(Entity<XenoResinSurgeComponent> xeno, ref ResinSurgeStickyResinDoafter args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		xeno.Comp.ResinDoafter = null;
		if (args.Cancelled)
		{
			return;
		}
		EntityCoordinates coords = ((EntitySystem)this).GetCoordinates(args.Coordinates);
		EntityUid? grid = _transform.GetGrid(coords);
		if (!grid.HasValue)
		{
			return;
		}
		EntityUid gridId = grid.GetValueOrDefault();
		MapGridComponent grid2 = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
		{
			return;
		}
		string popupSelf = base.Loc.GetString("rmc-xeno-resin-surge-sticky-self");
		string popupOthers = base.Loc.GetString("rmc-xeno-resin-surge-sticky-others", (ValueTuple<string, object>)("xeno", xeno));
		_popup.PopupPredicted(popupSelf, popupOthers, Entity<XenoResinSurgeComponent>.op_Implicit(xeno), Entity<XenoResinSurgeComponent>.op_Implicit(xeno));
		if (_net.IsServer)
		{
			foreach (TileRef turf in _sharedMap.GetTilesIntersecting(gridId, grid2, Box2.CenteredAround(coords.Position, new Vector2(xeno.Comp.StickyResinRadius * 2, xeno.Comp.StickyResinRadius * 2)), false, (Predicate<TileRef>)null))
			{
				if (!_rmcMap.HasAnchoredEntityEnumerator<StickyResinSurgeBlockerComponent>(_turf.GetTileCenter(turf), out Entity<StickyResinSurgeBlockerComponent> _, (Direction?)null, (DirectionFlag)0))
				{
					SurgeStickyResin(xeno, _turf.GetTileCenter(turf));
				}
			}
		}
		SetSurgeCooldown(xeno);
	}
}

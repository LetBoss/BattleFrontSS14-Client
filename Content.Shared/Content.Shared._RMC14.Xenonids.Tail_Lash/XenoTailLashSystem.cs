using System;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared._RMC14.Xenonids.TailLash;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Tail_Lash;

public sealed class XenoTailLashSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private XenoPlasmaSystem _plasma;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private RMCSizeStunSystem _size;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private RMCPullingSystem _pulling;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoTailLashComponent, XenoTailLashActionEvent>((EntityEventRefHandler<XenoTailLashComponent, XenoTailLashActionEvent>)OnTailLashAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTailLashComponent, XenoTailLashDoAfterEvent>((EntityEventRefHandler<XenoTailLashComponent, XenoTailLashDoAfterEvent>)OnTailLashDoAfter, (Type[])null, (Type[])null);
	}

	private void OnTailLashAction(Entity<XenoTailLashComponent> xeno, ref XenoTailLashActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_plasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.Cost))
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
		Angle direction = DirectionExtensions.ToAngle(Vector2Helpers.Normalized(args.Target.Position - _transform.GetMoverCoordinates(Entity<XenoTailLashComponent>.op_Implicit(xeno)).Position)) - Angle.FromDegrees(90.0);
		EntityCoordinates xenoCoord = _transform.GetMoverCoordinates(Entity<XenoTailLashComponent>.op_Implicit(xeno));
		Box2 val = Box2.CenteredAround(xenoCoord.Position, new Vector2(xeno.Comp.Width, xeno.Comp.Height));
		Box2 area = ((Box2)(ref val)).Translated(new Vector2(0f, xeno.Comp.Height / 2f + 0.5f));
		Box2Rotated rot = default(Box2Rotated);
		((Box2Rotated)(ref rot))._002Ector(area, direction, xenoCoord.Position);
		bool valid = false;
		Box2 bounds = ((Box2Rotated)(ref rot)).CalcBoundingBox();
		foreach (TileRef tile in _map.GetTilesIntersecting(gridId, grid2, rot, true, (Predicate<TileRef>)null))
		{
			if (!_interaction.InRangeUnobstructed(xeno.Owner, _turf.GetTileCenter(tile), xeno.Comp.Width * xeno.Comp.Height, CollisionGroup.MobMask))
			{
				continue;
			}
			valid = true;
			if (!_net.IsClient)
			{
				EntProtoId spawn = xeno.Comp.Effect;
				val = Box2.CenteredAround(_turf.GetTileCenter(tile).Position, Vector2.One);
				if (!((Box2)(ref bounds)).Encloses(ref val))
				{
					spawn = xeno.Comp.EffectEdge;
				}
				((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(spawn), _turf.GetTileCenter(tile), (ComponentRegistry)null);
			}
		}
		if (!valid)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-tail-lash-no-room"), Entity<XenoTailLashComponent>.op_Implicit(xeno), Entity<XenoTailLashComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			return;
		}
		xeno.Comp.Area = rot;
		DoAfterArgs ar = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoTailLashComponent>.op_Implicit(xeno), xeno.Comp.Windup, new XenoTailLashDoAfterEvent(), Entity<XenoTailLashComponent>.op_Implicit(xeno))
		{
			BreakOnMove = true,
			DuplicateCondition = DuplicateConditions.SameEvent
		};
		_doAfter.TryStartDoAfter(ar);
	}

	private void OnTailLashDoAfter(Entity<XenoTailLashComponent> xeno, ref XenoTailLashDoAfterEvent args)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled || !xeno.Comp.Area.HasValue || !_plasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.Cost))
		{
			xeno.Comp.Area = null;
			return;
		}
		((EntitySystem)this).EnsureComp<XenoSweepingComponent>(Entity<XenoTailLashComponent>.op_Implicit(xeno));
		DoCooldown(xeno);
		if (_net.IsClient)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		SharedPhysicsSystem physics = _physics;
		MapId mapID = ((EntitySystem)this).Transform(Entity<XenoTailLashComponent>.op_Implicit(xeno)).MapID;
		Box2Rotated value = xeno.Comp.Area.Value;
		foreach (Entity<PhysicsComponent> ent in physics.GetCollidingEntities(mapID, ref value))
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoTailLashComponent>.op_Implicit(xeno), Entity<PhysicsComponent>.op_Implicit(ent)) && _interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno.Owner), Entity<TransformComponent>.op_Implicit(ent.Owner), xeno.Comp.Width * xeno.Comp.Height, CollisionGroup.MobMask) && (!_size.TryGetSize(Entity<PhysicsComponent>.op_Implicit(ent), out var size) || (int)size < 5))
			{
				_stun.TryParalyze(Entity<PhysicsComponent>.op_Implicit(ent), _xeno.TryApplyXenoDebuffMultiplier(Entity<PhysicsComponent>.op_Implicit(ent), xeno.Comp.StunTime), refresh: true);
				_slow.TrySlowdown(Entity<PhysicsComponent>.op_Implicit(ent), _xeno.TryApplyXenoDebuffMultiplier(Entity<PhysicsComponent>.op_Implicit(ent), xeno.Comp.SlowTime));
				_pulling.TryStopAllPullsFromAndOn(Entity<PhysicsComponent>.op_Implicit(ent));
				MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoTailLashComponent>.op_Implicit(xeno), (TransformComponent)null);
				_size.KnockBack(Entity<PhysicsComponent>.op_Implicit(ent), origin, xeno.Comp.FlingDistance, xeno.Comp.FlingDistance, 10f);
			}
		}
		xeno.Comp.Area = null;
		((EntitySystem)this).Dirty<XenoTailLashComponent>(xeno, (MetaDataComponent)null);
	}

	private void DoCooldown(Entity<XenoTailLashComponent> xeno)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		foreach (Entity<ActionComponent> item in _rmcActions.GetActionsWithEvent<XenoTailLashActionEvent>(Entity<XenoTailLashComponent>.op_Implicit(xeno)))
		{
			item.Deconstruct(ref val, ref actionComponent);
			EntityUid actionId = val;
			_actions.SetCooldown(Entity<ActionComponent>.op_Implicit(actionId), xeno.Comp.Cooldown);
		}
	}
}

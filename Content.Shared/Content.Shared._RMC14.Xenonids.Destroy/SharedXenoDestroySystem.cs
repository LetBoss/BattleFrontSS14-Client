using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Gibbing;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Explosion;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Jittering;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Destroy;

public abstract class SharedXenoDestroySystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private SharedDoAfterSystem _doafter;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	[Dependency]
	private RotateToFaceSystem _rotateToFace;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private MobStateSystem _mob;

	[Dependency]
	protected IGameTiming _timing;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private RMCSizeStunSystem _size;

	[Dependency]
	private SharedBodySystem _body;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private RMCGibSystem _rmcGib;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private RMCCameraShakeSystem _cameraShake;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RMCPullingSystem _rmcPull;

	[Dependency]
	private ActionBlockerSystem _blocker;

	private readonly HashSet<Entity<MobStateComponent>> _mobs = new HashSet<Entity<MobStateComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyComponent, XenoDestroyActionEvent>((EntityEventRefHandler<XenoDestroyComponent, XenoDestroyActionEvent>)OnXenoDestroyAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyComponent, XenoDestroyLeapDoafter>((EntityEventRefHandler<XenoDestroyComponent, XenoDestroyLeapDoafter>)OnXenoDestroyDoafter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, AttemptMobCollideEvent>((EntityEventRefHandler<XenoDestroyLeapingComponent, AttemptMobCollideEvent>)OnLeapCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, AttemptMobTargetCollideEvent>((EntityEventRefHandler<XenoDestroyLeapingComponent, AttemptMobTargetCollideEvent>)OnLeapTargetCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, ComponentInit>((EntityEventRefHandler<XenoDestroyLeapingComponent, ComponentInit>)OnLeapingInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, ComponentRemove>((EntityEventRefHandler<XenoDestroyLeapingComponent, ComponentRemove>)OnLeapingRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, DropAttemptEvent>((EntityEventRefHandler<XenoDestroyLeapingComponent, DropAttemptEvent>)OnLeapingCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, UseAttemptEvent>((EntityEventRefHandler<XenoDestroyLeapingComponent, UseAttemptEvent>)OnLeapingCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, PickupAttemptEvent>((EntityEventRefHandler<XenoDestroyLeapingComponent, PickupAttemptEvent>)OnLeapingCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, AttackAttemptEvent>((EntityEventRefHandler<XenoDestroyLeapingComponent, AttackAttemptEvent>)OnLeapingCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, ThrowAttemptEvent>((EntityEventRefHandler<XenoDestroyLeapingComponent, ThrowAttemptEvent>)OnLeapingCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, ChangeDirectionAttemptEvent>((EntityEventRefHandler<XenoDestroyLeapingComponent, ChangeDirectionAttemptEvent>)OnLeapingCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, InteractionAttemptEvent>((EntityEventRefHandler<XenoDestroyLeapingComponent, InteractionAttemptEvent>)OnLeapingCancelInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, PullAttemptEvent>((EntityEventRefHandler<XenoDestroyLeapingComponent, PullAttemptEvent>)OnLeapingCancelPull, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDestroyLeapingComponent, UpdateCanMoveEvent>((EntityEventRefHandler<XenoDestroyLeapingComponent, UpdateCanMoveEvent>)OnLeapingCancel, (Type[])null, (Type[])null);
	}

	private void OnXenoDestroyAction(Entity<XenoDestroyComponent> xeno, ref XenoDestroyActionEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _turf.TryGetTileRef(args.Target, out var tile))
		{
			EntityCoordinates target = _turf.GetTileCenter(tile.Value);
			if (!_interaction.InRangeUnobstructed(Entity<XenoDestroyComponent>.op_Implicit(xeno), target, xeno.Comp.Range) || _rmcMap.IsTileBlocked(target))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-destroy-cant-reach"), Entity<XenoDestroyComponent>.op_Implicit(xeno), Entity<XenoDestroyComponent>.op_Implicit(xeno), PopupType.SmallCaution);
				return;
			}
			if (!_area.TryGetArea(target, out Entity<AreaComponent>? area, out EntityPrototype _) || area.Value.Comp.NoTunnel)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-destroy-cant-area"), Entity<XenoDestroyComponent>.op_Implicit(xeno), Entity<XenoDestroyComponent>.op_Implicit(xeno), PopupType.SmallCaution);
				return;
			}
			_jitter.DoJitter(Entity<XenoDestroyComponent>.op_Implicit(xeno), xeno.Comp.JumpTime, refresh: true, 80f, 8f, forceValueChange: true);
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoDestroyComponent>.op_Implicit(xeno), xeno.Comp.JumpTime, new XenoDestroyLeapDoafter(((EntitySystem)this).GetNetCoordinates(target, (MetaDataComponent)null)), Entity<XenoDestroyComponent>.op_Implicit(xeno))
			{
				BreakOnMove = true,
				BreakOnRest = true
			};
			_doafter.TryStartDoAfter(doAfter);
			((EntitySystem)this).Dirty<XenoDestroyComponent>(xeno, (MetaDataComponent)null);
		}
	}

	private void OnXenoDestroyDoafter(Entity<XenoDestroyComponent> xeno, ref XenoDestroyLeapDoafter args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled || _net.IsClient)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityCoordinates coords = ((EntitySystem)this).GetCoordinates(args.TargetCoords);
		if (!_interaction.InRangeUnobstructed(Entity<XenoDestroyComponent>.op_Implicit(xeno), coords, xeno.Comp.Range) || _rmcMap.IsTileBlocked(coords))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-destroy-cant-reach"), Entity<XenoDestroyComponent>.op_Implicit(xeno), Entity<XenoDestroyComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		_rotateToFace.TryFaceCoordinates(Entity<XenoDestroyComponent>.op_Implicit(xeno), _transform.ToMapCoordinates(args.TargetCoords).Position);
		_rmcPull.TryStopAllPullsFromAndOn(Entity<XenoDestroyComponent>.op_Implicit(xeno));
		if (_net.IsServer)
		{
			XenoDestroyLeapingComponent leaping = ((EntitySystem)this).EnsureComp<XenoDestroyLeapingComponent>(Entity<XenoDestroyComponent>.op_Implicit(xeno));
			leaping.Target = coords;
			leaping.LeapMoveAt = _timing.CurTime + xeno.Comp.CrashTime / 2.0;
			leaping.LeapEndAt = _timing.CurTime + xeno.Comp.CrashTime;
			((EntitySystem)this).Dirty(xeno.Owner, (IComponent)(object)leaping, (MetaDataComponent)null);
			Filter filter = Filter.Pvs(Entity<XenoDestroyComponent>.op_Implicit(xeno), 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null);
			Vector2 offset = _transform.ToMapCoordinates(coords, true).Position - _transform.GetMapCoordinates(Entity<XenoDestroyComponent>.op_Implicit(xeno), (TransformComponent)null).Position;
			XenoDestroyLeapStartEvent ev = new XenoDestroyLeapStartEvent(((EntitySystem)this).GetNetEntity(Entity<XenoDestroyComponent>.op_Implicit(xeno), (MetaDataComponent)null), offset);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev, filter, true);
		}
		((EntitySystem)this).PredictedSpawnAtPosition(EntProtoId.op_Implicit(xeno.Comp.Telegraph), coords, (ComponentRegistry)null);
		_emote.TryEmoteWithChat(Entity<XenoDestroyComponent>.op_Implicit(xeno), xeno.Comp.Emote);
	}

	private void OnLeapCollide(Entity<XenoDestroyLeapingComponent> xeno, ref AttemptMobCollideEvent args)
	{
		args.Cancelled = true;
	}

	private void OnLeapTargetCollide(Entity<XenoDestroyLeapingComponent> xeno, ref AttemptMobTargetCollideEvent args)
	{
		args.Cancelled = true;
	}

	private void OnLeapingCancel<T>(Entity<XenoDestroyLeapingComponent> ent, ref T args) where T : CancellableEntityEventArgs
	{
		((CancellableEntityEventArgs)args/*cast due to constrained. prefix*/).Cancel();
	}

	private void OnLeapingCancelInteract(Entity<XenoDestroyLeapingComponent> ent, ref InteractionAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnLeapingCancelPull(Entity<XenoDestroyLeapingComponent> ent, ref PullAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void CrashDown(Entity<XenoDestroyComponent> xeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<XenoDestroyLeapingComponent>(Entity<XenoDestroyComponent>.op_Implicit(xeno));
		EntityUid? grid = _transform.GetGrid(Entity<TransformComponent>.op_Implicit(xeno.Owner));
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
		if (_net.IsServer)
		{
			_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoDestroyComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		BodyComponent body = default(BodyComponent);
		foreach (TileRef tile in _map.GetTilesIntersecting(gridId, grid2, Box2.CenteredAround(_transform.GetMoverCoordinates(Entity<XenoDestroyComponent>.op_Implicit(xeno)).Position, new Vector2(2f, 2f)), true, (Predicate<TileRef>)null))
		{
			foreach (EntityUid ent in _entityLookup.GetEntitiesInTile(tile, (LookupFlags)110))
			{
				if (CanGib(Entity<XenoDestroyComponent>.op_Implicit(xeno), ent))
				{
					if (!xeno.Comp.Gibs || !((EntitySystem)this).TryComp<BodyComponent>(ent, ref body))
					{
						_damage.TryChangeDamage(ent, xeno.Comp.MobDamage, ignoreResistances: true, interruptsDoAfters: true, null, Entity<XenoDestroyComponent>.op_Implicit(xeno), Entity<XenoDestroyComponent>.op_Implicit(xeno));
					}
					else if (_net.IsServer)
					{
						_rmcGib.ScatterInventoryItems(ent);
						_body.GibBody(ent, gibOrgans: true, body);
					}
				}
				else if (((EntitySystem)this).HasComp<ItemComponent>(ent) && !((EntitySystem)this).Transform(ent).Anchored)
				{
					_size.KnockBack(ent, _transform.GetMapCoordinates(Entity<XenoDestroyComponent>.op_Implicit(xeno), (TransformComponent)null), xeno.Comp.Knockback, xeno.Comp.Knockback, 15f, ignoreSize: true);
				}
				else if (_whitelist.IsWhitelistPass(xeno.Comp.Structures, ent))
				{
					GetExplosionResistanceEvent ev = new GetExplosionResistanceEvent(xeno.Comp.ExplosionType.Id);
					((EntitySystem)this).RaiseLocalEvent<GetExplosionResistanceEvent>(ent, ref ev, false);
					_damage.TryChangeDamage(ent, xeno.Comp.StructureDamage * ev.DamageCoefficient, ignoreResistances: true, interruptsDoAfters: true, null, Entity<XenoDestroyComponent>.op_Implicit(xeno), Entity<XenoDestroyComponent>.op_Implicit(xeno));
				}
			}
			((EntitySystem)this).PredictedSpawnAtPosition(EntProtoId.op_Implicit(xeno.Comp.SmokeEffect), _turf.GetTileCenter(tile), (ComponentRegistry)null);
		}
		_mobs.Clear();
		_entityLookup.GetEntitiesInRange<MobStateComponent>(((EntitySystem)this).Transform(Entity<XenoDestroyComponent>.op_Implicit(xeno)).Coordinates, xeno.Comp.ShakeCameraRange, _mobs, (LookupFlags)110);
		foreach (Entity<MobStateComponent> mob in _mobs)
		{
			if (mob.Owner == xeno.Owner)
			{
				_cameraShake.ShakeCamera(Entity<MobStateComponent>.op_Implicit(mob), 5, 1);
			}
			else
			{
				_cameraShake.ShakeCamera(Entity<MobStateComponent>.op_Implicit(mob), 15, 1);
			}
		}
		SetCooldown(xeno);
	}

	private bool CanGib(EntityUid king, EntityUid target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (king == target)
		{
			return false;
		}
		if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(king), Entity<HiveMemberComponent>.op_Implicit(target)))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<DevouredComponent>(target))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<XenoNestedComponent>(target))
		{
			return false;
		}
		return ((EntitySystem)this).HasComp<MobStateComponent>(target);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoDestroyLeapingComponent, XenoDestroyComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoDestroyLeapingComponent, XenoDestroyComponent>();
		EntityUid uid = default(EntityUid);
		XenoDestroyLeapingComponent leaping = default(XenoDestroyLeapingComponent);
		XenoDestroyComponent destroy = default(XenoDestroyComponent);
		while (query.MoveNext(ref uid, ref leaping, ref destroy))
		{
			if (_mob.IsDead(uid))
			{
				((EntitySystem)this).RemCompDeferred<XenoDestroyLeapingComponent>(uid);
				continue;
			}
			if (leaping.LeapMoveAt.HasValue)
			{
				TimeSpan value = time;
				TimeSpan? leapMoveAt = leaping.LeapMoveAt;
				if (value > leapMoveAt)
				{
					if (leaping.Target.HasValue)
					{
						_transform.SetCoordinates(uid, leaping.Target.Value);
					}
					leaping.LeapMoveAt = null;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)leaping, (MetaDataComponent)null);
				}
			}
			if (leaping.LeapEndAt.HasValue)
			{
				TimeSpan value = time;
				TimeSpan? leapMoveAt = leaping.LeapEndAt;
				if (!(value < leapMoveAt))
				{
					CrashDown(Entity<XenoDestroyComponent>.op_Implicit((uid, destroy)));
				}
			}
		}
	}

	private void SetCooldown(Entity<XenoDestroyComponent> xeno)
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
		using IEnumerator<Entity<ActionComponent>> enumerator = _rmcActions.GetActionsWithEvent<XenoDestroyActionEvent>(Entity<XenoDestroyComponent>.op_Implicit(xeno)).GetEnumerator();
		if (enumerator.MoveNext())
		{
			EntityUid val = default(EntityUid);
			ActionComponent actionComponent = default(ActionComponent);
			enumerator.Current.Deconstruct(ref val, ref actionComponent);
			EntityUid actionId = val;
			_actions.SetCooldown(Entity<ActionComponent>.op_Implicit(actionId), xeno.Comp.Cooldown);
		}
	}

	private void OnLeapingInit(Entity<XenoDestroyLeapingComponent> xeno, ref ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<XenoDestroyLeapingComponent>.op_Implicit(xeno)))
		{
			_actions.SetEnabled(action.AsNullable(), enabled: false);
		}
		_blocker.UpdateCanMove(Entity<XenoDestroyLeapingComponent>.op_Implicit(xeno));
	}

	protected virtual void OnLeapingRemove(Entity<XenoDestroyLeapingComponent> xeno, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<XenoDestroyLeapingComponent>.op_Implicit(xeno)))
		{
			_actions.SetEnabled(action.AsNullable(), enabled: true);
		}
		_blocker.UpdateCanMove(Entity<XenoDestroyLeapingComponent>.op_Implicit(xeno));
	}
}

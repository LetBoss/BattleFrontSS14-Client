using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.ActionBlocker;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.StatusEffectNew;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Burrow;

public abstract class SharedXenoBurrowSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private EntityManager _entities;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private IGameTiming _time;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoBurrowComponent, ExamineAttemptEvent>((EntityEventRefHandler<XenoBurrowComponent, ExamineAttemptEvent>)PreventExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBurrowComponent, BeforeStatusEffectAddedEvent>((EntityEventRefHandler<XenoBurrowComponent, BeforeStatusEffectAddedEvent>)PreventEffects, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBurrowComponent, BeforeDamageChangedEvent>((EntityEventRefHandler<XenoBurrowComponent, BeforeDamageChangedEvent>)PreventDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBurrowComponent, PreventCollideEvent>((EntityEventRefHandler<XenoBurrowComponent, PreventCollideEvent>)PreventCollision, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBurrowComponent, InteractionAttemptEvent>((ComponentEventRefHandler<XenoBurrowComponent, InteractionAttemptEvent>)PreventInteraction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBurrowComponent, RMCIgniteAttemptEvent>((EntityEventRefHandler<XenoBurrowComponent, RMCIgniteAttemptEvent>)OnBurrowedCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBurrowComponent, AttackAttemptEvent>((EntityEventRefHandler<XenoBurrowComponent, AttackAttemptEvent>)OnBurrowedCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBurrowComponent, XenoBurrowActionEvent>((EntityEventRefHandler<XenoBurrowComponent, XenoBurrowActionEvent>)OnBeginBurrow, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBurrowComponent, XenoBurrowDownDoAfter>((EntityEventRefHandler<XenoBurrowComponent, XenoBurrowDownDoAfter>)OnFinishBurrow, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBurrowComponent, BurrowedEvent>((EntityEventRefHandler<XenoBurrowComponent, BurrowedEvent>)SetBurrow, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoBurrowComponent, XenoBurrowMoveDoAfter>((EntityEventRefHandler<XenoBurrowComponent, XenoBurrowMoveDoAfter>)OnFinishTunnel, (Type[])null, (Type[])null);
	}

	private void PreventExamine(Entity<XenoBurrowComponent> burrower, ref ExamineAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && burrower.Comp.Active && !((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void PreventEffects(Entity<XenoBurrowComponent> burrower, ref BeforeStatusEffectAddedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && burrower.Comp.Active)
		{
			args.Cancelled = true;
		}
	}

	private void OnBurrowedCancel<T>(Entity<XenoBurrowComponent> burrower, ref T args) where T : CancellableEntityEventArgs
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && burrower.Comp.Active)
		{
			((CancellableEntityEventArgs)args/*cast due to constrained. prefix*/).Cancel();
		}
	}

	private void PreventDamage(Entity<XenoBurrowComponent> burrower, ref BeforeDamageChangedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && burrower.Comp.Active)
		{
			args.Cancelled = true;
		}
	}

	private void PreventCollision(Entity<XenoBurrowComponent> burrower, ref PreventCollideEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && burrower.Comp.Active)
		{
			args.Cancelled = true;
		}
	}

	private void PreventInteraction(EntityUid ent, XenoBurrowComponent comp, ref InteractionAttemptEvent args)
	{
		if (!args.Cancelled && comp.Active)
		{
			args.Cancelled = true;
		}
	}

	private void SetBurrow(Entity<XenoBurrowComponent> burrower, ref BurrowedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if (args.burrowed == burrower.Comp.Active)
		{
			return;
		}
		burrower.Comp.Active = args.burrowed;
		_actionBlocker.UpdateCanMove(Entity<XenoBurrowComponent>.op_Implicit(burrower));
		if (args.burrowed)
		{
			_transform.AnchorEntity(Entity<XenoBurrowComponent>.op_Implicit(burrower));
			return;
		}
		_transform.Unanchor(Entity<XenoBurrowComponent>.op_Implicit(burrower));
		PhysicsComponent body = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(Entity<XenoBurrowComponent>.op_Implicit(burrower), ref body))
		{
			_physics.TrySetBodyType(Entity<XenoBurrowComponent>.op_Implicit(burrower), (BodyType)2, (FixturesComponent)null, body, (TransformComponent)null);
			((EntitySystem)this).Dirty(Entity<XenoBurrowComponent>.op_Implicit(burrower), (IComponent)(object)body, (MetaDataComponent)null);
		}
		if (_net.IsServer)
		{
			_audio.PlayPvs(burrower.Comp.BurrowUpSound, Entity<XenoBurrowComponent>.op_Implicit(burrower), (AudioParams?)null);
			foreach (EntityUid entity in _entityLookup.GetEntitiesInRange(Entity<XenoBurrowComponent>.op_Implicit(burrower), burrower.Comp.UnburrowStunRange, (LookupFlags)110))
			{
				if (_xeno.CanAbilityAttackTarget(Entity<XenoBurrowComponent>.op_Implicit(burrower), entity))
				{
					_stun.TryParalyze(entity, burrower.Comp.UnburrowStunLength, refresh: false);
				}
			}
		}
		((EntitySystem)this).Dirty<XenoBurrowComponent>(burrower, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _time.CurTime;
		EntityQueryEnumerator<XenoBurrowComponent> querry = ((EntitySystem)this).EntityQueryEnumerator<XenoBurrowComponent>();
		EntityUid ent = default(EntityUid);
		XenoBurrowComponent comp = default(XenoBurrowComponent);
		while (querry.MoveNext(ref ent, ref comp))
		{
			if (comp.NextBurrowAt < time)
			{
				if (!comp.Active)
				{
					_popup.PopupEntity(base.Loc.GetString("rmc-xeno-burrow-cooldown-finish"), ent, ent);
				}
				comp.NextBurrowAt = null;
			}
			if (comp.NextTunnelAt < time)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-burrow-move-cooldown-finish"), ent, ent);
				comp.NextTunnelAt = null;
			}
			if (!comp.Tunneling && comp.ForcedUnburrowAt < time)
			{
				BurrowedEvent ev = new BurrowedEvent(burrowed: false);
				((EntitySystem)this).RaiseLocalEvent<BurrowedEvent>(ent, ref ev, false);
				comp.ForcedUnburrowAt = null;
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-burrow-move-forced-unburrow"), ent, ent, PopupType.MediumCaution);
				comp.NextBurrowAt = time + comp.BurrowCooldown;
			}
		}
	}

	private void OnBeginBurrow(Entity<XenoBurrowComponent> burrower, ref XenoBurrowActionEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		if (burrower.Comp.Active)
		{
			EntityCoordinates target = args.Target;
			if (CanTunnelPopup(burrower, target, out var distance))
			{
				TimeSpan duration = new TimeSpan(0, 0, (int)distance.Value);
				if (duration < burrower.Comp.MinimumTunnelTime)
				{
					duration = burrower.Comp.MinimumTunnelTime;
				}
				XenoBurrowMoveDoAfter moveEv = new XenoBurrowMoveDoAfter(_entities.GetNetCoordinates(target, (MetaDataComponent)null));
				DoAfterArgs moveDoAfterArgs = new DoAfterArgs((IEntityManager)(object)_entities, Entity<XenoBurrowComponent>.op_Implicit(burrower), duration, moveEv, Entity<XenoBurrowComponent>.op_Implicit(burrower))
				{
					RequireCanInteract = false,
					DuplicateCondition = DuplicateConditions.SameEvent
				};
				if (_doAfter.TryStartDoAfter(moveDoAfterArgs))
				{
					burrower.Comp.Tunneling = true;
					((EntitySystem)this).Dirty<XenoBurrowComponent>(burrower, (MetaDataComponent)null);
				}
				((EntitySystem)this).Dirty<XenoBurrowComponent>(burrower, (MetaDataComponent)null);
				if (_net.IsServer)
				{
					_audio.PlayPvs(burrower.Comp.BurrowDownSound, Entity<XenoBurrowComponent>.op_Implicit(burrower), (AudioParams?)null);
				}
			}
			return;
		}
		DoAfterComponent doAfterComp = default(DoAfterComponent);
		if (((EntitySystem)this).TryComp<DoAfterComponent>(Entity<XenoBurrowComponent>.op_Implicit(burrower), ref doAfterComp))
		{
			foreach (KeyValuePair<ushort, Content.Shared.DoAfter.DoAfter> doAfter in doAfterComp.DoAfters)
			{
				if (!doAfter.Value.Cancelled && !doAfter.Value.Completed)
				{
					_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-down-doafter-stop"), Entity<XenoBurrowComponent>.op_Implicit(burrower), Entity<XenoBurrowComponent>.op_Implicit(burrower), PopupType.SmallCaution);
					return;
				}
			}
		}
		if (CanBurrowPopup(burrower))
		{
			XenoBurrowDownDoAfter burrowEv = new XenoBurrowDownDoAfter();
			DoAfterArgs burrowDoAfterArgs = new DoAfterArgs((IEntityManager)(object)_entities, Entity<XenoBurrowComponent>.op_Implicit(burrower), burrower.Comp.BurrowLength, burrowEv, Entity<XenoBurrowComponent>.op_Implicit(burrower))
			{
				BreakOnMove = true,
				DuplicateCondition = DuplicateConditions.SameEvent,
				CancelDuplicate = true
			};
			if (_doAfter.TryStartDoAfter(burrowDoAfterArgs))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-down-start"), Entity<XenoBurrowComponent>.op_Implicit(burrower), Entity<XenoBurrowComponent>.op_Implicit(burrower));
			}
		}
	}

	private void OnFinishBurrow(Entity<XenoBurrowComponent> burrower, ref XenoBurrowDownDoAfter args)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			if (args.Cancelled)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-down-failure-break"), Entity<XenoBurrowComponent>.op_Implicit(burrower), Entity<XenoBurrowComponent>.op_Implicit(burrower));
				burrower.Comp.NextBurrowAt = _time.CurTime + burrower.Comp.BurrowCooldown;
				return;
			}
			if (((EntitySystem)this).HasComp<XenoRestingComponent>(Entity<XenoBurrowComponent>.op_Implicit(burrower)))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-down-failure-rest"), Entity<XenoBurrowComponent>.op_Implicit(burrower), Entity<XenoBurrowComponent>.op_Implicit(burrower));
				return;
			}
			burrower.Comp.ForcedUnburrowAt = _time.CurTime + burrower.Comp.BurrowMaxDuration;
			burrower.Comp.NextBurrowAt = _time.CurTime + burrower.Comp.BurrowCooldown;
			_rmcPulling.TryStopAllPullsFromAndOn(Entity<XenoBurrowComponent>.op_Implicit(burrower));
			((EntitySystem)this).Dirty<XenoBurrowComponent>(burrower, (MetaDataComponent)null);
			BurrowedEvent ev = new BurrowedEvent(burrowed: true);
			((EntitySystem)this).RaiseLocalEvent<BurrowedEvent>(Entity<XenoBurrowComponent>.op_Implicit(burrower), ref ev, false);
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-down-finish"), Entity<XenoBurrowComponent>.op_Implicit(burrower), Entity<XenoBurrowComponent>.op_Implicit(burrower));
		}
	}

	private bool CanBurrowPopup(Entity<XenoBurrowComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.NextBurrowAt > _time.CurTime)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-down-failure-cooldown"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
			return false;
		}
		EntityCoordinates coordinates = _transform.GetMoverCoordinates(ent.Owner).SnapToGrid();
		if (!_area.TryGetArea(coordinates, out Entity<AreaComponent>? area, out EntityPrototype _) || area.Value.Comp.NoTunnel)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-down-failure-bad-area"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
			return false;
		}
		EntityUid? gridId = _transform.GetGrid(Entity<TransformComponent>.op_Implicit(ent.Owner));
		MapGridComponent grid = default(MapGridComponent);
		if (((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid))
		{
			TileRef tile = _map.GetTileRef(gridId.Value, grid, coordinates);
			if (_turf.IsSpace(tile))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-failure-space"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
				return false;
			}
			return true;
		}
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-failure-space"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
		return false;
	}

	private bool CanTunnelPopup(Entity<XenoBurrowComponent> ent, EntityCoordinates target, [NotNullWhen(true)] out float? distance)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		distance = null;
		if (ent.Comp.NextTunnelAt > _time.CurTime)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-move-failure-coolown"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
			return false;
		}
		if (!_area.TryGetArea(target, out Entity<AreaComponent>? area, out EntityPrototype _) || area.Value.Comp.NoTunnel)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-move-failure-bad-area"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
			return false;
		}
		EntityUid? gridId = _transform.GetGrid(Entity<TransformComponent>.op_Implicit(ent.Owner));
		MapGridComponent grid = default(MapGridComponent);
		if (((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid))
		{
			TileRef tile = _map.GetTileRef(gridId.Value, grid, target);
			if (_turf.IsSpace(tile))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-failure-space"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
				return false;
			}
			if (_turf.IsTileBlocked(tile, CollisionGroup.Impassable))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-move-failure-solid"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
				return false;
			}
			float burrowDistance = default(float);
			if (!((EntityCoordinates)(ref target)).TryDistance((IEntityManager)(object)_entities, ent.Owner.ToCoordinates(), ref burrowDistance))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-move-failure"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
				return false;
			}
			if (distance > ent.Comp.MaxTunnelingDistance)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-move-failure"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
				return false;
			}
			if (!ent.Comp.Tunneling)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-move-start"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-move-break"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
			}
			distance = burrowDistance;
			return true;
		}
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-failure-space"), Entity<XenoBurrowComponent>.op_Implicit(ent), Entity<XenoBurrowComponent>.op_Implicit(ent));
		return false;
	}

	private void OnFinishTunnel(Entity<XenoBurrowComponent> burrower, ref XenoBurrowMoveDoAfter args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			burrower.Comp.Tunneling = false;
			burrower.Comp.NextTunnelAt = _time.CurTime + burrower.Comp.TunnelCooldown;
			return;
		}
		burrower.Comp.Tunneling = false;
		burrower.Comp.NextTunnelAt = null;
		burrower.Comp.ForcedUnburrowAt = null;
		_rmcPulling.TryStopAllPullsFromAndOn(Entity<XenoBurrowComponent>.op_Implicit(burrower));
		((EntitySystem)this).Dirty<XenoBurrowComponent>(burrower, (MetaDataComponent)null);
		if (_net.IsServer)
		{
			_transform.SetCoordinates(Entity<XenoBurrowComponent>.op_Implicit(burrower), _entities.GetCoordinates(args.TargetCoords));
		}
		BurrowedEvent ev = new BurrowedEvent(burrowed: false);
		((EntitySystem)this).RaiseLocalEvent<BurrowedEvent>(Entity<XenoBurrowComponent>.op_Implicit(burrower), ref ev, false);
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-burrow-move-finish"), Entity<XenoBurrowComponent>.op_Implicit(burrower), Entity<XenoBurrowComponent>.op_Implicit(burrower));
	}
}

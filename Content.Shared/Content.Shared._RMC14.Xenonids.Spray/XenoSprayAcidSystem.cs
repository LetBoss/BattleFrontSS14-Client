using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.OnCollide;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Spray;

public sealed class XenoSprayAcidSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private LineSystem _line;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedOnCollideSystem _onCollide;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	private static readonly ProtoId<ReagentPrototype> AcidRemovedBy = ProtoId<ReagentPrototype>.op_Implicit("Water");

	private EntityQuery<BarricadeComponent> _barricadeQuery;

	private EntityQuery<XenoSprayAcidComponent> _xenoSprayAcidQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_barricadeQuery = ((EntitySystem)this).GetEntityQuery<BarricadeComponent>();
		_xenoSprayAcidQuery = ((EntitySystem)this).GetEntityQuery<XenoSprayAcidComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoSprayAcidComponent, XenoSprayAcidActionEvent>((EntityEventRefHandler<XenoSprayAcidComponent, XenoSprayAcidActionEvent>)OnSprayAcidAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoSprayAcidComponent, XenoSprayAcidDoAfter>((EntityEventRefHandler<XenoSprayAcidComponent, XenoSprayAcidDoAfter>)OnSprayAcidDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SprayAcidedComponent, MapInitEvent>((EntityEventRefHandler<SprayAcidedComponent, MapInitEvent>)OnSprayAcidedMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SprayAcidedComponent, ComponentRemove>((EntityEventRefHandler<SprayAcidedComponent, ComponentRemove>)OnSprayAcidedRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SprayAcidedComponent, VaporHitEvent>((EntityEventRefHandler<SprayAcidedComponent, VaporHitEvent>)OnSprayAcidedVaporHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAcidSplatterComponent, ExtinguishFireAttemptEvent>((EntityEventRefHandler<XenoAcidSplatterComponent, ExtinguishFireAttemptEvent>)OnAcidSplatterExtinguishFireAttempt, (Type[])null, (Type[])null);
	}

	private void OnSprayAcidAction(Entity<XenoSprayAcidComponent> xeno, ref XenoSprayAcidActionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (_xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
		{
			NetCoordinates target = ((EntitySystem)this).GetNetCoordinates(args.Target, (MetaDataComponent)null);
			EntityCoordinates xenoCoords = _transform.GetMoverCoordinates(Entity<XenoSprayAcidComponent>.op_Implicit(xeno));
			if ((target.Position - xenoCoords.Position).Length() > xeno.Comp.Range)
			{
				Vector2 newTile = Vector2Helpers.Normalized(target.Position - xenoCoords.Position) * xeno.Comp.Range;
				((NetCoordinates)(ref target))._002Ector(((EntitySystem)this).GetNetEntity(args.Target.EntityId, (MetaDataComponent)null), xenoCoords.Position + newTile);
			}
			XenoSprayAcidDoAfter ev = new XenoSprayAcidDoAfter(target);
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoSprayAcidComponent>.op_Implicit(xeno), xeno.Comp.DoAfter, ev, Entity<XenoSprayAcidComponent>.op_Implicit(xeno))
			{
				BreakOnMove = true
			};
			_doAfter.TryStartDoAfter(doAfter);
		}
	}

	private void OnSprayAcidDoAfter(Entity<XenoSprayAcidComponent> xeno, ref XenoSprayAcidDoAfter args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
		{
			return;
		}
		_audio.PlayPredicted(xeno.Comp.Sound, Entity<XenoSprayAcidComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoSprayAcidComponent>.op_Implicit(xeno), (AudioParams?)null);
		if (_net.IsClient)
		{
			return;
		}
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoSprayAcidActionEvent>(Entity<XenoSprayAcidComponent>.op_Implicit(xeno)))
		{
			_actions.StartUseDelay(action.AsNullable());
		}
		EntityCoordinates start = xeno.Owner.ToCoordinates();
		EntityCoordinates end = ((EntitySystem)this).GetCoordinates(args.Coordinates);
		EntityUid? blocker;
		List<LineTile> tiles = _line.DrawLine(start, end, xeno.Comp.Delay, xeno.Comp.Range, out blocker);
		ActiveAcidSprayingComponent active = ((EntitySystem)this).EnsureComp<ActiveAcidSprayingComponent>(Entity<XenoSprayAcidComponent>.op_Implicit(xeno));
		active.Blocker = blocker;
		active.Acid = xeno.Comp.Acid;
		active.Spawn = tiles;
		((EntitySystem)this).Dirty(Entity<XenoSprayAcidComponent>.op_Implicit(xeno), (IComponent)(object)active, (MetaDataComponent)null);
	}

	private void OnSprayAcidedMapInit(Entity<SprayAcidedComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<SprayAcidedComponent>.op_Implicit(ent), (Enum)SprayAcidedVisuals.Acided, (object)true, (AppearanceComponent)null);
	}

	private void OnSprayAcidedRemove(Entity<SprayAcidedComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<SprayAcidedComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			_appearance.SetData(Entity<SprayAcidedComponent>.op_Implicit(ent), (Enum)SprayAcidedVisuals.Acided, (object)false, (AppearanceComponent)null);
		}
	}

	private void OnSprayAcidedVaporHit(Entity<SprayAcidedComponent> ent, ref VaporHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionContainerManagerComponent> solEnt = args.Solution;
		foreach (var item in _solutionContainer.EnumerateSolutions(Entity<SolutionContainerManagerComponent>.op_Implicit((Entity<SolutionContainerManagerComponent>.op_Implicit(solEnt), Entity<SolutionContainerManagerComponent>.op_Implicit(solEnt)))))
		{
			if (item.Solution.Comp.Solution.ContainsReagent(ProtoId<ReagentPrototype>.op_Implicit(AcidRemovedBy), null))
			{
				((EntitySystem)this).RemCompDeferred<SprayAcidedComponent>(Entity<SprayAcidedComponent>.op_Implicit(ent));
				break;
			}
		}
	}

	private void OnAcidSplatterExtinguishFireAttempt(Entity<XenoAcidSplatterComponent> ent, ref ExtinguishFireAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? xeno = ent.Comp.Xeno;
		EntityUid target = args.Target;
		if (xeno.HasValue && xeno.GetValueOrDefault() == target)
		{
			args.Cancelled = true;
		}
	}

	private void TryAcid(Entity<XenoSprayAcidComponent> acid, RMCAnchoredEntitiesEnumerator anchored)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid;
		while (anchored.MoveNext(out uid))
		{
			TryAcid(acid, uid);
		}
	}

	private void TryAcid(Entity<XenoSprayAcidComponent> acid, EntityUid target)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		if (_barricadeQuery.HasComp(target))
		{
			SprayAcidedComponent comp = ((EntitySystem)this).EnsureComp<SprayAcidedComponent>(target);
			comp.Damage = acid.Comp.BarricadeDamage;
			comp.ExpireAt = time + acid.Comp.BarricadeDuration;
			((EntitySystem)this).Dirty(target, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<ActiveAcidSprayingComponent> spraying = ((EntitySystem)this).EntityQueryEnumerator<ActiveAcidSprayingComponent>();
		EntityUid uid = default(EntityUid);
		ActiveAcidSprayingComponent active = default(ActiveAcidSprayingComponent);
		XenoSprayAcidComponent xenoSprayAcid = default(XenoSprayAcidComponent);
		Entity<XenoSprayAcidComponent> spray = default(Entity<XenoSprayAcidComponent>);
		while (spraying.MoveNext(ref uid, ref active))
		{
			ActiveAcidSprayingComponent activeAcidSprayingComponent = active;
			EntityUid valueOrDefault = activeAcidSprayingComponent.Chain.GetValueOrDefault();
			if (!activeAcidSprayingComponent.Chain.HasValue)
			{
				valueOrDefault = Entity<CollideChainComponent>.op_Implicit(_onCollide.SpawnChain());
				activeAcidSprayingComponent.Chain = valueOrDefault;
			}
			for (int i = active.Spawn.Count - 1; i >= 0; i--)
			{
				LineTile acid = active.Spawn[i];
				if (!(time < acid.At))
				{
					EntityUid spawned = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(active.Acid), acid.Coordinates, (ComponentRegistry)null, default(Angle));
					XenoAcidSplatterComponent splatter = ((EntitySystem)this).EnsureComp<XenoAcidSplatterComponent>(spawned);
					_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(uid), Entity<HiveMemberComponent>.op_Implicit(spawned));
					splatter.Xeno = uid;
					((EntitySystem)this).Dirty(spawned, (IComponent)(object)splatter, (MetaDataComponent)null);
					if (_xenoSprayAcidQuery.TryComp(uid, ref xenoSprayAcid))
					{
						spray._002Ector(uid, xenoSprayAcid);
						TryAcid(spray, _rmcMap.GetAnchoredEntitiesEnumerator(spawned, null, (DirectionFlag)0));
						if (active.Spawn.Count <= 1 && active.Blocker.HasValue)
						{
							TryAcid(spray, active.Blocker.Value);
							active.Blocker = null;
							((EntitySystem)this).Dirty(uid, (IComponent)(object)active, (MetaDataComponent)null);
						}
					}
					_onCollide.SetChain(Entity<DamageOnCollideComponent>.op_Implicit(spawned), active.Chain.Value);
					active.Spawn.RemoveAt(i);
				}
			}
			if (active.Spawn.Count == 0)
			{
				((EntitySystem)this).RemCompDeferred<ActiveAcidSprayingComponent>(uid);
			}
		}
		EntityQueryEnumerator<SprayAcidedComponent> acidedQuery = ((EntitySystem)this).EntityQueryEnumerator<SprayAcidedComponent>();
		EntityUid uid2 = default(EntityUid);
		SprayAcidedComponent acided = default(SprayAcidedComponent);
		while (acidedQuery.MoveNext(ref uid2, ref acided))
		{
			if (time >= acided.ExpireAt)
			{
				((EntitySystem)this).RemCompDeferred<SprayAcidedComponent>(uid2);
			}
			else if (!(time < acided.NextDamageAt))
			{
				acided.NextDamageAt = time + acided.DamageEvery;
				_damageable.TryChangeDamage(uid2, acided.Damage, ignoreResistances: false, interruptsDoAfters: true, null, uid2);
			}
		}
	}
}

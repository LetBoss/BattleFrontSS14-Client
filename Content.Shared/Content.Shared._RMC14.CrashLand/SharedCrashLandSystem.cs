using System;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Rules;
using Content.Shared.ActionBlocker;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Maps;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.ParaDrop;
using Content.Shared.Physics;
using Content.Shared.Shuttles.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.CrashLand;

public abstract class SharedCrashLandSystem : EntitySystem
{
	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	protected ActionBlockerSystem Blocker;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	protected DamageableSystem Damageable;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private TurfSystem _turf;

	protected static readonly ProtoId<DamageTypePrototype> CrashLandDamageType = ProtoId<DamageTypePrototype>.op_Implicit("Blunt");

	protected const int CrashLandDamageAmount = 10000;

	private bool _crashLandEnabled;

	private EntityQuery<CrashLandableComponent> _crashLandableQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_crashLandableQuery = ((EntitySystem)this).GetEntityQuery<CrashLandableComponent>();
		((EntitySystem)this).SubscribeLocalEvent<CrashLandableComponent, EntParentChangedMessage>((EntityEventRefHandler<CrashLandableComponent, EntParentChangedMessage>)OnCrashLandableParentChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CrashLandOnTouchComponent, StartCollideEvent>((EntityEventRefHandler<CrashLandOnTouchComponent, StartCollideEvent>)OnCrashLandOnTouchStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeleteCrashLandableOnTouchComponent, StartCollideEvent>((EntityEventRefHandler<DeleteCrashLandableOnTouchComponent, StartCollideEvent>)OnDeleteCrashLandableOnTouchStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CrashLandingComponent, UpdateCanMoveEvent>((EntityEventRefHandler<CrashLandingComponent, UpdateCanMoveEvent>)OnUpdateCanMove, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _config, RMCCVars.RMCFTLCrashLand, (Action<bool>)delegate(bool v)
		{
			_crashLandEnabled = v;
		}, true);
	}

	private void OnCrashLandableParentChanged(Entity<CrashLandableComponent> crashLandable, ref EntParentChangedMessage args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (_crashLandEnabled && ((EntitySystem)this).HasComp<FTLMapComponent>(((EntParentChangedMessage)(ref args)).Transform.ParentUid) && ((EntParentChangedMessage)(ref args)).OldParent.HasValue)
		{
			PullerComponent puller = default(PullerComponent);
			CrashLandableComponent pullingCrashLandable = default(CrashLandableComponent);
			if (((EntitySystem)this).TryComp<PullerComponent>(Entity<CrashLandableComponent>.op_Implicit(crashLandable), ref puller) && puller.Pulling.HasValue && _crashLandableQuery.TryComp(puller.Pulling.Value, ref pullingCrashLandable) && ShouldCrash(puller.Pulling.Value, ((EntParentChangedMessage)(ref args)).OldParent.Value))
			{
				TryCrashLand(Entity<CrashLandableComponent>.op_Implicit((puller.Pulling.Value, pullingCrashLandable)), doDamage: true);
			}
			if (ShouldCrash(Entity<CrashLandableComponent>.op_Implicit(crashLandable), ((EntParentChangedMessage)(ref args)).OldParent.Value))
			{
				TryCrashLand(Entity<CrashLandableComponent>.op_Implicit(crashLandable.Owner), doDamage: true);
			}
		}
	}

	private void OnCrashLandOnTouchStartCollide(Entity<CrashLandOnTouchComponent> ent, ref StartCollideEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		CrashLandableComponent crashLandable = default(CrashLandableComponent);
		if (_crashLandEnabled && _crashLandableQuery.TryGetComponent(args.OtherEntity, ref crashLandable))
		{
			AttemptCrashLandEvent ev = new AttemptCrashLandEvent(args.OtherEntity);
			((EntitySystem)this).RaiseLocalEvent<AttemptCrashLandEvent>(Entity<CrashLandOnTouchComponent>.op_Implicit(ent), ref ev, false);
			if (!ev.Cancelled)
			{
				TryCrashLand(Entity<CrashLandableComponent>.op_Implicit((args.OtherEntity, crashLandable)), doDamage: true);
			}
		}
	}

	private void OnDeleteCrashLandableOnTouchStartCollide(Entity<DeleteCrashLandableOnTouchComponent> ent, ref StartCollideEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && _crashLandEnabled && _crashLandableQuery.HasComp(args.OtherEntity))
		{
			((EntitySystem)this).QueueDel((EntityUid?)args.OtherEntity);
		}
	}

	private void OnUpdateCanMove(Entity<CrashLandingComponent> ent, ref UpdateCanMoveEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private bool ShouldCrash(EntityUid crashing, EntityUid oldParent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		AttemptCrashLandEvent ev = new AttemptCrashLandEvent(crashing);
		((EntitySystem)this).RaiseLocalEvent<AttemptCrashLandEvent>(oldParent, ref ev, false);
		if (ev.Cancelled)
		{
			return false;
		}
		return true;
	}

	public void ApplyFallingDamage(EntityUid uid)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier
		{
			DamageDict = { [ProtoId<DamageTypePrototype>.op_Implicit(CrashLandDamageType)] = 10000 }
		};
		Damageable.TryChangeDamage(uid, damage);
	}

	public bool IsLandableTile(Entity<MapGridComponent> grid, TileRef tileRef)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Invalid comparison between Unknown and I4
		Vector2i tile = tileRef.GridIndices;
		EntityCoordinates location = _mapSystem.GridTileToLocal(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), tile);
		if (_turf.GetContentTileDefinition(tileRef).ID == "Space")
		{
			return false;
		}
		if (_turf.IsSpace(tileRef) || _turf.IsTileBlocked(tileRef, CollisionGroup.MobMask))
		{
			return false;
		}
		if (!_area.CanParadrop(location))
		{
			return false;
		}
		EntityQuery<PhysicsComponent> physQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		bool valid = true;
		AnchoredEntitiesEnumerator anchored = _mapSystem.GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit(grid), grid.Comp, tile);
		EntityUid? ent = default(EntityUid?);
		PhysicsComponent body = default(PhysicsComponent);
		while (((AnchoredEntitiesEnumerator)(ref anchored)).MoveNext(ref ent))
		{
			if (physQuery.TryGetComponent(ent, ref body) && (int)body.BodyType == 4 && body.Hard && (body.CollisionLayer & 2) != 0)
			{
				valid = false;
				break;
			}
		}
		return valid;
	}

	public bool TryGetCrashLandLocation(out EntityCoordinates location)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		location = default(EntityCoordinates);
		EntityQueryEnumerator<RMCPlanetComponent> distressQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCPlanetComponent>();
		EntityUid grid = default(EntityUid);
		RMCPlanetComponent rMCPlanetComponent = default(RMCPlanetComponent);
		MapGridComponent gridComp = default(MapGridComponent);
		Vector2i tile = default(Vector2i);
		TileRef tileRef = default(TileRef);
		while (distressQuery.MoveNext(ref grid, ref rMCPlanetComponent))
		{
			if (!((EntitySystem)this).TryComp<MapGridComponent>(grid, ref gridComp))
			{
				return false;
			}
			TransformComponent xform = ((EntitySystem)this).Transform(grid);
			location = xform.Coordinates;
			for (int i = 0; i < 250; i++)
			{
				int randomX = _random.Next(-200, 200);
				int randomY = _random.Next(-200, 200);
				((Vector2i)(ref tile))._002Ector(randomX, randomY);
				if (_mapSystem.TryGetTileRef(grid, gridComp, tile, ref tileRef) && IsLandableTile(Entity<MapGridComponent>.op_Implicit((grid, gridComp)), tileRef))
				{
					location = _mapSystem.GridTileToLocal(grid, gridComp, tile);
					return true;
				}
			}
		}
		return false;
	}

	public void TryCrashLand(Entity<CrashLandableComponent?> crashLandable, bool doDamage)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && TryGetCrashLandLocation(out var location))
		{
			TryCrashLand(Entity<CrashLandableComponent>.op_Implicit(crashLandable.Owner), doDamage, location);
		}
	}

	public void TryCrashLand(Entity<CrashLandableComponent?> crashLandable, bool doDamage, EntityCoordinates location)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && ((EntitySystem)this).Resolve<CrashLandableComponent>(Entity<CrashLandableComponent>.op_Implicit(crashLandable), ref crashLandable.Comp, false) && !((EntitySystem)this).HasComp<CrashLandingComponent>(Entity<CrashLandableComponent>.op_Implicit(crashLandable)))
		{
			SkyFallingComponent skyFalling = ((EntitySystem)this).EnsureComp<SkyFallingComponent>(Entity<CrashLandableComponent>.op_Implicit(crashLandable));
			skyFalling.TargetCoordinates = location;
			((EntitySystem)this).Dirty(Entity<CrashLandableComponent>.op_Implicit(crashLandable), (IComponent)(object)skyFalling, (MetaDataComponent)null);
			CrashLandingComponent crashLanding = ((EntitySystem)this).EnsureComp<CrashLandingComponent>(Entity<CrashLandableComponent>.op_Implicit(crashLandable));
			crashLanding.DoDamage = doDamage;
			crashLanding.RemainingTime = crashLandable.Comp.CrashDuration;
			((EntitySystem)this).Dirty(Entity<CrashLandableComponent>.op_Implicit(crashLandable), (IComponent)(object)crashLanding, (MetaDataComponent)null);
			Blocker.UpdateCanMove(Entity<CrashLandableComponent>.op_Implicit(crashLandable));
			crashLandable.Comp.LastCrash = _timing.CurTime;
			((EntitySystem)this).Dirty<CrashLandableComponent>(crashLandable, (MetaDataComponent)null);
			_rmcPulling.TryStopAllPullsFromAndOn(Entity<CrashLandableComponent>.op_Implicit(crashLandable));
			CrashLandStartedEvent ev = default(CrashLandStartedEvent);
			((EntitySystem)this).RaiseLocalEvent<CrashLandStartedEvent>(Entity<CrashLandableComponent>.op_Implicit(crashLandable), ref ev, false);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityQueryEnumerator<CrashLandableComponent, CrashLandingComponent> crashLandingQuery = ((EntitySystem)this).EntityQueryEnumerator<CrashLandableComponent, CrashLandingComponent>();
		EntityUid uid = default(EntityUid);
		CrashLandableComponent crashLandable = default(CrashLandableComponent);
		CrashLandingComponent crashLanding = default(CrashLandingComponent);
		while (crashLandingQuery.MoveNext(ref uid, ref crashLandable, ref crashLanding))
		{
			if (((EntitySystem)this).HasComp<SkyFallingComponent>(uid))
			{
				continue;
			}
			crashLanding.RemainingTime -= frameTime;
			if (crashLanding.RemainingTime <= 0f)
			{
				if (crashLanding.DoDamage)
				{
					ApplyFallingDamage(uid);
				}
				CrashLandedEvent ev = new CrashLandedEvent(crashLanding.DoDamage);
				((EntitySystem)this).RaiseLocalEvent<CrashLandedEvent>(uid, ref ev, false);
				if (_net.IsServer)
				{
					_audio.PlayPvs(crashLandable.CrashSound, uid, (AudioParams?)null);
				}
				((EntitySystem)this).RemComp<CrashLandingComponent>(uid);
				Blocker.UpdateCanMove(uid);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.CrashLand;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Rules;
using Content.Shared.ActionBlocker;
using Content.Shared.Damage;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.ParaDrop;

public abstract class SharedParaDropSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	protected ActionBlockerSystem Blocker;

	[Dependency]
	private SharedCrashLandSystem _crashLand;

	[Dependency]
	private SharedDropshipSystem _dropship;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private const int CrashScatter = 7;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<CrashLandOnTouchComponent, AttemptCrashLandEvent>((EntityEventRefHandler<CrashLandOnTouchComponent, AttemptCrashLandEvent>)OnAttemptCrashLand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MapGridComponent, AttemptCrashLandEvent>((EntityEventRefHandler<MapGridComponent, AttemptCrashLandEvent>)OnAttemptCrashLand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrantParaDroppableComponent, GotEquippedEvent>((EntityEventRefHandler<GrantParaDroppableComponent, GotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrantParaDroppableComponent, GotUnequippedEvent>((EntityEventRefHandler<GrantParaDroppableComponent, GotUnequippedEvent>)OnGotUnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParaDroppingComponent, MapInitEvent>((EntityEventRefHandler<ParaDroppingComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParaDroppingComponent, ComponentShutdown>((EntityEventRefHandler<ParaDroppingComponent, ComponentShutdown>)OnComponentShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParaDroppingComponent, RMCIgniteAttemptEvent>((EntityEventRefHandler<ParaDroppingComponent, RMCIgniteAttemptEvent>)OnIgniteAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParaDroppingComponent, GettingAttackedAttemptEvent>((EntityEventRefHandler<ParaDroppingComponent, GettingAttackedAttemptEvent>)OnGettingAttacked, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParaDroppingComponent, AttemptMobCollideEvent>((EntityEventRefHandler<ParaDroppingComponent, AttemptMobCollideEvent>)OnAttemptMobCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParaDroppingComponent, AttemptMobTargetCollideEvent>((EntityEventRefHandler<ParaDroppingComponent, AttemptMobTargetCollideEvent>)OnAttemptMobTargetCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParaDroppingComponent, ThrowPushbackAttemptEvent>((EntityEventRefHandler<ParaDroppingComponent, ThrowPushbackAttemptEvent>)OnThrowPushbackAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParaDroppingComponent, BeforeDamageChangedEvent>((EntityEventRefHandler<ParaDroppingComponent, BeforeDamageChangedEvent>)OnBeforeDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParaDroppingComponent, UpdateCanMoveEvent>((EntityEventRefHandler<ParaDroppingComponent, UpdateCanMoveEvent>)OnUpdateCanMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SkyFallingComponent, ComponentShutdown>((EntityEventRefHandler<SkyFallingComponent, ComponentShutdown>)OnComponentShutdown, (Type[])null, (Type[])null);
	}

	private void OnGotEquipped(Entity<GrantParaDroppableComponent> ent, ref GotEquippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (ent.Comp.Slots & args.SlotFlags) != SlotFlags.NONE)
		{
			((EntitySystem)this).EnsureComp<ParaDroppableComponent>(args.Equipee);
		}
	}

	private void OnGotUnEquipped(Entity<GrantParaDroppableComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (ent.Comp.Slots & args.SlotFlags) != SlotFlags.NONE)
		{
			((EntitySystem)this).RemComp<ParaDroppableComponent>(args.Equipee);
		}
	}

	private void OnAttemptCrashLand(Entity<CrashLandOnTouchComponent> ent, ref AttemptCrashLandEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ActiveParaDropComponent paraDrop = default(ActiveParaDropComponent);
		if (_dropship.TryGetGridDropship(Entity<CrashLandOnTouchComponent>.op_Implicit(ent), out Entity<DropshipComponent> dropShip) && (((EntitySystem)this).TryComp<ActiveParaDropComponent>(Entity<DropshipComponent>.op_Implicit(dropShip), ref paraDrop) || ((EntitySystem)this).HasComp<ParaDroppableComponent>(args.Crashing)))
		{
			args.Cancelled = true;
			AttemptParaDrop(Entity<ActiveParaDropComponent>.op_Implicit((Entity<DropshipComponent>.op_Implicit(dropShip), paraDrop)), args.Crashing);
		}
	}

	private void OnAttemptCrashLand(Entity<MapGridComponent> ent, ref AttemptCrashLandEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ActiveParaDropComponent paraDrop = default(ActiveParaDropComponent);
		if (_dropship.TryGetGridDropship(Entity<MapGridComponent>.op_Implicit(ent), out Entity<DropshipComponent> dropShip) && (((EntitySystem)this).TryComp<ActiveParaDropComponent>(Entity<DropshipComponent>.op_Implicit(dropShip), ref paraDrop) || ((EntitySystem)this).HasComp<ParaDroppableComponent>(args.Crashing)))
		{
			args.Cancelled = true;
			AttemptParaDrop(Entity<ActiveParaDropComponent>.op_Implicit((Entity<DropshipComponent>.op_Implicit(dropShip), paraDrop)), args.Crashing);
		}
	}

	private void OnMapInit(Entity<ParaDroppingComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		FixturesComponent fixtures = default(FixturesComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(Entity<ParaDroppingComponent>.op_Implicit(ent), ref physics) || !((EntitySystem)this).TryComp<FixturesComponent>(Entity<ParaDroppingComponent>.op_Implicit(ent), ref fixtures))
		{
			return;
		}
		foreach (KeyValuePair<string, Fixture> fixture in fixtures.Fixtures)
		{
			ent.Comp.OriginalLayers.TryAdd(fixture.Key, fixture.Value.CollisionLayer);
			ent.Comp.OriginalMasks.TryAdd(fixture.Key, fixture.Value.CollisionMask);
			_physics.SetCollisionLayer(Entity<ParaDroppingComponent>.op_Implicit(ent), fixture.Key, fixture.Value, 0, (FixturesComponent)null, (PhysicsComponent)null);
			_physics.SetCollisionMask(Entity<ParaDroppingComponent>.op_Implicit(ent), fixture.Key, fixture.Value, 0, (FixturesComponent)null, (PhysicsComponent)null);
		}
		((EntitySystem)this).Dirty<ParaDroppingComponent>(ent, (MetaDataComponent)null);
	}

	private void OnComponentShutdown(Entity<ParaDroppingComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		FixturesComponent fixtures = default(FixturesComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(Entity<ParaDroppingComponent>.op_Implicit(ent), ref physics) || !((EntitySystem)this).TryComp<FixturesComponent>(Entity<ParaDroppingComponent>.op_Implicit(ent), ref fixtures))
		{
			return;
		}
		foreach (KeyValuePair<string, Fixture> fixture in fixtures.Fixtures)
		{
			if (ent.Comp.OriginalLayers.TryGetValue(fixture.Key, out var originalLayer) && ent.Comp.OriginalMasks.TryGetValue(fixture.Key, out var originalMask))
			{
				_physics.SetCollisionLayer(Entity<ParaDroppingComponent>.op_Implicit(ent), fixture.Key, fixture.Value, originalLayer, (FixturesComponent)null, (PhysicsComponent)null);
				_physics.SetCollisionMask(Entity<ParaDroppingComponent>.op_Implicit(ent), fixture.Key, fixture.Value, originalMask, (FixturesComponent)null, (PhysicsComponent)null);
			}
		}
	}

	private void OnComponentShutdown(Entity<SkyFallingComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.TargetCoordinates.HasValue)
		{
			_transform.SetMapCoordinates(Entity<SkyFallingComponent>.op_Implicit(ent), _transform.ToMapCoordinates(ent.Comp.TargetCoordinates.Value, true));
			ParaDroppableComponent paraDroppable = default(ParaDroppableComponent);
			if (((EntitySystem)this).TryComp<ParaDroppableComponent>(Entity<SkyFallingComponent>.op_Implicit(ent), ref paraDroppable))
			{
				_audio.PlayPvs(paraDroppable.DropSound, Entity<SkyFallingComponent>.op_Implicit(ent), (AudioParams?)null);
			}
		}
	}

	private void OnIgniteAttempt(Entity<ParaDroppingComponent> ent, ref RMCIgniteAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnAttemptMobCollide(Entity<ParaDroppingComponent> ent, ref AttemptMobCollideEvent args)
	{
		args.Cancelled = true;
	}

	private void OnAttemptMobTargetCollide(Entity<ParaDroppingComponent> ent, ref AttemptMobTargetCollideEvent args)
	{
		args.Cancelled = true;
	}

	private void OnGettingAttacked(Entity<ParaDroppingComponent> ent, ref GettingAttackedAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnThrowPushbackAttempt(Entity<ParaDroppingComponent> ent, ref ThrowPushbackAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnBeforeDamageChanged(Entity<ParaDroppingComponent> ent, ref BeforeDamageChangedEvent args)
	{
		args.Cancelled = true;
	}

	private void OnUpdateCanMove(Entity<ParaDroppingComponent> ent, ref UpdateCanMoveEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void AttemptParaDrop(Entity<ActiveParaDropComponent?> dropShip, EntityUid dropping)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || ((EntitySystem)this).HasComp<ParaDroppingComponent>(dropping))
		{
			return;
		}
		EntityUid? dropTarget = null;
		ActiveParaDropComponent comp = dropShip.Comp;
		if (comp != null && comp.DropTarget.HasValue)
		{
			dropTarget = dropShip.Comp.DropTarget;
		}
		if (!dropTarget.HasValue)
		{
			EntityCoordinates? randomCoordinates = null;
			if (_crashLand.TryGetCrashLandLocation(out var location))
			{
				randomCoordinates = location;
			}
			if (!randomCoordinates.HasValue)
			{
				_popup.PopupClient("Your harness got stuck and is preventing your from jumping", dropping, PopupType.SmallCaution);
			}
			else
			{
				TryDrop(dropping, randomCoordinates.Value);
			}
		}
		else
		{
			TryDrop(dropping, _transform.GetMoverCoordinates(dropTarget.Value));
		}
	}

	private bool TryDrop(EntityUid dropping, EntityCoordinates dropCoordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		ParaDroppableComponent paraDroppable = default(ParaDroppableComponent);
		if (!((EntitySystem)this).TryComp<ParaDroppableComponent>(dropping, ref paraDroppable))
		{
			if (TryGetParaDropLocation(dropCoordinates, 7, out var adjustedCrashCoordinates))
			{
				dropCoordinates = adjustedCrashCoordinates;
			}
			_crashLand.TryCrashLand(Entity<CrashLandableComponent>.op_Implicit(dropping), doDamage: true, dropCoordinates);
			return false;
		}
		paraDroppable.LastParaDrop = _timing.CurTime;
		((EntitySystem)this).Dirty(dropping, (IComponent)(object)paraDroppable, (MetaDataComponent)null);
		_rmcPulling.TryStopAllPullsFromAndOn(dropping);
		PhysicsComponent physics = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(dropping, ref physics))
		{
			_physics.SetLinearVelocity(dropping, Vector2.Zero, true, true, (FixturesComponent)null, physics);
		}
		if (TryGetParaDropLocation(dropCoordinates, paraDroppable.DropScatter, out var adjustedCoordinates))
		{
			dropCoordinates = adjustedCoordinates;
		}
		SkyFallingComponent skyFalling = ((EntitySystem)this).EnsureComp<SkyFallingComponent>(dropping);
		skyFalling.TargetCoordinates = dropCoordinates;
		((EntitySystem)this).Dirty(dropping, (IComponent)(object)skyFalling, (MetaDataComponent)null);
		ParaDroppingComponent droppingComp = ((EntitySystem)this).EnsureComp<ParaDroppingComponent>(dropping);
		droppingComp.RemainingTime = paraDroppable.DropDuration;
		((EntitySystem)this).Dirty(dropping, (IComponent)(object)droppingComp, (MetaDataComponent)null);
		Blocker.UpdateCanMove(dropping);
		return true;
	}

	private bool TryGetParaDropLocation(EntityCoordinates targetLocation, int dropScatter, out EntityCoordinates adjustedLocation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		adjustedLocation = default(EntityCoordinates);
		EntityUid grid = default(EntityUid);
		RMCPlanetComponent rMCPlanetComponent = default(RMCPlanetComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<RMCPlanetComponent>().MoveNext(ref grid, ref rMCPlanetComponent))
		{
			MapGridComponent gridComp = default(MapGridComponent);
			if (!((EntitySystem)this).TryComp<MapGridComponent>(grid, ref gridComp))
			{
				return false;
			}
			Vector2i position = _mapSystem.LocalToTile(grid, gridComp, targetLocation);
			Box2 dropArea = default(Box2);
			((Box2)(ref dropArea))._002Ector((float)(position.X - dropScatter), (float)(position.Y - dropScatter), (float)(position.X + dropScatter), (float)(position.Y + dropScatter));
			TilesEnumerator enumerable = _mapSystem.GetTilesEnumerator(grid, gridComp, dropArea, true, (Predicate<TileRef>)null);
			List<TileRef> viableTiles = new List<TileRef>();
			TileRef tileRef = default(TileRef);
			while (((TilesEnumerator)(ref enumerable)).MoveNext(ref tileRef))
			{
				if (_crashLand.IsLandableTile(Entity<MapGridComponent>.op_Implicit((grid, gridComp)), tileRef))
				{
					viableTiles.Add(tileRef);
				}
			}
			if (viableTiles.Count == 0)
			{
				return false;
			}
			int random = _random.Next(0, viableTiles.Count);
			adjustedLocation = _mapSystem.GridTileToLocal(grid, gridComp, viableTiles[random].GridIndices);
			return true;
		}
		return false;
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<ActiveParaDropComponent, DropshipComponent> dropshipQuery = ((EntitySystem)this).EntityQueryEnumerator<ActiveParaDropComponent, DropshipComponent>();
		EntityUid uid = default(EntityUid);
		ActiveParaDropComponent paraDrop = default(ActiveParaDropComponent);
		DropshipComponent dropship = default(DropshipComponent);
		while (dropshipQuery.MoveNext(ref uid, ref paraDrop, ref dropship))
		{
			if (dropship.State == FTLState.Arriving || !((EntitySystem)this).HasComp<DropshipTargetComponent>(paraDrop.DropTarget))
			{
				((EntitySystem)this).RemComp<ActiveParaDropComponent>(uid);
			}
		}
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityQueryEnumerator<ParaDroppingComponent> paraDroppingQuery = ((EntitySystem)this).EntityQueryEnumerator<ParaDroppingComponent>();
		EntityUid uid2 = default(EntityUid);
		ParaDroppingComponent paraDropping = default(ParaDroppingComponent);
		while (paraDroppingQuery.MoveNext(ref uid2, ref paraDropping))
		{
			if (!((EntitySystem)this).HasComp<SkyFallingComponent>(uid2))
			{
				paraDropping.RemainingTime -= frameTime;
				if (paraDropping.RemainingTime <= 0f)
				{
					((EntitySystem)this).RemComp<ParaDroppingComponent>(uid2);
				}
				Blocker.UpdateCanMove(uid2);
			}
		}
		EntityQueryEnumerator<SkyFallingComponent> skyFallingQuery = ((EntitySystem)this).EntityQueryEnumerator<SkyFallingComponent>();
		EntityUid uid3 = default(EntityUid);
		SkyFallingComponent skyFalling = default(SkyFallingComponent);
		while (skyFallingQuery.MoveNext(ref uid3, ref skyFalling))
		{
			skyFalling.RemainingTime -= frameTime;
			if (skyFalling.RemainingTime <= 0f)
			{
				((EntitySystem)this).RemComp<SkyFallingComponent>(uid3);
			}
		}
	}
}

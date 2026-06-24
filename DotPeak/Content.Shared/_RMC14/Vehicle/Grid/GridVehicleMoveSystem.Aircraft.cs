// Decompiled with JetBrains decompiler
// Type: Content.Shared.Vehicle.GridVehicleMoverSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Aircraft;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Vehicle;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Destructible;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Foldable;
using Content.Shared.Item;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Physics;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Vehicle.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.Vehicle;

public sealed class GridVehicleMoverSystem : EntitySystem
{
  [Dependency]
  private readonly SharedTransformSystem transform;
  [Dependency]
  private readonly SharedMapSystem map;
  [Dependency]
  private readonly SharedPhysicsSystem physics;
  [Dependency]
  private readonly EntityLookupSystem lookup;
  [Dependency]
  private readonly IGameTiming _timing;
  [Dependency]
  private readonly SharedAudioSystem _audio;
  [Dependency]
  private readonly DamageableSystem _damageable;
  [Dependency]
  private readonly SharedDoorSystem _door;
  [Dependency]
  private readonly MobStateSystem _mobState;
  [Dependency]
  private readonly INetManager _net;
  [Dependency]
  private readonly ISharedPlayerManager _player;
  [Dependency]
  private readonly StandingStateSystem _standing;
  [Dependency]
  private readonly SharedStunSystem _stun;
  [Dependency]
  private readonly RMCSizeStunSystem _size;
  [Dependency]
  private readonly VehicleWheelSystem _wheels;
  [Dependency]
  private readonly SharedDestructibleSystem _destructible;
  [Dependency]
  private readonly SharedRMCPowerSystem _rmcPower;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> gridQ;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> physicsQ;
  private Robust.Shared.GameObjects.EntityQuery<FixturesComponent> fixtureQ;
  private const float Clearance = 0.0075f;
  private const double MobCollisionDamage = 8.0;
  private static readonly TimeSpan MobCollisionKnockdown = TimeSpan.FromSeconds(1.5);
  private static readonly TimeSpan MobCollisionCooldown = TimeSpan.FromSeconds(0.75);
  private static readonly ProtoId<DamageTypePrototype> CollisionDamageType = (ProtoId<DamageTypePrototype>) "Blunt";
  private const int GridVehicleStaticBlockerMask = 335544350;
  private const CollisionGroup GridVehiclePushHardBlockMask = CollisionGroup.MobMask | CollisionGroup.DropshipImpassable;
  private const float PushTileBlockFraction = 0.005f;
  private const float PushOverlapEpsilon = 0.05f;
  private const float PushAxisHysteresis = 0.05f;
  private const float PushWallSkin = 0.1f;
  private const float PushWallOverlapArea = 0.01f;
  private const float MovementFixedStep = 0.0166666675f;
  private const int MaxFixedStepsPerFrame = 6;
  private const float ClientSmoothingSnapDistance = 1.25f;
  private const float ClientSmoothingRate = 22f;
  private const float ClientSmoothingSnapAngle = 1f;
  public static readonly List<(EntityUid grid, Vector2i tile)> DebugTestedTiles = new List<(EntityUid, Vector2i)>();
  public static readonly List<GridVehicleMoverSystem.DebugCollisionProbe> DebugCollisionProbes = new List<GridVehicleMoverSystem.DebugCollisionProbe>();
  public static readonly List<GridVehicleMoverSystem.DebugCollision> DebugCollisions = new List<GridVehicleMoverSystem.DebugCollision>();
  public static readonly List<GridVehicleMoverSystem.DebugMovementDecision> DebugMovementDecisions = new List<GridVehicleMoverSystem.DebugMovementDecision>();
  public static bool ForceAllFreeMovement = true;
  private readonly Dictionary<EntityUid, TimeSpan> _lastMobCollision = new Dictionary<EntityUid, TimeSpan>();
  private readonly Dictionary<EntityUid, bool> _hardState = new Dictionary<EntityUid, bool>();
  private readonly Dictionary<EntityUid, bool> _lastMobPushAxis = new Dictionary<EntityUid, bool>();
  private readonly Dictionary<EntityUid, float> _movementAccumulator = new Dictionary<EntityUid, float>();
  private readonly Dictionary<EntityUid, EntityUid> _activeXenoPushers = new Dictionary<EntityUid, EntityUid>();
  private readonly HashSet<EntityUid> _directMoveBlockers = new HashSet<EntityUid>();
  private readonly HashSet<EntityUid> _pushIgnoredEntities = new HashSet<EntityUid>();
  private readonly HashSet<EntityUid> _depenetrateBlockers = new HashSet<EntityUid>();
  private const float StuckTurnFactor = 0.4f;
  private const float MinVehicleSpeed = 0.01f;
  private const float MinMoveDistance = 0.0001f;
  [Dependency]
  private readonly TagSystem _tags;
  [Dependency]
  private readonly ITileDefinitionManager _tileDefinitions;
  [Dependency]
  private readonly TileSystem _tileSystem;
  [Dependency]
  private readonly IRobustRandom _random;
  private readonly Dictionary<(EntityUid Grid, Vector2i Pos), GridVehicleMoverSystem.TrackRestore> _trackRestores = new Dictionary<(EntityUid, Vector2i), GridVehicleMoverSystem.TrackRestore>();
  private TimeSpan _nextTrackRestoreScan;
  private readonly List<(EntityUid Grid, Vector2i Pos)> _dueTrackRestores = new List<(EntityUid, Vector2i)>();

  public bool CanOccupyCurrent(EntityUid uid)
  {
    GridVehicleMoverComponent comp;
    if (!this.TryComp<GridVehicleMoverComponent>(uid, out comp))
      return false;
    EntityUid? gridUid = this.Transform(uid).GridUid;
    if (!gridUid.HasValue)
      return false;
    EntityUid valueOrDefault = gridUid.GetValueOrDefault();
    return this.CanOccupyTransform(uid, comp, valueOrDefault, comp.Position, new Angle?(comp.Heading), 0.0075f, false, false);
  }

  private void UpdateAircraftMovement(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    VehicleComponent vehicle,
    EntityUid grid,
    MapGridComponent gridComp,
    float frameTime)
  {
    if (vehicle.Operator.HasValue)
    {
      VehicleCanRunEvent args = new VehicleCanRunEvent((Entity<VehicleComponent>) (uid, vehicle));
      this.RaiseLocalEvent<VehicleCanRunEvent>(uid, ref args);
      if (!args.CanRun)
      {
        this.StopMover(mover);
        this.SetGridPosition(uid, grid, mover.Position);
        this.transform.SetLocalRotation(uid, mover.Heading);
        this.Dirty(uid, (IComponent) mover);
        return;
      }
    }
    double slowdownMultiplier = (double) this.GetSmashSlowdownMultiplier(mover);
    AircraftComponent comp1;
    this.TryComp<AircraftComponent>(uid, out comp1);
    bool flag = comp1 != null && comp1.Airborne;
    int throttle = 0;
    int num1 = 0;
    EntityUid? nullable = vehicle.Operator;
    InputMoverComponent comp2;
    if (nullable.HasValue && this.TryComp<InputMoverComponent>(nullable.GetValueOrDefault(), out comp2))
    {
      int heldMoveButtons = (int) comp2.HeldMoveButtons;
      if ((heldMoveButtons & 1) != 0)
        ++throttle;
      if ((heldMoveButtons & 2) != 0)
        --throttle;
      if ((heldMoveButtons & 8) != 0)
        ++num1;
      if ((heldMoveButtons & 4) != 0)
        --num1;
    }
    float accelerationModifier = this.GetAccelerationModifier(uid);
    float deceleration = comp1 != null ? comp1.BrakeDeceleration : mover.Deceleration;
    mover.CurrentSpeed = GridVehicleMotionSimulator.StepFreeSpeed(mover.CurrentSpeed, throttle, this.GetModifiedMaxSpeed(uid, mover), 0.0f, mover.Acceleration * accelerationModifier, 0.0f, deceleration, false, frameTime);
    if (flag && comp1 != null && (double) mover.CurrentSpeed < (double) comp1.StallSpeed)
      mover.CurrentSpeed = comp1.StallSpeed;
    if (num1 != 0)
    {
      float num2 = MathF.Max(0.01f, mover.TurnFullSpeed);
      float x = Math.Clamp(MathF.Abs(mover.CurrentSpeed) / num2, 0.0f, 1f);
      if (flag)
        x = MathF.Max(x, comp1 != null ? comp1.MinAirTurnFactor : 0.5f);
      else if (throttle != 0)
        x = MathF.Max(x, 0.4f);
      if ((double) x > 0.0)
      {
        float radians = MathHelper.DegreesToRadians(mover.TurnRate);
        float num3 = (float) -num1 * radians * x * frameTime;
        Angle angle1 = new Angle(mover.Heading.Theta + (double) num3);
        Angle angle2 = ((Angle) ref angle1).Reduced();
        if (flag || this.CanOccupyTransform(uid, mover, grid, mover.Position, new Angle?(angle2), 0.0075f, false, false))
          mover.Heading = angle2;
      }
    }
    if ((double) MathF.Abs(mover.CurrentSpeed) > 0.0099999997764825821)
    {
      Vector2 worldVec = ((Angle) ref mover.Heading).ToWorldVec();
      float num4 = mover.CurrentSpeed * frameTime;
      Vector2 target = mover.Position + worldVec * num4;
      if (flag)
      {
        mover.Position = target;
      }
      else
      {
        bool blocked;
        this.TryMoveContinuous(uid, mover, grid, target, new Angle?(mover.Heading), out blocked);
        if (blocked)
          mover.CurrentSpeed = 0.0f;
      }
    }
    this.UpdateDerivedTileState(grid, gridComp, mover);
    mover.IsPushMove = false;
    mover.IsMoving = (double) MathF.Abs(mover.CurrentSpeed) > 0.0099999997764825821;
    this.UpdateRunSound(uid, mover);
    this.transform.SetLocalRotation(uid, mover.Heading);
    this.SetGridPosition(uid, grid, mover.Position);
    if (mover.IsMoving)
      this.physics.WakeBody(uid);
    this.Dirty(uid, (IComponent) mover);
  }

  private bool CanOccupyTransform(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    Vector2 gridPos,
    Angle? overrideRotation,
    float clearance,
    bool applyEffects,
    bool debug = true,
    HashSet<EntityUid>? blockers = null,
    HashSet<EntityUid>? ignoredEntities = null)
  {
    PhysicsComponent component1;
    FixturesComponent fixtures;
    if (!this.physicsQ.TryComp(uid, out component1) || !this.fixtureQ.TryComp(uid, out fixtures))
      return true;
    EntityUid? nullable = new EntityUid?();
    VehicleComponent comp1;
    if (this.TryComp<VehicleComponent>(uid, out comp1))
      nullable = comp1.Operator;
    MapGridComponent component2;
    if (!component1.CanCollide || !this.gridQ.TryComp(grid, out component2))
      return true;
    EntityCoordinates coords = new EntityCoordinates(grid, gridPos);
    MapCoordinates world = coords.ToMap((IEntityManager) this.EntityManager, this.transform);
    bool debugEnabled = debug && GridVehicleMoverSystem.CollisionDebugEnabled;
    if (debugEnabled)
    {
      Vector2i vector2i = this.map.TileIndicesFor(grid, component2, coords);
      GridVehicleMoverSystem.DebugTestedTiles.Add((grid, vector2i));
    }
    Angle collisionWorldRotation = this.GetCollisionWorldRotation(uid, grid, overrideRotation);
    Robust.Shared.Physics.Transform tx = new Robust.Shared.Physics.Transform(world.Position, collisionWorldRotation);
    float wheelDamage = this._net.IsClient ? 0.0f : this.GetWheelCollisionDamage(uid, mover);
    Box2 aabb;
    if (!GridVehicleMoverSystem.TryGetFixtureAabb(fixtures, tx, out aabb))
      return true;
    Box2 movementAabb = GridVehicleMoverSystem.GetMovementAabb(aabb, mover);
    GridVehicleMoverSystem.VehicleOrientedBox? orientedBox = new GridVehicleMoverSystem.VehicleOrientedBox?();
    bool flag;
    if (comp1 != null)
    {
      switch (comp1.MovementKind)
      {
        case VehicleMovementKind.Free:
        case VehicleMovementKind.Aircraft:
          flag = true;
          goto label_14;
      }
    }
    flag = false;
label_14:
    Box2 aabb1;
    if ((flag || GridVehicleMoverSystem.ForceAllFreeMovement) && GridVehicleMoverSystem.TryGetFixtureLocalAabb(fixtures, out aabb1))
    {
      float num = Math.Clamp(mover.MovementCollisionInset, 0.0f, 0.45f);
      Box2 box2 = ((Box2) ref aabb1).Enlarged(-num);
      if (!((Box2) ref box2).IsValid())
        box2 = aabb1;
      ref GridVehicleMoverSystem.VehicleOrientedBox? local1 = ref orientedBox;
      Vector2 position = world.Position;
      ref Angle local2 = ref collisionWorldRotation;
      Vector2 center = ((Box2) ref aabb1).Center;
      ref Vector2 local3 = ref center;
      Vector2 vector2 = ((Angle) ref local2).RotateVec(ref local3);
      GridVehicleMoverSystem.VehicleOrientedBox vehicleOrientedBox = new GridVehicleMoverSystem.VehicleOrientedBox(position + vector2, collisionWorldRotation, ((Box2) ref aabb1).Size / 2f, ((Box2) ref box2).Size / 2f);
      local1 = new GridVehicleMoverSystem.VehicleOrientedBox?(vehicleOrientedBox);
    }
    HashSet<EntityUid> entitiesIntersecting = this.lookup.GetEntitiesIntersecting(world.MapId, aabb, LookupFlags.Dynamic | LookupFlags.Static);
    bool playedCollisionSound = false;
    ValueList<EntityUid> valueList = new ValueList<EntityUid>(0);
    foreach (EntityUid other in entitiesIntersecting)
    {
      GridVehicleMoverSystem.CollisionCandidate candidate;
      if (!(other == uid) && (ignoredEntities == null || !ignoredEntities.Contains(other)) && this.TryBuildCollisionCandidate(uid, fixtures, component1, other, aabb, movementAabb, nullable, orientedBox, out candidate))
      {
        if (candidate.CollisionClass == GridVehicleMoverSystem.VehicleCollisionClass.SoftMob && candidate.IsXeno)
        {
          if (this.HandleSoftXenoCollision(uid, mover, grid, world.Position, world.MapId, candidate.Entity, aabb, candidate.Aabb, candidate.CollisionAabb, clearance, applyEffects, debugEnabled, blockers, wheelDamage, ref playedCollisionSound) == GridVehicleMoverSystem.CollisionHandlingResult.Blocked)
          {
            AddProbe(true);
            return false;
          }
        }
        else if (candidate.CollisionClass != GridVehicleMoverSystem.VehicleCollisionClass.SoftMob || candidate.MobState == null || !this._standing.IsDown(candidate.Entity))
        {
          if (applyEffects)
          {
            DoorComponent door = candidate.Door;
            if (door != null && !this._net.IsClient && !candidate.IsUnpoweredDoor)
            {
              this._door.TryOpen(candidate.Entity, door, nullable);
              if (candidate.IsBarricade)
                this._door.OnPartialOpen(candidate.Entity, door);
            }
          }
          if (candidate.CollisionClass != GridVehicleMoverSystem.VehicleCollisionClass.Ignore)
          {
            if (candidate.CollisionClass == GridVehicleMoverSystem.VehicleCollisionClass.Breakable)
            {
              if (this.HandleBreakableCollision(uid, mover, candidate.Entity, candidate.CollisionAabb, candidate.Aabb, clearance, world.MapId, candidate.Door != null, candidate.IsUnpoweredDoor, applyEffects, debugEnabled, blockers, wheelDamage, ref playedCollisionSound) == GridVehicleMoverSystem.CollisionHandlingResult.Blocked)
              {
                AddProbe(true);
                return false;
              }
            }
            else if (candidate.CollisionClass == GridVehicleMoverSystem.VehicleCollisionClass.Hard)
            {
              if (this.HandleHardCollision(uid, mover, grid, gridPos, candidate.Entity, candidate.CollisionAabb, candidate.Aabb, clearance, world.MapId, candidate.IsVehicle, applyEffects, debugEnabled, blockers, wheelDamage, ref playedCollisionSound) == GridVehicleMoverSystem.CollisionHandlingResult.Blocked)
              {
                AddProbe(true);
                return false;
              }
            }
            else
            {
              if (applyEffects && this._net.IsClient && !candidate.IsXeno && candidate.MobState != null && this.ShouldPredictVehicleInteractions(uid))
                this.PredictRunover(uid, candidate.Entity, candidate.MobState);
              if (applyEffects && !this._net.IsClient && candidate.MobState != null && !valueList.Contains(candidate.Entity))
                valueList.Add(candidate.Entity);
            }
          }
        }
      }
    }
    if (!this._net.IsClient && valueList.Count > 0)
    {
      foreach (EntityUid entityUid in valueList)
      {
        MobStateComponent comp2;
        if (this.TryComp<MobStateComponent>(entityUid, out comp2))
          this.HandleMobCollision(uid, entityUid, comp2, ref playedCollisionSound);
      }
    }
    AddProbe(false);
    return true;

    void AddProbe(bool probeBlocked)
    {
      if (!debugEnabled)
        return;
      GridVehicleMoverSystem.AddDebugCollisionProbe(uid, mover, fixtures, tx, aabb, movementAabb, world.MapId, probeBlocked, applyEffects);
    }
  }

  private bool TryBuildCollisionCandidate(
    EntityUid vehicle,
    FixturesComponent vehicleFixtures,
    PhysicsComponent vehicleBody,
    EntityUid other,
    Box2 vehicleAabb,
    Box2 movementAabb,
    EntityUid? operatorUid,
    GridVehicleMoverSystem.VehicleOrientedBox? orientedBox,
    out GridVehicleMoverSystem.CollisionCandidate candidate)
  {
    candidate = new GridVehicleMoverSystem.CollisionCandidate();
    TransformComponent transformComponent = this.Transform(other);
    PhysicsComponent component1;
    if (!transformComponent.Anchored && this.HasComp<ItemComponent>(other) || !this.physicsQ.TryComp(other, out component1) || !component1.CanCollide)
      return false;
    DoorComponent comp1;
    bool hasDoor = this.TryComp<DoorComponent>(other, out comp1);
    bool flag1 = this.HasComp<BarricadeComponent>(other);
    bool isFoldable = this.HasComp<FoldableComponent>(other);
    MobStateComponent comp2;
    bool isMob = this.TryComp<MobStateComponent>(other, out comp2);
    bool flag2 = this.HasComp<XenoComponent>(other);
    bool flag3 = this.HasComp<VehicleComponent>(other);
    bool isSmashable = this.HasComp<VehicleSmashableComponent>(other);
    FixturesComponent component2;
    if (!isMob && !flag2 && !transformComponent.Anchored && component1.BodyType != BodyType.Static && !flag1 && !isFoldable && !flag3 && !isSmashable || !this.fixtureQ.TryComp(other, out component2))
      return false;
    Robust.Shared.Physics.Transform physicsTransform = this.physics.GetPhysicsTransform(other, transformComponent);
    Box2 aabb1;
    if (!GridVehicleMoverSystem.TryGetFixtureAabb(component2, physicsTransform, out aabb1) || !((Box2) ref vehicleAabb).Intersects(ref aabb1))
      return false;
    bool hardCollidable = this.physics.IsHardCollidable((Entity<FixturesComponent, PhysicsComponent>) (vehicle, vehicleFixtures, vehicleBody), (Entity<FixturesComponent, PhysicsComponent>) (other, component2, component1));
    GridVehicleMoverSystem.VehicleCollisionClass vehicleCollisionClass = this.ClassifyCollisionCandidate(other, transformComponent, component1, component2, hardCollidable, isMob, flag1, isFoldable, hasDoor, flag2, flag3, isSmashable);
    bool powered;
    bool doorPowered = this.TryGetDoorPowered(other, out powered);
    bool IsUnpoweredDoor = hasDoor & doorPowered && !powered;
    if (hasDoor & doorPowered & powered && comp1 != null && this._door.CanOpen(other, comp1, operatorUid))
      vehicleCollisionClass = GridVehicleMoverSystem.VehicleCollisionClass.Ignore;
    Box2 collisionAabb = GridVehicleMoverSystem.GetCollisionAabb(vehicleCollisionClass, vehicleAabb, movementAabb);
    if (!GridVehicleMoverSystem.HasCollisionOverlap(collisionAabb, aabb1))
      return false;
    if (orientedBox.HasValue)
    {
      GridVehicleMoverSystem.VehicleOrientedBox valueOrDefault = orientedBox.GetValueOrDefault();
      Vector2 vector2_1 = vehicleCollisionClass == GridVehicleMoverSystem.VehicleCollisionClass.SoftMob ? valueOrDefault.FullHalf : valueOrDefault.MovementHalf;
      Box2 aabb2;
      if (flag3 && GridVehicleMoverSystem.TryGetFixtureLocalAabb(component2, out aabb2))
      {
        Angle rotB;
        // ISSUE: explicit constructor call
        ((Angle) ref rotB).\u002Ector((double) physicsTransform.Quaternion2D.Angle);
        Vector2 position = physicsTransform.Position;
        ref Angle local1 = ref rotB;
        Vector2 center = ((Box2) ref aabb2).Center;
        ref Vector2 local2 = ref center;
        Vector2 vector2_2 = ((Angle) ref local1).RotateVec(ref local2);
        Vector2 centerB = position + vector2_2;
        if (!GridVehicleMoverSystem.OrientedBoxIntersectsOrientedBox(valueOrDefault.Center, vector2_1, valueOrDefault.Rotation, centerB, ((Box2) ref aabb2).Size / 2f, rotB))
          return false;
      }
      else if (!GridVehicleMoverSystem.OrientedBoxIntersectsAabb(valueOrDefault.Center, vector2_1, valueOrDefault.Rotation, aabb1))
        return false;
    }
    candidate = new GridVehicleMoverSystem.CollisionCandidate(other, aabb1, collisionAabb, vehicleCollisionClass, comp1, comp2, flag1, flag2, flag3, IsUnpoweredDoor);
    return true;
  }

  private GridVehicleMoverSystem.CollisionHandlingResult HandleSoftXenoCollision(
    EntityUid vehicle,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    Vector2 vehicleWorldPosition,
    MapId mapId,
    EntityUid xeno,
    Box2 vehicleAabb,
    Box2 xenoAabb,
    Box2 collisionAabb,
    float clearance,
    bool applyEffects,
    bool debug,
    HashSet<EntityUid>? blockers,
    float wheelDamage,
    ref bool playedCollisionSound)
  {
    if (this.ShouldBlockXeno(mover, xeno))
    {
      if (applyEffects)
      {
        this.PlayMobCollisionSound(vehicle, ref playedCollisionSound);
        this.ApplyWheelCollisionDamage(vehicle, mover, wheelDamage);
      }
      GridVehicleMoverSystem.AddBlockingCollision(vehicle, xeno, collisionAabb, xenoAabb, clearance, mapId, debug, blockers);
      return GridVehicleMoverSystem.CollisionHandlingResult.Blocked;
    }
    if (!applyEffects)
      return GridVehicleMoverSystem.CollisionHandlingResult.Continue;
    this.PlayMobCollisionSound(vehicle, ref playedCollisionSound);
    Vector2 vehicleMoveDelta = this.GetVehicleMoveDelta(grid, vehicleWorldPosition, mapId, mover);
    if (this.PushMobOutOfVehicle(vehicle, xeno, vehicleAabb, xenoAabb, vehicleMoveDelta))
      return GridVehicleMoverSystem.CollisionHandlingResult.Continue;
    this.ApplyWheelCollisionDamage(vehicle, mover, wheelDamage);
    GridVehicleMoverSystem.AddBlockingCollision(vehicle, xeno, collisionAabb, xenoAabb, clearance, mapId, debug, blockers);
    return GridVehicleMoverSystem.CollisionHandlingResult.Blocked;
  }

  private GridVehicleMoverSystem.CollisionHandlingResult HandleBreakableCollision(
    EntityUid vehicle,
    GridVehicleMoverComponent mover,
    EntityUid other,
    Box2 collisionAabb,
    Box2 otherAabb,
    float clearance,
    MapId mapId,
    bool hasDoor,
    bool isUnpoweredDoor,
    bool applyEffects,
    bool debug,
    HashSet<EntityUid>? blockers,
    float wheelDamage,
    ref bool playedCollisionSound)
  {
    VehicleSmashableComponent comp;
    this.TryComp<VehicleSmashableComponent>(other, out comp);
    bool flag = this.CanVehicleSmash(vehicle, comp);
    if (((comp == null ? 0 : (comp.RequiresDoorUnpowered ? 1 : 0)) & (hasDoor ? 1 : 0)) != 0 && !isUnpoweredDoor)
      flag = false;
    if (!flag)
    {
      if (applyEffects)
      {
        this.PlayCollisionSound(vehicle, ref playedCollisionSound);
        this.ApplyWheelCollisionDamage(vehicle, mover, wheelDamage);
      }
      GridVehicleMoverSystem.AddBlockingCollision(vehicle, other, collisionAabb, otherAabb, clearance, mapId, debug, blockers);
      return GridVehicleMoverSystem.CollisionHandlingResult.Blocked;
    }
    if (applyEffects)
      this.TrySmash(other, vehicle, ref playedCollisionSound);
    return GridVehicleMoverSystem.CollisionHandlingResult.Continue;
  }

  private bool CanVehicleSmash(EntityUid vehicle, VehicleSmashableComponent? smashable)
  {
    return smashable == null || smashable.RequiredVehicleTags.Count <= 0 || this._tags.HasAnyTag(vehicle, smashable.RequiredVehicleTags);
  }

  private GridVehicleMoverSystem.CollisionHandlingResult HandleHardCollision(
    EntityUid vehicle,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    Vector2 gridPos,
    EntityUid other,
    Box2 collisionAabb,
    Box2 otherAabb,
    float clearance,
    MapId mapId,
    bool isVehicle,
    bool applyEffects,
    bool debug,
    HashSet<EntityUid>? blockers,
    float wheelDamage,
    ref bool playedCollisionSound)
  {
    if (isVehicle && this.TryPushVehicle(vehicle, mover, grid, gridPos, other, applyEffects))
      return GridVehicleMoverSystem.CollisionHandlingResult.Continue;
    if (applyEffects)
    {
      this.PlayCollisionSound(vehicle, ref playedCollisionSound);
      this.ApplyWheelCollisionDamage(vehicle, mover, wheelDamage);
    }
    GridVehicleMoverSystem.AddBlockingCollision(vehicle, other, collisionAabb, otherAabb, clearance, mapId, debug, blockers);
    return GridVehicleMoverSystem.CollisionHandlingResult.Blocked;
  }

  private static void AddBlockingCollision(
    EntityUid vehicle,
    EntityUid blocker,
    Box2 collisionAabb,
    Box2 blockerAabb,
    float clearance,
    MapId mapId,
    bool debug,
    HashSet<EntityUid>? blockers)
  {
    blockers?.Add(blocker);
    if (!debug)
      return;
    GridVehicleMoverSystem.DebugCollisions.Add(new GridVehicleMoverSystem.DebugCollision(vehicle, blocker, collisionAabb, blockerAabb, 0.0f, 0.0f, clearance, mapId));
  }

  private static void AddDebugCollisionProbe(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    FixturesComponent fixtures,
    Robust.Shared.Physics.Transform transformData,
    Box2 aabb,
    Box2 movementAabb,
    MapId map,
    bool blocked,
    bool applyEffects)
  {
    Box2 aabb1;
    if (!GridVehicleMoverSystem.TryGetFixtureLocalAabb(fixtures, out aabb1))
      return;
    Box2 movementAabb1 = GridVehicleMoverSystem.GetMovementAabb(aabb1, mover);
    Angle Rotation;
    // ISSUE: explicit constructor call
    ((Angle) ref Rotation).\u002Ector((double) transformData.Quaternion2D.Angle);
    Box2Rotated FixtureBounds;
    // ISSUE: explicit constructor call
    ((Box2Rotated) ref FixtureBounds).\u002Ector(((Box2) ref aabb1).Translated(transformData.Position), Rotation, transformData.Position);
    Box2Rotated MovementBounds;
    // ISSUE: explicit constructor call
    ((Box2Rotated) ref MovementBounds).\u002Ector(((Box2) ref movementAabb1).Translated(transformData.Position), Rotation, transformData.Position);
    GridVehicleMoverSystem.DebugCollisionProbes.Add(new GridVehicleMoverSystem.DebugCollisionProbe(uid, aabb, movementAabb, FixtureBounds, MovementBounds, transformData.Position, Rotation, blocked, applyEffects, map));
  }

  private static Box2 GetCollisionAabb(
    GridVehicleMoverSystem.VehicleCollisionClass collisionClass,
    Box2 fullAabb,
    Box2 movementAabb)
  {
    return collisionClass != GridVehicleMoverSystem.VehicleCollisionClass.SoftMob ? movementAabb : fullAabb;
  }

  private static bool HasCollisionOverlap(Box2 vehicleAabb, Box2 otherAabb)
  {
    Box2 box2 = ((Box2) ref vehicleAabb).Intersect(ref otherAabb);
    return (double) ((Box2) ref box2).Width > 0.0 && (double) ((Box2) ref box2).Height > 0.0;
  }

  private static bool OrientedBoxIntersectsAabb(
    Vector2 obbCenter,
    Vector2 obbHalf,
    Angle rotation,
    Box2 aabb)
  {
    float num = (float) Math.Cos(rotation.Theta);
    float y = (float) Math.Sin(rotation.Theta);
    Vector2 vector2_1 = new Vector2(num, y);
    Vector2 vector2_2 = new Vector2(-y, num);
    Vector2 vector2_3 = ((Box2) ref aabb).Size / 2f;
    Vector2 vector2_4 = obbCenter - ((Box2) ref aabb).Center;
    return (double) MathF.Abs(vector2_4.X) <= (double) vector2_3.X + (double) obbHalf.X * (double) MathF.Abs(vector2_1.X) + (double) obbHalf.Y * (double) MathF.Abs(vector2_2.X) && (double) MathF.Abs(vector2_4.Y) <= (double) vector2_3.Y + (double) obbHalf.X * (double) MathF.Abs(vector2_1.Y) + (double) obbHalf.Y * (double) MathF.Abs(vector2_2.Y) && (double) MathF.Abs(Vector2.Dot(vector2_4, vector2_1)) <= (double) obbHalf.X + (double) vector2_3.X * (double) MathF.Abs(vector2_1.X) + (double) vector2_3.Y * (double) MathF.Abs(vector2_1.Y) && (double) MathF.Abs(Vector2.Dot(vector2_4, vector2_2)) <= (double) obbHalf.Y + (double) vector2_3.X * (double) MathF.Abs(vector2_2.X) + (double) vector2_3.Y * (double) MathF.Abs(vector2_2.Y);
  }

  private static bool OrientedBoxIntersectsOrientedBox(
    Vector2 centerA,
    Vector2 halfA,
    Angle rotA,
    Vector2 centerB,
    Vector2 halfB,
    Angle rotB)
  {
    Vector2 vector2_1 = new Vector2((float) Math.Cos(rotA.Theta), (float) Math.Sin(rotA.Theta));
    Vector2 vector2_2 = new Vector2(-vector2_1.Y, vector2_1.X);
    Vector2 vector2_3 = new Vector2((float) Math.Cos(rotB.Theta), (float) Math.Sin(rotB.Theta));
    Vector2 vector2_4 = new Vector2(-vector2_3.Y, vector2_3.X);
    Vector2 d = centerB - centerA;
    return !GridVehicleMoverSystem.SeparatedOnAxis(d, vector2_1, halfA, vector2_1, vector2_2, halfB, vector2_3, vector2_4) && !GridVehicleMoverSystem.SeparatedOnAxis(d, vector2_2, halfA, vector2_1, vector2_2, halfB, vector2_3, vector2_4) && !GridVehicleMoverSystem.SeparatedOnAxis(d, vector2_3, halfA, vector2_1, vector2_2, halfB, vector2_3, vector2_4) && !GridVehicleMoverSystem.SeparatedOnAxis(d, vector2_4, halfA, vector2_1, vector2_2, halfB, vector2_3, vector2_4);
  }

  private static bool SeparatedOnAxis(
    Vector2 d,
    Vector2 axis,
    Vector2 halfA,
    Vector2 aX,
    Vector2 aY,
    Vector2 halfB,
    Vector2 bX,
    Vector2 bY)
  {
    float num1 = (float) ((double) halfA.X * (double) MathF.Abs(Vector2.Dot(aX, axis)) + (double) halfA.Y * (double) MathF.Abs(Vector2.Dot(aY, axis)));
    float num2 = (float) ((double) halfB.X * (double) MathF.Abs(Vector2.Dot(bX, axis)) + (double) halfB.Y * (double) MathF.Abs(Vector2.Dot(bY, axis)));
    return (double) MathF.Abs(Vector2.Dot(d, axis)) > (double) num1 + (double) num2;
  }

  private static Box2 GetMovementAabb(Box2 aabb, GridVehicleMoverComponent mover)
  {
    float num = Math.Clamp(mover.MovementCollisionInset, 0.0f, 0.45f);
    if ((double) num <= 0.0)
      return aabb;
    Box2 box2 = ((Box2) ref aabb).Enlarged(-num);
    return !((Box2) ref box2).IsValid() ? aabb : box2;
  }

  private Angle GetCollisionWorldRotation(EntityUid uid, EntityUid grid, Angle? overrideRotation)
  {
    if (!overrideRotation.HasValue)
      return this.transform.GetWorldRotation(uid);
    Angle valueOrDefault = overrideRotation.GetValueOrDefault();
    TransformComponent transformComponent = this.Transform(uid);
    return transformComponent.ParentUid.IsValid() ? Angle.op_Addition(this.transform.GetWorldRotation(transformComponent.ParentUid), valueOrDefault) : Angle.op_Addition(this.transform.GetWorldRotation(grid), valueOrDefault);
  }

  private bool TryGetDoorPowered(EntityUid target, out bool powered)
  {
    AirlockComponent comp1;
    if (this.TryComp<AirlockComponent>(target, out comp1))
    {
      powered = comp1.Powered;
      return true;
    }
    FirelockComponent comp2;
    if (this.TryComp<FirelockComponent>(target, out comp2))
    {
      powered = comp2.Powered;
      return true;
    }
    if (this.HasComp<RMCPowerReceiverComponent>(target))
    {
      powered = this._rmcPower.IsPowered(target);
      return true;
    }
    powered = false;
    return false;
  }

  private void ApplyWheelCollisionDamage(
    EntityUid vehicle,
    GridVehicleMoverComponent mover,
    float damage)
  {
    if (this._net.IsClient || (double) damage <= 0.0)
      return;
    this._wheels.DamageWheels(vehicle, damage);
  }

  private float GetWheelCollisionDamage(EntityUid vehicle, GridVehicleMoverComponent mover)
  {
    VehicleWheelSlotsComponent comp;
    if (!this.TryComp<VehicleWheelSlotsComponent>(vehicle, out comp))
      return 0.0f;
    float num = MathF.Abs(mover.CurrentSpeed);
    if ((double) num <= 0.0)
      return 0.0f;
    float y = num * comp.CollisionDamagePerSpeed;
    if ((double) comp.MinCollisionDamage > 0.0)
      y = MathF.Max(comp.MinCollisionDamage, y);
    return y;
  }

  private bool ShouldBlockXeno(GridVehicleMoverComponent mover, EntityUid xeno)
  {
    RMCSizes? blockMinimumSize = mover.XenoBlockMinimumSize;
    if (!blockMinimumSize.HasValue)
      return true;
    RMCSizes valueOrDefault = blockMinimumSize.GetValueOrDefault();
    RMCSizes size;
    return !this._size.TryGetSize(xeno, out size) || size >= valueOrDefault;
  }

  private bool HasBlockingVehicleMob(GridVehicleMoverComponent mover, HashSet<EntityUid> blockers)
  {
    foreach (EntityUid blocker in blockers)
    {
      if (this.IsBlockingVehicleMob(mover, blocker))
        return true;
    }
    return false;
  }

  private bool IsBlockingVehicleMob(GridVehicleMoverComponent mover, EntityUid blocker)
  {
    return this.HasComp<XenoComponent>(blocker) && this.ShouldBlockXeno(mover, blocker);
  }

  private static bool TryGetFixtureAabb(
    FixturesComponent fixtures,
    Robust.Shared.Physics.Transform transformData,
    out Box2 aabb)
  {
    bool flag = true;
    aabb = new Box2();
    foreach (Fixture fixture in fixtures.Fixtures.Values)
    {
      if (fixture.Hard)
      {
        for (int childIndex = 0; childIndex < fixture.Shape.ChildCount; ++childIndex)
        {
          Box2 aabb1 = fixture.Shape.ComputeAABB(transformData, childIndex);
          if (flag)
          {
            aabb = aabb1;
            flag = false;
          }
          else
            aabb = ((Box2) ref aabb).Union(ref aabb1);
        }
      }
    }
    return !flag;
  }

  private static bool TryGetFixtureLocalAabb(FixturesComponent fixtures, out Box2 aabb)
  {
    return GridVehicleMoverSystem.TryGetFixtureAabb(fixtures, Robust.Shared.Physics.Transform.Empty, out aabb);
  }

  private bool TryPushVehicle(
    EntityUid pusher,
    GridVehicleMoverComponent pusherMover,
    EntityUid grid,
    Vector2 pusherTargetPosition,
    EntityUid pushed,
    bool applyEffects)
  {
    VehicleComponent comp1;
    GridVehicleMoverComponent comp2;
    MapGridComponent component;
    if (!pusherMover.CanPushVehicles || !this.TryComp<VehicleComponent>(pushed, out comp1) || comp1.MovementKind != VehicleMovementKind.Grid || !this.TryComp<GridVehicleMoverComponent>(pushed, out comp2) || !this.gridQ.TryComp(grid, out component))
      return false;
    TransformComponent xform = this.Transform(pushed);
    EntityUid? nullable = xform.GridUid;
    EntityUid entityUid1 = grid;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid1 ? 1 : 0) : 1) != 0)
      return false;
    Vector2 direction = pusherTargetPosition - pusherMover.Position;
    if ((double) direction.LengthSquared() <= 9.99999905104687E-09)
      return false;
    this.TrySyncMoverToCurrentGrid((Entity<GridVehicleMoverComponent>) (pushed, comp2), false, xform);
    nullable = comp2.SyncedGrid;
    EntityUid entityUid2 = grid;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid2 ? 1 : 0) : 1) != 0)
      return false;
    HashSet<EntityUid> ignoredEntities = new HashSet<EntityUid>()
    {
      pusher
    };
    Vector2 gridPos = comp2.Position + direction;
    if (!this.CanOccupyTransform(pushed, comp2, grid, gridPos, new Angle?(), 0.0075f, false, false, ignoredEntities: ignoredEntities))
      return false;
    if (!applyEffects)
      return true;
    if (!this.CanOccupyTransform(pushed, comp2, grid, gridPos, new Angle?(), 0.0075f, true, false, ignoredEntities: ignoredEntities))
      return false;
    comp2.Position = gridPos;
    comp2.CurrentSpeed = 0.0f;
    comp2.IsCommittedToMove = false;
    comp2.IsPushMove = true;
    comp2.PushDirection = GridVehicleMoverSystem.GetCardinalDirection(direction);
    comp2.IsMoving = true;
    this.UpdateDerivedTileState(grid, component, comp2);
    this.SetGridPosition(pushed, grid, comp2.Position);
    this.physics.WakeBody(pushed);
    this.Dirty(pushed, (IComponent) comp2);
    return true;
  }

  private static Vector2i GetCardinalDirection(Vector2 direction)
  {
    if ((double) direction.LengthSquared() <= 0.0)
      return Vector2i.Zero;
    return (double) MathF.Abs(direction.X) >= (double) MathF.Abs(direction.Y) ? new Vector2i(Math.Sign(direction.X), 0) : new Vector2i(0, Math.Sign(direction.Y));
  }

  private bool TrySmash(EntityUid target, EntityUid vehicle, ref bool playedCollisionSound)
  {
    VehicleSmashableComponent comp1;
    if (!this.TryComp<VehicleSmashableComponent>(target, out comp1) || comp1.RequiredVehicleTags.Count > 0 && !this._tags.HasAnyTag(vehicle, comp1.RequiredVehicleTags))
      return false;
    this.PlayCollisionSound(vehicle, ref playedCollisionSound);
    GridVehicleMoverComponent comp2;
    if (this.TryComp<GridVehicleMoverComponent>(vehicle, out comp2))
      this.ApplySmashSlowdown(vehicle, comp2, comp1);
    if (this._net.IsClient)
      return true;
    if (comp1.SmashSound != null)
      this._audio.PlayPvs(comp1.SmashSound, this.Transform(target).Coordinates);
    RMCVehicleSmashAttemptEvent args = new RMCVehicleSmashAttemptEvent(vehicle);
    this.RaiseLocalEvent<RMCVehicleSmashAttemptEvent>(target, ref args);
    if (args.Handled)
      return true;
    this.SmashTarget(target, vehicle, comp1);
    return true;
  }

  private void SmashTarget(
    EntityUid target,
    EntityUid vehicle,
    VehicleSmashableComponent smashable)
  {
    DamageSpecifier damage = new DamageSpecifier()
    {
      DamageDict = {
        [(string) GridVehicleMoverSystem.CollisionDamageType] = (FixedPoint2) smashable.DamageOnHit
      }
    };
    this._damageable.TryChangeDamage(new EntityUid?(target), damage, true, origin: new EntityUid?(vehicle), tool: new EntityUid?(vehicle));
    if (!smashable.DeleteOnHit || this.TerminatingOrDeleted(target))
      return;
    this._destructible.DestroyEntity(target);
  }

  private void PlayCollisionSound(EntityUid uid, ref bool played)
  {
    VehicleSoundComponent comp;
    if (played || !this.TryComp<VehicleSoundComponent>(uid, out comp) || comp.CollisionSound == null || this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (comp.NextCollisionSound > curTime)
      return;
    this._audio.PlayPvs(comp.CollisionSound, uid);
    comp.NextCollisionSound = curTime + TimeSpan.FromSeconds((double) comp.CollisionSoundCooldown);
    this.Dirty(uid, (IComponent) comp);
    played = true;
  }

  private void PlayMobCollisionSound(EntityUid uid, ref bool played)
  {
    VehicleSoundComponent comp;
    if (played || !this.TryComp<VehicleSoundComponent>(uid, out comp))
      return;
    SoundSpecifier sound = comp.MobCollisionSound ?? comp.CollisionSound;
    if (sound == null || this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (comp.NextCollisionSound > curTime)
      return;
    this._audio.PlayPvs(sound, uid);
    comp.NextCollisionSound = curTime + TimeSpan.FromSeconds((double) comp.CollisionSoundCooldown);
    this.Dirty(uid, (IComponent) comp);
    played = true;
  }

  private void HandleMobCollision(
    EntityUid vehicle,
    EntityUid target,
    MobStateComponent mobState,
    ref bool playedCollisionSound)
  {
    if (this._net.IsClient || this._mobState.IsDead(target, mobState))
      return;
    TimeSpan curTime = this._timing.CurTime;
    TimeSpan timeSpan;
    if (this._lastMobCollision.TryGetValue(target, out timeSpan) && curTime < timeSpan + GridVehicleMoverSystem.MobCollisionCooldown)
      return;
    this._lastMobCollision[target] = curTime;
    this.PlayMobCollisionSound(vehicle, ref playedCollisionSound);
    VehicleComponent comp;
    double num = this.TryComp<VehicleComponent>(vehicle, out comp) ? (double) comp.RunoverDamage : 8.0;
    DamageSpecifier damage = new DamageSpecifier()
    {
      DamageDict = {
        [(string) GridVehicleMoverSystem.CollisionDamageType] = (FixedPoint2) num
      }
    };
    this._damageable.TryChangeDamage(new EntityUid?(target), damage);
    if (this.HasComp<XenoComponent>(target))
      return;
    this._stun.TryKnockdown(target, GridVehicleMoverSystem.MobCollisionKnockdown, true);
    VehicleRunoverComponent runoverComponent = this.EnsureComp<VehicleRunoverComponent>(target);
    runoverComponent.Vehicle = vehicle;
    runoverComponent.Duration = GridVehicleMoverSystem.MobCollisionKnockdown;
    runoverComponent.ExpiresAt = curTime + runoverComponent.Duration + VehicleRunoverSystem.StandUpGrace;
    this.Dirty(target, (IComponent) runoverComponent);
    PhysicsComponent component;
    if (!this.physicsQ.TryComp(target, out component))
      return;
    this.physics.SetLinearVelocity(target, Vector2.Zero, body: component);
    this.physics.SetAngularVelocity(target, 0.0f, body: component);
  }

  private Vector2 GetVehicleMoveDelta(
    EntityUid grid,
    Vector2 worldPos,
    MapId mapId,
    GridVehicleMoverComponent mover)
  {
    MapCoordinates map = new EntityCoordinates(grid, mover.Position).ToMap((IEntityManager) this.EntityManager, this.transform);
    return map.MapId != mapId ? Vector2.Zero : worldPos - map.Position;
  }

  private bool PushMobOutOfVehicle(
    EntityUid vehicle,
    EntityUid mob,
    Box2 vehicleAabb,
    Box2 mobAabb,
    Vector2 vehicleMove)
  {
    if (this.Transform(mob).Anchored)
      return false;
    Box2 centeredMobAabb = this.GetCenteredMobAabb(mob, mobAabb);
    EntityCoordinates target;
    if (!this.TryGetMobPush(vehicle, mob, vehicleAabb, centeredMobAabb, vehicleMove, out target))
      return false;
    if (!this._net.IsClient || this.ShouldPredictVehicleInteractions(vehicle))
      this.ApplyMobPush(mob, target);
    return true;
  }

  private Box2 GetCenteredMobAabb(EntityUid mob, Box2 mobAabb)
  {
    Vector2 worldPosition = this.transform.GetWorldPosition(mob);
    return (double) (((Box2) ref mobAabb).Center - worldPosition).LengthSquared() <= 9.9999997473787516E-05 ? mobAabb : Box2.CenteredAround(worldPosition, ((Box2) ref mobAabb).Size);
  }

  private void ApplyMobPush(EntityUid mob, EntityCoordinates target)
  {
    if (target == EntityCoordinates.Invalid || this.transform.GetMapCoordinates(mob).MapId != this.transform.ToMapCoordinates(target).MapId)
      return;
    PhysicsComponent component;
    if (this.physicsQ.TryComp(mob, out component))
    {
      this.physics.SetLinearVelocity(mob, Vector2.Zero, body: component);
      this.physics.SetAngularVelocity(mob, 0.0f, body: component);
    }
    TransformComponent xform = this.Transform(mob);
    this.transform.SetCoordinates(mob, xform, target);
  }

  private bool ShouldPredictVehicleInteractions(EntityUid vehicle)
  {
    PhysicsComponent component;
    VehicleComponent comp;
    if (!this._net.IsClient || !this._timing.InPrediction || !this.physicsQ.TryComp(vehicle, out component) || !component.Predict || !this.TryComp<VehicleComponent>(vehicle, out comp) || !comp.Operator.HasValue)
      return false;
    EntityUid? nullable = comp.Operator;
    EntityUid? localEntity = this._player.LocalEntity;
    if (nullable.HasValue != localEntity.HasValue)
      return false;
    return !nullable.HasValue || nullable.GetValueOrDefault() == localEntity.GetValueOrDefault();
  }

  private void PredictRunover(EntityUid vehicle, EntityUid mob, MobStateComponent mobState)
  {
    if (!this.ShouldPredictVehicleInteractions(vehicle) || this._mobState.IsDead(mob, mobState) || this._standing.IsDown(mob))
      return;
    this._stun.TryKnockdown(mob, GridVehicleMoverSystem.MobCollisionKnockdown, true);
    VehicleRunoverComponent runoverComponent = this.EnsureComp<VehicleRunoverComponent>(mob);
    runoverComponent.Vehicle = vehicle;
    runoverComponent.Duration = GridVehicleMoverSystem.MobCollisionKnockdown;
    runoverComponent.ExpiresAt = this._timing.CurTime + runoverComponent.Duration + VehicleRunoverSystem.StandUpGrace;
    this.Dirty(mob, (IComponent) runoverComponent);
    PhysicsComponent component;
    if (!this.physicsQ.TryComp(mob, out component))
      return;
    this.physics.SetLinearVelocity(mob, Vector2.Zero, body: component);
    this.physics.SetAngularVelocity(mob, 0.0f, body: component);
  }

  private bool TryGetMobPush(
    EntityUid vehicle,
    EntityUid mob,
    Box2 vehicleAabb,
    Box2 mobAabb,
    Vector2 vehicleMove,
    out EntityCoordinates target)
  {
    target = EntityCoordinates.Invalid;
    Vector2 vector2_1 = ((Box2) ref vehicleAabb).Size / 2f;
    Vector2 vector2_2 = ((Box2) ref mobAabb).Size / 2f;
    Vector2 center = ((Box2) ref vehicleAabb).Center;
    Vector2 vector2_3 = ((Box2) ref mobAabb).Center - center;
    float num1 = vector2_1.X + vector2_2.X - Math.Abs(vector2_3.X);
    float num2 = vector2_1.Y + vector2_2.Y - Math.Abs(vector2_3.Y);
    if ((double) num1 <= 0.0 || (double) num2 <= 0.0 || (double) num1 <= 0.05000000074505806 && (double) num2 <= 0.05000000074505806)
      return false;
    Vector2 pushX = (double) num1 > 0.0 ? new Vector2((float) Math.Sign((double) vector2_3.X == 0.0 ? 1f : vector2_3.X) * num1, 0.0f) : Vector2.Zero;
    Vector2 pushY = (double) num2 > 0.0 ? new Vector2(0.0f, (float) Math.Sign((double) vector2_3.Y == 0.0 ? 1f : vector2_3.Y) * num2) : Vector2.Zero;
    Box2 vehicleBounds = vehicleAabb;
    if (this.TryGetMovementSidePushTarget(vehicle, mob, mobAabb, vehicleBounds, vehicleMove, pushX, pushY, out target))
      return true;
    if ((double) vehicleMove.LengthSquared() > 9.9999997473787516E-05)
      return false;
    bool flag1 = (double) num1 < (double) num2;
    bool flag2;
    if ((double) MathF.Abs(num1 - num2) <= 0.05000000074505806 && this._lastMobPushAxis.TryGetValue(mob, out flag2))
      flag1 = flag2;
    Vector2 push1 = flag1 ? pushX : pushY;
    Vector2 push2 = flag1 ? pushY : pushX;
    if (this.TryGetSidePushTarget(vehicle, mob, mobAabb, vehicleBounds, push1, out target))
    {
      this._lastMobPushAxis[mob] = flag1;
      return true;
    }
    if (!this.TryGetSidePushTarget(vehicle, mob, mobAabb, vehicleBounds, push2, out target))
      return false;
    this._lastMobPushAxis[mob] = !flag1;
    return true;
  }

  private bool TryGetMovementSidePushTarget(
    EntityUid vehicle,
    EntityUid mob,
    Box2 mobAabb,
    Box2 vehicleBounds,
    Vector2 vehicleMove,
    Vector2 pushX,
    Vector2 pushY,
    out EntityCoordinates target)
  {
    target = EntityCoordinates.Invalid;
    if ((double) vehicleMove.LengthSquared() <= 9.9999997473787516E-05)
      return false;
    bool flag1 = (double) MathF.Abs(vehicleMove.X) >= (double) MathF.Abs(vehicleMove.Y);
    Vector2 push = flag1 ? pushY : pushX;
    if (push == Vector2.Zero)
      return false;
    bool flag2 = !flag1;
    if (this.TryGetSidePushTarget(vehicle, mob, mobAabb, vehicleBounds, push, out target))
    {
      this._lastMobPushAxis[mob] = flag2;
      return true;
    }
    if (!this.TryGetSidePushTarget(vehicle, mob, mobAabb, vehicleBounds, -push, out target))
      return false;
    this._lastMobPushAxis[mob] = flag2;
    return true;
  }

  private bool TryGetSidePushTarget(
    EntityUid vehicle,
    EntityUid mob,
    Box2 mobAabb,
    Box2 vehicleBounds,
    Vector2 push,
    out EntityCoordinates target)
  {
    target = EntityCoordinates.Invalid;
    if (push == Vector2.Zero)
      return false;
    Vector2 push1 = push;
    if ((double) Math.Abs(push1.X) > 0.0)
      push1.X += (float) Math.Sign(push1.X) * 0.0075f;
    if ((double) Math.Abs(push1.Y) > 0.0)
      push1.Y += (float) Math.Sign(push1.Y) * 0.0075f;
    Box2 box2 = ((Box2) ref mobAabb).Translated(push1);
    if (((Box2) ref box2).Intersects(ref vehicleBounds) || this.IsPushBlocked(vehicle, mob, mobAabb, push1))
      return false;
    MapCoordinates mapCoordinates = this.transform.GetMapCoordinates(mob);
    MapCoordinates coordinates1 = new MapCoordinates(mapCoordinates.Position + push1, mapCoordinates.MapId);
    EntityUid? gridUid = this.Transform(mob).GridUid;
    if (gridUid.HasValue)
    {
      EntityUid valueOrDefault = gridUid.GetValueOrDefault();
      MapGridComponent component;
      if (this.gridQ.TryComp(valueOrDefault, out component))
      {
        EntityCoordinates coordinates2 = this.transform.ToCoordinates((Entity<TransformComponent>) valueOrDefault, coordinates1);
        Vector2i indices = this.map.TileIndicesFor(valueOrDefault, component, coordinates2);
        if (this.IsPushTileBlocked(valueOrDefault, component, indices, vehicle, mob, out EntityUid _))
          return false;
        target = this.transform.ToCoordinates((Entity<TransformComponent>) valueOrDefault, coordinates1);
        goto label_14;
      }
    }
    target = this.transform.ToCoordinates(coordinates1);
label_14:
    return !(target == EntityCoordinates.Invalid);
  }

  private bool IsPushTileBlocked(
    EntityUid gridUid,
    MapGridComponent gridComp,
    Vector2i indices,
    EntityUid vehicle,
    EntityUid mob,
    out EntityUid blocker)
  {
    blocker = EntityUid.Invalid;
    TransformComponent component1 = this.Transform(gridUid);
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> entityQuery = this.GetEntityQuery<TransformComponent>();
    (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 matrix3x2) = this.transform.GetWorldPositionRotationMatrix(component1, entityQuery);
    ushort tileSize = gridComp.TileSize;
    Vector2 position = new Vector2((float) (indices.X * (int) tileSize) + (float) tileSize / 2f, (float) (indices.Y * (int) tileSize) + (float) tileSize / 2f);
    Vector2 vector2_1 = Vector2.Transform(position, matrix3x2);
    Box2 box2_1 = ((Box2) ref Box2.UnitCentered).Scale(0.95f * (float) tileSize);
    Box2Rotated worldBounds;
    // ISSUE: explicit constructor call
    ((Box2Rotated) ref worldBounds).\u002Ector(((Box2) ref box2_1).Translated(vector2_1), WorldRotation, vector2_1);
    box2_1 = ((Box2) ref box2_1).Translated(position);
    float num = (float) ((double) ((Box2) ref box2_1).Width * (double) ((Box2) ref box2_1).Height * 0.004999999888241291);
    foreach (EntityUid entityUid in this.lookup.GetEntitiesIntersecting(gridUid, worldBounds, LookupFlags.Dynamic | LookupFlags.Static))
    {
      if (!(entityUid == vehicle) && !(entityUid == mob) && !this.IsDescendantOf(entityUid, vehicle) && !this.IsDescendantOf(entityUid, mob))
      {
        TransformComponent component2 = this.Transform(entityUid);
        PhysicsComponent component3;
        if (!this.HasComp<MobStateComponent>(entityUid) && !this.HasComp<VehicleSmashableComponent>(entityUid) && !this.HasComp<FoldableComponent>(entityUid) && !this.TryComp<DoorComponent>(entityUid, out DoorComponent _) && !this.HasComp<BarricadeComponent>(entityUid) && (!this.HasComp<ItemComponent>(entityUid) || component2.Anchored) && this.physicsQ.TryComp(entityUid, out component3))
        {
          bool flag = this.HasComp<VehicleComponent>(entityUid);
          FixturesComponent component4;
          PhysicsComponent component5;
          FixturesComponent component6;
          if ((component2.Anchored || component3.BodyType == BodyType.Static || flag) && this.fixtureQ.TryComp(entityUid, out component4) && (!this.physicsQ.TryComp(mob, out component5) || !this.fixtureQ.TryComp(mob, out component6) || this.physics.IsHardCollidable((Entity<FixturesComponent, PhysicsComponent>) (mob, component6, component5), (Entity<FixturesComponent, PhysicsComponent>) (entityUid, component4, component3))))
          {
            (Vector2 WorldPosition, Angle WorldRotation) positionRotation = this.transform.GetWorldPositionRotation(component2, entityQuery);
            Vector2 worldPosition = positionRotation.WorldPosition;
            Angle angle1 = Angle.op_Subtraction(positionRotation.WorldRotation, WorldRotation);
            Angle angle2 = Angle.op_UnaryNegation(WorldRotation);
            ref Angle local1 = ref angle2;
            Vector2 vector2_2 = worldPosition - WorldPosition;
            ref Vector2 local2 = ref vector2_2;
            Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(((Angle) ref local1).RotateVec(ref local2), (float) angle1.Theta);
            foreach (Fixture fixture in component4.Fixtures.Values)
            {
              if (fixture.Hard && (fixture.CollisionLayer & 268435486 /*0x1000001E*/) != 0)
              {
                for (int childIndex = 0; childIndex < fixture.Shape.ChildCount; ++childIndex)
                {
                  Box2 aabb = fixture.Shape.ComputeAABB(transform, childIndex);
                  Box2 box2_2 = ((Box2) ref aabb).Intersect(ref box2_1);
                  if ((double) ((Box2) ref box2_2).Width * (double) ((Box2) ref box2_2).Height > (double) num)
                  {
                    blocker = entityUid;
                    return true;
                  }
                }
              }
            }
          }
        }
      }
    }
    return false;
  }

  private bool IsDescendantOf(EntityUid ent, EntityUid root)
  {
    if (ent == root)
      return true;
    EntityUid parentUid;
    for (EntityUid uid = ent; uid.IsValid(); uid = parentUid)
    {
      TransformComponent transformComponent = this.Transform(uid);
      parentUid = transformComponent.ParentUid;
      if (!parentUid.IsValid())
        return false;
      if (parentUid == root)
        return true;
      EntityUid entityUid1 = parentUid;
      EntityUid? nullable = transformComponent.GridUid;
      if ((nullable.HasValue ? (entityUid1 == nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
      {
        EntityUid entityUid2 = parentUid;
        nullable = transformComponent.MapUid;
        if ((nullable.HasValue ? (entityUid2 == nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
          continue;
      }
      return false;
    }
    return false;
  }

  private bool IsPushBlocked(EntityUid vehicle, EntityUid mob, Box2 mobAabb, Vector2 push)
  {
    if (push == Vector2.Zero)
      return false;
    MapId mapId = this.Transform(mob).MapID;
    PhysicsComponent component1;
    FixturesComponent component2;
    if (mapId == MapId.Nullspace || !this.physicsQ.TryComp(mob, out component1) || !this.fixtureQ.TryComp(mob, out component2))
      return false;
    Box2 box2_1 = ((Box2) ref mobAabb).Translated(push);
    Box2 worldAABB = ((Box2) ref box2_1).Enlarged(-0.1f);
    if (!((Box2) ref worldAABB).IsValid())
      worldAABB = box2_1;
    foreach (EntityUid entityUid in this.lookup.GetEntitiesIntersecting(mapId, worldAABB, LookupFlags.Dynamic | LookupFlags.Static))
    {
      PhysicsComponent component3;
      if (!(entityUid == mob) && !(entityUid == vehicle) && !this.IsDescendantOf(entityUid, vehicle) && !this.IsDescendantOf(entityUid, mob) && this.physicsQ.TryComp(entityUid, out component3) && component3.CanCollide)
      {
        TransformComponent xform = this.Transform(entityUid);
        FixturesComponent component4;
        if (this.fixtureQ.TryComp(entityUid, out component4) && !this.HasComp<MobStateComponent>(entityUid) && !this.HasComp<VehicleSmashableComponent>(entityUid) && !this.HasComp<FoldableComponent>(entityUid) && !this.TryComp<DoorComponent>(entityUid, out DoorComponent _) && !this.HasComp<BarricadeComponent>(entityUid))
        {
          bool flag1 = false;
          bool flag2 = false;
          Robust.Shared.Physics.Transform physicsTransform = this.physics.GetPhysicsTransform(entityUid, xform);
          foreach (Fixture fixture in component4.Fixtures.Values)
          {
            if (fixture.Hard && (fixture.CollisionLayer & 2) != 0)
            {
              flag1 = true;
              for (int childIndex = 0; childIndex < fixture.Shape.ChildCount; ++childIndex)
              {
                Box2 aabb = fixture.Shape.ComputeAABB(physicsTransform, childIndex);
                Box2 box2_2 = ((Box2) ref aabb).Intersect(ref worldAABB);
                if ((double) Box2.Area(ref box2_2) > 0.0099999997764825821)
                {
                  flag2 = true;
                  break;
                }
              }
              if (flag2)
                break;
            }
          }
          if (flag1 && flag2 && this.physics.IsHardCollidable((Entity<FixturesComponent, PhysicsComponent>) (mob, component2, component1), (Entity<FixturesComponent, PhysicsComponent>) (entityUid, component4, component3)))
            return true;
        }
      }
    }
    return false;
  }

  private GridVehicleMoverSystem.VehicleCollisionClass ClassifyCollisionCandidate(
    EntityUid other,
    TransformComponent otherXform,
    PhysicsComponent otherBody,
    FixturesComponent otherFixtures,
    bool hardCollidable,
    bool isMob,
    bool isBarricade,
    bool isFoldable,
    bool hasDoor,
    bool isXeno,
    bool isVehicle,
    bool isSmashable)
  {
    if (!otherXform.Anchored && this.HasComp<ItemComponent>(other))
      return GridVehicleMoverSystem.VehicleCollisionClass.Ignore;
    if (isMob | isXeno)
      return GridVehicleMoverSystem.VehicleCollisionClass.SoftMob;
    if (!isSmashable && !hardCollidable && GridVehicleMoverSystem.IsNormallyMobPassable(otherFixtures) || (otherXform.Anchored || otherBody.BodyType == BodyType.Static || isMob || isBarricade || isFoldable || isVehicle ? 0 : (!isSmashable ? 1 : 0)) != 0)
      return GridVehicleMoverSystem.VehicleCollisionClass.Ignore;
    if (isSmashable || isBarricade && hasDoor | isFoldable)
      return GridVehicleMoverSystem.VehicleCollisionClass.Breakable;
    return isFoldable && !hardCollidable || !hardCollidable ? GridVehicleMoverSystem.VehicleCollisionClass.Ignore : GridVehicleMoverSystem.VehicleCollisionClass.Hard;
  }

  private static bool IsNormallyMobPassable(FixturesComponent fixtures)
  {
    foreach (Fixture fixture in fixtures.Fixtures.Values)
    {
      if (!GridVehicleMoverSystem.IsNormallyMobPassable(fixture))
        return false;
    }
    return true;
  }

  private static bool IsNormallyMobPassable(Fixture fixture)
  {
    if (!fixture.Hard)
      return true;
    return (fixture.CollisionMask & 65) == 0 && (fixture.CollisionLayer & 30) == 0;
  }

  public static bool CollisionDebugEnabled { get; set; }

  public static bool MovementDebugEnabled { get; set; }

  public override void Initialize()
  {
    base.Initialize();
    this.gridQ = this.GetEntityQuery<MapGridComponent>();
    this.physicsQ = this.GetEntityQuery<PhysicsComponent>();
    this.fixtureQ = this.GetEntityQuery<FixturesComponent>();
    this.SubscribeLocalEvent<GridVehicleMoverComponent, ComponentStartup>(new EntityEventRefHandler<GridVehicleMoverComponent, ComponentStartup>(this.OnMoverStartup));
    this.SubscribeLocalEvent<GridVehicleMoverComponent, ComponentShutdown>(new EntityEventRefHandler<GridVehicleMoverComponent, ComponentShutdown>(this.OnMoverShutdown));
    this.SubscribeLocalEvent<GridVehicleMoverComponent, MoveEvent>(new EntityEventRefHandler<GridVehicleMoverComponent, MoveEvent>(this.OnMoverMove));
    this.SubscribeLocalEvent<GridVehicleMoverComponent, ReAnchorEvent>(new EntityEventRefHandler<GridVehicleMoverComponent, ReAnchorEvent>(this.OnMoverReAnchor));
    this.SubscribeLocalEvent<GridVehicleMoverComponent, VehicleCanRunEvent>(new EntityEventRefHandler<GridVehicleMoverComponent, VehicleCanRunEvent>(this.OnMoverCanRun));
    this.SubscribeLocalEvent<GridVehicleMoverComponent, PreventCollideEvent>(new EntityEventRefHandler<GridVehicleMoverComponent, PreventCollideEvent>(this.OnMoverPreventCollide));
  }

  private void OnMoverStartup(Entity<GridVehicleMoverComponent> ent, ref ComponentStartup args)
  {
    this.TrySyncMoverToCurrentGrid(ent, true, force: true);
    VehicleComponent comp;
    bool flag1 = this.TryComp<VehicleComponent>(ent.Owner, out comp);
    if (flag1)
    {
      bool flag2;
      switch (comp.MovementKind)
      {
        case VehicleMovementKind.Free:
        case VehicleMovementKind.Aircraft:
          flag2 = true;
          break;
        default:
          flag2 = false;
          break;
      }
      flag1 = flag2 || GridVehicleMoverSystem.ForceAllFreeMovement;
    }
    if (!flag1)
      return;
    ent.Comp.Heading = this.Transform(ent.Owner).LocalRotation;
    this.Dirty(ent.Owner, (IComponent) ent.Comp);
  }

  private void OnMoverShutdown(Entity<GridVehicleMoverComponent> ent, ref ComponentShutdown args)
  {
    this._hardState.Remove(ent.Owner);
    this._movementAccumulator.Remove(ent.Owner);
    this._activeXenoPushers.Remove(ent.Owner);
  }

  private void OnMoverMove(Entity<GridVehicleMoverComponent> ent, ref MoveEvent args)
  {
    if (!args.ParentChanged)
      return;
    this.TrySyncMoverToCurrentGrid(ent, false);
  }

  private void OnMoverReAnchor(Entity<GridVehicleMoverComponent> ent, ref ReAnchorEvent args)
  {
    this.TrySyncMoverToCurrentGrid(ent, false);
  }

  private bool TrySyncMoverToCurrentGrid(
    Entity<GridVehicleMoverComponent> ent,
    bool centerOnTile,
    TransformComponent? xform = null,
    bool force = false)
  {
    EntityUid owner = ent.Owner;
    if (xform == null)
      xform = this.Transform(owner);
    EntityUid? gridUid = xform.GridUid;
    if (gridUid.HasValue)
    {
      EntityUid valueOrDefault = gridUid.GetValueOrDefault();
      MapGridComponent component;
      if (this.gridQ.TryComp(valueOrDefault, out component))
      {
        if (!force)
        {
          EntityUid? syncedGrid = ent.Comp.SyncedGrid;
          EntityUid entityUid = valueOrDefault;
          if ((syncedGrid.HasValue ? (syncedGrid.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
            return false;
        }
        EntityCoordinates coords = xform.Coordinates.WithEntityId(valueOrDefault, this.transform, (IEntityManager) this.EntityManager);
        Vector2i vector2i = this.map.TileIndicesFor(valueOrDefault, component, coords);
        ent.Comp.SyncedGrid = new EntityUid?(valueOrDefault);
        Vector2 vector2 = centerOnTile ? new Vector2((float) vector2i.X + 0.5f, (float) vector2i.Y + 0.5f) : coords.Position;
        Vector2 freePosition;
        if (!this.CanOccupyTransform(owner, ent.Comp, valueOrDefault, vector2, new Angle?(), 0.0075f, false, false) && this.TryFindFreePlacement(owner, ent.Comp, valueOrDefault, component, vector2, out freePosition))
          vector2 = freePosition;
        ent.Comp.Position = vector2;
        ent.Comp.CurrentTile = this.map.TileIndicesFor(valueOrDefault, component, new EntityCoordinates(valueOrDefault, vector2));
        ent.Comp.TargetTile = ent.Comp.CurrentTile;
        ent.Comp.TargetPosition = ent.Comp.Position;
        ent.Comp.CurrentSpeed = 0.0f;
        ent.Comp.PushDirection = Vector2i.Zero;
        ent.Comp.NextPushTime = TimeSpan.Zero;
        ent.Comp.NextTurnTime = TimeSpan.Zero;
        ent.Comp.InPlaceTurnBlockUntil = TimeSpan.Zero;
        ent.Comp.IsCommittedToMove = false;
        ent.Comp.IsPushMove = false;
        ent.Comp.IsMoving = false;
        this._hardState[owner] = true;
        this._movementAccumulator[owner] = 0.0f;
        this.Dirty(owner, (IComponent) ent.Comp);
        return true;
      }
    }
    if (!ent.Comp.SyncedGrid.HasValue)
      return false;
    ent.Comp.SyncedGrid = new EntityUid?();
    ent.Comp.CurrentSpeed = 0.0f;
    ent.Comp.PushDirection = Vector2i.Zero;
    ent.Comp.IsCommittedToMove = false;
    ent.Comp.IsPushMove = false;
    ent.Comp.IsMoving = false;
    this._hardState[owner] = true;
    this._movementAccumulator[owner] = 0.0f;
    this.Dirty(owner, (IComponent) ent.Comp);
    return true;
  }

  private bool TryFindFreePlacement(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2 seed,
    out Vector2 freePosition)
  {
    freePosition = seed;
    int num1 = (int) MathF.Ceiling(12f);
    for (int index1 = 1; index1 <= num1; ++index1)
    {
      float num2 = (float) index1 * 0.25f;
      for (int index2 = 0; index2 < 360; index2 += 30)
      {
        float x = (float) index2 * ((float) Math.PI / 180f);
        Vector2 gridPos = seed + new Vector2(MathF.Cos(x), MathF.Sin(x)) * num2;
        if (this.CanOccupyTransform(uid, mover, grid, gridPos, new Angle?(), 0.0075f, false, false))
        {
          freePosition = gridPos;
          return true;
        }
      }
    }
    return false;
  }

  private void OnMoverCanRun(Entity<GridVehicleMoverComponent> ent, ref VehicleCanRunEvent args)
  {
    VehicleComponent comp;
    if (!args.CanRun || !this.TryComp<VehicleComponent>(ent.Owner, out comp))
      return;
    EntityUid? nullable = comp.Operator;
    if (!nullable.HasValue || !this.HasComp<XenoComponent>(nullable.GetValueOrDefault()))
      return;
    args.CanRun = false;
  }

  private void OnMoverPreventCollide(
    Entity<GridVehicleMoverComponent> ent,
    ref PreventCollideEvent args)
  {
    if (args.Cancelled)
      return;
    VehicleComponent comp;
    bool flag1 = !this.TryComp<VehicleComponent>(ent.Owner, out comp);
    if (!flag1)
    {
      bool flag2;
      switch (comp.MovementKind)
      {
        case VehicleMovementKind.Grid:
        case VehicleMovementKind.Free:
        case VehicleMovementKind.Aircraft:
          flag2 = true;
          break;
        default:
          flag2 = false;
          break;
      }
      flag1 = !flag2;
    }
    if (flag1 || args.OtherEntity == ent.Owner)
      return;
    if (!this.Transform(args.OtherEntity).Anchored && this.HasComp<ItemComponent>(args.OtherEntity))
    {
      args.Cancelled = true;
    }
    else
    {
      if (args.OtherBody.BodyType != BodyType.Static)
        return;
      if (GridVehicleMoverSystem.IsNormallyMobPassable(args.OtherFixture))
      {
        args.Cancelled = true;
      }
      else
      {
        if ((args.OtherFixture.CollisionLayer & 335544350) == 0)
          return;
        args.Cancelled = true;
      }
    }
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    this.UpdateTrackRestores();
    if (GridVehicleMoverSystem.CollisionDebugEnabled)
    {
      GridVehicleMoverSystem.DebugTestedTiles.Clear();
      GridVehicleMoverSystem.DebugCollisionProbes.Clear();
      GridVehicleMoverSystem.DebugCollisions.Clear();
    }
    if (GridVehicleMoverSystem.MovementDebugEnabled)
      GridVehicleMoverSystem.DebugMovementDecisions.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<GridVehicleMoverComponent, VehicleComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<GridVehicleMoverComponent, VehicleComponent, TransformComponent>();
    EntityUid uid;
    GridVehicleMoverComponent comp1;
    VehicleComponent comp2;
    TransformComponent comp3;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2, out comp3))
    {
      bool flag1;
      switch (comp2.MovementKind)
      {
        case VehicleMovementKind.Grid:
        case VehicleMovementKind.Free:
        case VehicleMovementKind.Aircraft:
          flag1 = true;
          break;
        default:
          flag1 = false;
          break;
      }
      if (flag1)
      {
        bool flag2 = comp2.MovementKind == VehicleMovementKind.Aircraft;
        bool flag3 = !flag2 && (comp2.MovementKind == VehicleMovementKind.Free || GridVehicleMoverSystem.ForceAllFreeMovement);
        this.TrySyncMoverToCurrentGrid((Entity<GridVehicleMoverComponent>) (uid, comp1), false, comp3);
        EntityUid? gridUid = comp3.GridUid;
        if (gridUid.HasValue)
        {
          EntityUid valueOrDefault1 = gridUid.GetValueOrDefault();
          if (this.gridQ.TryComp(valueOrDefault1, out MapGridComponent _))
          {
            if (this._net.IsClient && !this.ShouldPredictVehicleMovement(comp2))
            {
              this.SmoothReplicatedVehicle(uid, valueOrDefault1, comp1, frameTime, flag3 | flag2);
            }
            else
            {
              Vector2i inputDir = Vector2i.Zero;
              bool pushing = false;
              if (!flag3 && !flag2)
                inputDir = this.GetMoverInput(uid, comp1, comp2, out pushing);
              float num1 = this._movementAccumulator.GetValueOrDefault<EntityUid, float>(uid) + frameTime;
              float num2 = 0.100000009f;
              if ((double) num1 > (double) num2)
                num1 = num2;
              for (int index = 0; (double) num1 >= 0.01666666753590107 && index < 6; ++index)
              {
                TransformComponent xform = this.Transform(uid);
                this.TrySyncMoverToCurrentGrid((Entity<GridVehicleMoverComponent>) (uid, comp1), false, xform);
                gridUid = xform.GridUid;
                if (gridUid.HasValue)
                {
                  EntityUid valueOrDefault2 = gridUid.GetValueOrDefault();
                  MapGridComponent component;
                  if (this.gridQ.TryComp(valueOrDefault2, out component))
                  {
                    if (flag2)
                      this.UpdateAircraftMovement(uid, comp1, comp2, valueOrDefault2, component, 0.0166666675f);
                    else if (flag3)
                      this.UpdateFreeMovement(uid, comp1, comp2, valueOrDefault2, component, 0.0166666675f);
                    else
                      this.UpdateMovement(uid, comp1, comp2, valueOrDefault2, component, inputDir, pushing, 0.0166666675f);
                    num1 -= 0.0166666675f;
                  }
                  else
                    break;
                }
                else
                  break;
              }
              this._movementAccumulator[uid] = num1;
            }
          }
        }
      }
    }
  }

  private bool ShouldPredictVehicleMovement(VehicleComponent vehicle)
  {
    if (!this._net.IsClient)
      return true;
    if (!this._timing.InPrediction || !vehicle.Operator.HasValue)
      return false;
    EntityUid? nullable = vehicle.Operator;
    EntityUid? localEntity = this._player.LocalEntity;
    if (nullable.HasValue != localEntity.HasValue)
      return false;
    return !nullable.HasValue || nullable.GetValueOrDefault() == localEntity.GetValueOrDefault();
  }

  private void SmoothReplicatedVehicle(
    EntityUid uid,
    EntityUid grid,
    GridVehicleMoverComponent mover,
    float frameTime,
    bool smoothAngle)
  {
    TransformComponent xform = this.Transform(uid);
    if (!xform.ParentUid.IsValid())
      return;
    Vector2 position = new EntityCoordinates(grid, mover.Position).WithEntityId(xform.ParentUid, this.transform, (IEntityManager) this.EntityManager).Position;
    Vector2 localPosition = xform.LocalPosition;
    Vector2 vector2 = position - localPosition;
    float amount = 1f - MathF.Exp(-22f * frameTime);
    if ((double) vector2.LengthSquared() >= 25.0 / 16.0)
      this.transform.SetLocalPosition(uid, position, xform);
    else
      this.transform.SetLocalPosition(uid, Vector2.Lerp(localPosition, position, amount), xform);
    if (!smoothAngle)
      return;
    Angle localRotation = xform.LocalRotation;
    Angle angle = Angle.op_Subtraction(mover.Heading, localRotation);
    if ((double) MathF.Abs((float) ((Angle) ref angle).Reduced().Theta) >= 1.0)
      this.transform.SetLocalRotation(uid, mover.Heading, xform);
    else
      this.transform.SetLocalRotation(uid, Angle.Lerp(ref localRotation, ref mover.Heading, amount), xform);
  }

  private void UpdateFreeMovement(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    VehicleComponent vehicle,
    EntityUid grid,
    MapGridComponent gridComp,
    float frameTime)
  {
    if (vehicle.Operator.HasValue)
    {
      VehicleCanRunEvent args = new VehicleCanRunEvent((Entity<VehicleComponent>) (uid, vehicle));
      this.RaiseLocalEvent<VehicleCanRunEvent>(uid, ref args);
      if (!args.CanRun)
      {
        this.StopMover(mover);
        this.SetGridPosition(uid, grid, mover.Position);
        this.transform.SetLocalRotation(uid, mover.Heading);
        this.Dirty(uid, (IComponent) mover);
        return;
      }
    }
    double slowdownMultiplier = (double) this.GetSmashSlowdownMultiplier(mover);
    this.TryDepenetrateFree(uid, mover, grid, gridComp);
    int throttle = 0;
    int num1 = 0;
    EntityUid? nullable = vehicle.Operator;
    InputMoverComponent comp;
    if (nullable.HasValue && this.TryComp<InputMoverComponent>(nullable.GetValueOrDefault(), out comp))
    {
      int heldMoveButtons = (int) comp.HeldMoveButtons;
      if ((heldMoveButtons & 1) != 0)
        ++throttle;
      if ((heldMoveButtons & 2) != 0)
        --throttle;
      if ((heldMoveButtons & 8) != 0)
        ++num1;
      if ((heldMoveButtons & 4) != 0)
        --num1;
    }
    float accelerationModifier = this.GetAccelerationModifier(uid);
    mover.CurrentSpeed = GridVehicleMotionSimulator.StepFreeSpeed(mover.CurrentSpeed, throttle, this.GetModifiedMaxSpeed(uid, mover), this.GetModifiedMaxReverseSpeed(uid, mover), mover.Acceleration * accelerationModifier, mover.ReverseAcceleration * accelerationModifier, mover.Deceleration, mover.AllowReverse, frameTime);
    if (num1 != 0)
    {
      float num2 = MathF.Abs(mover.CurrentSpeed);
      float num3;
      float num4;
      if (throttle == 0 && (double) num2 <= 0.0099999997764825821)
      {
        num3 = mover.PivotTurnRate;
        num4 = 1f;
      }
      else
      {
        float num5 = MathF.Max(0.01f, mover.TurnFullSpeed);
        float x = Math.Clamp(num2 / num5, 0.0f, 1f);
        if (throttle != 0)
          x = MathF.Max(x, 0.4f);
        num3 = mover.TurnRate * x;
        num4 = (double) mover.CurrentSpeed < -0.0099999997764825821 ? -1f : 1f;
      }
      if ((double) num3 > 0.0)
      {
        float radians = MathHelper.DegreesToRadians(num3);
        float num6 = (float) -num1 * radians * frameTime * num4;
        Angle angle = new Angle(mover.Heading.Theta + (double) num6);
        Angle newHeading = ((Angle) ref angle).Reduced();
        this.TryApplyFreeHeading(uid, mover, grid, newHeading);
      }
    }
    if ((double) MathF.Abs(mover.CurrentSpeed) > 0.0099999997764825821)
    {
      Vector2 worldVec = ((Angle) ref mover.Heading).ToWorldVec();
      float num7 = mover.CurrentSpeed * frameTime;
      Vector2 target = mover.Position + worldVec * num7;
      bool blocked;
      this.TryMoveContinuous(uid, mover, grid, target, new Angle?(mover.Heading), out blocked);
      if (blocked)
        mover.CurrentSpeed = 0.0f;
    }
    this.UpdateDerivedTileState(grid, gridComp, mover);
    mover.IsPushMove = false;
    mover.IsMoving = (double) MathF.Abs(mover.CurrentSpeed) > 0.0099999997764825821;
    this.UpdateRunSound(uid, mover);
    this.transform.SetLocalRotation(uid, mover.Heading);
    this.SetGridPosition(uid, grid, mover.Position);
    if (mover.IsMoving)
      this.physics.WakeBody(uid);
    this.Dirty(uid, (IComponent) mover);
  }

  private void TryApplyFreeHeading(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    Angle newHeading)
  {
    if (this.CanOccupyTransform(uid, mover, grid, mover.Position, new Angle?(newHeading), 0.0075f, false, false))
    {
      mover.Heading = newHeading;
    }
    else
    {
      Vector2 nudged;
      if (!this.TryFindFreeHeadingNudge(uid, mover, grid, newHeading, out nudged))
        return;
      mover.Position = nudged;
      mover.Heading = newHeading;
    }
  }

  private bool TryFindFreeHeadingNudge(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    Angle newHeading,
    out Vector2 nudged)
  {
    nudged = mover.Position;
    float y = MathF.Max(0.0f, mover.TurnCollisionGraceDistance);
    if ((double) y <= 0.0)
      return false;
    Vector2 worldVec = ((Angle) ref mover.Heading).ToWorldVec();
    if ((double) worldVec.LengthSquared() <= 0.0)
      return false;
    float num1 = Math.Clamp(mover.MovementProbeStep, 0.02f, 0.5f);
    int num2 = Math.Max(1, (int) MathF.Ceiling(y / num1));
    for (int index = 1; index <= num2; ++index)
    {
      float num3 = MathF.Min((float) index * num1, y);
      Vector2 gridPos1 = mover.Position + worldVec * num3;
      if (this.CanOccupyTransform(uid, mover, grid, gridPos1, new Angle?(newHeading), 0.0075f, false, false))
      {
        nudged = gridPos1;
        return true;
      }
      Vector2 gridPos2 = mover.Position - worldVec * num3;
      if (this.CanOccupyTransform(uid, mover, grid, gridPos2, new Angle?(newHeading), 0.0075f, false, false))
      {
        nudged = gridPos2;
        return true;
      }
    }
    return false;
  }

  private void TryDepenetrateFree(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp)
  {
    this._depenetrateBlockers.Clear();
    if (this.CanOccupyTransform(uid, mover, grid, mover.Position, new Angle?(mover.Heading), 0.0075f, false, false, this._depenetrateBlockers) || this._depenetrateBlockers.Count == 0)
      return;
    Vector2 vector2_1 = Vector2.Zero;
    foreach (EntityUid depenetrateBlocker in this._depenetrateBlockers)
    {
      if (!this.HasComp<VehicleComponent>(depenetrateBlocker))
        return;
      MapCoordinates mapCoordinates = this.transform.GetMapCoordinates(depenetrateBlocker);
      Vector2 position = this.transform.ToCoordinates((Entity<TransformComponent>) grid, mapCoordinates).Position;
      Vector2 vector2_2 = mover.Position - position;
      if ((double) vector2_2.LengthSquared() > 9.9999997473787516E-05)
        vector2_1 += Vector2.Normalize(vector2_2);
    }
    if ((double) vector2_1.LengthSquared() <= 9.9999997473787516E-05)
      vector2_1 = ((Angle) ref mover.Heading).ToWorldVec();
    if ((double) vector2_1.LengthSquared() <= 9.9999997473787516E-05)
      return;
    Vector2 vector2_3 = Vector2.Normalize(vector2_1);
    float num1 = Math.Clamp(mover.MovementProbeStep, 0.02f, 0.5f);
    int num2 = Math.Max(1, (int) MathF.Ceiling(2f / num1));
    for (int index = 1; index <= num2; ++index)
    {
      Vector2 gridPos = mover.Position + vector2_3 * ((float) index * num1);
      if (this.CanOccupyTransform(uid, mover, grid, gridPos, new Angle?(mover.Heading), 0.0075f, false, false))
      {
        mover.Position = gridPos;
        mover.CurrentSpeed = 0.0f;
        this.UpdateDerivedTileState(grid, gridComp, mover);
        break;
      }
    }
  }

  private void UpdateMovement(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    VehicleComponent vehicle,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i inputDir,
    bool pushing,
    float frameTime)
  {
    if (vehicle.Operator.HasValue)
    {
      VehicleCanRunEvent args = new VehicleCanRunEvent((Entity<VehicleComponent>) (uid, vehicle));
      this.RaiseLocalEvent<VehicleCanRunEvent>(uid, ref args);
      if (!args.CanRun)
      {
        this.StopMover(mover);
        this.SetGridPosition(uid, grid, mover.Position);
        this.Dirty(uid, (IComponent) mover);
        return;
      }
    }
    double slowdownMultiplier = (double) this.GetSmashSlowdownMultiplier(mover);
    mover.IsCommittedToMove = false;
    if (!pushing)
    {
      mover.IsPushMove = false;
      mover.PushDirection = Vector2i.Zero;
    }
    int num = pushing ? (this.UpdatePushMovement(uid, mover, grid, gridComp, inputDir, frameTime) ? 1 : 0) : (this.UpdateDriveMovement(uid, mover, grid, gridComp, inputDir, frameTime) ? 1 : 0);
    this.UpdateDerivedTileState(grid, gridComp, mover);
    mover.IsMoving = (double) MathF.Abs(mover.CurrentSpeed) > 0.0099999997764825821;
    if (!mover.IsMoving)
      mover.IsPushMove = false;
    if (num != 0 && !pushing)
      this.TryLayTrackTile(uid, mover, grid, gridComp, mover.CurrentTile);
    this.UpdateRunSound(uid, mover);
    this.SetGridPosition(uid, grid, mover.Position);
    if (num != 0 || mover.IsMoving)
      this.physics.WakeBody(uid);
    this.Dirty(uid, (IComponent) mover);
  }

  private bool UpdatePushMovement(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i inputDir,
    float frameTime)
  {
    bool hasInput = Vector2i.op_Inequality(inputDir, Vector2i.Zero);
    if (hasInput)
    {
      mover.IsPushMove = true;
      mover.PushDirection = inputDir;
    }
    if (!hasInput && !mover.IsPushMove)
    {
      mover.CurrentSpeed = GridVehicleMotionSimulator.StepIdleSpeed(mover.CurrentSpeed, mover.Deceleration, frameTime);
      return false;
    }
    float modifiedMaxSpeed = this.GetModifiedMaxSpeed(uid, mover);
    float accelerationModifier = this.GetAccelerationModifier(uid);
    if (hasInput && (double) mover.PushImpulseSpeed > 0.0)
    {
      float num = MathF.Min(mover.PushImpulseSpeed, modifiedMaxSpeed);
      if ((double) mover.CurrentSpeed < (double) num)
        mover.CurrentSpeed = num;
    }
    mover.CurrentSpeed = GridVehicleMotionSimulator.StepPushSpeed(MathF.Max(0.0f, mover.CurrentSpeed), modifiedMaxSpeed, mover.Acceleration * accelerationModifier, mover.Deceleration, hasInput, false, frameTime);
    if (Vector2i.op_Equality(mover.PushDirection, Vector2i.Zero) || (double) mover.CurrentSpeed <= 0.0099999997764825821)
      return false;
    Vector2i pushDirection = mover.PushDirection;
    float travel = mover.CurrentSpeed * frameTime;
    bool blocked;
    int num1 = this.TryMoveWithLaneGuidance(uid, mover, grid, gridComp, pushDirection, new Angle?(), travel, frameTime, out blocked, out bool _) ? 1 : 0;
    if (blocked)
      mover.CurrentSpeed = 0.0f;
    if ((num1 & (hasInput ? 1 : 0)) == 0)
      return num1 != 0;
    if ((double) mover.PushCooldown <= 0.0)
      return num1 != 0;
    mover.NextPushTime = this._timing.CurTime + TimeSpan.FromSeconds((double) mover.PushCooldown);
    return num1 != 0;
  }

  private bool UpdateDriveMovement(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i inputDir,
    float frameTime)
  {
    bool hasInput = Vector2i.op_Inequality(inputDir, Vector2i.Zero);
    Vector2i currentDirection1 = mover.CurrentDirection;
    bool flag1 = Vector2i.op_Inequality(currentDirection1, Vector2i.Zero);
    if (hasInput && !flag1)
    {
      if (!this.TryApplyFacing(uid, mover, grid, gridComp, inputDir, false, false, false))
      {
        mover.CurrentSpeed = 0.0f;
        return false;
      }
      currentDirection1 = mover.CurrentDirection;
      flag1 = true;
    }
    bool flag2 = hasInput & flag1 && Vector2i.op_Equality(inputDir, Vector2i.op_UnaryNegation(currentDirection1));
    int num1 = !(hasInput & flag1) || flag2 ? 0 : (Vector2i.op_Inequality(inputDir, currentDirection1) ? 1 : 0);
    float num2 = MathF.Max(0.0f, mover.TurnInPlaceMaxSpeed);
    bool flag3 = (double) MathF.Abs(mover.CurrentSpeed) <= (double) num2;
    if (num1 != 0)
    {
      if (!this.CanApplyTurn(mover))
      {
        if ((double) MathF.Abs(mover.CurrentSpeed) <= 0.0099999997764825821)
        {
          mover.CurrentSpeed = 0.0f;
          return false;
        }
      }
      else
      {
        if (mover.TurnInPlace & flag3)
        {
          int num3 = this.TryApplyFacing(uid, mover, grid, gridComp, inputDir, true, true, false) ? 1 : 0;
          mover.CurrentSpeed = 0.0f;
          return num3 != 0;
        }
        if (this.TryApplyFacing(uid, mover, grid, gridComp, inputDir, true, false, true))
        {
          Vector2i currentDirection2 = mover.CurrentDirection;
          if (!flag3)
          {
            TimeSpan curTime = this._timing.CurTime;
            bool flag4 = curTime - mover.LastMovingTurnTime <= TimeSpan.FromSeconds((double) mover.RapidTurnWindow);
            mover.TurnStreak = flag4 ? mover.TurnStreak + 1 : 0;
            float y = mover.TurnStreak >= mover.RapidTurnGraceCount ? mover.RapidTurnSpeedMultiplier : mover.TurnSpeedMultiplier;
            if ((double) y < 1.0)
              mover.CurrentSpeed *= MathF.Max(0.0f, y);
            mover.LastMovingTurnTime = curTime;
          }
        }
        else if ((double) MathF.Abs(mover.CurrentSpeed) <= 0.0099999997764825821)
        {
          mover.CurrentSpeed = 0.0f;
          return false;
        }
      }
    }
    if (((!(mover.TurnInPlace & hasInput) ? 0 : (!flag2 ? 1 : 0)) & (flag3 ? 1 : 0)) != 0 && mover.InPlaceTurnBlockUntil > this._timing.CurTime)
    {
      mover.CurrentSpeed = GridVehicleMotionSimulator.StepIdleSpeed(mover.CurrentSpeed, mover.Deceleration, frameTime);
      return false;
    }
    if (Vector2i.op_Equality(mover.CurrentDirection, Vector2i.Zero))
    {
      mover.CurrentSpeed = 0.0f;
      return false;
    }
    GridVehicleMotionSimulator.DriveProfile driveProfile = this.GetDriveProfile(uid, mover);
    GridVehicleMotionSimulator.DriveSpeedResult driveSpeedResult = hasInput ? GridVehicleMotionSimulator.StepDriveSpeed(mover.CurrentSpeed, driveProfile, mover.CurrentDirection, inputDir, hasInput, false, frameTime) : new GridVehicleMotionSimulator.DriveSpeedResult(GridVehicleMotionSimulator.StepIdleSpeed(mover.CurrentSpeed, mover.Deceleration, frameTime), false, false);
    mover.CurrentSpeed = driveSpeedResult.CurrentSpeed;
    if (driveSpeedResult.ChangingDirection)
    {
      mover.CurrentSpeed = 0.0f;
      return false;
    }
    float travel = MathF.Abs(mover.CurrentSpeed) * frameTime;
    if ((double) travel <= 9.9999997473787516E-05)
      return false;
    Vector2i moveDir = (double) mover.CurrentSpeed >= 0.0 ? mover.CurrentDirection : Vector2i.op_UnaryNegation(mover.CurrentDirection);
    Angle vehicleRotation = GridVehicleMoverSystem.DirectionToVehicleRotation(mover.CurrentDirection);
    bool blocked;
    bool scraped;
    int num4 = this.TryMoveWithLaneGuidance(uid, mover, grid, gridComp, moveDir, new Angle?(vehicleRotation), travel, frameTime, out blocked, out scraped) ? 1 : 0;
    if (blocked)
    {
      mover.CurrentSpeed = 0.0f;
      return num4 != 0;
    }
    if (!scraped)
      return num4 != 0;
    GridVehicleMoverSystem.ApplyScrapeSlowdown(mover, frameTime);
    return num4 != 0;
  }

  private static void ApplyScrapeSlowdown(GridVehicleMoverComponent mover, float frameTime)
  {
    if ((double) mover.ScrapeDeceleration <= 0.0)
      return;
    float x1 = MathF.Abs(mover.CurrentSpeed);
    float x2 = MathF.Max(MathF.Min(x1, MathF.Max(0.0f, mover.ScrapeMinSpeed)), x1 - mover.ScrapeDeceleration * frameTime);
    mover.CurrentSpeed = MathF.CopySign(x2, mover.CurrentSpeed);
  }

  private bool TryApplyFacing(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i desiredFacing,
    bool startDelay,
    bool blockAfterTurn,
    bool allowMoveClearance)
  {
    if (Vector2i.op_Equality(desiredFacing, Vector2i.Zero))
      return false;
    Angle vehicleRotation = GridVehicleMoverSystem.DirectionToVehicleRotation(desiredFacing);
    Vector2 gridPos;
    if (!this.TryFindTurnPosition(uid, mover, grid, gridComp, vehicleRotation, out gridPos) && (!allowMoveClearance || !this.TryFindTransientTurnClearance(uid, mover, grid, desiredFacing, vehicleRotation, out gridPos)) || !this.CanOccupyTransform(uid, mover, grid, gridPos, new Angle?(vehicleRotation), 0.0075f, true))
      return false;
    int num = Vector2i.op_Inequality(mover.CurrentDirection, desiredFacing) ? 1 : 0;
    bool flag = gridPos != mover.Position;
    mover.Position = gridPos;
    mover.CurrentTile = this.GetTile(grid, gridComp, mover.Position);
    mover.CurrentDirection = desiredFacing;
    this.transform.SetLocalRotation(uid, vehicleRotation);
    if ((num & (startDelay ? 1 : 0)) != 0)
    {
      this.StartTurnDelay(mover);
      if (blockAfterTurn && (double) mover.TurnDelay > 0.0)
        mover.InPlaceTurnBlockUntil = this._timing.CurTime + TimeSpan.FromSeconds((double) mover.TurnDelay);
    }
    return (num | (flag ? 1 : 0)) != 0;
  }

  private bool TryFindTransientTurnClearance(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    Vector2i desiredFacing,
    Angle desiredRot,
    out Vector2 clearPosition)
  {
    clearPosition = mover.Position;
    float y = MathF.Max(0.0f, mover.TurnCollisionGraceDistance);
    if ((double) y <= 0.0)
      return false;
    Vector2 vector2 = new Vector2((float) desiredFacing.X, (float) desiredFacing.Y);
    if ((double) vector2.LengthSquared() <= 0.0)
      return false;
    float num1 = Math.Clamp(mover.MovementProbeStep, 0.02f, 0.5f);
    int num2 = Math.Max(1, (int) MathF.Ceiling(y / num1));
    HashSet<EntityUid> blockers1 = new HashSet<EntityUid>();
    if (this.CanOccupyTransform(uid, mover, grid, mover.Position, new Angle?(desiredRot), 0.0075f, false, blockers: blockers1) || blockers1.Count == 0)
      return false;
    HashSet<EntityUid> blockers2 = new HashSet<EntityUid>();
    for (int index = 1; index <= num2; ++index)
    {
      float num3 = MathF.Min((float) index * num1, y);
      Vector2 gridPos = mover.Position + vector2 * num3;
      blockers2.Clear();
      if (this.CanOccupyTransform(uid, mover, grid, gridPos, new Angle?(desiredRot), 0.0075f, false, blockers: blockers2))
      {
        clearPosition = gridPos;
        return true;
      }
      foreach (EntityUid entityUid in blockers2)
      {
        if (!blockers1.Contains(entityUid))
          return false;
      }
      if (blockers2.Count <= 0)
        return false;
    }
    return false;
  }

  private bool TryMoveWithLaneGuidance(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i moveDir,
    Angle? rotation,
    float travel,
    float frameTime,
    out bool blocked,
    out bool scraped)
  {
    blocked = false;
    scraped = false;
    Vector2 moveDirection = new Vector2((float) moveDir.X, (float) moveDir.Y);
    Vector2 vector2_1 = mover.Position + moveDirection * travel;
    bool flag1 = false;
    this._directMoveBlockers.Clear();
    HashSet<EntityUid> pushIgnoredEntities = this.GetPushIgnoredEntities(uid, mover);
    if (this.CanMoveContinuous(uid, mover, grid, vector2_1, rotation, GridVehicleMoverSystem.CollisionDebugEnabled, this._directMoveBlockers, pushIgnoredEntities))
    {
      GridVehicleMoverSystem.AddDebugMovementDecision(uid, grid, mover.Position, vector2_1, moveDirection, GridVehicleMoverSystem.DebugMovementDecisionKind.DirectClear, true);
      return this.TryMoveKnownClear(uid, mover, grid, vector2_1, rotation, out blocked, ignoredEntities: pushIgnoredEntities);
    }
    scraped = true;
    GridVehicleMoverSystem.AddDebugMovementDecision(uid, grid, mover.Position, vector2_1, moveDirection, GridVehicleMoverSystem.DebugMovementDecisionKind.DirectBlocked, false);
    float correction1;
    if (this.TryGetBlockingMobBypassCorrection(uid, mover, grid, gridComp, moveDir, rotation, vector2_1, frameTime, this._directMoveBlockers, pushIgnoredEntities, out correction1))
    {
      flag1 = this.TryApplyLateralCorrection(uid, mover, grid, moveDir, rotation, correction1, pushIgnoredEntities);
      if (flag1)
        return true;
    }
    float correction2;
    if (this.TryGetLaneCorrection(uid, mover, grid, gridComp, moveDir, rotation, vector2_1, frameTime, pushIgnoredEntities, out correction2))
      flag1 = this.TryApplyLateralCorrection(uid, mover, grid, moveDir, rotation, correction2, pushIgnoredEntities);
    Vector2 position = mover.Position;
    Vector2 vector2_2 = mover.Position + moveDirection * travel;
    bool flag2 = this.TryMoveContinuous(uid, mover, grid, vector2_2, rotation, out blocked, ignoredEntities: pushIgnoredEntities);
    GridVehicleMoverSystem.AddDebugMovementDecision(uid, grid, position, vector2_2, moveDirection, blocked ? GridVehicleMoverSystem.DebugMovementDecisionKind.ForwardBlocked : GridVehicleMoverSystem.DebugMovementDecisionKind.ForwardAfterCorrection, flag2 && !blocked);
    return flag1 | flag2;
  }

  private bool TryApplyLateralCorrection(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    Vector2i moveDir,
    Angle? rotation,
    float correction,
    HashSet<EntityUid>? ignoredEntities)
  {
    Vector2 vector2 = GridVehicleMoverSystem.SetLateralCoordinate(mover.Position, moveDir, GridVehicleMoverSystem.GetLateralCoordinate(mover.Position, moveDir) + correction);
    if ((double) (vector2 - mover.Position).LengthSquared() <= 9.99999905104687E-09)
      return false;
    Vector2 position = mover.Position;
    Vector2 moveDirection = vector2 - position;
    bool success = this.TryMoveContinuous(uid, mover, grid, vector2, rotation, out bool _, false, false, ignoredEntities);
    GridVehicleMoverSystem.AddDebugMovementDecision(uid, grid, position, vector2, moveDirection, success ? GridVehicleMoverSystem.DebugMovementDecisionKind.LaneCorrection : GridVehicleMoverSystem.DebugMovementDecisionKind.LaneCorrectionFailed, success);
    return success;
  }

  private static void AddDebugMovementDecision(
    EntityUid uid,
    EntityUid grid,
    Vector2 start,
    Vector2 end,
    Vector2 moveDirection,
    GridVehicleMoverSystem.DebugMovementDecisionKind kind,
    bool success)
  {
    if (!GridVehicleMoverSystem.MovementDebugEnabled)
      return;
    moveDirection = (double) moveDirection.LengthSquared() <= 9.9999997473787516E-05 ? Vector2.Zero : Vector2.Normalize(moveDirection);
    GridVehicleMoverSystem.DebugMovementDecisions.Add(new GridVehicleMoverSystem.DebugMovementDecision(uid, grid, start, end, moveDirection, kind, success));
  }

  private bool CanMoveContinuous(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    Vector2 target,
    Angle? rotation,
    bool debugProbes)
  {
    return this.CanMoveContinuous(uid, mover, grid, target, rotation, debugProbes, (HashSet<EntityUid>) null);
  }

  private bool CanMoveContinuous(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    Vector2 target,
    Angle? rotation,
    bool debugProbes,
    HashSet<EntityUid>? blockers,
    HashSet<EntityUid>? ignoredEntities = null)
  {
    Vector2 position = mover.Position;
    Vector2 vector2 = target - position;
    float num1 = vector2.Length();
    if ((double) num1 <= 9.9999997473787516E-05)
      return true;
    float num2 = Math.Clamp(mover.MovementProbeStep, 0.02f, 0.5f);
    int num3 = Math.Max(1, (int) MathF.Ceiling(num1 / num2));
    for (int index = 1; index <= num3; ++index)
    {
      Vector2 gridPos = position + vector2 * ((float) index / (float) num3);
      if (!this.CanOccupyTransform(uid, mover, grid, gridPos, rotation, 0.0075f, false, debugProbes, blockers, ignoredEntities))
        return false;
    }
    return true;
  }

  private bool TryGetBlockingMobBypassCorrection(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i moveDir,
    Angle? rotation,
    Vector2 target,
    float frameTime,
    HashSet<EntityUid> directBlockers,
    HashSet<EntityUid>? ignoredEntities,
    out float correction)
  {
    correction = 0.0f;
    float laneOffset;
    return this.HasBlockingVehicleMob(mover, directBlockers) && this.TryFindBlockingMobBypassOffset(uid, mover, grid, gridComp, moveDir, rotation, target, ignoredEntities, out laneOffset) && this.TryGetLateralCorrection(mover, grid, gridComp, moveDir, target, laneOffset, frameTime, out correction);
  }

  private bool TryFindBlockingMobBypassOffset(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i moveDir,
    Angle? rotation,
    Vector2 target,
    HashSet<EntityUid>? ignoredEntities,
    out float laneOffset)
  {
    laneOffset = 0.0f;
    float num1 = Math.Clamp(MathF.Max(mover.TileOffsetLimit, mover.BlockingMobBypassNudgeLimit), 0.0f, 3f);
    if ((double) num1 <= 0.0 || Vector2i.op_Equality(moveDir, Vector2i.Zero))
      return false;
    float num2 = Math.Clamp((double) mover.BlockingMobBypassNudgeStep > 0.0 ? mover.BlockingMobBypassNudgeStep : mover.TileOffsetStep, 0.01f, num1);
    float lateralCoordinate = GridVehicleMoverSystem.GetLateralCoordinate(GridVehicleMoverSystem.GetTileCenter(this.GetTile(grid, gridComp, target)), moveDir);
    float num3 = Math.Clamp(GridVehicleMoverSystem.GetLateralCoordinate(target, moveDir) - lateralCoordinate, -num1, num1);
    int lookahead = Math.Max(1, mover.TileOffsetLookahead);
    int num4 = (int) MathF.Ceiling(num1 / num2);
    float lastOffset1 = float.NaN;
    float lastOffset2 = float.NaN;
    for (int index = 1; index <= num4; ++index)
    {
      float num5 = MathF.Min((float) index * num2, num1);
      if (this.TryBlockingMobBypassOffset(uid, mover, grid, gridComp, moveDir, rotation, target, num3 + num5, num1, lateralCoordinate, lookahead, ignoredEntities, ref lastOffset1, out laneOffset) || this.TryBlockingMobBypassOffset(uid, mover, grid, gridComp, moveDir, rotation, target, num3 - num5, num1, lateralCoordinate, lookahead, ignoredEntities, ref lastOffset2, out laneOffset))
        return true;
    }
    return false;
  }

  private bool TryBlockingMobBypassOffset(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i moveDir,
    Angle? rotation,
    Vector2 target,
    float offset,
    float limit,
    float centerLateral,
    int lookahead,
    HashSet<EntityUid>? ignoredEntities,
    ref float lastOffset,
    out float laneOffset)
  {
    laneOffset = Math.Clamp(offset, -limit, limit);
    if ((double) MathF.Abs(laneOffset - lastOffset) <= 1.0 / 1000.0)
      return false;
    lastOffset = laneOffset;
    float lateral = centerLateral + laneOffset;
    Vector2 target1 = GridVehicleMoverSystem.SetLateralCoordinate(mover.Position, moveDir, lateral);
    return (double) (target1 - mover.Position).LengthSquared() > 9.99999905104687E-09 && this.CanMoveContinuous(uid, mover, grid, target1, rotation, false, (HashSet<EntityUid>) null, ignoredEntities) && this.CanOccupyMoveLane(uid, mover, grid, gridComp, moveDir, rotation, target, laneOffset, lookahead, ignoredEntities);
  }

  private bool TryGetLaneCorrection(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i moveDir,
    Angle? rotation,
    Vector2 target,
    float frameTime,
    HashSet<EntityUid>? ignoredEntities,
    out float correction)
  {
    correction = 0.0f;
    float laneOffset;
    return this.TryFindBestLaneOffset(uid, mover, grid, gridComp, moveDir, rotation, target, ignoredEntities, out laneOffset) && this.TryGetLateralCorrection(mover, grid, gridComp, moveDir, target, laneOffset, frameTime, out correction);
  }

  private bool TryGetLateralCorrection(
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i moveDir,
    Vector2 target,
    float laneOffset,
    float frameTime,
    out float correction)
  {
    Vector2 tileCenter = GridVehicleMoverSystem.GetTileCenter(this.GetTile(grid, gridComp, target));
    float lateralCoordinate = GridVehicleMoverSystem.GetLateralCoordinate(mover.Position, moveDir);
    Vector2i moveDir1 = moveDir;
    float num1 = GridVehicleMoverSystem.GetLateralCoordinate(tileCenter, moveDir1) + laneOffset;
    float num2 = MathF.Max(0.0f, mover.LaneCorrectionSpeed);
    if ((double) num2 <= 0.0)
    {
      correction = 0.0f;
      return false;
    }
    float max = num2 * frameTime;
    correction = Math.Clamp(num1 - lateralCoordinate, -max, max);
    return (double) MathF.Abs(correction) > 9.9999997473787516E-05;
  }

  private bool TryFindBestLaneOffset(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i moveDir,
    Angle? rotation,
    Vector2 target,
    HashSet<EntityUid>? ignoredEntities,
    out float laneOffset)
  {
    laneOffset = 0.0f;
    float max = Math.Clamp(mover.TileOffsetLimit, 0.0f, 2f);
    if ((double) max <= 0.0 || Vector2i.op_Equality(moveDir, Vector2i.Zero))
      return false;
    float step = Math.Clamp(mover.TileOffsetStep, 0.01f, max);
    Vector2 tileCenter = GridVehicleMoverSystem.GetTileCenter(this.GetTile(grid, gridComp, target));
    float baseOffset = Math.Clamp(GridVehicleMoverSystem.GetLateralCoordinate(target, moveDir) - GridVehicleMoverSystem.GetLateralCoordinate(tileCenter, moveDir), -max, max);
    int num = (int) MathF.Ceiling(max / step);
    int lookahead = Math.Max(1, mover.TileOffsetLookahead);
    bool flag1 = false;
    float bestOffset = baseOffset;
    float maxValue = float.MaxValue;
    bool flag2 = false;
    float laneStart = 0.0f;
    float laneEnd = 0.0f;
    for (int index = -num; index <= num; ++index)
    {
      float offset = Math.Clamp((float) index * step, -max, max);
      if (this.CanOccupyMoveLane(uid, mover, grid, gridComp, moveDir, rotation, target, offset, lookahead, ignoredEntities))
      {
        if (!flag2)
        {
          flag2 = true;
          laneStart = offset;
        }
        laneEnd = offset;
      }
      else if (flag2)
      {
        flag1 = true;
        this.SelectMoveLane(laneStart, laneEnd, baseOffset, step, ref bestOffset, ref maxValue);
        flag2 = false;
      }
    }
    if (flag2)
    {
      flag1 = true;
      this.SelectMoveLane(laneStart, laneEnd, baseOffset, step, ref bestOffset, ref maxValue);
    }
    if (!flag1)
      return false;
    laneOffset = bestOffset;
    return true;
  }

  private bool CanOccupyMoveLane(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i moveDir,
    Angle? rotation,
    Vector2 target,
    float offset,
    int lookahead,
    HashSet<EntityUid>? ignoredEntities)
  {
    Vector2 vector2_1 = new Vector2((float) moveDir.X, (float) moveDir.Y);
    float num = MathF.Max(1f, (float) gridComp.TileSize);
    for (int index = 0; index < lookahead; ++index)
    {
      Vector2 vector2_2 = target + vector2_1 * (num * (float) index);
      float lateral = GridVehicleMoverSystem.GetLateralCoordinate(GridVehicleMoverSystem.GetTileCenter(this.GetTile(grid, gridComp, vector2_2)), moveDir) + offset;
      Vector2 gridPos = GridVehicleMoverSystem.SetLateralCoordinate(vector2_2, moveDir, lateral);
      if (!this.CanOccupyTransform(uid, mover, grid, gridPos, rotation, 0.0075f, false, false, ignoredEntities: ignoredEntities))
        return false;
    }
    return true;
  }

  private bool TryMoveContinuous(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    Vector2 target,
    Angle? rotation,
    out bool blocked,
    bool applyBlockEffects = true,
    bool debugProbes = true,
    HashSet<EntityUid>? ignoredEntities = null)
  {
    blocked = false;
    Vector2 position = mover.Position;
    Vector2 vector2 = target - position;
    float num1 = vector2.Length();
    if ((double) num1 <= 9.9999997473787516E-05)
      return false;
    float num2 = Math.Clamp(mover.MovementProbeStep, 0.02f, 0.5f);
    int num3 = Math.Max(1, (int) MathF.Ceiling(num1 / num2));
    Vector2 gridPos1 = position;
    for (int index = 1; index <= num3; ++index)
    {
      Vector2 gridPos2 = position + vector2 * ((float) index / (float) num3);
      if (!this.CanOccupyTransform(uid, mover, grid, gridPos2, rotation, 0.0075f, false, debugProbes, ignoredEntities: ignoredEntities))
      {
        if (applyBlockEffects)
          this.CanOccupyTransform(uid, mover, grid, gridPos2, rotation, 0.0075f, true, false, ignoredEntities: ignoredEntities);
        mover.Position = gridPos1;
        blocked = true;
        return gridPos1 != position;
      }
      gridPos1 = gridPos2;
    }
    if (applyBlockEffects && !this.CanOccupyTransform(uid, mover, grid, gridPos1, rotation, 0.0075f, true, false, ignoredEntities: ignoredEntities))
    {
      mover.Position = position;
      blocked = true;
      return false;
    }
    mover.Position = gridPos1;
    return true;
  }

  private bool TryMoveKnownClear(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    Vector2 target,
    Angle? rotation,
    out bool blocked,
    bool applyBlockEffects = true,
    HashSet<EntityUid>? ignoredEntities = null)
  {
    blocked = false;
    if (applyBlockEffects && !this.CanOccupyTransform(uid, mover, grid, target, rotation, 0.0075f, true, false, ignoredEntities: ignoredEntities))
    {
      blocked = true;
      return false;
    }
    mover.Position = target;
    return true;
  }

  private bool TryFindTurnPosition(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Angle desiredRot,
    out Vector2 turnPosition)
  {
    turnPosition = mover.Position;
    if (this.CanOccupyTransform(uid, mover, grid, turnPosition, new Angle?(desiredRot), 0.0075f, false))
      return true;
    float num1 = Math.Clamp(mover.TurnNudgeLimit, 0.0f, 0.49f);
    if ((double) num1 <= 0.0)
      return false;
    float num2 = Math.Clamp(mover.TurnNudgeStep, 0.01f, num1);
    Vector2i tile = this.GetTile(grid, gridComp, mover.Position);
    Vector2 tileCenter = GridVehicleMoverSystem.GetTileCenter(tile);
    Vector2 min = tileCenter - new Vector2(num1, num1);
    Vector2 max = tileCenter + new Vector2(num1, num1);
    int num3 = (int) MathF.Ceiling(num1 / num2);
    for (int index1 = 1; index1 <= num3; ++index1)
    {
      float num4 = Math.Clamp((float) index1 * num2, -num1, num1);
      if (this.TryTurnNudgePosition(uid, mover, grid, gridComp, tile, desiredRot, new Vector2(num4, 0.0f), min, max, out turnPosition) || this.TryTurnNudgePosition(uid, mover, grid, gridComp, tile, desiredRot, new Vector2(-num4, 0.0f), min, max, out turnPosition) || this.TryTurnNudgePosition(uid, mover, grid, gridComp, tile, desiredRot, new Vector2(0.0f, num4), min, max, out turnPosition) || this.TryTurnNudgePosition(uid, mover, grid, gridComp, tile, desiredRot, new Vector2(0.0f, -num4), min, max, out turnPosition))
        return true;
      for (int index2 = -index1; index2 <= index1; ++index2)
      {
        for (int index3 = -index1; index3 <= index1; ++index3)
        {
          if (Math.Max(Math.Abs(index2), Math.Abs(index3)) == index1 && index2 != 0 && index3 != 0)
          {
            Vector2 offset = new Vector2(Math.Clamp((float) index2 * num2, -num1, num1), Math.Clamp((float) index3 * num2, -num1, num1));
            if (this.TryTurnNudgePosition(uid, mover, grid, gridComp, tile, desiredRot, offset, min, max, out turnPosition))
              return true;
          }
        }
      }
    }
    return false;
  }

  private bool TryTurnNudgePosition(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i currentTile,
    Angle desiredRot,
    Vector2 offset,
    Vector2 min,
    Vector2 max,
    out Vector2 turnPosition)
  {
    turnPosition = new Vector2(Math.Clamp(mover.Position.X + offset.X, min.X, max.X), Math.Clamp(mover.Position.Y + offset.Y, min.Y, max.Y));
    return !(turnPosition == mover.Position) && !Vector2i.op_Inequality(this.GetTile(grid, gridComp, turnPosition), currentTile) && this.CanOccupyTransform(uid, mover, grid, turnPosition, new Angle?(desiredRot), 0.0075f, false);
  }

  private void SelectMoveLane(
    float laneStart,
    float laneEnd,
    float baseOffset,
    float step,
    ref float bestOffset,
    ref float bestScore)
  {
    float num1 = laneEnd - laneStart;
    float num2 = (float) (((double) laneStart + (double) laneEnd) * 0.5);
    float num3 = MathF.Min(step * 2f, num1 * 0.5f);
    float min = laneStart + num3;
    float max = laneEnd - num3;
    float num4 = (double) min <= (double) max ? Math.Clamp(baseOffset, min, max) : num2;
    float num5 = MathF.Abs(num4 - baseOffset) - num1 * 0.25f;
    if ((double) num5 >= (double) bestScore)
      return;
    bestScore = num5;
    bestOffset = num4;
  }

  private GridVehicleMotionSimulator.DriveProfile GetDriveProfile(
    EntityUid uid,
    GridVehicleMoverComponent mover)
  {
    float accelerationModifier = this.GetAccelerationModifier(uid);
    return new GridVehicleMotionSimulator.DriveProfile(this.GetModifiedMaxSpeed(uid, mover), this.GetModifiedMaxReverseSpeed(uid, mover), mover.Acceleration * accelerationModifier, mover.ReverseAcceleration * accelerationModifier, mover.Deceleration);
  }

  private float GetModifiedMaxSpeed(EntityUid uid, GridVehicleMoverComponent mover)
  {
    float modifiedMaxSpeed = mover.MaxSpeed * this.GetSmashSlowdownMultiplier(mover);
    VehicleOverchargeComponent comp1;
    if (this.TryComp<VehicleOverchargeComponent>(uid, out comp1) && this._timing.CurTime < comp1.ActiveUntil)
      modifiedMaxSpeed *= comp1.SpeedMultiplier;
    VehicleSpeedModifierComponent comp2;
    if (this.TryComp<VehicleSpeedModifierComponent>(uid, out comp2))
      modifiedMaxSpeed *= comp2.SpeedMultiplier;
    return modifiedMaxSpeed;
  }

  private float GetModifiedMaxReverseSpeed(EntityUid uid, GridVehicleMoverComponent mover)
  {
    float modifiedMaxReverseSpeed = mover.MaxReverseSpeed * this.GetSmashSlowdownMultiplier(mover);
    VehicleOverchargeComponent comp1;
    if (this.TryComp<VehicleOverchargeComponent>(uid, out comp1) && this._timing.CurTime < comp1.ActiveUntil)
      modifiedMaxReverseSpeed *= comp1.SpeedMultiplier;
    VehicleSpeedModifierComponent comp2;
    if (this.TryComp<VehicleSpeedModifierComponent>(uid, out comp2))
      modifiedMaxReverseSpeed *= comp2.SpeedMultiplier;
    return modifiedMaxReverseSpeed;
  }

  private float GetAccelerationModifier(EntityUid uid)
  {
    VehicleAccelerationModifierComponent comp;
    return this.TryComp<VehicleAccelerationModifierComponent>(uid, out comp) ? MathF.Max(0.05f, comp.AccelerationMultiplier) : 1f;
  }

  private void StopMover(GridVehicleMoverComponent mover)
  {
    mover.CurrentSpeed = 0.0f;
    mover.IsCommittedToMove = false;
    mover.IsPushMove = false;
    mover.IsMoving = false;
    mover.TargetPosition = mover.Position;
    mover.TargetTile = mover.CurrentTile;
    mover.PushDirection = Vector2i.Zero;
  }

  private void UpdateDerivedTileState(
    EntityUid grid,
    MapGridComponent gridComp,
    GridVehicleMoverComponent mover)
  {
    Vector2i tile = this.GetTile(grid, gridComp, mover.Position);
    mover.CurrentTile = tile;
    mover.TargetTile = tile;
    mover.TargetPosition = mover.Position;
  }

  private static Angle DirectionToVehicleRotation(Vector2i direction)
  {
    return DirectionExtensions.ToWorldAngle(new Vector2((float) direction.X, (float) direction.Y));
  }

  private static float GetLateralCoordinate(Vector2 position, Vector2i moveDir)
  {
    return moveDir.X == 0 ? position.X : position.Y;
  }

  private static Vector2 SetLateralCoordinate(Vector2 position, Vector2i moveDir, float lateral)
  {
    if (moveDir.X != 0)
      position.Y = lateral;
    else
      position.X = lateral;
    return position;
  }

  private static Vector2 GetTileCenter(Vector2i tile)
  {
    return new Vector2((float) tile.X + 0.5f, (float) tile.Y + 0.5f);
  }

  private HashSet<EntityUid>? GetPushIgnoredEntities(EntityUid uid, GridVehicleMoverComponent mover)
  {
    if (!mover.IsPushMove)
      return (HashSet<EntityUid>) null;
    EntityUid uid1;
    if (!this._activeXenoPushers.TryGetValue(uid, out uid1))
      return (HashSet<EntityUid>) null;
    if (!uid1.IsValid() || this.TerminatingOrDeleted(uid1))
    {
      this._activeXenoPushers.Remove(uid);
      return (HashSet<EntityUid>) null;
    }
    this._pushIgnoredEntities.Clear();
    this._pushIgnoredEntities.Add(uid1);
    return this._pushIgnoredEntities;
  }

  private bool CanApplyTurn(GridVehicleMoverComponent mover)
  {
    return (double) mover.TurnDelay <= 0.0 || this._timing.CurTime >= mover.NextTurnTime;
  }

  private void StartTurnDelay(GridVehicleMoverComponent mover)
  {
    if ((double) mover.TurnDelay <= 0.0)
      return;
    mover.NextTurnTime = this._timing.CurTime + TimeSpan.FromSeconds((double) mover.TurnDelay);
  }

  private void SetGridPosition(EntityUid uid, EntityUid grid, Vector2 gridPos)
  {
    TransformComponent xform = this.Transform(uid);
    if (!xform.ParentUid.IsValid())
      return;
    Vector2 position = new EntityCoordinates(grid, gridPos).WithEntityId(xform.ParentUid, this.transform, (IEntityManager) this.EntityManager).Position;
    this.transform.SetLocalPosition(uid, position, xform);
  }

  private Vector2i GetTile(EntityUid grid, MapGridComponent gridComp, Vector2 pos)
  {
    EntityCoordinates coords = new EntityCoordinates(grid, pos);
    return this.map.TileIndicesFor(grid, gridComp, coords);
  }

  private void PlayRunningSound(EntityUid uid)
  {
    VehicleSoundComponent comp;
    if (!this.TryComp<VehicleSoundComponent>(uid, out comp) || comp.RunningSound == null || this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (comp.NextRunningSound > curTime)
      return;
    this._audio.PlayPvs(comp.RunningSound, uid);
    comp.NextRunningSound = curTime + TimeSpan.FromSeconds((double) comp.RunningSoundCooldown);
    this.Dirty(uid, (IComponent) comp);
  }

  private float GetSmashSlowdownMultiplier(GridVehicleMoverComponent mover)
  {
    if ((double) mover.SmashSlowdownMultiplier >= 1.0 && mover.SmashSlowdownUntil == TimeSpan.Zero)
      return 1f;
    TimeSpan curTime = this._timing.CurTime;
    if (!(mover.SmashSlowdownUntil != TimeSpan.Zero) || !(curTime >= mover.SmashSlowdownUntil))
      return Math.Clamp(mover.SmashSlowdownMultiplier, 0.0f, 1f);
    mover.SmashSlowdownMultiplier = 1f;
    mover.SmashSlowdownUntil = TimeSpan.Zero;
    return 1f;
  }

  private void ApplySmashSlowdown(
    EntityUid vehicle,
    GridVehicleMoverComponent mover,
    VehicleSmashableComponent smashable)
  {
    if ((double) smashable.SlowdownDuration <= 0.0 || (double) smashable.SlowdownMultiplier >= 1.0)
      return;
    TimeSpan curTime = this._timing.CurTime;
    mover.SmashSlowdownMultiplier = MathF.Min(mover.SmashSlowdownMultiplier, smashable.SlowdownMultiplier);
    TimeSpan timeSpan1 = TimeSpan.FromSeconds((double) smashable.SlowdownDuration);
    TimeSpan timeSpan2 = curTime + timeSpan1;
    if (timeSpan2 > mover.SmashSlowdownUntil)
      mover.SmashSlowdownUntil = timeSpan2;
    mover.CurrentSpeed *= smashable.SlowdownMultiplier;
  }

  private Vector2i GetInputDirection(InputMoverComponent input)
  {
    int heldMoveButtons = (int) input.HeldMoveButtons;
    Vector2i inputDirection = Vector2i.Zero;
    if ((heldMoveButtons & 1) != 0)
      inputDirection = Vector2i.op_Addition(inputDirection, new Vector2i(0, 1));
    if ((heldMoveButtons & 2) != 0)
      inputDirection = Vector2i.op_Addition(inputDirection, new Vector2i(0, -1));
    if ((heldMoveButtons & 8) != 0)
      inputDirection = Vector2i.op_Addition(inputDirection, new Vector2i(1, 0));
    if ((heldMoveButtons & 4) != 0)
      inputDirection = Vector2i.op_Addition(inputDirection, new Vector2i(-1, 0));
    if (Vector2i.op_Equality(inputDirection, Vector2i.Zero) || inputDirection.X == 0 || inputDirection.Y == 0)
      return inputDirection;
    if (Math.Abs(inputDirection.X) >= Math.Abs(inputDirection.Y))
    {
      // ISSUE: explicit constructor call
      ((Vector2i) ref inputDirection).\u002Ector(Math.Sign(inputDirection.X), 0);
    }
    else
    {
      // ISSUE: explicit constructor call
      ((Vector2i) ref inputDirection).\u002Ector(0, Math.Sign(inputDirection.Y));
    }
    return inputDirection;
  }

  private Vector2i GetMoverInput(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    VehicleComponent vehicle,
    out bool pushing)
  {
    pushing = false;
    EntityUid? nullable = vehicle.Operator;
    InputMoverComponent comp;
    if (nullable.HasValue && this.TryComp<InputMoverComponent>(nullable.GetValueOrDefault(), out comp))
    {
      this._activeXenoPushers.Remove(uid);
      return this.GetInputDirection(comp);
    }
    if (vehicle.Operator.HasValue)
    {
      this._activeXenoPushers.Remove(uid);
      return Vector2i.Zero;
    }
    EntityUid pusher;
    if (!this.TryGetActivePusher(uid, mover, out pusher))
    {
      if (mover.IsPushMove && Vector2i.op_Inequality(mover.PushDirection, Vector2i.Zero) && (double) mover.CurrentSpeed > 0.0099999997764825821)
      {
        pushing = true;
        return Vector2i.Zero;
      }
      this._activeXenoPushers.Remove(uid);
      return Vector2i.Zero;
    }
    pushing = true;
    if (!mover.IsPushMove && !this.CanPushNow(mover))
    {
      this._activeXenoPushers.Remove(uid);
      return Vector2i.Zero;
    }
    Vector2i pushDirection = this.GetPushDirection(uid, pusher);
    if (Vector2i.op_Equality(pushDirection, Vector2i.Zero))
    {
      this._activeXenoPushers.Remove(uid);
      return Vector2i.Zero;
    }
    this._activeXenoPushers[uid] = pusher;
    return pushDirection;
  }

  private bool TryGetActivePusher(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    out EntityUid pusher)
  {
    pusher = new EntityUid();
    PhysicsComponent component1;
    FixturesComponent component2;
    if (!this.physicsQ.TryComp(uid, out component1) || !component1.CanCollide || !this.fixtureQ.TryComp(uid, out component2))
      return false;
    Vector2 worldPosition1 = this.transform.GetWorldPosition(uid);
    ContactEnumerator contacts = this.physics.GetContacts((Entity<FixturesComponent>) (uid, component2));
    float num1 = 0.0f;
    Contact contact;
    while (contacts.MoveNext(out contact))
    {
      if (contact != null && contact.IsTouching)
      {
        EntityUid entityUid = contact.OtherEnt(uid);
        InputMoverComponent comp;
        if (this.HasComp<XenoComponent>(entityUid) && contact.Hard && this.CanXenoPushVehicle(mover, entityUid) && this.TryComp<InputMoverComponent>(entityUid, out comp))
        {
          Vector2i inputDirection = this.GetInputDirection(comp);
          if (!Vector2i.op_Equality(inputDirection, Vector2i.Zero))
          {
            Vector2 worldPosition2 = this.transform.GetWorldPosition(entityUid);
            Vector2 vector2 = worldPosition1 - worldPosition2;
            if ((double) vector2.LengthSquared() > 9.9999997473787516E-05)
            {
              float num2 = Vector2.Dot(new Vector2((float) inputDirection.X, (float) inputDirection.Y), Vector2.Normalize(vector2));
              if ((double) num2 > 0.0 && (double) num2 > (double) num1)
              {
                num1 = num2;
                pusher = entityUid;
              }
            }
          }
        }
      }
    }
    return (double) num1 > 0.0;
  }

  private Vector2i GetPushDirection(EntityUid uid, EntityUid pusher)
  {
    Vector2 vector2 = this.transform.GetWorldPosition(uid) - this.transform.GetWorldPosition(pusher);
    if ((double) vector2.LengthSquared() <= 9.9999997473787516E-05)
      return Vector2i.Zero;
    Angle angle = Angle.FromWorldVec(vector2);
    return DirectionExtensions.ToIntVec(((Angle) ref angle).GetCardinalDir());
  }

  private bool CanPushNow(GridVehicleMoverComponent mover)
  {
    return (double) mover.PushCooldown <= 0.0 || this._timing.CurTime >= mover.NextPushTime;
  }

  private bool CanXenoPushVehicle(GridVehicleMoverComponent mover, EntityUid xeno)
  {
    if (!mover.CanXenosPush)
      return false;
    RMCSizes? xenoPushMinimumSize = mover.XenoPushMinimumSize;
    if (!xenoPushMinimumSize.HasValue)
      return true;
    RMCSizes valueOrDefault = xenoPushMinimumSize.GetValueOrDefault();
    RMCSizes size;
    return this._size.TryGetSize(xeno, out size) && size >= valueOrDefault;
  }

  public bool CanPlanTileStep(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i currentTile,
    Vector2i currentDirection,
    Vector2i moveDir)
  {
    if (Vector2i.op_Equality(moveDir, Vector2i.Zero))
      return false;
    // ISSUE: explicit constructor call
    ((Vector2i) ref moveDir).\u002Ector(Math.Sign(moveDir.X), Math.Sign(moveDir.Y));
    if (moveDir.X != 0 && moveDir.Y != 0 || Vector2i.op_Inequality(currentDirection, Vector2i.Zero) && Vector2i.op_Equality(moveDir, Vector2i.op_UnaryNegation(currentDirection)))
      return false;
    Angle worldAngle = DirectionExtensions.ToWorldAngle(new Vector2((float) moveDir.X, (float) moveDir.Y));
    Vector2 gridPos1 = new Vector2((float) currentTile.X + 0.5f, (float) currentTile.Y + 0.5f);
    if (!this.CanOccupyTransform(uid, mover, grid, gridPos1, new Angle?(worldAngle), 0.0075f, false))
      return false;
    Vector2i vector2i = Vector2i.op_Addition(currentTile, moveDir);
    Vector2 gridPos2 = new Vector2((float) vector2i.X + 0.5f, (float) vector2i.Y + 0.5f);
    return this.CanOccupyTransform(uid, mover, grid, gridPos2, new Angle?(worldAngle), 0.0075f, false);
  }

  private void UpdateRunSound(EntityUid uid, GridVehicleMoverComponent mover)
  {
    VehicleSoundComponent comp1;
    if (this._net.IsClient || !this.TryComp<VehicleSoundComponent>(uid, out comp1) || comp1.RunningSound == null)
      return;
    if (!comp1.RunLoop)
    {
      if (mover.IsMoving && !mover.IsPushMove)
        this.PlayRunOneShot(uid, comp1);
      else
        this.StopRunAudio(uid, comp1);
    }
    else if (mover.IsPushMove)
    {
      this.StopRunAudio(uid, comp1);
    }
    else
    {
      float runRatio = this.GetRunRatio(mover);
      if (!mover.IsMoving || (double) runRatio <= (double) comp1.RunThreshold)
      {
        this.StopRunAudio(uid, comp1);
      }
      else
      {
        float num1 = MathF.Round(runRatio * 6f) / 6f;
        float volume1 = comp1.RunMinVolume + (comp1.RunMaxVolume - comp1.RunMinVolume) * num1;
        float pitch = comp1.RunMinPitch + (comp1.RunMaxPitch - comp1.RunMinPitch) * num1;
        AudioParams audioParams1 = comp1.RunningSound.Params.WithLoop(true).WithVolume(volume1).WithPitchScale(pitch);
        EntityUid? runAudio = comp1.RunAudio;
        if (runAudio.HasValue)
        {
          EntityUid valueOrDefault = runAudio.GetValueOrDefault();
          AudioComponent comp2;
          if (this.TryComp<AudioComponent>(valueOrDefault, out comp2))
          {
            this.TagSound(valueOrDefault, RMCVehicleSoundKind.Run);
            comp1.LastRunPitch = pitch;
            EntityCoordinates coordinates = this.Transform(uid).Coordinates;
            if (this.Transform(valueOrDefault).Coordinates != coordinates)
              this.transform.SetCoordinates(valueOrDefault, coordinates);
            VehicleFarSoundComponent comp3;
            int num2 = this.TryComp<VehicleFarSoundComponent>(uid, out comp3) ? 1 : 0;
            if (num2 != 0)
              volume1 += comp3.VolumeBoost;
            if (float.IsNaN(comp1.LastRunVolume) || (double) MathF.Abs(comp1.LastRunVolume - volume1) > 0.11999999731779099)
            {
              this._audio.SetVolume(new EntityUid?(valueOrDefault), volume1, comp2);
              comp1.LastRunVolume = volume1;
            }
            if (num2 == 0 || !(this._timing.CurTime >= comp3.NextFilterRefresh))
              return;
            comp3.NextFilterRefresh = this._timing.CurTime + TimeSpan.FromSeconds((double) comp3.FilterRefreshInterval);
            this.StopRunAudio(uid, comp1);
            return;
          }
        }
        VehicleFarSoundComponent comp4;
        if (this.TryComp<VehicleFarSoundComponent>(uid, out comp4))
        {
          float volume2 = volume1 + comp4.VolumeBoost;
          AudioParams audioParams2 = audioParams1.WithVolume(volume2);
          audioParams2 = audioParams2.WithMaxDistance(comp4.AudioRange);
          audioParams2 = audioParams2.WithReferenceDistance(comp4.ReferenceDistance);
          AudioParams audioParams3 = audioParams2.WithRolloffFactor(comp4.RolloffFactor);
          EntityCoordinates coordinates = this.Transform(uid).Coordinates;
          MapCoordinates mapCoordinates = this.transform.ToMapCoordinates(coordinates);
          if (!(mapCoordinates.MapId != MapId.Nullspace))
            return;
          Filter playerFilter = Filter.Empty().AddInRange(mapCoordinates, comp4.AudioRange, this._player, (IEntityManager) this.EntityManager);
          (EntityUid Entity, AudioComponent Component)? nullable = this._audio.PlayStatic(comp1.RunningSound, playerFilter, coordinates, true, new AudioParams?(audioParams3));
          if (!nullable.HasValue)
            return;
          comp1.RunAudio = new EntityUid?(nullable.Value.Entity);
          comp1.LastRunVolume = volume2;
          comp1.LastRunPitch = pitch;
          this.TagSound(nullable.Value.Entity, RMCVehicleSoundKind.Run);
        }
        else
        {
          (EntityUid Entity, AudioComponent Component)? nullable = this._audio.PlayPvs(comp1.RunningSound, uid, new AudioParams?(audioParams1));
          if (!nullable.HasValue)
            return;
          comp1.RunAudio = new EntityUid?(nullable.Value.Entity);
          comp1.LastRunVolume = volume1;
          comp1.LastRunPitch = pitch;
          this.TagSound(nullable.Value.Entity, RMCVehicleSoundKind.Run);
        }
      }
    }
  }

  private float GetRunRatio(GridVehicleMoverComponent mover)
  {
    float num = (double) mover.CurrentSpeed < 0.0 ? mover.MaxReverseSpeed : mover.MaxSpeed;
    return (double) num <= 0.0 ? 0.0f : Math.Clamp(MathF.Abs(mover.CurrentSpeed) / num, 0.0f, 1f);
  }

  private void PlayRunOneShot(EntityUid uid, VehicleSoundComponent sound)
  {
    this.StopRunAudio(uid, sound);
    TimeSpan curTime = this._timing.CurTime;
    if (sound.NextRunningSound > curTime || sound.RunningSound == null)
      return;
    VehicleFarSoundComponent comp;
    if (this.TryComp<VehicleFarSoundComponent>(uid, out comp))
    {
      EntityCoordinates coordinates = this.Transform(uid).Coordinates;
      MapCoordinates mapCoordinates = this.transform.ToMapCoordinates(coordinates);
      if (mapCoordinates.MapId != MapId.Nullspace)
      {
        AudioParams audioParams1 = sound.RunningSound.Params.WithVolume(sound.RunningSound.Params.Volume + comp.VolumeBoost);
        audioParams1 = audioParams1.WithMaxDistance(comp.AudioRange);
        audioParams1 = audioParams1.WithReferenceDistance(comp.ReferenceDistance);
        AudioParams audioParams2 = audioParams1.WithRolloffFactor(comp.RolloffFactor);
        Filter playerFilter = Filter.Empty().AddInRange(mapCoordinates, comp.AudioRange, this._player, (IEntityManager) this.EntityManager);
        (EntityUid Entity, AudioComponent Component)? nullable = this._audio.PlayStatic(sound.RunningSound, playerFilter, coordinates, true, new AudioParams?(audioParams2));
        if (nullable.HasValue)
          this.TagSound(nullable.Value.Entity, RMCVehicleSoundKind.Run);
      }
    }
    else
    {
      (EntityUid Entity, AudioComponent Component)? nullable = this._audio.PlayPvs(sound.RunningSound, uid);
      if (nullable.HasValue)
        this.TagSound(nullable.Value.Entity, RMCVehicleSoundKind.Run);
    }
    sound.NextRunningSound = curTime + TimeSpan.FromSeconds((double) sound.RunningSoundCooldown);
  }

  private void StopRunAudio(EntityUid uid, VehicleSoundComponent? sound = null)
  {
    if (!this.Resolve<VehicleSoundComponent>(uid, ref sound, false))
      return;
    sound.RunAudio = this._audio.Stop(sound.RunAudio);
    sound.LastRunVolume = float.NaN;
    sound.LastRunPitch = float.NaN;
  }

  private void TagSound(EntityUid uid, RMCVehicleSoundKind kind)
  {
    this.EnsureComp<RMCVehicleSoundTagComponent>(uid).Kind = kind;
  }

  private void TryLayTrackTile(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i tile)
  {
    if (this._net.IsClient)
      return;
    ProtoId<ContentTileDefinition>? trackTile = mover.TrackTile;
    if (!trackTile.HasValue)
      return;
    ProtoId<ContentTileDefinition> valueOrDefault = trackTile.GetValueOrDefault();
    if (Vector2i.op_Equality(mover.CurrentDirection, Vector2i.Zero) || (double) mover.TrackTileChance <= 0.0 || (double) mover.TrackTileChance < 1.0 && !this._random.Prob(mover.TrackTileChance))
      return;
    Vector2i vector2i1;
    // ISSUE: explicit constructor call
    ((Vector2i) ref vector2i1).\u002Ector(-mover.CurrentDirection.Y, mover.CurrentDirection.X);
    Vector2i vector2i2;
    // ISSUE: explicit constructor call
    ((Vector2i) ref vector2i2).\u002Ector(mover.CurrentDirection.Y, -mover.CurrentDirection.X);
    this.TryLayTrackTileAt(uid, mover, grid, gridComp, Vector2i.op_Addition(tile, vector2i1), valueOrDefault);
    this.TryLayTrackTileAt(uid, mover, grid, gridComp, Vector2i.op_Addition(tile, vector2i2), valueOrDefault);
  }

  private void TryLayTrackTileAt(
    EntityUid uid,
    GridVehicleMoverComponent mover,
    EntityUid grid,
    MapGridComponent gridComp,
    Vector2i tile,
    ProtoId<ContentTileDefinition> trackTile)
  {
    TileRef tile1;
    if (!this.map.TryGetTileRef(grid, gridComp, tile, out tile1) || tile1.Tile.IsEmpty)
      return;
    ContentTileDefinition tileDefinition1 = (ContentTileDefinition) this._tileDefinitions[tile1.Tile.TypeId];
    ContentTileDefinition tileDefinition2 = (ContentTileDefinition) this._tileDefinitions[(string) trackTile];
    if (tile1.Tile.TypeId == (int) tileDefinition2.TileId)
    {
      GridVehicleMoverSystem.TrackRestore trackRestore;
      if ((double) mover.TrackLifetime <= 0.0 || !this._trackRestores.TryGetValue((grid, tile), out trackRestore))
        return;
      this._trackRestores[(grid, tile)] = trackRestore with
      {
        RestoreAt = this._timing.CurTime + TimeSpan.FromSeconds((double) mover.TrackLifetime)
      };
    }
    else
    {
      if (mover.TrackOnlyDiggable && !tileDefinition1.CanDig || mover.TrackTileWhitelist.Count > 0 && !mover.TrackTileWhitelist.Contains((ProtoId<ContentTileDefinition>) tileDefinition1.ID) || mover.TrackTileBlacklist.Contains((ProtoId<ContentTileDefinition>) tileDefinition1.ID))
        return;
      this.map.SetTile(grid, gridComp, tile1.GridIndices, this._tileSystem.GetVariantTile(tileDefinition2, uid.GetHashCode() ^ tile.X ^ tile.Y << 16 /*0x10*/));
      if ((double) mover.TrackLifetime <= 0.0)
        return;
      GridVehicleMoverSystem.TrackRestore trackRestore;
      int OriginalTileId = this._trackRestores.TryGetValue((grid, tile), out trackRestore) ? trackRestore.OriginalTileId : tile1.Tile.TypeId;
      this._trackRestores[(grid, tile)] = new GridVehicleMoverSystem.TrackRestore((int) tileDefinition2.TileId, OriginalTileId, this._timing.CurTime + TimeSpan.FromSeconds((double) mover.TrackLifetime));
    }
  }

  private void UpdateTrackRestores()
  {
    if (this._net.IsClient || this._trackRestores.Count == 0 || this._timing.CurTime < this._nextTrackRestoreScan)
      return;
    this._nextTrackRestoreScan = this._timing.CurTime + TimeSpan.FromSeconds(1L);
    this._dueTrackRestores.Clear();
    foreach (((EntityUid Grid, Vector2i Pos) key, GridVehicleMoverSystem.TrackRestore trackRestore) in this._trackRestores)
    {
      if (this._timing.CurTime >= trackRestore.RestoreAt)
        this._dueTrackRestores.Add(key);
    }
    foreach ((EntityUid Grid, Vector2i Pos) dueTrackRestore in this._dueTrackRestores)
    {
      GridVehicleMoverSystem.TrackRestore trackRestore = this._trackRestores[dueTrackRestore];
      this._trackRestores.Remove(dueTrackRestore);
      MapGridComponent component;
      TileRef tile;
      if (this.gridQ.TryComp(dueTrackRestore.Grid, out component) && this.map.TryGetTileRef(dueTrackRestore.Grid, component, dueTrackRestore.Pos, out tile) && tile.Tile.TypeId == trackRestore.TrackTileId)
      {
        ContentTileDefinition tileDefinition = (ContentTileDefinition) this._tileDefinitions[trackRestore.OriginalTileId];
        this.map.SetTile(dueTrackRestore.Grid, component, dueTrackRestore.Pos, this._tileSystem.GetVariantTile(tileDefinition, dueTrackRestore.Pos.X ^ dueTrackRestore.Pos.Y << 16 /*0x10*/));
      }
    }
  }

  private enum CollisionHandlingResult : byte
  {
    Continue,
    Blocked,
  }

  private readonly record struct CollisionCandidate(
    EntityUid Entity,
    Box2 Aabb,
    Box2 CollisionAabb,
    GridVehicleMoverSystem.VehicleCollisionClass CollisionClass,
    DoorComponent? Door,
    MobStateComponent? MobState,
    bool IsBarricade,
    bool IsXeno,
    bool IsVehicle,
    bool IsUnpoweredDoor)
  ;

  private readonly record struct VehicleOrientedBox(
    Vector2 Center,
    Angle Rotation,
    Vector2 FullHalf,
    Vector2 MovementHalf)
  ;

  private enum VehicleCollisionClass : byte
  {
    Ignore,
    SoftMob,
    Breakable,
    Hard,
  }

  public enum DebugMovementDecisionKind : byte
  {
    DirectClear,
    DirectBlocked,
    LaneCorrection,
    LaneCorrectionFailed,
    ForwardAfterCorrection,
    ForwardBlocked,
  }

  public readonly record struct DebugCollision(
    EntityUid Tested,
    EntityUid Blocker,
    Box2 TestedAabb,
    Box2 BlockerAabb,
    float Distance,
    float Skin,
    float Clearance,
    MapId Map)
  ;

  public readonly record struct DebugCollisionProbe(
    EntityUid Tested,
    Box2 TestedAabb,
    Box2 MovementAabb,
    Box2Rotated FixtureBounds,
    Box2Rotated MovementBounds,
    Vector2 Position,
    Angle Rotation,
    bool Blocked,
    bool ApplyEffects,
    MapId Map)
  ;

  public readonly record struct DebugMovementDecision(
    EntityUid Vehicle,
    EntityUid Grid,
    Vector2 Start,
    Vector2 End,
    Vector2 MoveDirection,
    GridVehicleMoverSystem.DebugMovementDecisionKind Kind,
    bool Success)
  ;

  private readonly record struct TrackRestore(
    int TrackTileId,
    int OriginalTileId,
    TimeSpan RestoreAt)
  ;
}

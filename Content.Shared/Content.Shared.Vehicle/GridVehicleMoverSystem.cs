using System;
using System.Collections.Generic;
using System.Numerics;
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
using Content.Shared.Foldable;
using Content.Shared.Item;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
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

namespace Content.Shared.Vehicle;

public sealed class GridVehicleMoverSystem : EntitySystem
{
	private enum CollisionHandlingResult : byte
	{
		Continue,
		Blocked
	}

	private readonly record struct CollisionCandidate(EntityUid Entity, Box2 Aabb, Box2 CollisionAabb, VehicleCollisionClass CollisionClass, DoorComponent? Door, MobStateComponent? MobState, bool IsBarricade, bool IsXeno, bool IsVehicle, bool IsUnpoweredDoor);

	private readonly record struct VehicleOrientedBox(Vector2 Center, Angle Rotation, Vector2 FullHalf, Vector2 MovementHalf);

	private enum VehicleCollisionClass : byte
	{
		Ignore,
		SoftMob,
		Breakable,
		Hard
	}

	public enum DebugMovementDecisionKind : byte
	{
		DirectClear,
		DirectBlocked,
		LaneCorrection,
		LaneCorrectionFailed,
		ForwardAfterCorrection,
		ForwardBlocked
	}

	public readonly record struct DebugCollision(EntityUid Tested, EntityUid Blocker, Box2 TestedAabb, Box2 BlockerAabb, float Distance, float Skin, float Clearance, MapId Map);

	public readonly record struct DebugCollisionProbe(EntityUid Tested, Box2 TestedAabb, Box2 MovementAabb, Box2Rotated FixtureBounds, Box2Rotated MovementBounds, Vector2 Position, Angle Rotation, bool Blocked, bool ApplyEffects, MapId Map);

	public readonly record struct DebugMovementDecision(EntityUid Vehicle, EntityUid Grid, Vector2 Start, Vector2 End, Vector2 MoveDirection, DebugMovementDecisionKind Kind, bool Success);

	private readonly record struct TrackRestore(int TrackTileId, int OriginalTileId, TimeSpan RestoreAt);

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

	private EntityQuery<MapGridComponent> gridQ;

	private EntityQuery<PhysicsComponent> physicsQ;

	private EntityQuery<FixturesComponent> fixtureQ;

	private const float Clearance = 0.0075f;

	private const double MobCollisionDamage = 8.0;

	private static readonly TimeSpan MobCollisionKnockdown = TimeSpan.FromSeconds(1.5);

	private static readonly TimeSpan MobCollisionCooldown = TimeSpan.FromSeconds(0.75);

	private static readonly ProtoId<DamageTypePrototype> CollisionDamageType = ProtoId<DamageTypePrototype>.op_Implicit("Blunt");

	private const int GridVehicleStaticBlockerMask = 335544350;

	private const CollisionGroup GridVehiclePushHardBlockMask = CollisionGroup.MobMask | CollisionGroup.DropshipImpassable;

	private const float PushTileBlockFraction = 0.005f;

	private const float PushOverlapEpsilon = 0.05f;

	private const float PushAxisHysteresis = 0.05f;

	private const float PushWallSkin = 0.1f;

	private const float PushWallOverlapArea = 0.01f;

	private const float MovementFixedStep = 1f / 60f;

	private const int MaxFixedStepsPerFrame = 6;

	private const float ClientSmoothingSnapDistance = 1.25f;

	private const float ClientSmoothingRate = 22f;

	private const float ClientSmoothingSnapAngle = 1f;

	public static readonly List<(EntityUid grid, Vector2i tile)> DebugTestedTiles = new List<(EntityUid, Vector2i)>();

	public static readonly List<DebugCollisionProbe> DebugCollisionProbes = new List<DebugCollisionProbe>();

	public static readonly List<DebugCollision> DebugCollisions = new List<DebugCollision>();

	public static readonly List<DebugMovementDecision> DebugMovementDecisions = new List<DebugMovementDecision>();

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

	private readonly Dictionary<(EntityUid Grid, Vector2i Pos), TrackRestore> _trackRestores = new Dictionary<(EntityUid, Vector2i), TrackRestore>();

	private TimeSpan _nextTrackRestoreScan;

	private readonly List<(EntityUid Grid, Vector2i Pos)> _dueTrackRestores = new List<(EntityUid, Vector2i)>();

	public static bool CollisionDebugEnabled { get; set; }

	public static bool MovementDebugEnabled { get; set; }

	public bool CanOccupyCurrent(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		GridVehicleMoverComponent mover = default(GridVehicleMoverComponent);
		if (!((EntitySystem)this).TryComp<GridVehicleMoverComponent>(uid, ref mover))
		{
			return false;
		}
		EntityUid? gridUid = ((EntitySystem)this).Transform(uid).GridUid;
		if (gridUid.HasValue)
		{
			EntityUid grid = gridUid.GetValueOrDefault();
			return CanOccupyTransform(uid, mover, grid, mover.Position, mover.Heading, 0.0075f, applyEffects: false, debug: false);
		}
		return false;
	}

	private void UpdateAircraftMovement(EntityUid uid, GridVehicleMoverComponent mover, VehicleComponent vehicle, EntityUid grid, MapGridComponent gridComp, float frameTime)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		if (vehicle.Operator.HasValue)
		{
			VehicleCanRunEvent canRunEvent = new VehicleCanRunEvent(Entity<VehicleComponent>.op_Implicit((uid, vehicle)));
			((EntitySystem)this).RaiseLocalEvent<VehicleCanRunEvent>(uid, ref canRunEvent, false);
			if (!canRunEvent.CanRun)
			{
				StopMover(mover);
				SetGridPosition(uid, grid, mover.Position);
				transform.SetLocalRotation(uid, mover.Heading, (TransformComponent)null);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)mover, (MetaDataComponent)null);
				return;
			}
		}
		GetSmashSlowdownMultiplier(mover);
		AircraftComponent aircraft = default(AircraftComponent);
		((EntitySystem)this).TryComp<AircraftComponent>(uid, ref aircraft);
		bool air = aircraft?.Airborne ?? false;
		int throttle = 0;
		int steer = 0;
		EntityUid? val = vehicle.Operator;
		if (val.HasValue)
		{
			EntityUid op = val.GetValueOrDefault();
			InputMoverComponent input = default(InputMoverComponent);
			if (((EntitySystem)this).TryComp<InputMoverComponent>(op, ref input))
			{
				MoveButtons heldMoveButtons = input.HeldMoveButtons;
				if ((heldMoveButtons & MoveButtons.Up) != MoveButtons.None)
				{
					throttle++;
				}
				if ((heldMoveButtons & MoveButtons.Down) != MoveButtons.None)
				{
					throttle--;
				}
				if ((heldMoveButtons & MoveButtons.Right) != MoveButtons.None)
				{
					steer++;
				}
				if ((heldMoveButtons & MoveButtons.Left) != MoveButtons.None)
				{
					steer--;
				}
			}
		}
		float accelMod = GetAccelerationModifier(uid);
		float decel = aircraft?.BrakeDeceleration ?? mover.Deceleration;
		mover.CurrentSpeed = GridVehicleMotionSimulator.StepFreeSpeed(mover.CurrentSpeed, throttle, GetModifiedMaxSpeed(uid, mover), 0f, mover.Acceleration * accelMod, 0f, decel, allowReverse: false, frameTime);
		if (air && aircraft != null && mover.CurrentSpeed < aircraft.StallSpeed)
		{
			mover.CurrentSpeed = aircraft.StallSpeed;
		}
		if (steer != 0)
		{
			float fullSpeed = MathF.Max(0.01f, mover.TurnFullSpeed);
			float speedFactor = Math.Clamp(MathF.Abs(mover.CurrentSpeed) / fullSpeed, 0f, 1f);
			if (air)
			{
				speedFactor = MathF.Max(speedFactor, aircraft?.MinAirTurnFactor ?? 0.5f);
			}
			else if (throttle != 0)
			{
				speedFactor = MathF.Max(speedFactor, 0.4f);
			}
			if (speedFactor > 0f)
			{
				float turnRateRad = MathHelper.DegreesToRadians(mover.TurnRate);
				float dHeading = (float)(-steer) * turnRateRad * speedFactor * frameTime;
				Angle val2 = new Angle(mover.Heading.Theta + (double)dHeading);
				Angle newHeading = ((Angle)(ref val2)).Reduced();
				if (air || CanOccupyTransform(uid, mover, grid, mover.Position, newHeading, 0.0075f, applyEffects: false, debug: false))
				{
					mover.Heading = newHeading;
				}
			}
		}
		if (MathF.Abs(mover.CurrentSpeed) > 0.01f)
		{
			Vector2 forward = ((Angle)(ref mover.Heading)).ToWorldVec();
			float travel = mover.CurrentSpeed * frameTime;
			Vector2 target = mover.Position + forward * travel;
			if (air)
			{
				mover.Position = target;
			}
			else
			{
				TryMoveContinuous(uid, mover, grid, target, mover.Heading, out var blocked);
				if (blocked)
				{
					mover.CurrentSpeed = 0f;
				}
			}
		}
		UpdateDerivedTileState(grid, gridComp, mover);
		mover.IsPushMove = false;
		mover.IsMoving = MathF.Abs(mover.CurrentSpeed) > 0.01f;
		UpdateRunSound(uid, mover);
		transform.SetLocalRotation(uid, mover.Heading, (TransformComponent)null);
		SetGridPosition(uid, grid, mover.Position);
		if (mover.IsMoving)
		{
			physics.WakeBody(uid, false, (FixturesComponent)null, (PhysicsComponent)null);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)mover, (MetaDataComponent)null);
	}

	private bool CanOccupyTransform(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, Vector2 gridPos, Angle? overrideRotation, float clearance, bool applyEffects, bool debug = true, HashSet<EntityUid>? blockers = null, HashSet<EntityUid>? ignoredEntities = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid2 = uid;
		GridVehicleMoverComponent mover2 = mover;
		bool applyEffects2 = applyEffects;
		PhysicsComponent body = default(PhysicsComponent);
		FixturesComponent fixtures = default(FixturesComponent);
		if (!physicsQ.TryComp(uid2, ref body) || !fixtureQ.TryComp(uid2, ref fixtures))
		{
			return true;
		}
		EntityUid? operatorUid = null;
		VehicleComponent vehicleComp = default(VehicleComponent);
		if (((EntitySystem)this).TryComp<VehicleComponent>(uid2, ref vehicleComp))
		{
			operatorUid = vehicleComp.Operator;
		}
		if (!body.CanCollide)
		{
			return true;
		}
		MapGridComponent gridComp = default(MapGridComponent);
		if (!gridQ.TryComp(grid, ref gridComp))
		{
			return true;
		}
		EntityCoordinates coords = default(EntityCoordinates);
		((EntityCoordinates)(ref coords))._002Ector(grid, gridPos);
		MapCoordinates world = ((EntityCoordinates)(ref coords)).ToMap((IEntityManager)(object)base.EntityManager, transform);
		bool debugEnabled = debug && CollisionDebugEnabled;
		if (debugEnabled)
		{
			Vector2i tileIndices = map.TileIndicesFor(grid, gridComp, coords);
			DebugTestedTiles.Add((grid, tileIndices));
		}
		Angle rotation = GetCollisionWorldRotation(uid2, grid, overrideRotation);
		Transform tx = new Transform(world.Position, rotation);
		float wheelDamage = (_net.IsClient ? 0f : GetWheelCollisionDamage(uid2, mover2));
		if (!TryGetFixtureAabb(fixtures, tx, out var aabb))
		{
			return true;
		}
		Box2 movementAabb = GetMovementAabb(aabb, mover2);
		VehicleOrientedBox? orientedBox = null;
		bool flag;
		if (vehicleComp != null)
		{
			VehicleMovementKind movementKind = vehicleComp.MovementKind;
			if (movementKind - 2 <= VehicleMovementKind.Grid)
			{
				flag = true;
				goto IL_0190;
			}
		}
		flag = false;
		goto IL_0190;
		IL_05d5:
		return flag;
		IL_0190:
		if ((flag || ForceAllFreeMovement) && TryGetFixtureLocalAabb(fixtures, out var localAabb))
		{
			float inset = Math.Clamp(mover2.MovementCollisionInset, 0f, 0.45f);
			Box2 insetLocal = ((Box2)(ref localAabb)).Enlarged(0f - inset);
			if (!((Box2)(ref insetLocal)).IsValid())
			{
				insetLocal = localAabb;
			}
			Vector2 position = world.Position;
			Vector2 center = ((Box2)(ref localAabb)).Center;
			orientedBox = new VehicleOrientedBox(position + ((Angle)(ref rotation)).RotateVec(ref center), rotation, ((Box2)(ref localAabb)).Size / 2f, ((Box2)(ref insetLocal)).Size / 2f);
		}
		HashSet<EntityUid> entitiesIntersecting = lookup.GetEntitiesIntersecting(world.MapId, aabb, (LookupFlags)6);
		bool playedCollisionSound = false;
		ValueList<EntityUid> mobHits = default(ValueList<EntityUid>);
		mobHits._002Ector(0);
		foreach (EntityUid other in entitiesIntersecting)
		{
			if (other == uid2 || (ignoredEntities != null && ignoredEntities.Contains(other)) || !TryBuildCollisionCandidate(uid2, fixtures, body, other, aabb, movementAabb, operatorUid, orientedBox, out var candidate))
			{
				continue;
			}
			if (candidate.CollisionClass == VehicleCollisionClass.SoftMob && candidate.IsXeno)
			{
				if (HandleSoftXenoCollision(uid2, mover2, grid, world.Position, world.MapId, candidate.Entity, aabb, candidate.Aabb, candidate.CollisionAabb, clearance, applyEffects2, debugEnabled, blockers, wheelDamage, ref playedCollisionSound) != CollisionHandlingResult.Blocked)
				{
					continue;
				}
				AddProbe(probeBlocked: true);
				flag = false;
			}
			else
			{
				if (candidate.CollisionClass == VehicleCollisionClass.SoftMob && candidate.MobState != null && _standing.IsDown(candidate.Entity))
				{
					continue;
				}
				if (applyEffects2)
				{
					DoorComponent door = candidate.Door;
					if (door != null && !_net.IsClient && !candidate.IsUnpoweredDoor)
					{
						_door.TryOpen(candidate.Entity, door, operatorUid);
						if (candidate.IsBarricade)
						{
							_door.OnPartialOpen(candidate.Entity, door);
						}
					}
				}
				if (candidate.CollisionClass == VehicleCollisionClass.Ignore)
				{
					continue;
				}
				if (candidate.CollisionClass == VehicleCollisionClass.Breakable)
				{
					if (HandleBreakableCollision(uid2, mover2, candidate.Entity, candidate.CollisionAabb, candidate.Aabb, clearance, world.MapId, candidate.Door != null, candidate.IsUnpoweredDoor, applyEffects2, debugEnabled, blockers, wheelDamage, ref playedCollisionSound) != CollisionHandlingResult.Blocked)
					{
						continue;
					}
					AddProbe(probeBlocked: true);
					flag = false;
				}
				else
				{
					if (candidate.CollisionClass != VehicleCollisionClass.Hard)
					{
						if (applyEffects2 && _net.IsClient && !candidate.IsXeno && candidate.MobState != null && ShouldPredictVehicleInteractions(uid2))
						{
							PredictRunover(uid2, candidate.Entity, candidate.MobState);
						}
						if (applyEffects2 && !_net.IsClient && candidate.MobState != null && !mobHits.Contains(candidate.Entity))
						{
							mobHits.Add(candidate.Entity);
						}
						continue;
					}
					if (HandleHardCollision(uid2, mover2, grid, gridPos, candidate.Entity, candidate.CollisionAabb, candidate.Aabb, clearance, world.MapId, candidate.IsVehicle, applyEffects2, debugEnabled, blockers, wheelDamage, ref playedCollisionSound) != CollisionHandlingResult.Blocked)
					{
						continue;
					}
					AddProbe(probeBlocked: true);
					flag = false;
				}
			}
			goto IL_05d5;
		}
		if (!_net.IsClient && mobHits.Count > 0)
		{
			Enumerator<EntityUid> enumerator2 = mobHits.GetEnumerator();
			try
			{
				MobStateComponent mob = default(MobStateComponent);
				while (enumerator2.MoveNext())
				{
					EntityUid mobUid = enumerator2.Current;
					if (((EntitySystem)this).TryComp<MobStateComponent>(mobUid, ref mob))
					{
						HandleMobCollision(uid2, mobUid, mob, ref playedCollisionSound);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
			}
		}
		AddProbe(probeBlocked: false);
		return true;
		void AddProbe(bool probeBlocked)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (debugEnabled)
			{
				AddDebugCollisionProbe(uid2, mover2, fixtures, tx, aabb, movementAabb, world.MapId, probeBlocked, applyEffects2);
			}
		}
	}

	private bool TryBuildCollisionCandidate(EntityUid vehicle, FixturesComponent vehicleFixtures, PhysicsComponent vehicleBody, EntityUid other, Box2 vehicleAabb, Box2 movementAabb, EntityUid? operatorUid, VehicleOrientedBox? orientedBox, out CollisionCandidate candidate)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Invalid comparison between Unknown and I4
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		candidate = default(CollisionCandidate);
		TransformComponent otherXform = ((EntitySystem)this).Transform(other);
		if (!otherXform.Anchored && ((EntitySystem)this).HasComp<ItemComponent>(other))
		{
			return false;
		}
		PhysicsComponent otherBody = default(PhysicsComponent);
		if (!physicsQ.TryComp(other, ref otherBody) || !otherBody.CanCollide)
		{
			return false;
		}
		DoorComponent door = default(DoorComponent);
		bool hasDoor = ((EntitySystem)this).TryComp<DoorComponent>(other, ref door);
		bool isBarricade = ((EntitySystem)this).HasComp<BarricadeComponent>(other);
		bool isFoldable = ((EntitySystem)this).HasComp<FoldableComponent>(other);
		MobStateComponent mob = default(MobStateComponent);
		bool isMob = ((EntitySystem)this).TryComp<MobStateComponent>(other, ref mob);
		bool isXeno = ((EntitySystem)this).HasComp<XenoComponent>(other);
		bool isVehicle = ((EntitySystem)this).HasComp<VehicleComponent>(other);
		bool isSmashable = ((EntitySystem)this).HasComp<VehicleSmashableComponent>(other);
		if (!isMob && !isXeno && !otherXform.Anchored && (int)otherBody.BodyType != 4 && !isBarricade && !isFoldable && !isVehicle && !isSmashable)
		{
			return false;
		}
		FixturesComponent otherFixtures = default(FixturesComponent);
		if (!fixtureQ.TryComp(other, ref otherFixtures))
		{
			return false;
		}
		Transform otherTx = physics.GetPhysicsTransform(other, otherXform);
		if (!TryGetFixtureAabb(otherFixtures, otherTx, out var otherAabb))
		{
			return false;
		}
		if (!((Box2)(ref vehicleAabb)).Intersects(ref otherAabb))
		{
			return false;
		}
		bool hardCollidable = physics.IsHardCollidable(Entity<FixturesComponent, PhysicsComponent>.op_Implicit((vehicle, vehicleFixtures, vehicleBody)), Entity<FixturesComponent, PhysicsComponent>.op_Implicit((other, otherFixtures, otherBody)));
		VehicleCollisionClass collisionClass = ClassifyCollisionCandidate(other, otherXform, otherBody, otherFixtures, hardCollidable, isMob, isBarricade, isFoldable, hasDoor, isXeno, isVehicle, isSmashable);
		bool doorPowered;
		bool doorPowerKnown = TryGetDoorPowered(other, out doorPowered);
		bool isUnpoweredDoor = hasDoor && doorPowerKnown && !doorPowered;
		if (hasDoor && doorPowerKnown && doorPowered && door != null && _door.CanOpen(other, door, operatorUid))
		{
			collisionClass = VehicleCollisionClass.Ignore;
		}
		Box2 collisionAabb = GetCollisionAabb(collisionClass, vehicleAabb, movementAabb);
		if (!HasCollisionOverlap(collisionAabb, otherAabb))
		{
			return false;
		}
		if (orientedBox.HasValue)
		{
			VehicleOrientedBox obb = orientedBox.GetValueOrDefault();
			Vector2 obbHalf = ((collisionClass == VehicleCollisionClass.SoftMob) ? obb.FullHalf : obb.MovementHalf);
			if (isVehicle && TryGetFixtureLocalAabb(otherFixtures, out var otherLocal))
			{
				Angle otherRot = default(Angle);
				((Angle)(ref otherRot))._002Ector((double)((Quaternion2D)(ref otherTx.Quaternion2D)).Angle);
				Vector2 position = otherTx.Position;
				Vector2 center = ((Box2)(ref otherLocal)).Center;
				Vector2 otherCenter = position + ((Angle)(ref otherRot)).RotateVec(ref center);
				if (!OrientedBoxIntersectsOrientedBox(obb.Center, obbHalf, obb.Rotation, otherCenter, ((Box2)(ref otherLocal)).Size / 2f, otherRot))
				{
					return false;
				}
			}
			else if (!OrientedBoxIntersectsAabb(obb.Center, obbHalf, obb.Rotation, otherAabb))
			{
				return false;
			}
		}
		candidate = new CollisionCandidate(other, otherAabb, collisionAabb, collisionClass, door, mob, isBarricade, isXeno, isVehicle, isUnpoweredDoor);
		return true;
	}

	private CollisionHandlingResult HandleSoftXenoCollision(EntityUid vehicle, GridVehicleMoverComponent mover, EntityUid grid, Vector2 vehicleWorldPosition, MapId mapId, EntityUid xeno, Box2 vehicleAabb, Box2 xenoAabb, Box2 collisionAabb, float clearance, bool applyEffects, bool debug, HashSet<EntityUid>? blockers, float wheelDamage, ref bool playedCollisionSound)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (ShouldBlockXeno(mover, xeno))
		{
			if (applyEffects)
			{
				PlayMobCollisionSound(vehicle, ref playedCollisionSound);
				ApplyWheelCollisionDamage(vehicle, mover, wheelDamage);
			}
			AddBlockingCollision(vehicle, xeno, collisionAabb, xenoAabb, clearance, mapId, debug, blockers);
			return CollisionHandlingResult.Blocked;
		}
		if (!applyEffects)
		{
			return CollisionHandlingResult.Continue;
		}
		PlayMobCollisionSound(vehicle, ref playedCollisionSound);
		Vector2 vehicleMove = GetVehicleMoveDelta(grid, vehicleWorldPosition, mapId, mover);
		if (PushMobOutOfVehicle(vehicle, xeno, vehicleAabb, xenoAabb, vehicleMove))
		{
			return CollisionHandlingResult.Continue;
		}
		ApplyWheelCollisionDamage(vehicle, mover, wheelDamage);
		AddBlockingCollision(vehicle, xeno, collisionAabb, xenoAabb, clearance, mapId, debug, blockers);
		return CollisionHandlingResult.Blocked;
	}

	private CollisionHandlingResult HandleBreakableCollision(EntityUid vehicle, GridVehicleMoverComponent mover, EntityUid other, Box2 collisionAabb, Box2 otherAabb, float clearance, MapId mapId, bool hasDoor, bool isUnpoweredDoor, bool applyEffects, bool debug, HashSet<EntityUid>? blockers, float wheelDamage, ref bool playedCollisionSound)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		VehicleSmashableComponent smashable = default(VehicleSmashableComponent);
		((EntitySystem)this).TryComp<VehicleSmashableComponent>(other, ref smashable);
		bool canSmash = CanVehicleSmash(vehicle, smashable);
		if ((smashable?.RequiresDoorUnpowered ?? false) && hasDoor && !isUnpoweredDoor)
		{
			canSmash = false;
		}
		if (!canSmash)
		{
			if (applyEffects)
			{
				PlayCollisionSound(vehicle, ref playedCollisionSound);
				ApplyWheelCollisionDamage(vehicle, mover, wheelDamage);
			}
			AddBlockingCollision(vehicle, other, collisionAabb, otherAabb, clearance, mapId, debug, blockers);
			return CollisionHandlingResult.Blocked;
		}
		if (applyEffects)
		{
			TrySmash(other, vehicle, ref playedCollisionSound);
		}
		return CollisionHandlingResult.Continue;
	}

	private bool CanVehicleSmash(EntityUid vehicle, VehicleSmashableComponent? smashable)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (smashable == null)
		{
			return true;
		}
		if (smashable.RequiredVehicleTags.Count > 0 && !_tags.HasAnyTag(vehicle, smashable.RequiredVehicleTags))
		{
			return false;
		}
		return true;
	}

	private CollisionHandlingResult HandleHardCollision(EntityUid vehicle, GridVehicleMoverComponent mover, EntityUid grid, Vector2 gridPos, EntityUid other, Box2 collisionAabb, Box2 otherAabb, float clearance, MapId mapId, bool isVehicle, bool applyEffects, bool debug, HashSet<EntityUid>? blockers, float wheelDamage, ref bool playedCollisionSound)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (isVehicle && TryPushVehicle(vehicle, mover, grid, gridPos, other, applyEffects))
		{
			return CollisionHandlingResult.Continue;
		}
		if (applyEffects)
		{
			PlayCollisionSound(vehicle, ref playedCollisionSound);
			ApplyWheelCollisionDamage(vehicle, mover, wheelDamage);
		}
		AddBlockingCollision(vehicle, other, collisionAabb, otherAabb, clearance, mapId, debug, blockers);
		return CollisionHandlingResult.Blocked;
	}

	private static void AddBlockingCollision(EntityUid vehicle, EntityUid blocker, Box2 collisionAabb, Box2 blockerAabb, float clearance, MapId mapId, bool debug, HashSet<EntityUid>? blockers)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		blockers?.Add(blocker);
		if (debug)
		{
			DebugCollisions.Add(new DebugCollision(vehicle, blocker, collisionAabb, blockerAabb, 0f, 0f, clearance, mapId));
		}
	}

	private static void AddDebugCollisionProbe(EntityUid uid, GridVehicleMoverComponent mover, FixturesComponent fixtures, Transform transformData, Box2 aabb, Box2 movementAabb, MapId map, bool blocked, bool applyEffects)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetFixtureLocalAabb(fixtures, out var localAabb))
		{
			Box2 localMovementAabb = GetMovementAabb(localAabb, mover);
			Angle rotation = default(Angle);
			((Angle)(ref rotation))._002Ector((double)((Quaternion2D)(ref transformData.Quaternion2D)).Angle);
			Box2Rotated fixtureBounds = default(Box2Rotated);
			((Box2Rotated)(ref fixtureBounds))._002Ector(((Box2)(ref localAabb)).Translated(transformData.Position), rotation, transformData.Position);
			Box2Rotated movementBounds = default(Box2Rotated);
			((Box2Rotated)(ref movementBounds))._002Ector(((Box2)(ref localMovementAabb)).Translated(transformData.Position), rotation, transformData.Position);
			DebugCollisionProbes.Add(new DebugCollisionProbe(uid, aabb, movementAabb, fixtureBounds, movementBounds, transformData.Position, rotation, blocked, applyEffects, map));
		}
	}

	private static Box2 GetCollisionAabb(VehicleCollisionClass collisionClass, Box2 fullAabb, Box2 movementAabb)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		if (collisionClass != VehicleCollisionClass.SoftMob)
		{
			return movementAabb;
		}
		return fullAabb;
	}

	private static bool HasCollisionOverlap(Box2 vehicleAabb, Box2 otherAabb)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Box2 intersection = ((Box2)(ref vehicleAabb)).Intersect(ref otherAabb);
		if (((Box2)(ref intersection)).Width > 0f)
		{
			return ((Box2)(ref intersection)).Height > 0f;
		}
		return false;
	}

	private static bool OrientedBoxIntersectsAabb(Vector2 obbCenter, Vector2 obbHalf, Angle rotation, Box2 aabb)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float cos = (float)Math.Cos(rotation.Theta);
		float sin = (float)Math.Sin(rotation.Theta);
		Vector2 axisX = new Vector2(cos, sin);
		Vector2 axisY = new Vector2(0f - sin, cos);
		Vector2 aabbHalf = ((Box2)(ref aabb)).Size / 2f;
		Vector2 d = obbCenter - ((Box2)(ref aabb)).Center;
		if (MathF.Abs(d.X) > aabbHalf.X + obbHalf.X * MathF.Abs(axisX.X) + obbHalf.Y * MathF.Abs(axisY.X))
		{
			return false;
		}
		if (MathF.Abs(d.Y) > aabbHalf.Y + obbHalf.X * MathF.Abs(axisX.Y) + obbHalf.Y * MathF.Abs(axisY.Y))
		{
			return false;
		}
		if (MathF.Abs(Vector2.Dot(d, axisX)) > obbHalf.X + aabbHalf.X * MathF.Abs(axisX.X) + aabbHalf.Y * MathF.Abs(axisX.Y))
		{
			return false;
		}
		if (MathF.Abs(Vector2.Dot(d, axisY)) > obbHalf.Y + aabbHalf.X * MathF.Abs(axisY.X) + aabbHalf.Y * MathF.Abs(axisY.Y))
		{
			return false;
		}
		return true;
	}

	private static bool OrientedBoxIntersectsOrientedBox(Vector2 centerA, Vector2 halfA, Angle rotA, Vector2 centerB, Vector2 halfB, Angle rotB)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Vector2 aX = new Vector2((float)Math.Cos(rotA.Theta), (float)Math.Sin(rotA.Theta));
		Vector2 aY = new Vector2(0f - aX.Y, aX.X);
		Vector2 bX = new Vector2((float)Math.Cos(rotB.Theta), (float)Math.Sin(rotB.Theta));
		Vector2 bY = new Vector2(0f - bX.Y, bX.X);
		Vector2 d = centerB - centerA;
		if (!SeparatedOnAxis(d, aX, halfA, aX, aY, halfB, bX, bY) && !SeparatedOnAxis(d, aY, halfA, aX, aY, halfB, bX, bY) && !SeparatedOnAxis(d, bX, halfA, aX, aY, halfB, bX, bY))
		{
			return !SeparatedOnAxis(d, bY, halfA, aX, aY, halfB, bX, bY);
		}
		return false;
	}

	private static bool SeparatedOnAxis(Vector2 d, Vector2 axis, Vector2 halfA, Vector2 aX, Vector2 aY, Vector2 halfB, Vector2 bX, Vector2 bY)
	{
		float rA = halfA.X * MathF.Abs(Vector2.Dot(aX, axis)) + halfA.Y * MathF.Abs(Vector2.Dot(aY, axis));
		float rB = halfB.X * MathF.Abs(Vector2.Dot(bX, axis)) + halfB.Y * MathF.Abs(Vector2.Dot(bY, axis));
		return MathF.Abs(Vector2.Dot(d, axis)) > rA + rB;
	}

	private static Box2 GetMovementAabb(Box2 aabb, GridVehicleMoverComponent mover)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		float inset = Math.Clamp(mover.MovementCollisionInset, 0f, 0.45f);
		if (inset <= 0f)
		{
			return aabb;
		}
		Box2 adjusted = ((Box2)(ref aabb)).Enlarged(0f - inset);
		if (!((Box2)(ref adjusted)).IsValid())
		{
			return aabb;
		}
		return adjusted;
	}

	private Angle GetCollisionWorldRotation(EntityUid uid, EntityUid grid, Angle? overrideRotation)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (overrideRotation.HasValue)
		{
			Angle localRotation = overrideRotation.GetValueOrDefault();
			TransformComponent xform = ((EntitySystem)this).Transform(uid);
			EntityUid parentUid = xform.ParentUid;
			if (((EntityUid)(ref parentUid)).IsValid())
			{
				return transform.GetWorldRotation(xform.ParentUid) + localRotation;
			}
			return transform.GetWorldRotation(grid) + localRotation;
		}
		return transform.GetWorldRotation(uid);
	}

	private bool TryGetDoorPowered(EntityUid target, out bool powered)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		AirlockComponent airlock = default(AirlockComponent);
		if (((EntitySystem)this).TryComp<AirlockComponent>(target, ref airlock))
		{
			powered = airlock.Powered;
			return true;
		}
		FirelockComponent firelock = default(FirelockComponent);
		if (((EntitySystem)this).TryComp<FirelockComponent>(target, ref firelock))
		{
			powered = firelock.Powered;
			return true;
		}
		if (((EntitySystem)this).HasComp<RMCPowerReceiverComponent>(target))
		{
			powered = _rmcPower.IsPowered(target);
			return true;
		}
		powered = false;
		return false;
	}

	private void ApplyWheelCollisionDamage(EntityUid vehicle, GridVehicleMoverComponent mover, float damage)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && !(damage <= 0f))
		{
			_wheels.DamageWheels(vehicle, damage);
		}
	}

	private float GetWheelCollisionDamage(EntityUid vehicle, GridVehicleMoverComponent mover)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		VehicleWheelSlotsComponent wheels = default(VehicleWheelSlotsComponent);
		if (!((EntitySystem)this).TryComp<VehicleWheelSlotsComponent>(vehicle, ref wheels))
		{
			return 0f;
		}
		float speedMag = MathF.Abs(mover.CurrentSpeed);
		if (speedMag <= 0f)
		{
			return 0f;
		}
		float damage = speedMag * wheels.CollisionDamagePerSpeed;
		if (wheels.MinCollisionDamage > 0f)
		{
			damage = MathF.Max(wheels.MinCollisionDamage, damage);
		}
		return damage;
	}

	private bool ShouldBlockXeno(GridVehicleMoverComponent mover, EntityUid xeno)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		RMCSizes? xenoBlockMinimumSize = mover.XenoBlockMinimumSize;
		if (xenoBlockMinimumSize.HasValue)
		{
			RMCSizes minSize = xenoBlockMinimumSize.GetValueOrDefault();
			if (!_size.TryGetSize(xeno, out var size))
			{
				return true;
			}
			return (int)size >= (int)minSize;
		}
		return true;
	}

	private bool HasBlockingVehicleMob(GridVehicleMoverComponent mover, HashSet<EntityUid> blockers)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid blocker in blockers)
		{
			if (IsBlockingVehicleMob(mover, blocker))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsBlockingVehicleMob(GridVehicleMoverComponent mover, EntityUid blocker)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(blocker))
		{
			return ShouldBlockXeno(mover, blocker);
		}
		return false;
	}

	private static bool TryGetFixtureAabb(FixturesComponent fixtures, Transform transformData, out Box2 aabb)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		bool first = true;
		aabb = default(Box2);
		foreach (Fixture fixture in fixtures.Fixtures.Values)
		{
			if (!fixture.Hard)
			{
				continue;
			}
			for (int i = 0; i < fixture.Shape.ChildCount; i++)
			{
				Box2 child = fixture.Shape.ComputeAABB(transformData, i);
				if (first)
				{
					aabb = child;
					first = false;
				}
				else
				{
					aabb = ((Box2)(ref aabb)).Union(ref child);
				}
			}
		}
		return !first;
	}

	private static bool TryGetFixtureLocalAabb(FixturesComponent fixtures, out Box2 aabb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TryGetFixtureAabb(fixtures, Transform.Empty, out aabb);
	}

	private bool TryPushVehicle(EntityUid pusher, GridVehicleMoverComponent pusherMover, EntityUid grid, Vector2 pusherTargetPosition, EntityUid pushed, bool applyEffects)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		if (!pusherMover.CanPushVehicles)
		{
			return false;
		}
		VehicleComponent pushedVehicle = default(VehicleComponent);
		if (!((EntitySystem)this).TryComp<VehicleComponent>(pushed, ref pushedVehicle) || pushedVehicle.MovementKind != VehicleMovementKind.Grid)
		{
			return false;
		}
		GridVehicleMoverComponent pushedMover = default(GridVehicleMoverComponent);
		if (!((EntitySystem)this).TryComp<GridVehicleMoverComponent>(pushed, ref pushedMover))
		{
			return false;
		}
		MapGridComponent gridComp = default(MapGridComponent);
		if (!gridQ.TryComp(grid, ref gridComp))
		{
			return false;
		}
		TransformComponent pushedXform = ((EntitySystem)this).Transform(pushed);
		EntityUid? gridUid = pushedXform.GridUid;
		EntityUid val = grid;
		if (!gridUid.HasValue || gridUid.GetValueOrDefault() != val)
		{
			return false;
		}
		Vector2 pushDelta = pusherTargetPosition - pusherMover.Position;
		if (pushDelta.LengthSquared() <= 9.999999E-09f)
		{
			return false;
		}
		TrySyncMoverToCurrentGrid(Entity<GridVehicleMoverComponent>.op_Implicit((pushed, pushedMover)), centerOnTile: false, pushedXform);
		gridUid = pushedMover.SyncedGrid;
		val = grid;
		if (!gridUid.HasValue || gridUid.GetValueOrDefault() != val)
		{
			return false;
		}
		HashSet<EntityUid> ignored = new HashSet<EntityUid> { pusher };
		Vector2 pushedTarget = pushedMover.Position + pushDelta;
		if (!CanOccupyTransform(pushed, pushedMover, grid, pushedTarget, null, 0.0075f, applyEffects: false, debug: false, null, ignored))
		{
			return false;
		}
		if (!applyEffects)
		{
			return true;
		}
		if (!CanOccupyTransform(pushed, pushedMover, grid, pushedTarget, null, 0.0075f, applyEffects: true, debug: false, null, ignored))
		{
			return false;
		}
		pushedMover.Position = pushedTarget;
		pushedMover.CurrentSpeed = 0f;
		pushedMover.IsCommittedToMove = false;
		pushedMover.IsPushMove = true;
		pushedMover.PushDirection = GetCardinalDirection(pushDelta);
		pushedMover.IsMoving = true;
		UpdateDerivedTileState(grid, gridComp, pushedMover);
		SetGridPosition(pushed, grid, pushedMover.Position);
		physics.WakeBody(pushed, false, (FixturesComponent)null, (PhysicsComponent)null);
		((EntitySystem)this).Dirty(pushed, (IComponent)(object)pushedMover, (MetaDataComponent)null);
		return true;
	}

	private static Vector2i GetCardinalDirection(Vector2 direction)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (direction.LengthSquared() <= 0f)
		{
			return Vector2i.Zero;
		}
		if (MathF.Abs(direction.X) >= MathF.Abs(direction.Y))
		{
			return new Vector2i(Math.Sign(direction.X), 0);
		}
		return new Vector2i(0, Math.Sign(direction.Y));
	}

	private bool TrySmash(EntityUid target, EntityUid vehicle, ref bool playedCollisionSound)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		VehicleSmashableComponent smashable = default(VehicleSmashableComponent);
		if (!((EntitySystem)this).TryComp<VehicleSmashableComponent>(target, ref smashable))
		{
			return false;
		}
		if (smashable.RequiredVehicleTags.Count > 0 && !_tags.HasAnyTag(vehicle, smashable.RequiredVehicleTags))
		{
			return false;
		}
		PlayCollisionSound(vehicle, ref playedCollisionSound);
		GridVehicleMoverComponent mover = default(GridVehicleMoverComponent);
		if (((EntitySystem)this).TryComp<GridVehicleMoverComponent>(vehicle, ref mover))
		{
			ApplySmashSlowdown(vehicle, mover, smashable);
		}
		if (_net.IsClient)
		{
			return true;
		}
		if (smashable.SmashSound != null)
		{
			_audio.PlayPvs(smashable.SmashSound, ((EntitySystem)this).Transform(target).Coordinates, (AudioParams?)null);
		}
		RMCVehicleSmashAttemptEvent smashEv = new RMCVehicleSmashAttemptEvent(vehicle);
		((EntitySystem)this).RaiseLocalEvent<RMCVehicleSmashAttemptEvent>(target, ref smashEv, false);
		if (smashEv.Handled)
		{
			return true;
		}
		SmashTarget(target, vehicle, smashable);
		return true;
	}

	private void SmashTarget(EntityUid target, EntityUid vehicle, VehicleSmashableComponent smashable)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier
		{
			DamageDict = { [ProtoId<DamageTypePrototype>.op_Implicit(CollisionDamageType)] = smashable.DamageOnHit }
		};
		_damageable.TryChangeDamage(target, damage, ignoreResistances: true, interruptsDoAfters: true, null, vehicle, vehicle);
		if (smashable.DeleteOnHit && !((EntitySystem)this).TerminatingOrDeleted(target, (MetaDataComponent)null))
		{
			_destructible.DestroyEntity(target);
		}
	}

	private void PlayCollisionSound(EntityUid uid, ref bool played)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		VehicleSoundComponent sound = default(VehicleSoundComponent);
		if (!played && ((EntitySystem)this).TryComp<VehicleSoundComponent>(uid, ref sound) && sound.CollisionSound != null && !_net.IsClient)
		{
			TimeSpan now = _timing.CurTime;
			if (!(sound.NextCollisionSound > now))
			{
				_audio.PlayPvs(sound.CollisionSound, uid, (AudioParams?)null);
				sound.NextCollisionSound = now + TimeSpan.FromSeconds(sound.CollisionSoundCooldown);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)sound, (MetaDataComponent)null);
				played = true;
			}
		}
	}

	private void PlayMobCollisionSound(EntityUid uid, ref bool played)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		VehicleSoundComponent sound = default(VehicleSoundComponent);
		if (played || !((EntitySystem)this).TryComp<VehicleSoundComponent>(uid, ref sound))
		{
			return;
		}
		SoundSpecifier mobSound = sound.MobCollisionSound ?? sound.CollisionSound;
		if (mobSound != null && !_net.IsClient)
		{
			TimeSpan now = _timing.CurTime;
			if (!(sound.NextCollisionSound > now))
			{
				_audio.PlayPvs(mobSound, uid, (AudioParams?)null);
				sound.NextCollisionSound = now + TimeSpan.FromSeconds(sound.CollisionSoundCooldown);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)sound, (MetaDataComponent)null);
				played = true;
			}
		}
	}

	private void HandleMobCollision(EntityUid vehicle, EntityUid target, MobStateComponent mobState, ref bool playedCollisionSound)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || _mobState.IsDead(target, mobState))
		{
			return;
		}
		TimeSpan now = _timing.CurTime;
		if (_lastMobCollision.TryGetValue(target, out var last) && now < last + MobCollisionCooldown)
		{
			return;
		}
		_lastMobCollision[target] = now;
		PlayMobCollisionSound(vehicle, ref playedCollisionSound);
		VehicleComponent vehicleComp = default(VehicleComponent);
		double runoverDamage = (((EntitySystem)this).TryComp<VehicleComponent>(vehicle, ref vehicleComp) ? ((double)vehicleComp.RunoverDamage) : 8.0);
		DamageSpecifier damage = new DamageSpecifier
		{
			DamageDict = { [ProtoId<DamageTypePrototype>.op_Implicit(CollisionDamageType)] = runoverDamage }
		};
		_damageable.TryChangeDamage(target, damage);
		if (!((EntitySystem)this).HasComp<XenoComponent>(target))
		{
			_stun.TryKnockdown(target, MobCollisionKnockdown, refresh: true);
			VehicleRunoverComponent runover = ((EntitySystem)this).EnsureComp<VehicleRunoverComponent>(target);
			runover.Vehicle = vehicle;
			runover.Duration = MobCollisionKnockdown;
			runover.ExpiresAt = now + runover.Duration + VehicleRunoverSystem.StandUpGrace;
			((EntitySystem)this).Dirty(target, (IComponent)(object)runover, (MetaDataComponent)null);
			PhysicsComponent targetBody = default(PhysicsComponent);
			if (physicsQ.TryComp(target, ref targetBody))
			{
				physics.SetLinearVelocity(target, Vector2.Zero, true, true, (FixturesComponent)null, targetBody);
				physics.SetAngularVelocity(target, 0f, true, (FixturesComponent)null, targetBody);
			}
		}
	}

	private Vector2 GetVehicleMoveDelta(EntityUid grid, Vector2 worldPos, MapId mapId, GridVehicleMoverComponent mover)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates currentCoords = default(EntityCoordinates);
		((EntityCoordinates)(ref currentCoords))._002Ector(grid, mover.Position);
		MapCoordinates currentWorld = ((EntityCoordinates)(ref currentCoords)).ToMap((IEntityManager)(object)base.EntityManager, transform);
		if (currentWorld.MapId != mapId)
		{
			return Vector2.Zero;
		}
		return worldPos - currentWorld.Position;
	}

	private bool PushMobOutOfVehicle(EntityUid vehicle, EntityUid mob, Box2 vehicleAabb, Box2 mobAabb, Vector2 vehicleMove)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Transform(mob).Anchored)
		{
			return false;
		}
		Box2 centeredAabb = GetCenteredMobAabb(mob, mobAabb);
		if (!TryGetMobPush(vehicle, mob, vehicleAabb, centeredAabb, vehicleMove, out var target))
		{
			return false;
		}
		if (!_net.IsClient || ShouldPredictVehicleInteractions(vehicle))
		{
			ApplyMobPush(mob, target);
		}
		return true;
	}

	private Box2 GetCenteredMobAabb(EntityUid mob, Box2 mobAabb)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector2 mobPos = transform.GetWorldPosition(mob);
		if ((((Box2)(ref mobAabb)).Center - mobPos).LengthSquared() <= 0.0001f)
		{
			return mobAabb;
		}
		return Box2.CenteredAround(mobPos, ((Box2)(ref mobAabb)).Size);
	}

	private void ApplyMobPush(EntityUid mob, EntityCoordinates target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (target == EntityCoordinates.Invalid)
		{
			return;
		}
		MapCoordinates mapCoordinates = transform.GetMapCoordinates(mob, (TransformComponent)null);
		MapCoordinates targetMap = transform.ToMapCoordinates(target, true);
		if (!(mapCoordinates.MapId != targetMap.MapId))
		{
			PhysicsComponent mobBody = default(PhysicsComponent);
			if (physicsQ.TryComp(mob, ref mobBody))
			{
				physics.SetLinearVelocity(mob, Vector2.Zero, true, true, (FixturesComponent)null, mobBody);
				physics.SetAngularVelocity(mob, 0f, true, (FixturesComponent)null, mobBody);
			}
			TransformComponent mobXform = ((EntitySystem)this).Transform(mob);
			transform.SetCoordinates(mob, mobXform, target, (Angle?)null, true, (TransformComponent)null, (TransformComponent)null);
		}
	}

	private bool ShouldPredictVehicleInteractions(EntityUid vehicle)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient || !_timing.InPrediction)
		{
			return false;
		}
		PhysicsComponent vehicleBody = default(PhysicsComponent);
		if (!physicsQ.TryComp(vehicle, ref vehicleBody) || !vehicleBody.Predict)
		{
			return false;
		}
		VehicleComponent vehicleComp = default(VehicleComponent);
		if (!((EntitySystem)this).TryComp<VehicleComponent>(vehicle, ref vehicleComp))
		{
			return false;
		}
		if (vehicleComp.Operator.HasValue)
		{
			EntityUid? val = vehicleComp.Operator;
			EntityUid? localEntity = _player.LocalEntity;
			if (val.HasValue != localEntity.HasValue)
			{
				return false;
			}
			if (!val.HasValue)
			{
				return true;
			}
			return val.GetValueOrDefault() == localEntity.GetValueOrDefault();
		}
		return false;
	}

	private void PredictRunover(EntityUid vehicle, EntityUid mob, MobStateComponent mobState)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (ShouldPredictVehicleInteractions(vehicle) && !_mobState.IsDead(mob, mobState) && !_standing.IsDown(mob))
		{
			_stun.TryKnockdown(mob, MobCollisionKnockdown, refresh: true);
			VehicleRunoverComponent runover = ((EntitySystem)this).EnsureComp<VehicleRunoverComponent>(mob);
			runover.Vehicle = vehicle;
			runover.Duration = MobCollisionKnockdown;
			runover.ExpiresAt = _timing.CurTime + runover.Duration + VehicleRunoverSystem.StandUpGrace;
			((EntitySystem)this).Dirty(mob, (IComponent)(object)runover, (MetaDataComponent)null);
			PhysicsComponent mobBody = default(PhysicsComponent);
			if (physicsQ.TryComp(mob, ref mobBody))
			{
				physics.SetLinearVelocity(mob, Vector2.Zero, true, true, (FixturesComponent)null, mobBody);
				physics.SetAngularVelocity(mob, 0f, true, (FixturesComponent)null, mobBody);
			}
		}
	}

	private bool TryGetMobPush(EntityUid vehicle, EntityUid mob, Box2 vehicleAabb, Box2 mobAabb, Vector2 vehicleMove, out EntityCoordinates target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		target = EntityCoordinates.Invalid;
		Vector2 vector = ((Box2)(ref vehicleAabb)).Size / 2f;
		Vector2 mobHalf = ((Box2)(ref mobAabb)).Size / 2f;
		Vector2 vehicleCenter = ((Box2)(ref vehicleAabb)).Center;
		Vector2 diff = ((Box2)(ref mobAabb)).Center - vehicleCenter;
		float overlapX = vector.X + mobHalf.X - Math.Abs(diff.X);
		float overlapY = vector.Y + mobHalf.Y - Math.Abs(diff.Y);
		if (overlapX <= 0f || overlapY <= 0f)
		{
			return false;
		}
		if (overlapX <= 0.05f && overlapY <= 0.05f)
		{
			return false;
		}
		Vector2 pushX = ((overlapX > 0f) ? new Vector2((float)Math.Sign((diff.X == 0f) ? 1f : diff.X) * overlapX, 0f) : Vector2.Zero);
		Vector2 pushY = ((overlapY > 0f) ? new Vector2(0f, (float)Math.Sign((diff.Y == 0f) ? 1f : diff.Y) * overlapY) : Vector2.Zero);
		Box2 vehicleBounds = vehicleAabb;
		if (TryGetMovementSidePushTarget(vehicle, mob, mobAabb, vehicleBounds, vehicleMove, pushX, pushY, out target))
		{
			return true;
		}
		if (vehicleMove.LengthSquared() > 0.0001f)
		{
			return false;
		}
		bool useX = overlapX < overlapY;
		if (MathF.Abs(overlapX - overlapY) <= 0.05f && _lastMobPushAxis.TryGetValue(mob, out var lastUseX))
		{
			useX = lastUseX;
		}
		Vector2 first = (useX ? pushX : pushY);
		Vector2 second = (useX ? pushY : pushX);
		if (TryGetSidePushTarget(vehicle, mob, mobAabb, vehicleBounds, first, out target))
		{
			_lastMobPushAxis[mob] = useX;
			return true;
		}
		if (TryGetSidePushTarget(vehicle, mob, mobAabb, vehicleBounds, second, out target))
		{
			_lastMobPushAxis[mob] = !useX;
			return true;
		}
		return false;
	}

	private bool TryGetMovementSidePushTarget(EntityUid vehicle, EntityUid mob, Box2 mobAabb, Box2 vehicleBounds, Vector2 vehicleMove, Vector2 pushX, Vector2 pushY, out EntityCoordinates target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		target = EntityCoordinates.Invalid;
		if (vehicleMove.LengthSquared() <= 0.0001f)
		{
			return false;
		}
		bool vehicleMovesX = MathF.Abs(vehicleMove.X) >= MathF.Abs(vehicleMove.Y);
		Vector2 sidePush = (vehicleMovesX ? pushY : pushX);
		if (sidePush == Vector2.Zero)
		{
			return false;
		}
		bool useX = !vehicleMovesX;
		if (TryGetSidePushTarget(vehicle, mob, mobAabb, vehicleBounds, sidePush, out target))
		{
			_lastMobPushAxis[mob] = useX;
			return true;
		}
		if (TryGetSidePushTarget(vehicle, mob, mobAabb, vehicleBounds, -sidePush, out target))
		{
			_lastMobPushAxis[mob] = useX;
			return true;
		}
		return false;
	}

	private bool TryGetSidePushTarget(EntityUid vehicle, EntityUid mob, Box2 mobAabb, Box2 vehicleBounds, Vector2 push, out EntityCoordinates target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		target = EntityCoordinates.Invalid;
		if (push == Vector2.Zero)
		{
			return false;
		}
		Vector2 adjusted = push;
		if (Math.Abs(adjusted.X) > 0f)
		{
			adjusted.X += (float)Math.Sign(adjusted.X) * 0.0075f;
		}
		if (Math.Abs(adjusted.Y) > 0f)
		{
			adjusted.Y += (float)Math.Sign(adjusted.Y) * 0.0075f;
		}
		Box2 targetAabb = ((Box2)(ref mobAabb)).Translated(adjusted);
		if (((Box2)(ref targetAabb)).Intersects(ref vehicleBounds))
		{
			return false;
		}
		if (IsPushBlocked(vehicle, mob, mobAabb, adjusted))
		{
			return false;
		}
		MapCoordinates mobMap = transform.GetMapCoordinates(mob, (TransformComponent)null);
		MapCoordinates mapCoords = default(MapCoordinates);
		((MapCoordinates)(ref mapCoords))._002Ector(mobMap.Position + adjusted, mobMap.MapId);
		EntityUid? gridUid = ((EntitySystem)this).Transform(mob).GridUid;
		if (gridUid.HasValue)
		{
			EntityUid grid = gridUid.GetValueOrDefault();
			MapGridComponent gridComp = default(MapGridComponent);
			if (gridQ.TryComp(grid, ref gridComp))
			{
				EntityCoordinates coords = transform.ToCoordinates(Entity<TransformComponent>.op_Implicit(grid), mapCoords);
				Vector2i indices = map.TileIndicesFor(grid, gridComp, coords);
				if (IsPushTileBlocked(grid, gridComp, indices, vehicle, mob, out var _))
				{
					return false;
				}
				target = transform.ToCoordinates(Entity<TransformComponent>.op_Implicit(grid), mapCoords);
				goto IL_0164;
			}
		}
		target = transform.ToCoordinates(mapCoords);
		goto IL_0164;
		IL_0164:
		if (target == EntityCoordinates.Invalid)
		{
			return false;
		}
		return true;
	}

	private bool IsPushTileBlocked(EntityUid gridUid, MapGridComponent gridComp, Vector2i indices, EntityUid vehicle, EntityUid mob, out EntityUid blocker)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Invalid comparison between Unknown and I4
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		blocker = EntityUid.Invalid;
		TransformComponent gridXform = ((EntitySystem)this).Transform(gridUid);
		EntityQuery<TransformComponent> xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		ValueTuple<Vector2, Angle, Matrix3x2> worldPositionRotationMatrix = transform.GetWorldPositionRotationMatrix(gridXform, xformQuery);
		Vector2 gridPos = worldPositionRotationMatrix.Item1;
		Angle gridRot = worldPositionRotationMatrix.Item2;
		Matrix3x2 matrix = worldPositionRotationMatrix.Item3;
		ushort size = gridComp.TileSize;
		Vector2 localPos = new Vector2((float)(indices.X * size) + (float)(int)size / 2f, (float)(indices.Y * size) + (float)(int)size / 2f);
		Vector2 worldPos = Vector2.Transform(localPos, matrix);
		Box2 tileAabb = ((Box2)(ref Box2.UnitCentered)).Scale(0.95f * (float)(int)size);
		Box2Rotated worldBox = default(Box2Rotated);
		((Box2Rotated)(ref worldBox))._002Ector(((Box2)(ref tileAabb)).Translated(worldPos), gridRot, worldPos);
		tileAabb = ((Box2)(ref tileAabb)).Translated(localPos);
		float minIntersectionArea = ((Box2)(ref tileAabb)).Width * ((Box2)(ref tileAabb)).Height * 0.005f;
		DoorComponent doorComponent = default(DoorComponent);
		PhysicsComponent otherBody = default(PhysicsComponent);
		FixturesComponent fixtures = default(FixturesComponent);
		PhysicsComponent mobBody = default(PhysicsComponent);
		FixturesComponent mobFixtures = default(FixturesComponent);
		Transform entXform = default(Transform);
		foreach (EntityUid ent in lookup.GetEntitiesIntersecting(gridUid, worldBox, (LookupFlags)6))
		{
			if (ent == vehicle || ent == mob || IsDescendantOf(ent, vehicle) || IsDescendantOf(ent, mob))
			{
				continue;
			}
			TransformComponent entXformComp = ((EntitySystem)this).Transform(ent);
			if (((EntitySystem)this).HasComp<MobStateComponent>(ent) || ((EntitySystem)this).HasComp<VehicleSmashableComponent>(ent) || ((EntitySystem)this).HasComp<FoldableComponent>(ent) || ((EntitySystem)this).TryComp<DoorComponent>(ent, ref doorComponent) || ((EntitySystem)this).HasComp<BarricadeComponent>(ent) || (((EntitySystem)this).HasComp<ItemComponent>(ent) && !entXformComp.Anchored) || !physicsQ.TryComp(ent, ref otherBody))
			{
				continue;
			}
			bool isVehicle = ((EntitySystem)this).HasComp<VehicleComponent>(ent);
			if ((!entXformComp.Anchored && (int)otherBody.BodyType != 4 && !isVehicle) || !fixtureQ.TryComp(ent, ref fixtures) || (physicsQ.TryComp(mob, ref mobBody) && fixtureQ.TryComp(mob, ref mobFixtures) && !physics.IsHardCollidable(Entity<FixturesComponent, PhysicsComponent>.op_Implicit((mob, mobFixtures, mobBody)), Entity<FixturesComponent, PhysicsComponent>.op_Implicit((ent, fixtures, otherBody)))))
			{
				continue;
			}
			ValueTuple<Vector2, Angle> worldPositionRotation = transform.GetWorldPositionRotation(entXformComp, xformQuery);
			Vector2 pos = worldPositionRotation.Item1;
			Angle rot = worldPositionRotation.Item2;
			rot -= gridRot;
			Angle val = -gridRot;
			Vector2 vector = pos - gridPos;
			pos = ((Angle)(ref val)).RotateVec(ref vector);
			((Transform)(ref entXform))._002Ector(pos, (float)rot.Theta);
			foreach (Fixture fixture in fixtures.Fixtures.Values)
			{
				if (!fixture.Hard || (fixture.CollisionLayer & 0x1000001E) == 0)
				{
					continue;
				}
				for (int i = 0; i < fixture.Shape.ChildCount; i++)
				{
					Box2 val2 = fixture.Shape.ComputeAABB(entXform, i);
					Box2 intersection = ((Box2)(ref val2)).Intersect(ref tileAabb);
					if (((Box2)(ref intersection)).Width * ((Box2)(ref intersection)).Height > minIntersectionArea)
					{
						blocker = ent;
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool IsDescendantOf(EntityUid ent, EntityUid root)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (ent == root)
		{
			return true;
		}
		EntityUid current = ent;
		while (((EntityUid)(ref current)).IsValid())
		{
			TransformComponent xform = ((EntitySystem)this).Transform(current);
			EntityUid parent = xform.ParentUid;
			if (!((EntityUid)(ref parent)).IsValid())
			{
				return false;
			}
			if (parent == root)
			{
				return true;
			}
			EntityUid val = parent;
			EntityUid? gridUid = xform.GridUid;
			if (!gridUid.HasValue || !(val == gridUid.GetValueOrDefault()))
			{
				val = parent;
				gridUid = xform.MapUid;
				if (!gridUid.HasValue || !(val == gridUid.GetValueOrDefault()))
				{
					current = parent;
					continue;
				}
			}
			return false;
		}
		return false;
	}

	private bool IsPushBlocked(EntityUid vehicle, EntityUid mob, Box2 mobAabb, Vector2 push)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		if (push == Vector2.Zero)
		{
			return false;
		}
		MapId mapId = ((EntitySystem)this).Transform(mob).MapID;
		if (mapId == MapId.Nullspace)
		{
			return false;
		}
		PhysicsComponent mobBody = default(PhysicsComponent);
		FixturesComponent mobFixtures = default(FixturesComponent);
		if (!physicsQ.TryComp(mob, ref mobBody) || !fixtureQ.TryComp(mob, ref mobFixtures))
		{
			return false;
		}
		Box2 targetAabb = ((Box2)(ref mobAabb)).Translated(push);
		Box2 checkAabb = ((Box2)(ref targetAabb)).Enlarged(-0.1f);
		if (!((Box2)(ref checkAabb)).IsValid())
		{
			checkAabb = targetAabb;
		}
		PhysicsComponent otherBody = default(PhysicsComponent);
		FixturesComponent otherFixtures = default(FixturesComponent);
		DoorComponent doorComponent = default(DoorComponent);
		foreach (EntityUid other in lookup.GetEntitiesIntersecting(mapId, checkAabb, (LookupFlags)6))
		{
			if (other == mob || other == vehicle || IsDescendantOf(other, vehicle) || IsDescendantOf(other, mob) || !physicsQ.TryComp(other, ref otherBody) || !otherBody.CanCollide)
			{
				continue;
			}
			TransformComponent otherXform = ((EntitySystem)this).Transform(other);
			if (!fixtureQ.TryComp(other, ref otherFixtures) || ((EntitySystem)this).HasComp<MobStateComponent>(other) || ((EntitySystem)this).HasComp<VehicleSmashableComponent>(other) || ((EntitySystem)this).HasComp<FoldableComponent>(other) || ((EntitySystem)this).TryComp<DoorComponent>(other, ref doorComponent) || ((EntitySystem)this).HasComp<BarricadeComponent>(other))
			{
				continue;
			}
			bool wallLike = false;
			bool overlaps = false;
			Transform otherTx = physics.GetPhysicsTransform(other, otherXform);
			foreach (Fixture fixture in otherFixtures.Fixtures.Values)
			{
				if (!fixture.Hard || (fixture.CollisionLayer & 2) == 0)
				{
					continue;
				}
				wallLike = true;
				for (int i = 0; i < fixture.Shape.ChildCount; i++)
				{
					Box2 otherAabb = fixture.Shape.ComputeAABB(otherTx, i);
					Box2 intersection = ((Box2)(ref otherAabb)).Intersect(ref checkAabb);
					if (Box2.Area(ref intersection) > 0.01f)
					{
						overlaps = true;
						break;
					}
				}
				if (overlaps)
				{
					break;
				}
			}
			if (wallLike && overlaps && physics.IsHardCollidable(Entity<FixturesComponent, PhysicsComponent>.op_Implicit((mob, mobFixtures, mobBody)), Entity<FixturesComponent, PhysicsComponent>.op_Implicit((other, otherFixtures, otherBody))))
			{
				return true;
			}
		}
		return false;
	}

	private VehicleCollisionClass ClassifyCollisionCandidate(EntityUid other, TransformComponent otherXform, PhysicsComponent otherBody, FixturesComponent otherFixtures, bool hardCollidable, bool isMob, bool isBarricade, bool isFoldable, bool hasDoor, bool isXeno, bool isVehicle, bool isSmashable)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		if (!otherXform.Anchored && ((EntitySystem)this).HasComp<ItemComponent>(other))
		{
			return VehicleCollisionClass.Ignore;
		}
		if (isMob || isXeno)
		{
			return VehicleCollisionClass.SoftMob;
		}
		if (!isSmashable && !hardCollidable && IsNormallyMobPassable(otherFixtures))
		{
			return VehicleCollisionClass.Ignore;
		}
		if (!otherXform.Anchored && (int)otherBody.BodyType != 4 && !isMob && !isBarricade && !isFoldable && !isVehicle && !isSmashable)
		{
			return VehicleCollisionClass.Ignore;
		}
		if (isSmashable)
		{
			return VehicleCollisionClass.Breakable;
		}
		if (isBarricade && (hasDoor || isFoldable))
		{
			return VehicleCollisionClass.Breakable;
		}
		if (isFoldable && !hardCollidable)
		{
			return VehicleCollisionClass.Ignore;
		}
		if (!hardCollidable)
		{
			return VehicleCollisionClass.Ignore;
		}
		return VehicleCollisionClass.Hard;
	}

	private static bool IsNormallyMobPassable(FixturesComponent fixtures)
	{
		foreach (Fixture value in fixtures.Fixtures.Values)
		{
			if (!IsNormallyMobPassable(value))
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsNormallyMobPassable(Fixture fixture)
	{
		if (fixture.Hard)
		{
			if ((fixture.CollisionMask & 0x41) == 0)
			{
				return (fixture.CollisionLayer & 0x1E) == 0;
			}
			return false;
		}
		return true;
	}

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		gridQ = ((EntitySystem)this).GetEntityQuery<MapGridComponent>();
		physicsQ = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		fixtureQ = ((EntitySystem)this).GetEntityQuery<FixturesComponent>();
		((EntitySystem)this).SubscribeLocalEvent<GridVehicleMoverComponent, ComponentStartup>((EntityEventRefHandler<GridVehicleMoverComponent, ComponentStartup>)OnMoverStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GridVehicleMoverComponent, ComponentShutdown>((EntityEventRefHandler<GridVehicleMoverComponent, ComponentShutdown>)OnMoverShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GridVehicleMoverComponent, MoveEvent>((EntityEventRefHandler<GridVehicleMoverComponent, MoveEvent>)OnMoverMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GridVehicleMoverComponent, ReAnchorEvent>((EntityEventRefHandler<GridVehicleMoverComponent, ReAnchorEvent>)OnMoverReAnchor, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GridVehicleMoverComponent, VehicleCanRunEvent>((EntityEventRefHandler<GridVehicleMoverComponent, VehicleCanRunEvent>)OnMoverCanRun, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GridVehicleMoverComponent, PreventCollideEvent>((EntityEventRefHandler<GridVehicleMoverComponent, PreventCollideEvent>)OnMoverPreventCollide, (Type[])null, (Type[])null);
	}

	private void OnMoverStartup(Entity<GridVehicleMoverComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		TrySyncMoverToCurrentGrid(ent, centerOnTile: true, null, force: true);
		VehicleComponent vehicle = default(VehicleComponent);
		bool flag = ((EntitySystem)this).TryComp<VehicleComponent>(ent.Owner, ref vehicle);
		if (flag)
		{
			VehicleMovementKind movementKind = vehicle.MovementKind;
			bool flag2 = movementKind - 2 <= VehicleMovementKind.Grid;
			flag = flag2 || ForceAllFreeMovement;
		}
		if (flag)
		{
			ent.Comp.Heading = ((EntitySystem)this).Transform(ent.Owner).LocalRotation;
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		}
	}

	private void OnMoverShutdown(Entity<GridVehicleMoverComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		_hardState.Remove(ent.Owner);
		_movementAccumulator.Remove(ent.Owner);
		_activeXenoPushers.Remove(ent.Owner);
	}

	private void OnMoverMove(Entity<GridVehicleMoverComponent> ent, ref MoveEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (((MoveEvent)(ref args)).ParentChanged)
		{
			TrySyncMoverToCurrentGrid(ent, centerOnTile: false);
		}
	}

	private void OnMoverReAnchor(Entity<GridVehicleMoverComponent> ent, ref ReAnchorEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TrySyncMoverToCurrentGrid(ent, centerOnTile: false);
	}

	private bool TrySyncMoverToCurrentGrid(Entity<GridVehicleMoverComponent> ent, bool centerOnTile, TransformComponent? xform = null, bool force = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = ent.Owner;
		if (xform == null)
		{
			xform = ((EntitySystem)this).Transform(uid);
		}
		EntityUid? gridUid = xform.GridUid;
		if (gridUid.HasValue)
		{
			EntityUid grid = gridUid.GetValueOrDefault();
			MapGridComponent gridComp = default(MapGridComponent);
			if (gridQ.TryComp(grid, ref gridComp))
			{
				if (!force)
				{
					gridUid = ent.Comp.SyncedGrid;
					EntityUid val = grid;
					if (gridUid.HasValue && gridUid.GetValueOrDefault() == val)
					{
						return false;
					}
				}
				EntityCoordinates coordinates = xform.Coordinates;
				EntityCoordinates coords = ((EntityCoordinates)(ref coordinates)).WithEntityId(grid, transform, (IEntityManager)(object)base.EntityManager);
				Vector2i tile = map.TileIndicesFor(grid, gridComp, coords);
				ent.Comp.SyncedGrid = grid;
				Vector2 seed = (centerOnTile ? new Vector2((float)tile.X + 0.5f, (float)tile.Y + 0.5f) : coords.Position);
				if (!CanOccupyTransform(uid, ent.Comp, grid, seed, null, 0.0075f, applyEffects: false, debug: false) && TryFindFreePlacement(uid, ent.Comp, grid, gridComp, seed, out var freeSeed))
				{
					seed = freeSeed;
				}
				ent.Comp.Position = seed;
				ent.Comp.CurrentTile = map.TileIndicesFor(grid, gridComp, new EntityCoordinates(grid, seed));
				ent.Comp.TargetTile = ent.Comp.CurrentTile;
				ent.Comp.TargetPosition = ent.Comp.Position;
				ent.Comp.CurrentSpeed = 0f;
				ent.Comp.PushDirection = Vector2i.Zero;
				ent.Comp.NextPushTime = TimeSpan.Zero;
				ent.Comp.NextTurnTime = TimeSpan.Zero;
				ent.Comp.InPlaceTurnBlockUntil = TimeSpan.Zero;
				ent.Comp.IsCommittedToMove = false;
				ent.Comp.IsPushMove = false;
				ent.Comp.IsMoving = false;
				_hardState[uid] = true;
				_movementAccumulator[uid] = 0f;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
				return true;
			}
		}
		if (!ent.Comp.SyncedGrid.HasValue)
		{
			return false;
		}
		ent.Comp.SyncedGrid = null;
		ent.Comp.CurrentSpeed = 0f;
		ent.Comp.PushDirection = Vector2i.Zero;
		ent.Comp.IsCommittedToMove = false;
		ent.Comp.IsPushMove = false;
		ent.Comp.IsMoving = false;
		_hardState[uid] = true;
		_movementAccumulator[uid] = 0f;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		return true;
	}

	private bool TryFindFreePlacement(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2 seed, out Vector2 freePosition)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		freePosition = seed;
		int rings = (int)MathF.Ceiling(12f);
		for (int ring = 1; ring <= rings; ring++)
		{
			float radius = (float)ring * 0.25f;
			for (int deg = 0; deg < 360; deg += 30)
			{
				float rad = (float)deg * ((float)Math.PI / 180f);
				Vector2 candidate = seed + new Vector2(MathF.Cos(rad), MathF.Sin(rad)) * radius;
				if (CanOccupyTransform(uid, mover, grid, candidate, null, 0.0075f, applyEffects: false, debug: false))
				{
					freePosition = candidate;
					return true;
				}
			}
		}
		return false;
	}

	private void OnMoverCanRun(Entity<GridVehicleMoverComponent> ent, ref VehicleCanRunEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		VehicleComponent vehicle = default(VehicleComponent);
		if (!args.CanRun || !((EntitySystem)this).TryComp<VehicleComponent>(ent.Owner, ref vehicle))
		{
			return;
		}
		EntityUid? val = vehicle.Operator;
		if (val.HasValue)
		{
			EntityUid operatorUid = val.GetValueOrDefault();
			if (((EntitySystem)this).HasComp<XenoComponent>(operatorUid))
			{
				args.CanRun = false;
			}
		}
	}

	private void OnMoverPreventCollide(Entity<GridVehicleMoverComponent> ent, ref PreventCollideEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Invalid comparison between Unknown and I4
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		VehicleComponent vehicle = default(VehicleComponent);
		bool flag = !((EntitySystem)this).TryComp<VehicleComponent>(ent.Owner, ref vehicle);
		if (!flag)
		{
			VehicleMovementKind movementKind = vehicle.MovementKind;
			bool flag2 = movementKind - 1 <= VehicleMovementKind.Free;
			flag = !flag2;
		}
		if (flag || args.OtherEntity == ent.Owner)
		{
			return;
		}
		if (!((EntitySystem)this).Transform(args.OtherEntity).Anchored && ((EntitySystem)this).HasComp<ItemComponent>(args.OtherEntity))
		{
			args.Cancelled = true;
		}
		else if ((int)args.OtherBody.BodyType == 4)
		{
			if (IsNormallyMobPassable(args.OtherFixture))
			{
				args.Cancelled = true;
			}
			else if ((args.OtherFixture.CollisionLayer & 0x1400001E) != 0)
			{
				args.Cancelled = true;
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		UpdateTrackRestores();
		if (CollisionDebugEnabled)
		{
			DebugTestedTiles.Clear();
			DebugCollisionProbes.Clear();
			DebugCollisions.Clear();
		}
		if (MovementDebugEnabled)
		{
			DebugMovementDecisions.Clear();
		}
		EntityQueryEnumerator<GridVehicleMoverComponent, VehicleComponent, TransformComponent> q = ((EntitySystem)this).EntityQueryEnumerator<GridVehicleMoverComponent, VehicleComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		GridVehicleMoverComponent mover = default(GridVehicleMoverComponent);
		VehicleComponent vehicle = default(VehicleComponent);
		TransformComponent xform = default(TransformComponent);
		MapGridComponent gridComp = default(MapGridComponent);
		MapGridComponent currentGridComp = default(MapGridComponent);
		while (q.MoveNext(ref uid, ref mover, ref vehicle, ref xform))
		{
			VehicleMovementKind movementKind = vehicle.MovementKind;
			if (movementKind - 1 > VehicleMovementKind.Free)
			{
				continue;
			}
			bool isAircraft = vehicle.MovementKind == VehicleMovementKind.Aircraft;
			bool isFree = !isAircraft && (vehicle.MovementKind == VehicleMovementKind.Free || ForceAllFreeMovement);
			TrySyncMoverToCurrentGrid(Entity<GridVehicleMoverComponent>.op_Implicit((uid, mover)), centerOnTile: false, xform);
			EntityUid? gridUid = xform.GridUid;
			if (!gridUid.HasValue)
			{
				continue;
			}
			EntityUid grid = gridUid.GetValueOrDefault();
			if (!gridQ.TryComp(grid, ref gridComp))
			{
				continue;
			}
			if (_net.IsClient && !ShouldPredictVehicleMovement(vehicle))
			{
				SmoothReplicatedVehicle(uid, grid, mover, frameTime, isFree || isAircraft);
				continue;
			}
			Vector2i inputDir = Vector2i.Zero;
			bool pushing = false;
			if (!isFree && !isAircraft)
			{
				inputDir = GetMoverInput(uid, mover, vehicle, out pushing);
			}
			float accumulator = _movementAccumulator.GetValueOrDefault(uid) + frameTime;
			float maxAccum = 71f / (226f * (float)Math.PI);
			if (accumulator > maxAccum)
			{
				accumulator = maxAccum;
			}
			int steps = 0;
			while (accumulator >= 1f / 60f && steps < 6)
			{
				TransformComponent currentXform = ((EntitySystem)this).Transform(uid);
				TrySyncMoverToCurrentGrid(Entity<GridVehicleMoverComponent>.op_Implicit((uid, mover)), centerOnTile: false, currentXform);
				gridUid = currentXform.GridUid;
				if (!gridUid.HasValue)
				{
					break;
				}
				EntityUid currentGrid = gridUid.GetValueOrDefault();
				if (!gridQ.TryComp(currentGrid, ref currentGridComp))
				{
					break;
				}
				if (isAircraft)
				{
					UpdateAircraftMovement(uid, mover, vehicle, currentGrid, currentGridComp, 1f / 60f);
				}
				else if (isFree)
				{
					UpdateFreeMovement(uid, mover, vehicle, currentGrid, currentGridComp, 1f / 60f);
				}
				else
				{
					UpdateMovement(uid, mover, vehicle, currentGrid, currentGridComp, inputDir, pushing, 1f / 60f);
				}
				accumulator -= 1f / 60f;
				steps++;
			}
			_movementAccumulator[uid] = accumulator;
		}
	}

	private bool ShouldPredictVehicleMovement(VehicleComponent vehicle)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			return true;
		}
		if (!_timing.InPrediction)
		{
			return false;
		}
		if (vehicle.Operator.HasValue)
		{
			EntityUid? val = vehicle.Operator;
			EntityUid? localEntity = _player.LocalEntity;
			if (val.HasValue != localEntity.HasValue)
			{
				return false;
			}
			if (!val.HasValue)
			{
				return true;
			}
			return val.GetValueOrDefault() == localEntity.GetValueOrDefault();
		}
		return false;
	}

	private void SmoothReplicatedVehicle(EntityUid uid, EntityUid grid, GridVehicleMoverComponent mover, float frameTime, bool smoothAngle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		EntityUid parentUid = xform.ParentUid;
		if (!((EntityUid)(ref parentUid)).IsValid())
		{
			return;
		}
		EntityCoordinates coords = default(EntityCoordinates);
		((EntityCoordinates)(ref coords))._002Ector(grid, mover.Position);
		Vector2 target = ((EntityCoordinates)(ref coords)).WithEntityId(xform.ParentUid, transform, (IEntityManager)(object)base.EntityManager).Position;
		Vector2 current = xform.LocalPosition;
		Vector2 delta = target - current;
		float alpha = 1f - MathF.Exp(-22f * frameTime);
		if (delta.LengthSquared() >= 1.5625f)
		{
			transform.SetLocalPosition(uid, target, xform);
		}
		else
		{
			transform.SetLocalPosition(uid, Vector2.Lerp(current, target, alpha), xform);
		}
		if (smoothAngle)
		{
			Angle currentRot = xform.LocalRotation;
			Angle val = mover.Heading - currentRot;
			if (MathF.Abs((float)((Angle)(ref val)).Reduced().Theta) >= 1f)
			{
				transform.SetLocalRotation(uid, mover.Heading, xform);
			}
			else
			{
				transform.SetLocalRotation(uid, Angle.Lerp(ref currentRot, ref mover.Heading, alpha), xform);
			}
		}
	}

	private void UpdateFreeMovement(EntityUid uid, GridVehicleMoverComponent mover, VehicleComponent vehicle, EntityUid grid, MapGridComponent gridComp, float frameTime)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		if (vehicle.Operator.HasValue)
		{
			VehicleCanRunEvent canRunEvent = new VehicleCanRunEvent(Entity<VehicleComponent>.op_Implicit((uid, vehicle)));
			((EntitySystem)this).RaiseLocalEvent<VehicleCanRunEvent>(uid, ref canRunEvent, false);
			if (!canRunEvent.CanRun)
			{
				StopMover(mover);
				SetGridPosition(uid, grid, mover.Position);
				transform.SetLocalRotation(uid, mover.Heading, (TransformComponent)null);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)mover, (MetaDataComponent)null);
				return;
			}
		}
		GetSmashSlowdownMultiplier(mover);
		TryDepenetrateFree(uid, mover, grid, gridComp);
		int throttle = 0;
		int steer = 0;
		EntityUid? val = vehicle.Operator;
		if (val.HasValue)
		{
			EntityUid op = val.GetValueOrDefault();
			InputMoverComponent input = default(InputMoverComponent);
			if (((EntitySystem)this).TryComp<InputMoverComponent>(op, ref input))
			{
				MoveButtons heldMoveButtons = input.HeldMoveButtons;
				if ((heldMoveButtons & MoveButtons.Up) != MoveButtons.None)
				{
					throttle++;
				}
				if ((heldMoveButtons & MoveButtons.Down) != MoveButtons.None)
				{
					throttle--;
				}
				if ((heldMoveButtons & MoveButtons.Right) != MoveButtons.None)
				{
					steer++;
				}
				if ((heldMoveButtons & MoveButtons.Left) != MoveButtons.None)
				{
					steer--;
				}
			}
		}
		float accelMod = GetAccelerationModifier(uid);
		mover.CurrentSpeed = GridVehicleMotionSimulator.StepFreeSpeed(mover.CurrentSpeed, throttle, GetModifiedMaxSpeed(uid, mover), GetModifiedMaxReverseSpeed(uid, mover), mover.Acceleration * accelMod, mover.ReverseAcceleration * accelMod, mover.Deceleration, mover.AllowReverse, frameTime);
		if (steer != 0)
		{
			float absSpeed = MathF.Abs(mover.CurrentSpeed);
			float turnRateDeg;
			float dir;
			if (throttle == 0 && absSpeed <= 0.01f)
			{
				turnRateDeg = mover.PivotTurnRate;
				dir = 1f;
			}
			else
			{
				float fullSpeed = MathF.Max(0.01f, mover.TurnFullSpeed);
				float speedFactor = Math.Clamp(absSpeed / fullSpeed, 0f, 1f);
				if (throttle != 0)
				{
					speedFactor = MathF.Max(speedFactor, 0.4f);
				}
				turnRateDeg = mover.TurnRate * speedFactor;
				dir = ((mover.CurrentSpeed < -0.01f) ? (-1f) : 1f);
			}
			if (turnRateDeg > 0f)
			{
				float turnRateRad = MathHelper.DegreesToRadians(turnRateDeg);
				float dHeading = (float)(-steer) * turnRateRad * frameTime * dir;
				Angle val2 = new Angle(mover.Heading.Theta + (double)dHeading);
				Angle newHeading = ((Angle)(ref val2)).Reduced();
				TryApplyFreeHeading(uid, mover, grid, newHeading);
			}
		}
		if (MathF.Abs(mover.CurrentSpeed) > 0.01f)
		{
			Vector2 forward = ((Angle)(ref mover.Heading)).ToWorldVec();
			float travel = mover.CurrentSpeed * frameTime;
			Vector2 target = mover.Position + forward * travel;
			TryMoveContinuous(uid, mover, grid, target, mover.Heading, out var blocked);
			if (blocked)
			{
				mover.CurrentSpeed = 0f;
			}
		}
		UpdateDerivedTileState(grid, gridComp, mover);
		mover.IsPushMove = false;
		mover.IsMoving = MathF.Abs(mover.CurrentSpeed) > 0.01f;
		UpdateRunSound(uid, mover);
		transform.SetLocalRotation(uid, mover.Heading, (TransformComponent)null);
		SetGridPosition(uid, grid, mover.Position);
		if (mover.IsMoving)
		{
			physics.WakeBody(uid, false, (FixturesComponent)null, (PhysicsComponent)null);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)mover, (MetaDataComponent)null);
	}

	private void TryApplyFreeHeading(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, Angle newHeading)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Vector2 nudged;
		if (CanOccupyTransform(uid, mover, grid, mover.Position, newHeading, 0.0075f, applyEffects: false, debug: false))
		{
			mover.Heading = newHeading;
		}
		else if (TryFindFreeHeadingNudge(uid, mover, grid, newHeading, out nudged))
		{
			mover.Position = nudged;
			mover.Heading = newHeading;
		}
	}

	private bool TryFindFreeHeadingNudge(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, Angle newHeading, out Vector2 nudged)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		nudged = mover.Position;
		float maxDist = MathF.Max(0f, mover.TurnCollisionGraceDistance);
		if (maxDist <= 0f)
		{
			return false;
		}
		Vector2 axis = ((Angle)(ref mover.Heading)).ToWorldVec();
		if (axis.LengthSquared() <= 0f)
		{
			return false;
		}
		float step = Math.Clamp(mover.MovementProbeStep, 0.02f, 0.5f);
		int steps = Math.Max(1, (int)MathF.Ceiling(maxDist / step));
		for (int i = 1; i <= steps; i++)
		{
			float distance = MathF.Min((float)i * step, maxDist);
			Vector2 forward = mover.Position + axis * distance;
			if (CanOccupyTransform(uid, mover, grid, forward, newHeading, 0.0075f, applyEffects: false, debug: false))
			{
				nudged = forward;
				return true;
			}
			Vector2 back = mover.Position - axis * distance;
			if (CanOccupyTransform(uid, mover, grid, back, newHeading, 0.0075f, applyEffects: false, debug: false))
			{
				nudged = back;
				return true;
			}
		}
		return false;
	}

	private void TryDepenetrateFree(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		_depenetrateBlockers.Clear();
		if (CanOccupyTransform(uid, mover, grid, mover.Position, mover.Heading, 0.0075f, applyEffects: false, debug: false, _depenetrateBlockers) || _depenetrateBlockers.Count == 0)
		{
			return;
		}
		Vector2 separation = Vector2.Zero;
		foreach (EntityUid blocker in _depenetrateBlockers)
		{
			if (!((EntitySystem)this).HasComp<VehicleComponent>(blocker))
			{
				return;
			}
			MapCoordinates blockerMap = transform.GetMapCoordinates(blocker, (TransformComponent)null);
			Vector2 blockerLocal = transform.ToCoordinates(Entity<TransformComponent>.op_Implicit(grid), blockerMap).Position;
			Vector2 away = mover.Position - blockerLocal;
			if (away.LengthSquared() > 0.0001f)
			{
				separation += Vector2.Normalize(away);
			}
		}
		if (separation.LengthSquared() <= 0.0001f)
		{
			separation = ((Angle)(ref mover.Heading)).ToWorldVec();
		}
		if (separation.LengthSquared() <= 0.0001f)
		{
			return;
		}
		separation = Vector2.Normalize(separation);
		float step = Math.Clamp(mover.MovementProbeStep, 0.02f, 0.5f);
		int steps = Math.Max(1, (int)MathF.Ceiling(2f / step));
		for (int i = 1; i <= steps; i++)
		{
			Vector2 candidate = mover.Position + separation * ((float)i * step);
			if (CanOccupyTransform(uid, mover, grid, candidate, mover.Heading, 0.0075f, applyEffects: false, debug: false))
			{
				mover.Position = candidate;
				mover.CurrentSpeed = 0f;
				UpdateDerivedTileState(grid, gridComp, mover);
				break;
			}
		}
	}

	private void UpdateMovement(EntityUid uid, GridVehicleMoverComponent mover, VehicleComponent vehicle, EntityUid grid, MapGridComponent gridComp, Vector2i inputDir, bool pushing, float frameTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (vehicle.Operator.HasValue)
		{
			VehicleCanRunEvent canRunEvent = new VehicleCanRunEvent(Entity<VehicleComponent>.op_Implicit((uid, vehicle)));
			((EntitySystem)this).RaiseLocalEvent<VehicleCanRunEvent>(uid, ref canRunEvent, false);
			if (!canRunEvent.CanRun)
			{
				StopMover(mover);
				SetGridPosition(uid, grid, mover.Position);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)mover, (MetaDataComponent)null);
				return;
			}
		}
		GetSmashSlowdownMultiplier(mover);
		mover.IsCommittedToMove = false;
		if (!pushing)
		{
			mover.IsPushMove = false;
			mover.PushDirection = Vector2i.Zero;
		}
		bool num = (pushing ? UpdatePushMovement(uid, mover, grid, gridComp, inputDir, frameTime) : UpdateDriveMovement(uid, mover, grid, gridComp, inputDir, frameTime));
		UpdateDerivedTileState(grid, gridComp, mover);
		mover.IsMoving = MathF.Abs(mover.CurrentSpeed) > 0.01f;
		if (!mover.IsMoving)
		{
			mover.IsPushMove = false;
		}
		if (num && !pushing)
		{
			TryLayTrackTile(uid, mover, grid, gridComp, mover.CurrentTile);
		}
		UpdateRunSound(uid, mover);
		SetGridPosition(uid, grid, mover.Position);
		if (num || mover.IsMoving)
		{
			physics.WakeBody(uid, false, (FixturesComponent)null, (PhysicsComponent)null);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)mover, (MetaDataComponent)null);
	}

	private bool UpdatePushMovement(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i inputDir, float frameTime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		bool hasInput = inputDir != Vector2i.Zero;
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
		float maxSpeed = GetModifiedMaxSpeed(uid, mover);
		float accelModifier = GetAccelerationModifier(uid);
		if (hasInput && mover.PushImpulseSpeed > 0f)
		{
			float impulseSpeed = MathF.Min(mover.PushImpulseSpeed, maxSpeed);
			if (mover.CurrentSpeed < impulseSpeed)
			{
				mover.CurrentSpeed = impulseSpeed;
			}
		}
		mover.CurrentSpeed = GridVehicleMotionSimulator.StepPushSpeed(MathF.Max(0f, mover.CurrentSpeed), maxSpeed, mover.Acceleration * accelModifier, mover.Deceleration, hasInput, isCommittedToMove: false, frameTime);
		if (mover.PushDirection == Vector2i.Zero || mover.CurrentSpeed <= 0.01f)
		{
			return false;
		}
		Vector2i moveDir = mover.PushDirection;
		float travel = mover.CurrentSpeed * frameTime;
		bool blocked;
		bool scraped;
		bool num = TryMoveWithLaneGuidance(uid, mover, grid, gridComp, moveDir, null, travel, frameTime, out blocked, out scraped);
		if (blocked)
		{
			mover.CurrentSpeed = 0f;
		}
		if (num && hasInput && mover.PushCooldown > 0f)
		{
			mover.NextPushTime = _timing.CurTime + TimeSpan.FromSeconds(mover.PushCooldown);
		}
		return num;
	}

	private bool UpdateDriveMovement(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i inputDir, float frameTime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		bool hasInput = inputDir != Vector2i.Zero;
		Vector2i facing = mover.CurrentDirection;
		bool hadFacing = facing != Vector2i.Zero;
		if (hasInput && !hadFacing)
		{
			if (!TryApplyFacing(uid, mover, grid, gridComp, inputDir, startDelay: false, blockAfterTurn: false, allowMoveClearance: false))
			{
				mover.CurrentSpeed = 0f;
				return false;
			}
			facing = mover.CurrentDirection;
			hadFacing = true;
		}
		bool reversing = hasInput && hadFacing && inputDir == -facing;
		bool num = hasInput && hadFacing && !reversing && inputDir != facing;
		float turnInPlaceMaxSpeed = MathF.Max(0f, mover.TurnInPlaceMaxSpeed);
		bool atTurnSpeed = MathF.Abs(mover.CurrentSpeed) <= turnInPlaceMaxSpeed;
		if (num)
		{
			if (!CanApplyTurn(mover))
			{
				if (MathF.Abs(mover.CurrentSpeed) <= 0.01f)
				{
					mover.CurrentSpeed = 0f;
					return false;
				}
			}
			else
			{
				if (mover.TurnInPlace && atTurnSpeed)
				{
					bool result = TryApplyFacing(uid, mover, grid, gridComp, inputDir, startDelay: true, blockAfterTurn: true, allowMoveClearance: false);
					mover.CurrentSpeed = 0f;
					return result;
				}
				if (TryApplyFacing(uid, mover, grid, gridComp, inputDir, startDelay: true, blockAfterTurn: false, allowMoveClearance: true))
				{
					facing = mover.CurrentDirection;
					if (!atTurnSpeed)
					{
						TimeSpan now = _timing.CurTime;
						bool withinWindow = now - mover.LastMovingTurnTime <= TimeSpan.FromSeconds(mover.RapidTurnWindow);
						mover.TurnStreak = (withinWindow ? (mover.TurnStreak + 1) : 0);
						float multiplier = ((mover.TurnStreak >= mover.RapidTurnGraceCount) ? mover.RapidTurnSpeedMultiplier : mover.TurnSpeedMultiplier);
						if (multiplier < 1f)
						{
							mover.CurrentSpeed *= MathF.Max(0f, multiplier);
						}
						mover.LastMovingTurnTime = now;
					}
				}
				else if (MathF.Abs(mover.CurrentSpeed) <= 0.01f)
				{
					mover.CurrentSpeed = 0f;
					return false;
				}
			}
		}
		if (mover.TurnInPlace && hasInput && !reversing && atTurnSpeed && mover.InPlaceTurnBlockUntil > _timing.CurTime)
		{
			mover.CurrentSpeed = GridVehicleMotionSimulator.StepIdleSpeed(mover.CurrentSpeed, mover.Deceleration, frameTime);
			return false;
		}
		if (mover.CurrentDirection == Vector2i.Zero)
		{
			mover.CurrentSpeed = 0f;
			return false;
		}
		GridVehicleMotionSimulator.DriveProfile profile = GetDriveProfile(uid, mover);
		GridVehicleMotionSimulator.DriveSpeedResult speedResult = (hasInput ? GridVehicleMotionSimulator.StepDriveSpeed(mover.CurrentSpeed, profile, mover.CurrentDirection, inputDir, hasInput, isCommittedToMove: false, frameTime) : new GridVehicleMotionSimulator.DriveSpeedResult(GridVehicleMotionSimulator.StepIdleSpeed(mover.CurrentSpeed, mover.Deceleration, frameTime), ReversingInput: false, ChangingDirection: false));
		mover.CurrentSpeed = speedResult.CurrentSpeed;
		if (speedResult.ChangingDirection)
		{
			mover.CurrentSpeed = 0f;
			return false;
		}
		float travel = MathF.Abs(mover.CurrentSpeed) * frameTime;
		if (travel <= 0.0001f)
		{
			return false;
		}
		Vector2i moveDir = ((mover.CurrentSpeed >= 0f) ? mover.CurrentDirection : (-mover.CurrentDirection));
		Angle rotation = DirectionToVehicleRotation(mover.CurrentDirection);
		bool blocked;
		bool scraped;
		bool result2 = TryMoveWithLaneGuidance(uid, mover, grid, gridComp, moveDir, rotation, travel, frameTime, out blocked, out scraped);
		if (blocked)
		{
			mover.CurrentSpeed = 0f;
			return result2;
		}
		if (scraped)
		{
			ApplyScrapeSlowdown(mover, frameTime);
		}
		return result2;
	}

	private static void ApplyScrapeSlowdown(GridVehicleMoverComponent mover, float frameTime)
	{
		if (!(mover.ScrapeDeceleration <= 0f))
		{
			float absSpeed = MathF.Abs(mover.CurrentSpeed);
			float slowed = MathF.Max(MathF.Min(absSpeed, MathF.Max(0f, mover.ScrapeMinSpeed)), absSpeed - mover.ScrapeDeceleration * frameTime);
			mover.CurrentSpeed = MathF.CopySign(slowed, mover.CurrentSpeed);
		}
	}

	private bool TryApplyFacing(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i desiredFacing, bool startDelay, bool blockAfterTurn, bool allowMoveClearance)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (desiredFacing == Vector2i.Zero)
		{
			return false;
		}
		Angle desiredRot = DirectionToVehicleRotation(desiredFacing);
		if (!TryFindTurnPosition(uid, mover, grid, gridComp, desiredRot, out var turnPosition) && (!allowMoveClearance || !TryFindTransientTurnClearance(uid, mover, grid, desiredFacing, desiredRot, out turnPosition)))
		{
			return false;
		}
		if (!CanOccupyTransform(uid, mover, grid, turnPosition, desiredRot, 0.0075f, applyEffects: true))
		{
			return false;
		}
		bool num = mover.CurrentDirection != desiredFacing;
		bool moved = turnPosition != mover.Position;
		mover.Position = turnPosition;
		mover.CurrentTile = GetTile(grid, gridComp, mover.Position);
		mover.CurrentDirection = desiredFacing;
		transform.SetLocalRotation(uid, desiredRot, (TransformComponent)null);
		if (num && startDelay)
		{
			StartTurnDelay(mover);
			if (blockAfterTurn && mover.TurnDelay > 0f)
			{
				mover.InPlaceTurnBlockUntil = _timing.CurTime + TimeSpan.FromSeconds(mover.TurnDelay);
			}
		}
		return num || moved;
	}

	private bool TryFindTransientTurnClearance(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, Vector2i desiredFacing, Angle desiredRot, out Vector2 clearPosition)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		clearPosition = mover.Position;
		float maxDistance = MathF.Max(0f, mover.TurnCollisionGraceDistance);
		if (maxDistance <= 0f)
		{
			return false;
		}
		Vector2 forward = new Vector2(desiredFacing.X, desiredFacing.Y);
		if (forward.LengthSquared() <= 0f)
		{
			return false;
		}
		float step = Math.Clamp(mover.MovementProbeStep, 0.02f, 0.5f);
		int steps = Math.Max(1, (int)MathF.Ceiling(maxDistance / step));
		HashSet<EntityUid> initialBlockers = new HashSet<EntityUid>();
		if (CanOccupyTransform(uid, mover, grid, mover.Position, desiredRot, 0.0075f, applyEffects: false, debug: true, initialBlockers) || initialBlockers.Count == 0)
		{
			return false;
		}
		HashSet<EntityUid> sampleBlockers = new HashSet<EntityUid>();
		for (int i = 1; i <= steps; i++)
		{
			float distance = MathF.Min((float)i * step, maxDistance);
			Vector2 sample = mover.Position + forward * distance;
			sampleBlockers.Clear();
			if (CanOccupyTransform(uid, mover, grid, sample, desiredRot, 0.0075f, applyEffects: false, debug: true, sampleBlockers))
			{
				clearPosition = sample;
				return true;
			}
			foreach (EntityUid blocker in sampleBlockers)
			{
				if (!initialBlockers.Contains(blocker))
				{
					return false;
				}
			}
			if (sampleBlockers.Count <= 0)
			{
				return false;
			}
		}
		return false;
	}

	private bool TryMoveWithLaneGuidance(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i moveDir, Angle? rotation, float travel, float frameTime, out bool blocked, out bool scraped)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		blocked = false;
		scraped = false;
		Vector2 forward = new Vector2(moveDir.X, moveDir.Y);
		Vector2 directTarget = mover.Position + forward * travel;
		bool moved = false;
		_directMoveBlockers.Clear();
		HashSet<EntityUid> ignoredEntities = GetPushIgnoredEntities(uid, mover);
		if (CanMoveContinuous(uid, mover, grid, directTarget, rotation, CollisionDebugEnabled, _directMoveBlockers, ignoredEntities))
		{
			AddDebugMovementDecision(uid, grid, mover.Position, directTarget, forward, DebugMovementDecisionKind.DirectClear, success: true);
			return TryMoveKnownClear(uid, mover, grid, directTarget, rotation, out blocked, applyBlockEffects: true, ignoredEntities);
		}
		scraped = true;
		AddDebugMovementDecision(uid, grid, mover.Position, directTarget, forward, DebugMovementDecisionKind.DirectBlocked, success: false);
		if (TryGetBlockingMobBypassCorrection(uid, mover, grid, gridComp, moveDir, rotation, directTarget, frameTime, _directMoveBlockers, ignoredEntities, out var mobBypassCorrection))
		{
			moved = TryApplyLateralCorrection(uid, mover, grid, moveDir, rotation, mobBypassCorrection, ignoredEntities);
			if (moved)
			{
				return true;
			}
		}
		if (TryGetLaneCorrection(uid, mover, grid, gridComp, moveDir, rotation, directTarget, frameTime, ignoredEntities, out var correction))
		{
			moved = TryApplyLateralCorrection(uid, mover, grid, moveDir, rotation, correction, ignoredEntities);
		}
		Vector2 forwardStart = mover.Position;
		Vector2 forwardTarget = mover.Position + forward * travel;
		bool forwardMoved = TryMoveContinuous(uid, mover, grid, forwardTarget, rotation, out blocked, applyBlockEffects: true, debugProbes: true, ignoredEntities);
		AddDebugMovementDecision(uid, grid, forwardStart, forwardTarget, forward, blocked ? DebugMovementDecisionKind.ForwardBlocked : DebugMovementDecisionKind.ForwardAfterCorrection, forwardMoved && !blocked);
		return moved || forwardMoved;
	}

	private bool TryApplyLateralCorrection(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, Vector2i moveDir, Angle? rotation, float correction, HashSet<EntityUid>? ignoredEntities)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 lateralTarget = SetLateralCoordinate(mover.Position, moveDir, GetLateralCoordinate(mover.Position, moveDir) + correction);
		if ((lateralTarget - mover.Position).LengthSquared() <= 9.999999E-09f)
		{
			return false;
		}
		Vector2 lateralStart = mover.Position;
		Vector2 lateralDirection = lateralTarget - lateralStart;
		bool blocked;
		bool moved = TryMoveContinuous(uid, mover, grid, lateralTarget, rotation, out blocked, applyBlockEffects: false, debugProbes: false, ignoredEntities);
		AddDebugMovementDecision(uid, grid, lateralStart, lateralTarget, lateralDirection, moved ? DebugMovementDecisionKind.LaneCorrection : DebugMovementDecisionKind.LaneCorrectionFailed, moved);
		return moved;
	}

	private static void AddDebugMovementDecision(EntityUid uid, EntityUid grid, Vector2 start, Vector2 end, Vector2 moveDirection, DebugMovementDecisionKind kind, bool success)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (MovementDebugEnabled)
		{
			moveDirection = ((!(moveDirection.LengthSquared() > 0.0001f)) ? Vector2.Zero : Vector2.Normalize(moveDirection));
			DebugMovementDecisions.Add(new DebugMovementDecision(uid, grid, start, end, moveDirection, kind, success));
		}
	}

	private bool CanMoveContinuous(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, Vector2 target, Angle? rotation, bool debugProbes)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return CanMoveContinuous(uid, mover, grid, target, rotation, debugProbes, null);
	}

	private bool CanMoveContinuous(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, Vector2 target, Angle? rotation, bool debugProbes, HashSet<EntityUid>? blockers, HashSet<EntityUid>? ignoredEntities = null)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Vector2 start = mover.Position;
		Vector2 delta = target - start;
		float distance = delta.Length();
		if (distance <= 0.0001f)
		{
			return true;
		}
		float probeStep = Math.Clamp(mover.MovementProbeStep, 0.02f, 0.5f);
		int steps = Math.Max(1, (int)MathF.Ceiling(distance / probeStep));
		for (int i = 1; i <= steps; i++)
		{
			Vector2 candidate = start + delta * ((float)i / (float)steps);
			if (!CanOccupyTransform(uid, mover, grid, candidate, rotation, 0.0075f, applyEffects: false, debugProbes, blockers, ignoredEntities))
			{
				return false;
			}
		}
		return true;
	}

	private bool TryGetBlockingMobBypassCorrection(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i moveDir, Angle? rotation, Vector2 target, float frameTime, HashSet<EntityUid> directBlockers, HashSet<EntityUid>? ignoredEntities, out float correction)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		correction = 0f;
		if (!HasBlockingVehicleMob(mover, directBlockers))
		{
			return false;
		}
		if (!TryFindBlockingMobBypassOffset(uid, mover, grid, gridComp, moveDir, rotation, target, ignoredEntities, out var laneOffset))
		{
			return false;
		}
		return TryGetLateralCorrection(mover, grid, gridComp, moveDir, target, laneOffset, frameTime, out correction);
	}

	private bool TryFindBlockingMobBypassOffset(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i moveDir, Angle? rotation, Vector2 target, HashSet<EntityUid>? ignoredEntities, out float laneOffset)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		laneOffset = 0f;
		float limit = Math.Clamp(MathF.Max(mover.TileOffsetLimit, mover.BlockingMobBypassNudgeLimit), 0f, 3f);
		if (limit <= 0f || moveDir == Vector2i.Zero)
		{
			return false;
		}
		float step = Math.Clamp((mover.BlockingMobBypassNudgeStep > 0f) ? mover.BlockingMobBypassNudgeStep : mover.TileOffsetStep, 0.01f, limit);
		float centerLateral = GetLateralCoordinate(GetTileCenter(GetTile(grid, gridComp, target)), moveDir);
		float baseOffset = Math.Clamp(GetLateralCoordinate(target, moveDir) - centerLateral, 0f - limit, limit);
		int lookahead = Math.Max(1, mover.TileOffsetLookahead);
		int sampleSteps = (int)MathF.Ceiling(limit / step);
		float lastPositiveOffset = float.NaN;
		float lastNegativeOffset = float.NaN;
		for (int i = 1; i <= sampleSteps; i++)
		{
			float distance = MathF.Min((float)i * step, limit);
			if (TryBlockingMobBypassOffset(uid, mover, grid, gridComp, moveDir, rotation, target, baseOffset + distance, limit, centerLateral, lookahead, ignoredEntities, ref lastPositiveOffset, out laneOffset))
			{
				return true;
			}
			if (TryBlockingMobBypassOffset(uid, mover, grid, gridComp, moveDir, rotation, target, baseOffset - distance, limit, centerLateral, lookahead, ignoredEntities, ref lastNegativeOffset, out laneOffset))
			{
				return true;
			}
		}
		return false;
	}

	private bool TryBlockingMobBypassOffset(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i moveDir, Angle? rotation, Vector2 target, float offset, float limit, float centerLateral, int lookahead, HashSet<EntityUid>? ignoredEntities, ref float lastOffset, out float laneOffset)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		laneOffset = Math.Clamp(offset, 0f - limit, limit);
		if (MathF.Abs(laneOffset - lastOffset) <= 0.001f)
		{
			return false;
		}
		lastOffset = laneOffset;
		float desiredLateral = centerLateral + laneOffset;
		Vector2 lateralTarget = SetLateralCoordinate(mover.Position, moveDir, desiredLateral);
		if ((lateralTarget - mover.Position).LengthSquared() <= 9.999999E-09f)
		{
			return false;
		}
		if (!CanMoveContinuous(uid, mover, grid, lateralTarget, rotation, debugProbes: false, null, ignoredEntities))
		{
			return false;
		}
		return CanOccupyMoveLane(uid, mover, grid, gridComp, moveDir, rotation, target, laneOffset, lookahead, ignoredEntities);
	}

	private bool TryGetLaneCorrection(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i moveDir, Angle? rotation, Vector2 target, float frameTime, HashSet<EntityUid>? ignoredEntities, out float correction)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		correction = 0f;
		if (!TryFindBestLaneOffset(uid, mover, grid, gridComp, moveDir, rotation, target, ignoredEntities, out var laneOffset))
		{
			return false;
		}
		return TryGetLateralCorrection(mover, grid, gridComp, moveDir, target, laneOffset, frameTime, out correction);
	}

	private bool TryGetLateralCorrection(GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i moveDir, Vector2 target, float laneOffset, float frameTime, out float correction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 tileCenter = GetTileCenter(GetTile(grid, gridComp, target));
		float currentLateral = GetLateralCoordinate(mover.Position, moveDir);
		float desiredLateral = GetLateralCoordinate(tileCenter, moveDir) + laneOffset;
		float correctionSpeed = MathF.Max(0f, mover.LaneCorrectionSpeed);
		if (correctionSpeed <= 0f)
		{
			correction = 0f;
			return false;
		}
		float maxCorrection = correctionSpeed * frameTime;
		correction = Math.Clamp(desiredLateral - currentLateral, 0f - maxCorrection, maxCorrection);
		return MathF.Abs(correction) > 0.0001f;
	}

	private bool TryFindBestLaneOffset(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i moveDir, Angle? rotation, Vector2 target, HashSet<EntityUid>? ignoredEntities, out float laneOffset)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		laneOffset = 0f;
		float limit = Math.Clamp(mover.TileOffsetLimit, 0f, 2f);
		if (limit <= 0f || moveDir == Vector2i.Zero)
		{
			return false;
		}
		float step = Math.Clamp(mover.TileOffsetStep, 0.01f, limit);
		Vector2 center = GetTileCenter(GetTile(grid, gridComp, target));
		float baseOffset = Math.Clamp(GetLateralCoordinate(target, moveDir) - GetLateralCoordinate(center, moveDir), 0f - limit, limit);
		int sampleSteps = (int)MathF.Ceiling(limit / step);
		int lookahead = Math.Max(1, mover.TileOffsetLookahead);
		bool foundLane = false;
		float bestOffset = baseOffset;
		float bestScore = float.MaxValue;
		bool inLane = false;
		float laneStart = 0f;
		float laneEnd = 0f;
		for (int i = -sampleSteps; i <= sampleSteps; i++)
		{
			float offset = Math.Clamp((float)i * step, 0f - limit, limit);
			if (CanOccupyMoveLane(uid, mover, grid, gridComp, moveDir, rotation, target, offset, lookahead, ignoredEntities))
			{
				if (!inLane)
				{
					inLane = true;
					laneStart = offset;
				}
				laneEnd = offset;
			}
			else if (inLane)
			{
				foundLane = true;
				SelectMoveLane(laneStart, laneEnd, baseOffset, step, ref bestOffset, ref bestScore);
				inLane = false;
			}
		}
		if (inLane)
		{
			foundLane = true;
			SelectMoveLane(laneStart, laneEnd, baseOffset, step, ref bestOffset, ref bestScore);
		}
		if (!foundLane)
		{
			return false;
		}
		laneOffset = bestOffset;
		return true;
	}

	private bool CanOccupyMoveLane(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i moveDir, Angle? rotation, Vector2 target, float offset, int lookahead, HashSet<EntityUid>? ignoredEntities)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Vector2 forward = new Vector2(moveDir.X, moveDir.Y);
		float tileSize = MathF.Max(1f, (int)gridComp.TileSize);
		for (int i = 0; i < lookahead; i++)
		{
			Vector2 sample = target + forward * (tileSize * (float)i);
			float lateral = GetLateralCoordinate(GetTileCenter(GetTile(grid, gridComp, sample)), moveDir) + offset;
			sample = SetLateralCoordinate(sample, moveDir, lateral);
			if (!CanOccupyTransform(uid, mover, grid, sample, rotation, 0.0075f, applyEffects: false, debug: false, null, ignoredEntities))
			{
				return false;
			}
		}
		return true;
	}

	private bool TryMoveContinuous(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, Vector2 target, Angle? rotation, out bool blocked, bool applyBlockEffects = true, bool debugProbes = true, HashSet<EntityUid>? ignoredEntities = null)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		blocked = false;
		Vector2 start = mover.Position;
		Vector2 delta = target - start;
		float distance = delta.Length();
		if (distance <= 0.0001f)
		{
			return false;
		}
		float probeStep = Math.Clamp(mover.MovementProbeStep, 0.02f, 0.5f);
		int steps = Math.Max(1, (int)MathF.Ceiling(distance / probeStep));
		Vector2 lastGood = start;
		for (int i = 1; i <= steps; i++)
		{
			Vector2 candidate = start + delta * ((float)i / (float)steps);
			if (!CanOccupyTransform(uid, mover, grid, candidate, rotation, 0.0075f, applyEffects: false, debugProbes, null, ignoredEntities))
			{
				if (applyBlockEffects)
				{
					CanOccupyTransform(uid, mover, grid, candidate, rotation, 0.0075f, applyEffects: true, debug: false, null, ignoredEntities);
				}
				mover.Position = lastGood;
				blocked = true;
				return lastGood != start;
			}
			lastGood = candidate;
		}
		if (applyBlockEffects && !CanOccupyTransform(uid, mover, grid, lastGood, rotation, 0.0075f, applyEffects: true, debug: false, null, ignoredEntities))
		{
			mover.Position = start;
			blocked = true;
			return false;
		}
		mover.Position = lastGood;
		return true;
	}

	private bool TryMoveKnownClear(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, Vector2 target, Angle? rotation, out bool blocked, bool applyBlockEffects = true, HashSet<EntityUid>? ignoredEntities = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		blocked = false;
		if (applyBlockEffects && !CanOccupyTransform(uid, mover, grid, target, rotation, 0.0075f, applyEffects: true, debug: false, null, ignoredEntities))
		{
			blocked = true;
			return false;
		}
		mover.Position = target;
		return true;
	}

	private bool TryFindTurnPosition(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Angle desiredRot, out Vector2 turnPosition)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		turnPosition = mover.Position;
		if (CanOccupyTransform(uid, mover, grid, turnPosition, desiredRot, 0.0075f, applyEffects: false))
		{
			return true;
		}
		float limit = Math.Clamp(mover.TurnNudgeLimit, 0f, 0.49f);
		if (limit <= 0f)
		{
			return false;
		}
		float step = Math.Clamp(mover.TurnNudgeStep, 0.01f, limit);
		Vector2i currentTile = GetTile(grid, gridComp, mover.Position);
		Vector2 tileCenter = GetTileCenter(currentTile);
		Vector2 min = tileCenter - new Vector2(limit, limit);
		Vector2 max = tileCenter + new Vector2(limit, limit);
		int steps = (int)MathF.Ceiling(limit / step);
		for (int ring = 1; ring <= steps; ring++)
		{
			float axialDistance = Math.Clamp((float)ring * step, 0f - limit, limit);
			if (TryTurnNudgePosition(uid, mover, grid, gridComp, currentTile, desiredRot, new Vector2(axialDistance, 0f), min, max, out turnPosition))
			{
				return true;
			}
			if (TryTurnNudgePosition(uid, mover, grid, gridComp, currentTile, desiredRot, new Vector2(0f - axialDistance, 0f), min, max, out turnPosition))
			{
				return true;
			}
			if (TryTurnNudgePosition(uid, mover, grid, gridComp, currentTile, desiredRot, new Vector2(0f, axialDistance), min, max, out turnPosition))
			{
				return true;
			}
			if (TryTurnNudgePosition(uid, mover, grid, gridComp, currentTile, desiredRot, new Vector2(0f, 0f - axialDistance), min, max, out turnPosition))
			{
				return true;
			}
			for (int x = -ring; x <= ring; x++)
			{
				for (int y = -ring; y <= ring; y++)
				{
					if (Math.Max(Math.Abs(x), Math.Abs(y)) == ring && x != 0 && y != 0)
					{
						Vector2 offset = new Vector2(Math.Clamp((float)x * step, 0f - limit, limit), Math.Clamp((float)y * step, 0f - limit, limit));
						if (TryTurnNudgePosition(uid, mover, grid, gridComp, currentTile, desiredRot, offset, min, max, out turnPosition))
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	private bool TryTurnNudgePosition(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i currentTile, Angle desiredRot, Vector2 offset, Vector2 min, Vector2 max, out Vector2 turnPosition)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		turnPosition = new Vector2(Math.Clamp(mover.Position.X + offset.X, min.X, max.X), Math.Clamp(mover.Position.Y + offset.Y, min.Y, max.Y));
		if (turnPosition == mover.Position)
		{
			return false;
		}
		if (GetTile(grid, gridComp, turnPosition) != currentTile)
		{
			return false;
		}
		return CanOccupyTransform(uid, mover, grid, turnPosition, desiredRot, 0.0075f, applyEffects: false);
	}

	private void SelectMoveLane(float laneStart, float laneEnd, float baseOffset, float step, ref float bestOffset, ref float bestScore)
	{
		float laneWidth = laneEnd - laneStart;
		float center = (laneStart + laneEnd) * 0.5f;
		float margin = MathF.Min(step * 2f, laneWidth * 0.5f);
		float safeStart = laneStart + margin;
		float safeEnd = laneEnd - margin;
		float offset = ((safeStart <= safeEnd) ? Math.Clamp(baseOffset, safeStart, safeEnd) : center);
		float score = MathF.Abs(offset - baseOffset) - laneWidth * 0.25f;
		if (!(score >= bestScore))
		{
			bestScore = score;
			bestOffset = offset;
		}
	}

	private GridVehicleMotionSimulator.DriveProfile GetDriveProfile(EntityUid uid, GridVehicleMoverComponent mover)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		float accelModifier = GetAccelerationModifier(uid);
		return new GridVehicleMotionSimulator.DriveProfile(GetModifiedMaxSpeed(uid, mover), GetModifiedMaxReverseSpeed(uid, mover), mover.Acceleration * accelModifier, mover.ReverseAcceleration * accelModifier, mover.Deceleration);
	}

	private float GetModifiedMaxSpeed(EntityUid uid, GridVehicleMoverComponent mover)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		float maxSpeed = mover.MaxSpeed * GetSmashSlowdownMultiplier(mover);
		VehicleOverchargeComponent overcharge = default(VehicleOverchargeComponent);
		if (((EntitySystem)this).TryComp<VehicleOverchargeComponent>(uid, ref overcharge) && _timing.CurTime < overcharge.ActiveUntil)
		{
			maxSpeed *= overcharge.SpeedMultiplier;
		}
		VehicleSpeedModifierComponent speedMod = default(VehicleSpeedModifierComponent);
		if (((EntitySystem)this).TryComp<VehicleSpeedModifierComponent>(uid, ref speedMod))
		{
			maxSpeed *= speedMod.SpeedMultiplier;
		}
		return maxSpeed;
	}

	private float GetModifiedMaxReverseSpeed(EntityUid uid, GridVehicleMoverComponent mover)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		float maxSpeed = mover.MaxReverseSpeed * GetSmashSlowdownMultiplier(mover);
		VehicleOverchargeComponent overcharge = default(VehicleOverchargeComponent);
		if (((EntitySystem)this).TryComp<VehicleOverchargeComponent>(uid, ref overcharge) && _timing.CurTime < overcharge.ActiveUntil)
		{
			maxSpeed *= overcharge.SpeedMultiplier;
		}
		VehicleSpeedModifierComponent speedMod = default(VehicleSpeedModifierComponent);
		if (((EntitySystem)this).TryComp<VehicleSpeedModifierComponent>(uid, ref speedMod))
		{
			maxSpeed *= speedMod.SpeedMultiplier;
		}
		return maxSpeed;
	}

	private float GetAccelerationModifier(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		VehicleAccelerationModifierComponent accelMod = default(VehicleAccelerationModifierComponent);
		if (((EntitySystem)this).TryComp<VehicleAccelerationModifierComponent>(uid, ref accelMod))
		{
			return MathF.Max(0.05f, accelMod.AccelerationMultiplier);
		}
		return 1f;
	}

	private void StopMover(GridVehicleMoverComponent mover)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		mover.CurrentSpeed = 0f;
		mover.IsCommittedToMove = false;
		mover.IsPushMove = false;
		mover.IsMoving = false;
		mover.TargetPosition = mover.Position;
		mover.TargetTile = mover.CurrentTile;
		mover.PushDirection = Vector2i.Zero;
	}

	private void UpdateDerivedTileState(EntityUid grid, MapGridComponent gridComp, GridVehicleMoverComponent mover)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		mover.TargetTile = (mover.CurrentTile = GetTile(grid, gridComp, mover.Position));
		mover.TargetPosition = mover.Position;
	}

	private static Angle DirectionToVehicleRotation(Vector2i direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return DirectionExtensions.ToWorldAngle(new Vector2(direction.X, direction.Y));
	}

	private static float GetLateralCoordinate(Vector2 position, Vector2i moveDir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (moveDir.X == 0)
		{
			return position.X;
		}
		return position.Y;
	}

	private static Vector2 SetLateralCoordinate(Vector2 position, Vector2i moveDir, float lateral)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (moveDir.X != 0)
		{
			position.Y = lateral;
		}
		else
		{
			position.X = lateral;
		}
		return position;
	}

	private static Vector2 GetTileCenter(Vector2i tile)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)tile.X + 0.5f, (float)tile.Y + 0.5f);
	}

	private HashSet<EntityUid>? GetPushIgnoredEntities(EntityUid uid, GridVehicleMoverComponent mover)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (!mover.IsPushMove)
		{
			return null;
		}
		if (!_activeXenoPushers.TryGetValue(uid, out var pusher))
		{
			return null;
		}
		if (!((EntityUid)(ref pusher)).IsValid() || ((EntitySystem)this).TerminatingOrDeleted(pusher, (MetaDataComponent)null))
		{
			_activeXenoPushers.Remove(uid);
			return null;
		}
		_pushIgnoredEntities.Clear();
		_pushIgnoredEntities.Add(pusher);
		return _pushIgnoredEntities;
	}

	private bool CanApplyTurn(GridVehicleMoverComponent mover)
	{
		if (mover.TurnDelay <= 0f)
		{
			return true;
		}
		return _timing.CurTime >= mover.NextTurnTime;
	}

	private void StartTurnDelay(GridVehicleMoverComponent mover)
	{
		if (!(mover.TurnDelay <= 0f))
		{
			mover.NextTurnTime = _timing.CurTime + TimeSpan.FromSeconds(mover.TurnDelay);
		}
	}

	private void SetGridPosition(EntityUid uid, EntityUid grid, Vector2 gridPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		EntityUid parentUid = xform.ParentUid;
		if (((EntityUid)(ref parentUid)).IsValid())
		{
			EntityCoordinates coords = default(EntityCoordinates);
			((EntityCoordinates)(ref coords))._002Ector(grid, gridPos);
			Vector2 local = ((EntityCoordinates)(ref coords)).WithEntityId(xform.ParentUid, transform, (IEntityManager)(object)base.EntityManager).Position;
			transform.SetLocalPosition(uid, local, xform);
		}
	}

	private Vector2i GetTile(EntityUid grid, MapGridComponent gridComp, Vector2 pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coords = default(EntityCoordinates);
		((EntityCoordinates)(ref coords))._002Ector(grid, pos);
		return map.TileIndicesFor(grid, gridComp, coords);
	}

	private void PlayRunningSound(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		VehicleSoundComponent sound = default(VehicleSoundComponent);
		if (((EntitySystem)this).TryComp<VehicleSoundComponent>(uid, ref sound) && sound.RunningSound != null && !_net.IsClient)
		{
			TimeSpan now = _timing.CurTime;
			if (!(sound.NextRunningSound > now))
			{
				_audio.PlayPvs(sound.RunningSound, uid, (AudioParams?)null);
				sound.NextRunningSound = now + TimeSpan.FromSeconds(sound.RunningSoundCooldown);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)sound, (MetaDataComponent)null);
			}
		}
	}

	private float GetSmashSlowdownMultiplier(GridVehicleMoverComponent mover)
	{
		if (mover.SmashSlowdownMultiplier >= 1f && mover.SmashSlowdownUntil == TimeSpan.Zero)
		{
			return 1f;
		}
		TimeSpan now = _timing.CurTime;
		if (mover.SmashSlowdownUntil != TimeSpan.Zero && now >= mover.SmashSlowdownUntil)
		{
			mover.SmashSlowdownMultiplier = 1f;
			mover.SmashSlowdownUntil = TimeSpan.Zero;
			return 1f;
		}
		return Math.Clamp(mover.SmashSlowdownMultiplier, 0f, 1f);
	}

	private void ApplySmashSlowdown(EntityUid vehicle, GridVehicleMoverComponent mover, VehicleSmashableComponent smashable)
	{
		if (!(smashable.SlowdownDuration <= 0f) && !(smashable.SlowdownMultiplier >= 1f))
		{
			TimeSpan curTime = _timing.CurTime;
			mover.SmashSlowdownMultiplier = MathF.Min(mover.SmashSlowdownMultiplier, smashable.SlowdownMultiplier);
			TimeSpan until = curTime + TimeSpan.FromSeconds(smashable.SlowdownDuration);
			if (until > mover.SmashSlowdownUntil)
			{
				mover.SmashSlowdownUntil = until;
			}
			mover.CurrentSpeed *= smashable.SlowdownMultiplier;
		}
	}

	private Vector2i GetInputDirection(InputMoverComponent input)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		MoveButtons heldMoveButtons = input.HeldMoveButtons;
		Vector2i dir = Vector2i.Zero;
		if ((heldMoveButtons & MoveButtons.Up) != MoveButtons.None)
		{
			dir += new Vector2i(0, 1);
		}
		if ((heldMoveButtons & MoveButtons.Down) != MoveButtons.None)
		{
			dir += new Vector2i(0, -1);
		}
		if ((heldMoveButtons & MoveButtons.Right) != MoveButtons.None)
		{
			dir += new Vector2i(1, 0);
		}
		if ((heldMoveButtons & MoveButtons.Left) != MoveButtons.None)
		{
			dir += new Vector2i(-1, 0);
		}
		if (dir == Vector2i.Zero)
		{
			return dir;
		}
		if (dir.X != 0 && dir.Y != 0)
		{
			if (Math.Abs(dir.X) >= Math.Abs(dir.Y))
			{
				((Vector2i)(ref dir))._002Ector(Math.Sign(dir.X), 0);
			}
			else
			{
				((Vector2i)(ref dir))._002Ector(0, Math.Sign(dir.Y));
			}
		}
		return dir;
	}

	private Vector2i GetMoverInput(EntityUid uid, GridVehicleMoverComponent mover, VehicleComponent vehicle, out bool pushing)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		pushing = false;
		EntityUid? val = vehicle.Operator;
		if (val.HasValue)
		{
			EntityUid op = val.GetValueOrDefault();
			InputMoverComponent inputComp = default(InputMoverComponent);
			if (((EntitySystem)this).TryComp<InputMoverComponent>(op, ref inputComp))
			{
				_activeXenoPushers.Remove(uid);
				return GetInputDirection(inputComp);
			}
		}
		if (vehicle.Operator.HasValue)
		{
			_activeXenoPushers.Remove(uid);
			return Vector2i.Zero;
		}
		if (!TryGetActivePusher(uid, mover, out var pusher))
		{
			if (mover.IsPushMove && mover.PushDirection != Vector2i.Zero && mover.CurrentSpeed > 0.01f)
			{
				pushing = true;
				return Vector2i.Zero;
			}
			_activeXenoPushers.Remove(uid);
			return Vector2i.Zero;
		}
		pushing = true;
		if (!mover.IsPushMove && !CanPushNow(mover))
		{
			_activeXenoPushers.Remove(uid);
			return Vector2i.Zero;
		}
		Vector2i pushDir = GetPushDirection(uid, pusher);
		if (pushDir == Vector2i.Zero)
		{
			_activeXenoPushers.Remove(uid);
			return Vector2i.Zero;
		}
		_activeXenoPushers[uid] = pusher;
		return pushDir;
	}

	private bool TryGetActivePusher(EntityUid uid, GridVehicleMoverComponent mover, out EntityUid pusher)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		pusher = default(EntityUid);
		PhysicsComponent body = default(PhysicsComponent);
		if (!physicsQ.TryComp(uid, ref body) || !body.CanCollide)
		{
			return false;
		}
		FixturesComponent fixtures = default(FixturesComponent);
		if (!fixtureQ.TryComp(uid, ref fixtures))
		{
			return false;
		}
		Vector2 vehiclePos = transform.GetWorldPosition(uid);
		ContactEnumerator contacts = physics.GetContacts(Entity<FixturesComponent>.op_Implicit((uid, fixtures)), false);
		float bestScore = 0f;
		Contact contact = default(Contact);
		InputMoverComponent input = default(InputMoverComponent);
		while (((ContactEnumerator)(ref contacts)).MoveNext(ref contact))
		{
			if (contact == null || !contact.IsTouching)
			{
				continue;
			}
			EntityUid other = contact.OtherEnt(uid);
			if (!((EntitySystem)this).HasComp<XenoComponent>(other) || !contact.Hard || !CanXenoPushVehicle(mover, other) || !((EntitySystem)this).TryComp<InputMoverComponent>(other, ref input))
			{
				continue;
			}
			Vector2i dir = GetInputDirection(input);
			if (dir == Vector2i.Zero)
			{
				continue;
			}
			Vector2 otherPos = transform.GetWorldPosition(other);
			Vector2 toVehicle = vehiclePos - otherPos;
			if (!(toVehicle.LengthSquared() <= 0.0001f))
			{
				float score = Vector2.Dot(new Vector2(dir.X, dir.Y), Vector2.Normalize(toVehicle));
				if (!(score <= 0f) && score > bestScore)
				{
					bestScore = score;
					pusher = other;
				}
			}
		}
		if (bestScore > 0f)
		{
			return true;
		}
		return false;
	}

	private Vector2i GetPushDirection(EntityUid uid, EntityUid pusher)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 worldPosition = transform.GetWorldPosition(uid);
		Vector2 pusherPos = transform.GetWorldPosition(pusher);
		Vector2 delta = worldPosition - pusherPos;
		if (delta.LengthSquared() <= 0.0001f)
		{
			return Vector2i.Zero;
		}
		Angle val = Angle.FromWorldVec(delta);
		return DirectionExtensions.ToIntVec(((Angle)(ref val)).GetCardinalDir());
	}

	private bool CanPushNow(GridVehicleMoverComponent mover)
	{
		if (mover.PushCooldown <= 0f)
		{
			return true;
		}
		return _timing.CurTime >= mover.NextPushTime;
	}

	private bool CanXenoPushVehicle(GridVehicleMoverComponent mover, EntityUid xeno)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!mover.CanXenosPush)
		{
			return false;
		}
		RMCSizes? xenoPushMinimumSize = mover.XenoPushMinimumSize;
		if (xenoPushMinimumSize.HasValue)
		{
			RMCSizes minSize = xenoPushMinimumSize.GetValueOrDefault();
			if (!_size.TryGetSize(xeno, out var size))
			{
				return false;
			}
			return (int)size >= (int)minSize;
		}
		return true;
	}

	public bool CanPlanTileStep(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i currentTile, Vector2i currentDirection, Vector2i moveDir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (moveDir == Vector2i.Zero)
		{
			return false;
		}
		((Vector2i)(ref moveDir))._002Ector(Math.Sign(moveDir.X), Math.Sign(moveDir.Y));
		if (moveDir.X != 0 && moveDir.Y != 0)
		{
			return false;
		}
		if (currentDirection != Vector2i.Zero && moveDir == -currentDirection)
		{
			return false;
		}
		Angle desiredRotation = DirectionExtensions.ToWorldAngle(new Vector2(moveDir.X, moveDir.Y));
		Vector2 currentCenter = new Vector2((float)currentTile.X + 0.5f, (float)currentTile.Y + 0.5f);
		if (!CanOccupyTransform(uid, mover, grid, currentCenter, desiredRotation, 0.0075f, applyEffects: false))
		{
			return false;
		}
		Vector2i targetTile = currentTile + moveDir;
		Vector2 targetCenter = new Vector2((float)targetTile.X + 0.5f, (float)targetTile.Y + 0.5f);
		return CanOccupyTransform(uid, mover, grid, targetCenter, desiredRotation, 0.0075f, applyEffects: false);
	}

	private void UpdateRunSound(EntityUid uid, GridVehicleMoverComponent mover)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		VehicleSoundComponent sound = default(VehicleSoundComponent);
		if (_net.IsClient || !((EntitySystem)this).TryComp<VehicleSoundComponent>(uid, ref sound) || sound.RunningSound == null)
		{
			return;
		}
		if (!sound.RunLoop)
		{
			if (mover.IsMoving && !mover.IsPushMove)
			{
				PlayRunOneShot(uid, sound);
			}
			else
			{
				StopRunAudio(uid, sound);
			}
			return;
		}
		if (mover.IsPushMove)
		{
			StopRunAudio(uid, sound);
			return;
		}
		float ratio = GetRunRatio(mover);
		if (!mover.IsMoving || ratio <= sound.RunThreshold)
		{
			StopRunAudio(uid, sound);
			return;
		}
		ratio = MathF.Round(ratio * 6f) / 6f;
		float volume = sound.RunMinVolume + (sound.RunMaxVolume - sound.RunMinVolume) * ratio;
		float pitch = sound.RunMinPitch + (sound.RunMaxPitch - sound.RunMinPitch) * ratio;
		AudioParams val = sound.RunningSound.Params;
		val = ((AudioParams)(ref val)).WithLoop(true);
		val = ((AudioParams)(ref val)).WithVolume(volume);
		AudioParams audioParams = ((AudioParams)(ref val)).WithPitchScale(pitch);
		EntityUid? runAudio = sound.RunAudio;
		if (runAudio.HasValue)
		{
			EntityUid audioUid = runAudio.GetValueOrDefault();
			AudioComponent audio = default(AudioComponent);
			if (((EntitySystem)this).TryComp<AudioComponent>(audioUid, ref audio))
			{
				TagSound(audioUid, RMCVehicleSoundKind.Run);
				sound.LastRunPitch = pitch;
				EntityCoordinates vehicleCoords = ((EntitySystem)this).Transform(uid).Coordinates;
				if (((EntitySystem)this).Transform(audioUid).Coordinates != vehicleCoords)
				{
					transform.SetCoordinates(audioUid, vehicleCoords);
				}
				VehicleFarSoundComponent farComp = default(VehicleFarSoundComponent);
				bool num = ((EntitySystem)this).TryComp<VehicleFarSoundComponent>(uid, ref farComp);
				if (num)
				{
					volume += farComp.VolumeBoost;
				}
				if (float.IsNaN(sound.LastRunVolume) || MathF.Abs(sound.LastRunVolume - volume) > 0.12f)
				{
					_audio.SetVolume((EntityUid?)audioUid, volume, audio);
					sound.LastRunVolume = volume;
				}
				if (num && _timing.CurTime >= farComp.NextFilterRefresh)
				{
					farComp.NextFilterRefresh = _timing.CurTime + TimeSpan.FromSeconds(farComp.FilterRefreshInterval);
					StopRunAudio(uid, sound);
				}
				return;
			}
		}
		VehicleFarSoundComponent far = default(VehicleFarSoundComponent);
		if (((EntitySystem)this).TryComp<VehicleFarSoundComponent>(uid, ref far))
		{
			float farVolume = volume + far.VolumeBoost;
			val = ((AudioParams)(ref audioParams)).WithVolume(farVolume);
			val = ((AudioParams)(ref val)).WithMaxDistance(far.AudioRange);
			val = ((AudioParams)(ref val)).WithReferenceDistance(far.ReferenceDistance);
			AudioParams farParams = ((AudioParams)(ref val)).WithRolloffFactor(far.RolloffFactor);
			EntityCoordinates coords = ((EntitySystem)this).Transform(uid).Coordinates;
			MapCoordinates mapCoords = transform.ToMapCoordinates(coords, true);
			if (mapCoords.MapId != MapId.Nullspace)
			{
				Filter filter = Filter.Empty().AddInRange(mapCoords, far.AudioRange, _player, (IEntityManager)(object)base.EntityManager);
				(EntityUid, AudioComponent)? played = _audio.PlayStatic(sound.RunningSound, filter, coords, true, (AudioParams?)farParams);
				if (played.HasValue)
				{
					sound.RunAudio = played.Value.Item1;
					sound.LastRunVolume = farVolume;
					sound.LastRunPitch = pitch;
					TagSound(played.Value.Item1, RMCVehicleSoundKind.Run);
				}
			}
		}
		else
		{
			(EntityUid, AudioComponent)? played2 = _audio.PlayPvs(sound.RunningSound, uid, (AudioParams?)audioParams);
			if (played2.HasValue)
			{
				sound.RunAudio = played2.Value.Item1;
				sound.LastRunVolume = volume;
				sound.LastRunPitch = pitch;
				TagSound(played2.Value.Item1, RMCVehicleSoundKind.Run);
			}
		}
	}

	private float GetRunRatio(GridVehicleMoverComponent mover)
	{
		float maxSpeed = ((mover.CurrentSpeed < 0f) ? mover.MaxReverseSpeed : mover.MaxSpeed);
		if (maxSpeed <= 0f)
		{
			return 0f;
		}
		return Math.Clamp(MathF.Abs(mover.CurrentSpeed) / maxSpeed, 0f, 1f);
	}

	private void PlayRunOneShot(EntityUid uid, VehicleSoundComponent sound)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		StopRunAudio(uid, sound);
		TimeSpan now = _timing.CurTime;
		if (sound.NextRunningSound > now || sound.RunningSound == null)
		{
			return;
		}
		VehicleFarSoundComponent far = default(VehicleFarSoundComponent);
		if (((EntitySystem)this).TryComp<VehicleFarSoundComponent>(uid, ref far))
		{
			EntityCoordinates coords = ((EntitySystem)this).Transform(uid).Coordinates;
			MapCoordinates mapCoords = transform.ToMapCoordinates(coords, true);
			if (mapCoords.MapId != MapId.Nullspace)
			{
				AudioParams val = sound.RunningSound.Params;
				AudioParams val2 = sound.RunningSound.Params;
				val = ((AudioParams)(ref val)).WithVolume(((AudioParams)(ref val2)).Volume + far.VolumeBoost);
				val = ((AudioParams)(ref val)).WithMaxDistance(far.AudioRange);
				val = ((AudioParams)(ref val)).WithReferenceDistance(far.ReferenceDistance);
				AudioParams farParams = ((AudioParams)(ref val)).WithRolloffFactor(far.RolloffFactor);
				Filter filter = Filter.Empty().AddInRange(mapCoords, far.AudioRange, _player, (IEntityManager)(object)base.EntityManager);
				(EntityUid, AudioComponent)? played = _audio.PlayStatic(sound.RunningSound, filter, coords, true, (AudioParams?)farParams);
				if (played.HasValue)
				{
					TagSound(played.Value.Item1, RMCVehicleSoundKind.Run);
				}
			}
		}
		else
		{
			(EntityUid, AudioComponent)? played2 = _audio.PlayPvs(sound.RunningSound, uid, (AudioParams?)null);
			if (played2.HasValue)
			{
				TagSound(played2.Value.Item1, RMCVehicleSoundKind.Run);
			}
		}
		sound.NextRunningSound = now + TimeSpan.FromSeconds(sound.RunningSoundCooldown);
	}

	private void StopRunAudio(EntityUid uid, VehicleSoundComponent? sound = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<VehicleSoundComponent>(uid, ref sound, false))
		{
			sound.RunAudio = _audio.Stop(sound.RunAudio, (AudioComponent)null);
			sound.LastRunVolume = float.NaN;
			sound.LastRunPitch = float.NaN;
		}
	}

	private void TagSound(EntityUid uid, RMCVehicleSoundKind kind)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<RMCVehicleSoundTagComponent>(uid).Kind = kind;
	}

	private void TryLayTrackTile(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i tile)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		ProtoId<ContentTileDefinition>? trackTile = mover.TrackTile;
		if (trackTile.HasValue)
		{
			ProtoId<ContentTileDefinition> trackTile2 = trackTile.GetValueOrDefault();
			if (!(mover.CurrentDirection == Vector2i.Zero) && !(mover.TrackTileChance <= 0f) && (!(mover.TrackTileChance < 1f) || RandomExtensions.Prob(_random, mover.TrackTileChance)))
			{
				Vector2i leftOffset = default(Vector2i);
				((Vector2i)(ref leftOffset))._002Ector(-mover.CurrentDirection.Y, mover.CurrentDirection.X);
				Vector2i rightOffset = default(Vector2i);
				((Vector2i)(ref rightOffset))._002Ector(mover.CurrentDirection.Y, -mover.CurrentDirection.X);
				TryLayTrackTileAt(uid, mover, grid, gridComp, tile + leftOffset, trackTile2);
				TryLayTrackTileAt(uid, mover, grid, gridComp, tile + rightOffset, trackTile2);
			}
		}
	}

	private unsafe void TryLayTrackTileAt(EntityUid uid, GridVehicleMoverComponent mover, EntityUid grid, MapGridComponent gridComp, Vector2i tile, ProtoId<ContentTileDefinition> trackTile)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		TileRef tileRef = default(TileRef);
		if (!map.TryGetTileRef(grid, gridComp, tile, ref tileRef) || ((Tile)(ref tileRef.Tile)).IsEmpty)
		{
			return;
		}
		ContentTileDefinition currentTile = (ContentTileDefinition)(object)_tileDefinitions[tileRef.Tile.TypeId];
		ContentTileDefinition replacementTile = (ContentTileDefinition)(object)_tileDefinitions[ProtoId<ContentTileDefinition>.op_Implicit(trackTile)];
		if (tileRef.Tile.TypeId == replacementTile.TileId)
		{
			if (mover.TrackLifetime > 0f && _trackRestores.TryGetValue((grid, tile), out var existing))
			{
				_trackRestores[(grid, tile)] = existing with
				{
					RestoreAt = _timing.CurTime + TimeSpan.FromSeconds(mover.TrackLifetime)
				};
			}
		}
		else if ((!mover.TrackOnlyDiggable || currentTile.CanDig) && (mover.TrackTileWhitelist.Count <= 0 || mover.TrackTileWhitelist.Contains(ProtoId<ContentTileDefinition>.op_Implicit(currentTile.ID))) && !mover.TrackTileBlacklist.Contains(ProtoId<ContentTileDefinition>.op_Implicit(currentTile.ID)))
		{
			map.SetTile(grid, gridComp, tileRef.GridIndices, _tileSystem.GetVariantTile(replacementTile, ((object)(*(EntityUid*)(&uid))/*cast due to constrained. prefix*/).GetHashCode() ^ tile.X ^ (tile.Y << 16)));
			if (mover.TrackLifetime > 0f)
			{
				TrackRestore prev;
				int original = (_trackRestores.TryGetValue((grid, tile), out prev) ? prev.OriginalTileId : tileRef.Tile.TypeId);
				_trackRestores[(grid, tile)] = new TrackRestore(replacementTile.TileId, original, _timing.CurTime + TimeSpan.FromSeconds(mover.TrackLifetime));
			}
		}
	}

	private void UpdateTrackRestores()
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || _trackRestores.Count == 0 || _timing.CurTime < _nextTrackRestoreScan)
		{
			return;
		}
		_nextTrackRestoreScan = _timing.CurTime + TimeSpan.FromSeconds(1L);
		_dueTrackRestores.Clear();
		foreach (var (key, restore) in _trackRestores)
		{
			if (_timing.CurTime >= restore.RestoreAt)
			{
				_dueTrackRestores.Add(key);
			}
		}
		MapGridComponent gridComp = default(MapGridComponent);
		TileRef tileRef = default(TileRef);
		foreach (var key2 in _dueTrackRestores)
		{
			TrackRestore restore2 = _trackRestores[key2];
			_trackRestores.Remove(key2);
			if (gridQ.TryComp(key2.Grid, ref gridComp) && map.TryGetTileRef(key2.Grid, gridComp, key2.Pos, ref tileRef) && tileRef.Tile.TypeId == restore2.TrackTileId)
			{
				ContentTileDefinition original = (ContentTileDefinition)(object)_tileDefinitions[restore2.OriginalTileId];
				map.SetTile(key2.Grid, gridComp, key2.Pos, _tileSystem.GetVariantTile(original, key2.Pos.X ^ (key2.Pos.Y << 16)));
			}
		}
	}
}

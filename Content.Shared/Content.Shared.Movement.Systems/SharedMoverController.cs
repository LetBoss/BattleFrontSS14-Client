using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._PUBG.Movement;
using Content.Shared.ActionBlocker;
using Content.Shared.Alert;
using Content.Shared.CCVar;
using Content.Shared.Follower.Components;
using Content.Shared.Friction;
using Content.Shared.Gravity;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Content.Shared.Maps;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Tag;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Movement.Systems;

public abstract class SharedMoverController : VirtualController
{
	private sealed class CameraRotateInputCmdHandler : InputCmdHandler
	{
		private readonly SharedMoverController _controller;

		private readonly Angle _angle;

		public CameraRotateInputCmdHandler(SharedMoverController controller, Direction direction)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			_controller = controller;
			_angle = DirectionExtensions.ToAngle(direction);
		}

		public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (session == null || !session.AttachedEntity.HasValue)
			{
				return false;
			}
			if ((int)message.State != 0)
			{
				return false;
			}
			_controller.RotateCamera(session.AttachedEntity.Value, _angle);
			return false;
		}
	}

	private sealed class CameraResetInputCmdHandler : InputCmdHandler
	{
		private readonly SharedMoverController _controller;

		public CameraResetInputCmdHandler(SharedMoverController controller)
		{
			_controller = controller;
		}

		public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (session == null || !session.AttachedEntity.HasValue)
			{
				return false;
			}
			if ((int)message.State != 0)
			{
				return false;
			}
			_controller.ResetCamera(session.AttachedEntity.Value);
			return false;
		}
	}

	private sealed class MoverDirInputCmdHandler : InputCmdHandler
	{
		private readonly SharedMoverController _controller;

		private readonly Direction _dir;

		public MoverDirInputCmdHandler(SharedMoverController controller, Direction dir)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			_controller = controller;
			_dir = dir;
		}

		public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Invalid comparison between Unknown and I4
			if (session == null || !session.AttachedEntity.HasValue)
			{
				return false;
			}
			_controller.HandleDirChange(session.AttachedEntity.Value, _dir, message.SubTick, (int)message.State == 1);
			return false;
		}
	}

	private sealed class WalkInputCmdHandler : InputCmdHandler
	{
		private SharedMoverController _controller;

		public WalkInputCmdHandler(SharedMoverController controller)
		{
			_controller = controller;
		}

		public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Invalid comparison between Unknown and I4
			if (session == null || !session.AttachedEntity.HasValue)
			{
				return false;
			}
			_controller.HandleRunChange(session.AttachedEntity.Value, message.SubTick, (int)message.State == 1);
			return false;
		}
	}

	private sealed class ShuttleInputCmdHandler : InputCmdHandler
	{
		private readonly SharedMoverController _controller;

		private readonly ShuttleButtons _button;

		public ShuttleInputCmdHandler(SharedMoverController controller, ShuttleButtons button)
		{
			_controller = controller;
			_button = button;
		}

		public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Invalid comparison between Unknown and I4
			if (session == null || !session.AttachedEntity.HasValue)
			{
				return false;
			}
			_controller.HandleShuttleInput(session.AttachedEntity.Value, _button, message.SubTick, (int)message.State == 1);
			return false;
		}
	}

	[Dependency]
	private IConfigurationManager _configManager;

	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	private ITileDefinitionManager _tileDefinitionManager;

	[Dependency]
	private ActionBlockerSystem _blocker;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private SharedGravitySystem _gravity;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TagSystem _tags;

	[Dependency]
	private FootstepCacheSystem _footstepCache;

	protected EntityQuery<CanMoveInAirComponent> CanMoveInAirQuery;

	protected EntityQuery<FootstepModifierComponent> FootstepModifierQuery;

	protected EntityQuery<FootstepVolumeModifierComponent> FootstepVolumeModifierQuery;

	protected EntityQuery<InputMoverComponent> MoverQuery;

	protected EntityQuery<MapComponent> MapQuery;

	protected EntityQuery<MapGridComponent> MapGridQuery;

	protected EntityQuery<MobMoverComponent> MobMoverQuery;

	protected EntityQuery<MovementRelayTargetComponent> RelayTargetQuery;

	protected EntityQuery<MovementSpeedModifierComponent> ModifierQuery;

	protected EntityQuery<NoRotateOnMoveComponent> NoRotateQuery;

	protected EntityQuery<PhysicsComponent> PhysicsQuery;

	protected EntityQuery<RelayInputMoverComponent> RelayQuery;

	protected EntityQuery<PullableComponent> PullableQuery;

	protected EntityQuery<TransformComponent> XformQuery;

	private static readonly ProtoId<TagPrototype> FootstepSoundTag = ProtoId<TagPrototype>.op_Implicit("FootstepSound");

	private bool _relativeMovement;

	private float _minDamping;

	private float _airDamping;

	private float _offGridDamping;

	public Dictionary<EntityUid, bool> UsedMobMovement = new Dictionary<EntityUid, bool>();

	public static ProtoId<AlertPrototype> WalkingAlert = ProtoId<AlertPrototype>.op_Implicit("Walking");

	public bool CameraRotationLocked { get; set; }

	public bool DiagonalMovementEnabled { get; private set; }

	public override void Initialize()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).UpdatesBefore.Add(typeof(TileFrictionController));
		((VirtualController)this).Initialize();
		MoverQuery = ((EntitySystem)this).GetEntityQuery<InputMoverComponent>();
		MobMoverQuery = ((EntitySystem)this).GetEntityQuery<MobMoverComponent>();
		ModifierQuery = ((EntitySystem)this).GetEntityQuery<MovementSpeedModifierComponent>();
		RelayTargetQuery = ((EntitySystem)this).GetEntityQuery<MovementRelayTargetComponent>();
		PhysicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		RelayQuery = ((EntitySystem)this).GetEntityQuery<RelayInputMoverComponent>();
		PullableQuery = ((EntitySystem)this).GetEntityQuery<PullableComponent>();
		XformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		NoRotateQuery = ((EntitySystem)this).GetEntityQuery<NoRotateOnMoveComponent>();
		CanMoveInAirQuery = ((EntitySystem)this).GetEntityQuery<CanMoveInAirComponent>();
		FootstepModifierQuery = ((EntitySystem)this).GetEntityQuery<FootstepModifierComponent>();
		FootstepVolumeModifierQuery = ((EntitySystem)this).GetEntityQuery<FootstepVolumeModifierComponent>();
		MapGridQuery = ((EntitySystem)this).GetEntityQuery<MapGridComponent>();
		MapQuery = ((EntitySystem)this).GetEntityQuery<MapComponent>();
		((EntitySystem)this).SubscribeLocalEvent<MovementSpeedModifierComponent, TileFrictionEvent>((EntityEventRefHandler<MovementSpeedModifierComponent, TileFrictionEvent>)OnTileFriction, (Type[])null, (Type[])null);
		InitializeInput();
		InitializeRelay();
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configManager, CCVars.RelativeMovement, (Action<bool>)delegate(bool value)
		{
			_relativeMovement = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.MinFriction, (Action<float>)delegate(float value)
		{
			_minDamping = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.AirFriction, (Action<float>)delegate(float value)
		{
			_airDamping = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.OffgridFriction, (Action<float>)delegate(float value)
		{
			_offGridDamping = value;
		}, true);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		ShutdownInput();
	}

	public override void UpdateAfterSolve(bool prediction, float frameTime)
	{
		((VirtualController)this).UpdateAfterSolve(prediction, frameTime);
		UsedMobMovement.Clear();
	}

	protected void HandleMobMovement(Entity<InputMoverComponent> entity, float frameTime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = entity.Owner;
		InputMoverComponent mover = entity.Comp;
		RelayInputMoverComponent relay = default(RelayInputMoverComponent);
		if (RelayQuery.TryComp(uid, ref relay))
		{
			InputMoverComponent relayTargetMover = default(InputMoverComponent);
			if (MoverQuery.TryComp(relay.RelayEntity, ref relayTargetMover))
			{
				LerpRotation(uid, mover, frameTime);
				bool dirtied = false;
				EntityUid? relativeEntity = relayTargetMover.RelativeEntity;
				EntityUid? relativeEntity2 = mover.RelativeEntity;
				if (relativeEntity.HasValue != relativeEntity2.HasValue || (relativeEntity.HasValue && relativeEntity.GetValueOrDefault() != relativeEntity2.GetValueOrDefault()))
				{
					relayTargetMover.RelativeEntity = mover.RelativeEntity;
					dirtied = true;
				}
				if (relayTargetMover.RelativeRotation != mover.RelativeRotation)
				{
					relayTargetMover.RelativeRotation = mover.RelativeRotation;
					dirtied = true;
				}
				if (relayTargetMover.TargetRelativeRotation != mover.TargetRelativeRotation)
				{
					relayTargetMover.TargetRelativeRotation = mover.TargetRelativeRotation;
					dirtied = true;
				}
				if (relayTargetMover.CanMove != mover.CanMove)
				{
					relayTargetMover.CanMove = mover.CanMove;
					dirtied = true;
				}
				if (dirtied)
				{
					((EntitySystem)this).Dirty(relay.RelayEntity, (IComponent)(object)relayTargetMover, (MetaDataComponent)null);
				}
			}
		}
		else
		{
			TransformComponent xform = default(TransformComponent);
			if (!XformQuery.TryComp(entity.Owner, ref xform))
			{
				return;
			}
			MovementRelayTargetComponent relayTarget = default(MovementRelayTargetComponent);
			RelayTargetQuery.TryComp(uid, ref relayTarget);
			EntityUid? relaySource = null;
			if (relayTarget != null && EnsureValidRelayTarget(uid, relayTarget))
			{
				relaySource = relayTarget.Source;
			}
			if (!relaySource.HasValue)
			{
				if (mover.LerpTarget < Timing.CurTime)
				{
					TryUpdateRelative(uid, mover, xform);
				}
				LerpRotation(uid, mover, frameTime);
			}
			PhysicsComponent physicsComponent = default(PhysicsComponent);
			PullableComponent pullable = default(PullableComponent);
			if (!mover.CanMove || !PhysicsQuery.TryComp(uid, ref physicsComponent) || (PullableQuery.TryGetComponent(uid, ref pullable) && pullable.BeingPulled))
			{
				UsedMobMovement[uid] = false;
				return;
			}
			bool weightless = _gravity.IsWeightless(uid, physicsComponent, xform);
			bool inAirHelpless = false;
			if ((int)physicsComponent.BodyStatus != 0 && !CanMoveInAirQuery.HasComponent(uid))
			{
				if (!weightless)
				{
					UsedMobMovement[uid] = false;
					return;
				}
				inAirHelpless = true;
			}
			UsedMobMovement[uid] = true;
			MovementSpeedModifierComponent moveSpeedComponent = ModifierQuery.CompOrNull(uid);
			Vector2 velocity = physicsComponent.LinearVelocity;
			ContentTileDefinition tileDef = null;
			bool touching = false;
			Vector2 wishDir;
			float accel;
			float friction;
			if (weightless || inAirHelpless)
			{
				float walkSpeed = moveSpeedComponent?.WeightlessWalkSpeed ?? 2.5f;
				float sprintSpeed = moveSpeedComponent?.WeightlessSprintSpeed ?? 4.5f;
				wishDir = AssertValidWish(mover, walkSpeed, sprintSpeed);
				CanWeightlessMoveEvent ev = new CanWeightlessMoveEvent(uid);
				((EntitySystem)this).RaiseLocalEvent<CanWeightlessMoveEvent>(uid, ref ev, true);
				touching = ev.CanMove || xform.GridUid.HasValue || MapGridQuery.HasComp(xform.GridUid);
				MobMoverComponent mobMover = default(MobMoverComponent);
				if (!touching && MobMoverQuery.TryComp(uid, ref mobMover))
				{
					touching |= IsAroundCollider(_lookup, Entity<PhysicsComponent, MobMoverComponent, TransformComponent>.op_Implicit((uid, physicsComponent, mobMover, xform)));
				}
				if (touching)
				{
					touching = true;
					friction = ((!(wishDir != Vector2.Zero)) ? (moveSpeedComponent?.WeightlessFrictionNoInput ?? _airDamping) : (moveSpeedComponent?.WeightlessFriction ?? _airDamping));
				}
				else
				{
					friction = moveSpeedComponent?.OffGridFriction ?? _offGridDamping;
				}
				accel = moveSpeedComponent?.WeightlessAcceleration ?? 1f;
			}
			else
			{
				MapGridComponent gridComp = default(MapGridComponent);
				TileRef tile = default(TileRef);
				if (MapGridQuery.TryComp(xform.GridUid, ref gridComp) && _mapSystem.TryGetTileRef(xform.GridUid.Value, gridComp, xform.Coordinates, ref tile) && (int)physicsComponent.BodyStatus == 0)
				{
					tileDef = (ContentTileDefinition)(object)_tileDefinitionManager[tile.Tile.TypeId];
				}
				float walkSpeed2 = moveSpeedComponent?.CurrentWalkSpeed ?? 2.5f;
				float sprintSpeed2 = moveSpeedComponent?.CurrentSprintSpeed ?? 4.5f;
				wishDir = AssertValidWish(mover, walkSpeed2, sprintSpeed2);
				if (wishDir != Vector2.Zero)
				{
					friction = moveSpeedComponent?.Friction ?? 2.5f;
					friction *= tileDef?.MobFriction ?? tileDef?.Friction ?? 1f;
				}
				else
				{
					friction = moveSpeedComponent?.FrictionNoInput ?? 2.5f;
					friction *= tileDef?.Friction ?? 1f;
				}
				accel = moveSpeedComponent?.Acceleration ?? 20f;
				accel *= tileDef?.MobAcceleration ?? 1f;
			}
			if (wishDir != Vector2.Zero)
			{
				friction = Math.Min(friction, accel);
			}
			friction = Math.Max(friction, _minDamping);
			float minimumFrictionSpeed = moveSpeedComponent?.MinimumFrictionSpeed ?? 0.005f;
			Friction(minimumFrictionSpeed, frameTime, friction, ref velocity);
			if (!weightless || touching)
			{
				Accelerate(ref velocity, in wishDir, accel, frameTime);
			}
			SetWishDir(Entity<InputMoverComponent>.op_Implicit((uid, mover)), wishDir);
			base.PhysicsSystem.SetLinearVelocity(uid, velocity, true, true, (FixturesComponent)null, physicsComponent);
			base.PhysicsSystem.SetAngularVelocity(uid, 0f, true, (FixturesComponent)null, physicsComponent);
			if (!(wishDir != Vector2.Zero))
			{
				return;
			}
			if (!NoRotateQuery.HasComponent(uid))
			{
				Angle worldRot = _transform.GetWorldRotation(xform);
				_transform.SetLocalRotation(uid, xform.LocalRotation + DirectionExtensions.ToWorldAngle(wishDir) - worldRot, xform);
			}
			MobMoverComponent mobMover2 = default(MobMoverComponent);
			if (weightless || !MobMoverQuery.TryGetComponent(uid, ref mobMover2) || !TryGetSound(weightless, uid, mover, mobMover2, xform, out SoundSpecifier sound, tileDef))
			{
				return;
			}
			float soundModifier = (mover.Sprinting ? 3.5f : 1.5f);
			AudioParams val = sound.Params;
			float volume = ((AudioParams)(ref val)).Volume + soundModifier;
			float? maxDistance = null;
			FootstepVolumeModifierComponent volumeModifier = default(FootstepVolumeModifierComponent);
			if (FootstepVolumeModifierQuery.TryGetComponent(uid, ref volumeModifier))
			{
				volume += (mover.Sprinting ? volumeModifier.SprintVolumeModifier : volumeModifier.WalkVolumeModifier);
				float configuredMaxDistance = (mover.Sprinting ? volumeModifier.SprintMaxDistance : volumeModifier.WalkMaxDistance);
				if (configuredMaxDistance > 0f)
				{
					maxDistance = configuredMaxDistance;
				}
			}
			val = sound.Params;
			val = ((AudioParams)(ref val)).WithVolume(volume);
			AudioParams val2 = sound.Params;
			AudioParams audioParams = ((AudioParams)(ref val)).WithVariation((float?)(((AudioParams)(ref val2)).Variation ?? mobMover2.FootstepVariation));
			if (maxDistance.HasValue)
			{
				audioParams = ((AudioParams)(ref audioParams)).WithMaxDistance(maxDistance.Value);
			}
			if (relaySource.HasValue)
			{
				_audio.PlayPredicted(sound, uid, (EntityUid?)relaySource.Value, (AudioParams?)audioParams);
			}
			else
			{
				_audio.PlayPredicted(sound, uid, (EntityUid?)uid, (AudioParams?)audioParams);
			}
		}
	}

	public Vector2 GetWishDir(Entity<InputMoverComponent?> mover)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!MoverQuery.Resolve(mover.Owner, ref mover.Comp, false))
		{
			return Vector2.Zero;
		}
		return mover.Comp.WishDir;
	}

	public void SetWishDir(Entity<InputMoverComponent> mover, Vector2 wishDir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!mover.Comp.WishDir.Equals(wishDir))
		{
			mover.Comp.WishDir = wishDir;
			((EntitySystem)this).Dirty<InputMoverComponent>(mover, (MetaDataComponent)null);
		}
	}

	public void LerpRotation(EntityUid uid, InputMoverComponent mover, float frameTime)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		Angle angleDiff = Angle.ShortestDistance(ref mover.RelativeRotation, ref mover.TargetRelativeRotation);
		if (!((Angle)(ref angleDiff)).EqualsApprox(Angle.Zero, 0.001))
		{
			double adjustment = Angle.op_Implicit(angleDiff) * 5.0 * (double)frameTime;
			double minAdjustment = 0.01 * (double)frameTime;
			if (Angle.op_Implicit(angleDiff) < 0.0)
			{
				adjustment = Math.Min(adjustment, 0.0 - minAdjustment);
				adjustment = Math.Clamp(adjustment, Angle.op_Implicit(angleDiff), Angle.op_Implicit(-angleDiff));
			}
			else
			{
				adjustment = Math.Max(adjustment, minAdjustment);
				adjustment = Math.Clamp(adjustment, Angle.op_Implicit(-angleDiff), Angle.op_Implicit(angleDiff));
			}
			Angle val = mover.RelativeRotation + Angle.op_Implicit(adjustment);
			mover.RelativeRotation = ((Angle)(ref val)).FlipPositive();
			((EntitySystem)this).Dirty(uid, (IComponent)(object)mover, (MetaDataComponent)null);
		}
		else if (!((Angle)(ref angleDiff)).Equals(Angle.Zero))
		{
			mover.RelativeRotation = ((Angle)(ref mover.TargetRelativeRotation)).FlipPositive();
			((EntitySystem)this).Dirty(uid, (IComponent)(object)mover, (MetaDataComponent)null);
		}
	}

	public void Friction(float minimumFrictionSpeed, float frameTime, float friction, ref Vector2 velocity)
	{
		if (!(velocity.Length() < minimumFrictionSpeed))
		{
			velocity *= Math.Clamp(1f - frameTime * friction, 0f, 1f);
		}
	}

	public void Friction(float minimumFrictionSpeed, float frameTime, float friction, ref float velocity)
	{
		if (!(Math.Abs(velocity) < minimumFrictionSpeed))
		{
			velocity *= Math.Clamp(1f - frameTime * friction, 0f, 1f);
		}
	}

	public static void Accelerate(ref Vector2 currentVelocity, in Vector2 velocity, float accel, float frameTime)
	{
		Vector2 wishDir = ((velocity != Vector2.Zero) ? Vector2Helpers.Normalized(velocity) : Vector2.Zero);
		float wishSpeed = velocity.Length();
		float currentSpeed = Vector2.Dot(currentVelocity, wishDir);
		float addSpeed = wishSpeed - currentSpeed;
		if (!(addSpeed <= 0f))
		{
			float accelSpeed = accel * frameTime * wishSpeed;
			accelSpeed = MathF.Min(accelSpeed, addSpeed);
			currentVelocity += wishDir * accelSpeed;
		}
	}

	public bool UseMobMovement(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		bool used;
		return UsedMobMovement.TryGetValue(uid, out used) && used;
	}

	private bool IsAroundCollider(EntityLookupSystem lookupSystem, Entity<PhysicsComponent, MobMoverComponent, TransformComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Invalid comparison between Unknown and I4
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		Entity<PhysicsComponent, MobMoverComponent, TransformComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		PhysicsComponent val3 = default(PhysicsComponent);
		MobMoverComponent mobMoverComponent = default(MobMoverComponent);
		TransformComponent val4 = default(TransformComponent);
		val.Deconstruct(ref val2, ref val3, ref mobMoverComponent, ref val4);
		EntityUid uid = val2;
		PhysicsComponent collider = val3;
		MobMoverComponent mover = mobMoverComponent;
		TransformComponent transform = val4;
		Box2 worldAABB = _lookup.GetWorldAABB(entity.Owner, transform);
		Box2 enlargedAABB = ((Box2)(ref worldAABB)).Enlarged(mover.GrabRange);
		PhysicsComponent otherCollider = default(PhysicsComponent);
		PullableComponent pullable = default(PullableComponent);
		foreach (EntityUid otherEntity in lookupSystem.GetEntitiesIntersecting(transform.MapID, enlargedAABB, (LookupFlags)110))
		{
			if (!(otherEntity == uid) && PhysicsQuery.TryComp(otherEntity, ref otherCollider) && (int)otherCollider.BodyType == 4 && otherCollider.CanCollide && ((collider.CollisionMask & otherCollider.CollisionLayer) != 0 || (otherCollider.CollisionMask & collider.CollisionLayer) != 0) && (!((EntitySystem)this).TryComp<PullableComponent>(otherEntity, ref pullable) || !pullable.BeingPulled))
			{
				return true;
			}
		}
		return false;
	}

	protected abstract bool CanSound();

	private bool TryGetSound(bool weightless, EntityUid uid, InputMoverComponent mover, MobMoverComponent mobMover, TransformComponent xform, [NotNullWhen(true)] out SoundSpecifier? sound, ContentTileDefinition? tileDef = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		sound = null;
		if (!CanSound() || !_tags.HasTag(uid, FootstepSoundTag))
		{
			return false;
		}
		EntityCoordinates coordinates = xform.Coordinates;
		float distanceNeeded = (mover.Sprinting ? mobMover.StepSoundMoveDistanceRunning : mobMover.StepSoundMoveDistanceWalking);
		if (!weightless)
		{
			float distance = default(float);
			if (!((EntityCoordinates)(ref coordinates)).TryDistance((IEntityManager)(object)((EntitySystem)this).EntityManager, mobMover.LastPosition, ref distance) || distance > distanceNeeded)
			{
				mobMover.StepSoundDistance = distanceNeeded;
			}
			else
			{
				mobMover.StepSoundDistance += distance;
			}
			mobMover.LastPosition = coordinates;
			if (mobMover.StepSoundDistance < distanceNeeded)
			{
				return false;
			}
			mobMover.StepSoundDistance -= distanceNeeded;
			FootstepModifierComponent moverModifier = default(FootstepModifierComponent);
			if (FootstepModifierQuery.TryComp(uid, ref moverModifier))
			{
				sound = moverModifier.FootstepSoundCollection;
				return sound != null;
			}
			FootstepModifierComponent modifier = default(FootstepModifierComponent);
			if (_inventory.TryGetSlotEntity(uid, "shoes", out var shoes) && FootstepModifierQuery.TryComp(shoes, ref modifier))
			{
				sound = modifier.FootstepSoundCollection;
				return sound != null;
			}
			return TryGetFootstepSound(uid, xform, shoes.HasValue, out sound, tileDef);
		}
		return false;
	}

	private bool TryGetFootstepSound(EntityUid uid, TransformComponent xform, bool haveShoes, [NotNullWhen(true)] out SoundSpecifier? sound, ContentTileDefinition? tileDef = null)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		sound = null;
		MapGridComponent grid = default(MapGridComponent);
		if (!MapGridQuery.TryComp(xform.GridUid, ref grid))
		{
			FootstepModifierComponent modifier = default(FootstepModifierComponent);
			if (FootstepModifierQuery.TryComp(xform.MapUid, ref modifier))
			{
				sound = modifier.FootstepSoundCollection;
			}
			return sound != null;
		}
		Vector2i position = _mapSystem.LocalToTile(xform.GridUid.Value, grid, xform.Coordinates);
		if (_footstepCache.TryGetCachedSound(uid, xform.GridUid.Value, position, out sound))
		{
			return sound != null;
		}
		GetFootstepSoundEvent soundEv = new GetFootstepSoundEvent(uid);
		AnchoredEntitiesEnumerator anchored = _mapSystem.GetAnchoredEntitiesEnumerator(xform.GridUid.Value, grid, position);
		EntityUid? firstEntity = default(EntityUid?);
		if (!((AnchoredEntitiesEnumerator)(ref anchored)).MoveNext(ref firstEntity))
		{
			TileRef emptyTileRef = default(TileRef);
			if (tileDef == null && _mapSystem.TryGetTileRef(xform.GridUid.Value, grid, position, ref emptyTileRef))
			{
				tileDef = (ContentTileDefinition)(object)_tileDefinitionManager[emptyTileRef.Tile.TypeId];
			}
			if (tileDef != null)
			{
				sound = (haveShoes ? tileDef.FootstepSounds : tileDef.BarestepSounds);
				_footstepCache.SetCachedSound(uid, xform.GridUid.Value, position, sound);
			}
			return sound != null;
		}
		((EntitySystem)this).RaiseLocalEvent<GetFootstepSoundEvent>(firstEntity.Value, ref soundEv, false);
		if (soundEv.Sound != null)
		{
			sound = soundEv.Sound;
			_footstepCache.SetCachedSound(uid, xform.GridUid.Value, position, sound);
			return true;
		}
		FootstepModifierComponent firstFootstep = default(FootstepModifierComponent);
		if (FootstepModifierQuery.TryComp(firstEntity, ref firstFootstep))
		{
			sound = firstFootstep.FootstepSoundCollection;
			_footstepCache.SetCachedSound(uid, xform.GridUid.Value, position, sound);
			return sound != null;
		}
		EntityUid? maybeFootstep = default(EntityUid?);
		FootstepModifierComponent footstep = default(FootstepModifierComponent);
		while (((AnchoredEntitiesEnumerator)(ref anchored)).MoveNext(ref maybeFootstep))
		{
			((EntitySystem)this).RaiseLocalEvent<GetFootstepSoundEvent>(maybeFootstep.Value, ref soundEv, false);
			if (soundEv.Sound != null)
			{
				sound = soundEv.Sound;
				_footstepCache.SetCachedSound(uid, xform.GridUid.Value, position, sound);
				return true;
			}
			if (FootstepModifierQuery.TryComp(maybeFootstep, ref footstep))
			{
				sound = footstep.FootstepSoundCollection;
				_footstepCache.SetCachedSound(uid, xform.GridUid.Value, position, sound);
				return sound != null;
			}
		}
		TileRef tileRef = default(TileRef);
		if (tileDef == null && _mapSystem.TryGetTileRef(xform.GridUid.Value, grid, position, ref tileRef))
		{
			tileDef = (ContentTileDefinition)(object)_tileDefinitionManager[tileRef.Tile.TypeId];
		}
		if (tileDef == null)
		{
			return false;
		}
		sound = (haveShoes ? tileDef.FootstepSounds : tileDef.BarestepSounds);
		_footstepCache.SetCachedSound(uid, xform.GridUid.Value, position, sound);
		return sound != null;
	}

	private Vector2 AssertValidWish(InputMoverComponent mover, float walkSpeed, float sprintSpeed)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		(Vector2 Walking, Vector2 Sprinting) velocityInput = GetVelocityInput(mover);
		Vector2 walkDir = velocityInput.Walking;
		Vector2 sprintDir = velocityInput.Sprinting;
		Vector2 total = walkDir * walkSpeed + sprintDir * sprintSpeed;
		Angle parentRotation = GetParentGridAngle(mover);
		if (!_relativeMovement)
		{
			return total;
		}
		return ((Angle)(ref parentRotation)).RotateVec(ref total);
	}

	private void OnTileFriction(Entity<MovementSpeedModifierComponent> ent, ref TileFrictionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physicsComponent = default(PhysicsComponent);
		TransformComponent xform = default(TransformComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(Entity<MovementSpeedModifierComponent>.op_Implicit(ent), ref physicsComponent) && XformQuery.TryComp(Entity<MovementSpeedModifierComponent>.op_Implicit(ent), ref xform))
		{
			if ((int)physicsComponent.BodyStatus != 0 || _gravity.IsWeightless(Entity<MovementSpeedModifierComponent>.op_Implicit(ent), physicsComponent, xform))
			{
				args.Modifier *= ent.Comp.BaseWeightlessFriction;
			}
			else
			{
				args.Modifier *= ent.Comp.BaseFriction;
			}
		}
	}

	private void InitializeInput()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		MoverDirInputCmdHandler moveUpCmdHandler = new MoverDirInputCmdHandler(this, (Direction)4);
		MoverDirInputCmdHandler moveLeftCmdHandler = new MoverDirInputCmdHandler(this, (Direction)6);
		MoverDirInputCmdHandler moveRightCmdHandler = new MoverDirInputCmdHandler(this, (Direction)2);
		MoverDirInputCmdHandler moveDownCmdHandler = new MoverDirInputCmdHandler(this, (Direction)0);
		CommandBinds.Builder.Bind(EngineKeyFunctions.MoveUp, (InputCmdHandler)(object)moveUpCmdHandler).Bind(EngineKeyFunctions.MoveLeft, (InputCmdHandler)(object)moveLeftCmdHandler).Bind(EngineKeyFunctions.MoveRight, (InputCmdHandler)(object)moveRightCmdHandler)
			.Bind(EngineKeyFunctions.MoveDown, (InputCmdHandler)(object)moveDownCmdHandler)
			.Bind(EngineKeyFunctions.Walk, (InputCmdHandler)(object)new WalkInputCmdHandler(this))
			.Bind(EngineKeyFunctions.CameraRotateLeft, (InputCmdHandler)(object)new CameraRotateInputCmdHandler(this, (Direction)2))
			.Bind(EngineKeyFunctions.CameraRotateRight, (InputCmdHandler)(object)new CameraRotateInputCmdHandler(this, (Direction)6))
			.Bind(EngineKeyFunctions.CameraReset, (InputCmdHandler)(object)new CameraResetInputCmdHandler(this))
			.Bind(ContentKeyFunctions.ShuttleStrafeUp, (InputCmdHandler)(object)new ShuttleInputCmdHandler(this, ShuttleButtons.StrafeUp))
			.Bind(ContentKeyFunctions.ShuttleStrafeLeft, (InputCmdHandler)(object)new ShuttleInputCmdHandler(this, ShuttleButtons.StrafeLeft))
			.Bind(ContentKeyFunctions.ShuttleStrafeRight, (InputCmdHandler)(object)new ShuttleInputCmdHandler(this, ShuttleButtons.StrafeRight))
			.Bind(ContentKeyFunctions.ShuttleStrafeDown, (InputCmdHandler)(object)new ShuttleInputCmdHandler(this, ShuttleButtons.StrafeDown))
			.Bind(ContentKeyFunctions.ShuttleRotateLeft, (InputCmdHandler)(object)new ShuttleInputCmdHandler(this, ShuttleButtons.RotateLeft))
			.Bind(ContentKeyFunctions.ShuttleRotateRight, (InputCmdHandler)(object)new ShuttleInputCmdHandler(this, ShuttleButtons.RotateRight))
			.Bind(ContentKeyFunctions.ShuttleBrake, (InputCmdHandler)(object)new ShuttleInputCmdHandler(this, ShuttleButtons.Brake))
			.Register<SharedMoverController>();
		((EntitySystem)this).SubscribeLocalEvent<InputMoverComponent, ComponentInit>((EntityEventRefHandler<InputMoverComponent, ComponentInit>)OnInputInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InputMoverComponent, ComponentGetState>((EntityEventRefHandler<InputMoverComponent, ComponentGetState>)OnMoverGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InputMoverComponent, ComponentHandleState>((EntityEventRefHandler<InputMoverComponent, ComponentHandleState>)OnMoverHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InputMoverComponent, EntParentChangedMessage>((EntityEventRefHandler<InputMoverComponent, EntParentChangedMessage>)OnInputParentChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FollowedComponent, EntParentChangedMessage>((EntityEventRefHandler<FollowedComponent, EntParentChangedMessage>)OnFollowedParentChange, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configManager, CCVars.CameraRotationLocked, (Action<bool>)delegate(bool obj)
		{
			CameraRotationLocked = obj;
		}, true);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configManager, CCVars.GameDiagonalMovement, (Action<bool>)delegate(bool value)
		{
			DiagonalMovementEnabled = value;
		}, true);
	}

	public static MoveButtons GetNormalizedMovement(MoveButtons buttons)
	{
		MoveButtons oldMovement = buttons;
		if ((oldMovement & (MoveButtons.Left | MoveButtons.Right)) == (MoveButtons.Left | MoveButtons.Right))
		{
			oldMovement &= ~MoveButtons.Left;
			oldMovement &= ~MoveButtons.Right;
		}
		if ((oldMovement & (MoveButtons.Up | MoveButtons.Down)) == (MoveButtons.Up | MoveButtons.Down))
		{
			oldMovement &= ~MoveButtons.Up;
			oldMovement &= ~MoveButtons.Down;
		}
		return oldMovement;
	}

	protected void SetMoveInput(Entity<InputMoverComponent> entity, MoveButtons buttons)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.HeldMoveButtons != buttons)
		{
			MoveInputEvent moveEvent = new MoveInputEvent(entity, entity.Comp.HeldMoveButtons);
			entity.Comp.HeldMoveButtons = buttons;
			((EntitySystem)this).RaiseLocalEvent<MoveInputEvent>(Entity<InputMoverComponent>.op_Implicit(entity), ref moveEvent, false);
			((EntitySystem)this).Dirty(Entity<InputMoverComponent>.op_Implicit(entity), (IComponent)(object)entity.Comp, (MetaDataComponent)null);
			SpriteMoveEvent ev = new SpriteMoveEvent(entity.Comp.HasDirectionalMovement);
			((EntitySystem)this).RaiseLocalEvent<SpriteMoveEvent>(Entity<InputMoverComponent>.op_Implicit(entity), ref ev, false);
		}
	}

	private void OnMoverHandleState(Entity<InputMoverComponent> entity, ref ComponentHandleState args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is InputMoverComponentState state)
		{
			entity.Comp.LerpTarget = state.LerpTarget;
			entity.Comp.RelativeRotation = state.RelativeRotation;
			entity.Comp.TargetRelativeRotation = state.TargetRelativeRotation;
			entity.Comp.CanMove = state.CanMove;
			entity.Comp.RelativeEntity = ((EntitySystem)this).EnsureEntity<InputMoverComponent>(state.RelativeEntity, entity.Owner);
			entity.Comp.LastInputTick = GameTick.Zero;
			entity.Comp.LastInputSubTick = 0;
			if (entity.Comp.HeldMoveButtons != state.HeldMoveButtons)
			{
				MoveInputEvent moveEvent = new MoveInputEvent(entity, entity.Comp.HeldMoveButtons);
				entity.Comp.HeldMoveButtons = state.HeldMoveButtons;
				((EntitySystem)this).RaiseLocalEvent<MoveInputEvent>(entity.Owner, ref moveEvent, false);
				SpriteMoveEvent ev = new SpriteMoveEvent(entity.Comp.HasDirectionalMovement);
				((EntitySystem)this).RaiseLocalEvent<SpriteMoveEvent>(Entity<InputMoverComponent>.op_Implicit(entity), ref ev, false);
			}
		}
	}

	private void OnMoverGetState(Entity<InputMoverComponent> entity, ref ComponentGetState args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new InputMoverComponentState
		{
			CanMove = entity.Comp.CanMove,
			RelativeEntity = ((EntitySystem)this).GetNetEntity(entity.Comp.RelativeEntity, (MetaDataComponent)null),
			LerpTarget = entity.Comp.LerpTarget,
			HeldMoveButtons = entity.Comp.HeldMoveButtons,
			RelativeRotation = entity.Comp.RelativeRotation,
			TargetRelativeRotation = entity.Comp.TargetRelativeRotation
		};
	}

	private void ShutdownInput()
	{
		CommandBinds.Unregister<SharedMoverController>();
	}

	protected virtual void HandleShuttleInput(EntityUid uid, ShuttleButtons button, ushort subTick, bool state)
	{
	}

	public void RotateCamera(EntityUid uid, Angle angle)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		InputMoverComponent mover = default(InputMoverComponent);
		if (!CameraRotationLocked && MoverQuery.TryGetComponent(uid, ref mover))
		{
			InputMoverComponent inputMoverComponent = mover;
			inputMoverComponent.TargetRelativeRotation += angle;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)mover, (MetaDataComponent)null);
		}
	}

	public void ResetCamera(EntityUid uid)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		InputMoverComponent mover = default(InputMoverComponent);
		if (!CameraRotationLocked && MoverQuery.TryGetComponent(uid, ref mover) && (TryUpdateRelative(uid, mover, XformQuery.GetComponent(uid)) || !((Angle)(ref mover.TargetRelativeRotation)).Equals(Angle.Zero)))
		{
			mover.LerpTarget = TimeSpan.Zero;
			mover.TargetRelativeRotation = Angle.Zero;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)mover, (MetaDataComponent)null);
		}
	}

	private bool TryUpdateRelative(EntityUid uid, InputMoverComponent mover, TransformComponent xform)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? relative = xform.GridUid;
		EntityUid? val = relative;
		if (!val.HasValue)
		{
			relative = xform.MapUid;
		}
		if (mover.RelativeEntity.Equals(relative))
		{
			return false;
		}
		Angle oldRelativeRot = Angle.Zero;
		Angle relativeRot = Angle.Zero;
		TransformComponent oldRelativeXform = default(TransformComponent);
		if (XformQuery.TryGetComponent(mover.RelativeEntity, ref oldRelativeXform))
		{
			oldRelativeRot = _transform.GetWorldRotation(oldRelativeXform);
		}
		TransformComponent relativeXform = default(TransformComponent);
		if (XformQuery.TryGetComponent(relative, ref relativeXform))
		{
			relativeRot = _transform.GetWorldRotation(relativeXform);
		}
		Angle diff = relativeRot - oldRelativeRot;
		if (MapQuery.HasComp(relative) && MapGridQuery.HasComp(mover.RelativeEntity))
		{
			mover.TargetRelativeRotation -= diff;
		}
		else if (MapGridQuery.HasComp(relative) && (MapQuery.HasComp(mover.RelativeEntity) || MapGridQuery.HasComp(mover.RelativeEntity)))
		{
			Angle targetDir = mover.TargetRelativeRotation - diff;
			Angle val2 = DirectionExtensions.ToAngle(((Angle)(ref targetDir)).GetCardinalDir());
			targetDir = (mover.TargetRelativeRotation = ((Angle)(ref val2)).Reduced());
		}
		mover.RelativeRotation -= diff;
		mover.RelativeEntity = relative;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)mover, (MetaDataComponent)null);
		return true;
	}

	public Angle GetParentGridAngle(InputMoverComponent mover)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Angle rotation = mover.RelativeRotation;
		TransformComponent relativeXform = default(TransformComponent);
		if (XformQuery.TryGetComponent(mover.RelativeEntity, ref relativeXform))
		{
			return _transform.GetWorldRotation(relativeXform) + rotation;
		}
		return rotation;
	}

	private void OnFollowedParentChange(Entity<FollowedComponent> entity, ref EntParentChangedMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		InputMoverComponent mover = default(InputMoverComponent);
		EntParentChangedMessage ev = default(EntParentChangedMessage);
		foreach (EntityUid foll in entity.Comp.Following)
		{
			if (MoverQuery.TryGetComponent(foll, ref mover))
			{
				((EntParentChangedMessage)(ref ev))._002Ector(foll, (EntityUid?)null, args.OldMapId, XformQuery.GetComponent(foll));
				OnInputParentChange(Entity<InputMoverComponent>.op_Implicit((foll, mover)), ref ev);
			}
		}
	}

	private void OnInputParentChange(Entity<InputMoverComponent> entity, ref EntParentChangedMessage args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? relative = ((EntParentChangedMessage)(ref args)).Transform.GridUid;
		EntityUid? val = relative;
		if (!val.HasValue)
		{
			relative = ((EntParentChangedMessage)(ref args)).Transform.MapUid;
		}
		if ((int)((Component)entity.Comp).LifeStage < 6)
		{
			entity.Comp.RelativeEntity = relative;
			((EntitySystem)this).Dirty(entity.Owner, (IComponent)(object)entity.Comp, (MetaDataComponent)null);
			return;
		}
		EntityUid? oldMapId = args.OldMapId;
		EntityUid? mapUid = ((EntParentChangedMessage)(ref args)).Transform.MapUid;
		val = oldMapId;
		EntityUid? val2 = mapUid;
		if (val.HasValue != val2.HasValue || (val.HasValue && val.GetValueOrDefault() != val2.GetValueOrDefault()))
		{
			entity.Comp.RelativeEntity = relative;
			entity.Comp.TargetRelativeRotation = Angle.Zero;
			entity.Comp.RelativeRotation = Angle.Zero;
			entity.Comp.LerpTarget = TimeSpan.Zero;
			((EntitySystem)this).Dirty(entity.Owner, (IComponent)(object)entity.Comp, (MetaDataComponent)null);
			return;
		}
		val2 = relative;
		val = entity.Comp.RelativeEntity;
		if (val2.HasValue == val.HasValue && (!val2.HasValue || val2.GetValueOrDefault() == val.GetValueOrDefault()))
		{
			if (entity.Comp.LerpTarget >= Timing.CurTime)
			{
				entity.Comp.LerpTarget = TimeSpan.Zero;
				((EntitySystem)this).Dirty(entity.Owner, (IComponent)(object)entity.Comp, (MetaDataComponent)null);
			}
		}
		else
		{
			entity.Comp.LerpTarget = TimeSpan.FromSeconds(1.0) + Timing.CurTime;
			((EntitySystem)this).Dirty(entity.Owner, (IComponent)(object)entity.Comp, (MetaDataComponent)null);
		}
	}

	private void HandleDirChange(EntityUid entity, Direction dir, ushort subTick, bool state)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		RelayInputMoverComponent relayMover = default(RelayInputMoverComponent);
		if (((EntitySystem)this).TryComp<RelayInputMoverComponent>(entity, ref relayMover))
		{
			InputMoverComponent mover = default(InputMoverComponent);
			if (MoverQuery.TryGetComponent(entity, ref mover))
			{
				SetMoveInput(Entity<InputMoverComponent>.op_Implicit((entity, mover)), MoveButtons.None);
			}
			if (!_mobState.IsIncapacitated(entity))
			{
				HandleDirChange(relayMover.RelayEntity, dir, subTick, state);
			}
		}
		else
		{
			InputMoverComponent moverComp = default(InputMoverComponent);
			if (!MoverQuery.TryGetComponent(entity, ref moverComp))
			{
				return;
			}
			TransformComponent xform = default(TransformComponent);
			if (_container.IsEntityInContainer(entity, (MetaDataComponent)null) && ((EntitySystem)this).TryComp(entity, ref xform))
			{
				EntityUid parentUid = xform.ParentUid;
				if (((EntityUid)(ref parentUid)).IsValid() && _mobState.IsAlive(entity))
				{
					ContainerRelayMovementEntityEvent relayMoveEvent = new ContainerRelayMovementEntityEvent(entity);
					((EntitySystem)this).RaiseLocalEvent<ContainerRelayMovementEntityEvent>(xform.ParentUid, ref relayMoveEvent, false);
				}
			}
			SetVelocityDirection(Entity<InputMoverComponent>.op_Implicit((entity, moverComp)), dir, subTick, state);
		}
	}

	private void OnInputInit(Entity<InputMoverComponent> entity, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = ((EntitySystem)this).Transform(entity.Owner);
		EntityUid parentUid = xform.ParentUid;
		if (((EntityUid)(ref parentUid)).IsValid())
		{
			entity.Comp.RelativeEntity = xform.GridUid ?? xform.MapUid;
			entity.Comp.TargetRelativeRotation = Angle.Zero;
		}
	}

	private void HandleRunChange(EntityUid uid, ushort subTick, bool walking)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		InputMoverComponent moverComp = default(InputMoverComponent);
		MoverQuery.TryGetComponent(uid, ref moverComp);
		RelayInputMoverComponent relayMover = default(RelayInputMoverComponent);
		if (((EntitySystem)this).TryComp<RelayInputMoverComponent>(uid, ref relayMover))
		{
			if (moverComp != null)
			{
				SetMoveInput(Entity<InputMoverComponent>.op_Implicit((uid, moverComp)), MoveButtons.None);
			}
			HandleRunChange(relayMover.RelayEntity, subTick, walking);
		}
		else if (moverComp != null)
		{
			SetSprinting(Entity<InputMoverComponent>.op_Implicit((uid, moverComp)), subTick, walking);
		}
	}

	public (Vector2 Walking, Vector2 Sprinting) GetVelocityInput(InputMoverComponent mover)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (!Timing.InSimulation)
		{
			Vector2 immediateDir = DirVecForButtons(mover.HeldMoveButtons);
			if (!mover.Sprinting)
			{
				return (Walking: immediateDir, Sprinting: Vector2.Zero);
			}
			return (Walking: Vector2.Zero, Sprinting: immediateDir);
		}
		Vector2 walk;
		Vector2 sprint;
		float remainingFraction;
		if (Timing.CurTick > mover.LastInputTick)
		{
			walk = Vector2.Zero;
			sprint = Vector2.Zero;
			remainingFraction = 1f;
		}
		else
		{
			walk = mover.CurTickWalkMovement;
			sprint = mover.CurTickSprintMovement;
			remainingFraction = (float)(65535 - mover.LastInputSubTick) / 65535f;
		}
		Vector2 curDir = DirVecForButtons(mover.HeldMoveButtons) * remainingFraction;
		if (mover.Sprinting)
		{
			sprint += curDir;
		}
		else
		{
			walk += curDir;
		}
		return (Walking: walk, Sprinting: sprint);
	}

	public void SetVelocityDirection(Entity<InputMoverComponent> entity, Direction direction, ushort subTick, bool enabled)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected I4, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		SetMoveInput(entity, subTick, enabled, (int)direction switch
		{
			2 => MoveButtons.Right, 
			4 => MoveButtons.Up, 
			6 => MoveButtons.Left, 
			0 => MoveButtons.Down, 
			_ => throw new ArgumentException("direction"), 
		});
	}

	private void SetMoveInput(Entity<InputMoverComponent> entity, ushort subTick, bool enabled, MoveButtons bit)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		ResetSubtick(entity.Comp);
		if (subTick >= entity.Comp.LastInputSubTick)
		{
			float fraction = (float)(subTick - entity.Comp.LastInputSubTick) / 65535f;
			entity.Comp.Sprinting ? ref entity.Comp.CurTickSprintMovement : ref entity.Comp.CurTickWalkMovement += DirVecForButtons(entity.Comp.HeldMoveButtons) * fraction;
			entity.Comp.LastInputSubTick = subTick;
		}
		MoveButtons buttons = entity.Comp.HeldMoveButtons;
		buttons = ((!enabled) ? ((MoveButtons)((uint)buttons & (uint)(byte)(~(int)bit))) : (buttons | bit));
		SetMoveInput(entity, buttons);
	}

	private void ResetSubtick(InputMoverComponent component)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!(Timing.CurTick <= component.LastInputTick))
		{
			component.CurTickWalkMovement = Vector2.Zero;
			component.CurTickSprintMovement = Vector2.Zero;
			component.LastInputTick = Timing.CurTick;
			component.LastInputSubTick = 0;
		}
	}

	public virtual void SetSprinting(Entity<InputMoverComponent> entity, ushort subTick, bool walking)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetMoveInput(entity, subTick, walking, MoveButtons.Walk);
	}

	public Vector2 DirVecForButtons(MoveButtons buttons)
	{
		int x = 0;
		x -= (HasFlag(buttons, MoveButtons.Left) ? 1 : 0);
		x += (HasFlag(buttons, MoveButtons.Right) ? 1 : 0);
		int y = 0;
		if (DiagonalMovementEnabled || x == 0)
		{
			y -= (HasFlag(buttons, MoveButtons.Down) ? 1 : 0);
			y += (HasFlag(buttons, MoveButtons.Up) ? 1 : 0);
		}
		Vector2 vec = new Vector2(x, y);
		if ((double)vec.LengthSquared() > 1E-06)
		{
			return Vector2Helpers.Normalized(vec);
		}
		return vec;
	}

	private static bool HasFlag(MoveButtons buttons, MoveButtons flag)
	{
		return (buttons & flag) == flag;
	}

	private void InitializeRelay()
	{
		((EntitySystem)this).SubscribeLocalEvent<RelayInputMoverComponent, ComponentShutdown>((EntityEventRefHandler<RelayInputMoverComponent, ComponentShutdown>)OnRelayShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MovementRelayTargetComponent, ComponentShutdown>((EntityEventRefHandler<MovementRelayTargetComponent, ComponentShutdown>)OnTargetRelayShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MovementRelayTargetComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<MovementRelayTargetComponent, AfterAutoHandleStateEvent>)OnAfterRelayTargetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RelayInputMoverComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<RelayInputMoverComponent, AfterAutoHandleStateEvent>)OnAfterRelayState, (Type[])null, (Type[])null);
	}

	private void OnAfterRelayTargetState(Entity<MovementRelayTargetComponent> entity, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base.PhysicsSystem.UpdateIsPredicted((EntityUid?)entity.Owner, (PhysicsComponent)null);
		EnsureValidRelayTarget(entity.Owner, entity.Comp);
	}

	private void OnAfterRelayState(Entity<RelayInputMoverComponent> entity, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.PhysicsSystem.UpdateIsPredicted((EntityUid?)entity.Owner, (PhysicsComponent)null);
	}

	public void SetRelay(EntityUid uid, EntityUid relayEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (uid == relayEntity)
		{
			((EntitySystem)this).Log.Error($"An entity attempted to relay movement to itself. Entity:{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
			return;
		}
		RelayInputMoverComponent component = ((EntitySystem)this).EnsureComp<RelayInputMoverComponent>(uid);
		if (!(component.RelayEntity == relayEntity))
		{
			MovementRelayTargetComponent oldTarget = default(MovementRelayTargetComponent);
			if (((EntitySystem)this).TryComp<MovementRelayTargetComponent>(component.RelayEntity, ref oldTarget))
			{
				oldTarget.Source = EntityUid.Invalid;
				((EntitySystem)this).RemComp(component.RelayEntity, (IComponent)(object)oldTarget);
				base.PhysicsSystem.UpdateIsPredicted((EntityUid?)component.RelayEntity, (PhysicsComponent)null);
			}
			MovementRelayTargetComponent targetComp = ((EntitySystem)this).EnsureComp<MovementRelayTargetComponent>(relayEntity);
			RelayInputMoverComponent oldRelay = default(RelayInputMoverComponent);
			if (((EntitySystem)this).TryComp<RelayInputMoverComponent>(targetComp.Source, ref oldRelay))
			{
				oldRelay.RelayEntity = EntityUid.Invalid;
				((EntitySystem)this).RemComp(targetComp.Source, (IComponent)(object)oldRelay);
				base.PhysicsSystem.UpdateIsPredicted((EntityUid?)targetComp.Source, (PhysicsComponent)null);
			}
			base.PhysicsSystem.UpdateIsPredicted((EntityUid?)uid, (PhysicsComponent)null);
			base.PhysicsSystem.UpdateIsPredicted((EntityUid?)relayEntity, (PhysicsComponent)null);
			component.RelayEntity = relayEntity;
			targetComp.Source = uid;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			((EntitySystem)this).Dirty(relayEntity, (IComponent)(object)targetComp, (MetaDataComponent)null);
			_blocker.UpdateCanMove(uid);
		}
	}

	private void OnRelayShutdown(Entity<RelayInputMoverComponent> entity, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		base.PhysicsSystem.UpdateIsPredicted((EntityUid?)entity.Owner, (PhysicsComponent)null);
		base.PhysicsSystem.UpdateIsPredicted((EntityUid?)entity.Comp.RelayEntity, (PhysicsComponent)null);
		InputMoverComponent inputMover = default(InputMoverComponent);
		if (((EntitySystem)this).TryComp<InputMoverComponent>(entity.Comp.RelayEntity, ref inputMover))
		{
			SetMoveInput(Entity<InputMoverComponent>.op_Implicit((entity.Comp.RelayEntity, inputMover)), MoveButtons.None);
		}
		if (!Timing.ApplyingState)
		{
			MovementRelayTargetComponent target = default(MovementRelayTargetComponent);
			if (((EntitySystem)this).TryComp<MovementRelayTargetComponent>(entity.Comp.RelayEntity, ref target) && (int)((Component)target).LifeStage <= 6)
			{
				((EntitySystem)this).RemComp(entity.Comp.RelayEntity, (IComponent)(object)target);
			}
			_blocker.UpdateCanMove(entity.Owner);
		}
	}

	private void OnTargetRelayShutdown(Entity<MovementRelayTargetComponent> entity, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I4
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		base.PhysicsSystem.UpdateIsPredicted((EntityUid?)entity.Owner, (PhysicsComponent)null);
		base.PhysicsSystem.UpdateIsPredicted((EntityUid?)entity.Comp.Source, (PhysicsComponent)null);
		RelayInputMoverComponent relay = default(RelayInputMoverComponent);
		if (!Timing.ApplyingState && ((EntitySystem)this).TryComp<RelayInputMoverComponent>(entity.Comp.Source, ref relay) && (int)((Component)relay).LifeStage <= 6)
		{
			((EntitySystem)this).RemComp(entity.Comp.Source, (IComponent)(object)relay);
		}
	}

	private bool EnsureValidRelayTarget(EntityUid uid, MovementRelayTargetComponent relayTarget)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		EntityUid source = relayTarget.Source;
		RelayInputMoverComponent sourceRelay = default(RelayInputMoverComponent);
		if (((EntityUid)(ref source)).IsValid() && RelayQuery.TryComp(source, ref sourceRelay) && sourceRelay.RelayEntity == uid)
		{
			return true;
		}
		InputMoverComponent mover = default(InputMoverComponent);
		if (MoverQuery.TryComp(uid, ref mover))
		{
			SetMoveInput(Entity<InputMoverComponent>.op_Implicit((uid, mover)), MoveButtons.None);
		}
		if (!Timing.ApplyingState)
		{
			((EntitySystem)this).RemCompDeferred<MovementRelayTargetComponent>(uid);
		}
		return false;
	}
}

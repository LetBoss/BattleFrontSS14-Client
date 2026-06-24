using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.Replay.UI;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Station.Components;
using Content.Shared.Throwing;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.Replays.Playback;
using Robust.Client.State;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Utility;

namespace Content.Client.Replay.Spectator;

public sealed class ReplaySpectatorSystem : EntitySystem
{
	private sealed class MoverHandler : InputCmdHandler
	{
		private readonly ReplaySpectatorSystem _sys;

		private readonly DirectionFlag _dir;

		public MoverHandler(ReplaySpectatorSystem sys, DirectionFlag dir)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			_sys = sys;
			_dir = dir;
		}

		public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if ((int)message.State == 1)
			{
				ReplaySpectatorSystem sys = _sys;
				sys.Direction |= _dir;
			}
			else
			{
				ReplaySpectatorSystem sys2 = _sys;
				sys2.Direction = (DirectionFlag)(sys2.Direction & (sbyte)(~_dir));
			}
			return true;
		}
	}

	public struct SpectatorData
	{
		public EntityUid Entity;

		public NetUserId Controller;

		public (EntityCoordinates Coords, Angle Rot)? Local;

		public (EntityCoordinates Coords, Angle Rot)? World;

		public (EntityUid? Ent, Angle Rot)? Eye;
	}

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IConsoleHost _conHost;

	[Dependency]
	private IStateManager _stateMan;

	[Dependency]
	private TransformSystem _transform;

	[Dependency]
	private SharedMoverController _mover;

	[Dependency]
	private SharedContentEyeSystem _eye;

	[Dependency]
	private IReplayPlaybackManager _replayPlayback;

	private SpectatorData? _spectatorData;

	public const string SpectateCmd = "replay_spectate";

	public static readonly NetUserId DefaultUser;

	public DirectionFlag Direction;

	public const float DefaultSpeed = 12f;

	private void InitializeBlockers()
	{
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, UseAttemptEvent>((ComponentEventHandler<ReplaySpectatorComponent, UseAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, PickupAttemptEvent>((ComponentEventHandler<ReplaySpectatorComponent, PickupAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, ThrowAttemptEvent>((ComponentEventHandler<ReplaySpectatorComponent, ThrowAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, InteractionAttemptEvent>((EntityEventRefHandler<ReplaySpectatorComponent, InteractionAttemptEvent>)OnInteractAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, AttackAttemptEvent>((ComponentEventHandler<ReplaySpectatorComponent, AttackAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, DropAttemptEvent>((ComponentEventHandler<ReplaySpectatorComponent, DropAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, IsEquippingAttemptEvent>((ComponentEventHandler<ReplaySpectatorComponent, IsEquippingAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, IsUnequippingAttemptEvent>((ComponentEventHandler<ReplaySpectatorComponent, IsUnequippingAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, UpdateCanMoveEvent>((ComponentEventHandler<ReplaySpectatorComponent, UpdateCanMoveEvent>)OnUpdateCanMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, ChangeDirectionAttemptEvent>((ComponentEventHandler<ReplaySpectatorComponent, ChangeDirectionAttemptEvent>)OnUpdateCanMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, PullAttemptEvent>((ComponentEventHandler<ReplaySpectatorComponent, PullAttemptEvent>)OnPullAttempt, (Type[])null, (Type[])null);
	}

	private void OnInteractAttempt(Entity<ReplaySpectatorComponent> ent, ref InteractionAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnAttempt(EntityUid uid, ReplaySpectatorComponent component, CancellableEntityEventArgs args)
	{
		args.Cancel();
	}

	private void OnUpdateCanMove(EntityUid uid, ReplaySpectatorComponent component, CancellableEntityEventArgs args)
	{
		args.Cancel();
	}

	private void OnPullAttempt(EntityUid uid, ReplaySpectatorComponent component, PullAttemptEvent args)
	{
		args.Cancelled = true;
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GetVerbsEvent<AlternativeVerb>>((EntityEventHandler<GetVerbsEvent<AlternativeVerb>>)OnGetAlternativeVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, EntityTerminatingEvent>((ComponentEventRefHandler<ReplaySpectatorComponent, EntityTerminatingEvent>)OnTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<ReplaySpectatorComponent, LocalPlayerDetachedEvent>)OnDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReplaySpectatorComponent, EntParentChangedMessage>((ComponentEventRefHandler<ReplaySpectatorComponent, EntParentChangedMessage>)OnParentChanged, (Type[])null, (Type[])null);
		InitializeBlockers();
		_replayPlayback.BeforeSetTick += OnBeforeSetTick;
		_replayPlayback.AfterSetTick += OnAfterSetTick;
		_replayPlayback.ReplayPlaybackStarted += OnPlaybackStarted;
		_replayPlayback.ReplayPlaybackStopped += OnPlaybackStopped;
		_replayPlayback.BeforeApplyState += OnBeforeApplyState;
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_replayPlayback.BeforeSetTick -= OnBeforeSetTick;
		_replayPlayback.AfterSetTick -= OnAfterSetTick;
		_replayPlayback.ReplayPlaybackStarted -= OnPlaybackStarted;
		_replayPlayback.ReplayPlaybackStopped -= OnPlaybackStopped;
		_replayPlayback.BeforeApplyState -= OnBeforeApplyState;
	}

	private void OnPlaybackStarted(MappingDataNode yamlMappingNode, List<object> objects)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_004f: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		InitializeMovement();
		_conHost.RegisterCommand("replay_spectate", base.Loc.GetString("cmd-replay-spectate-desc"), base.Loc.GetString("cmd-replay-spectate-help"), new ConCommandCallback(SpectateCommand), new ConCommandCompletionCallback(SpectateCompletions), false);
		EntityUid? val = default(EntityUid?);
		if (_replayPlayback.TryGetRecorderEntity(ref val))
		{
			SpectateEntity(val.Value);
		}
		else
		{
			SetSpectatorPosition(default(SpectatorData));
		}
	}

	private void OnPlaybackStopped()
	{
		ShutdownMovement();
		_conHost.UnregisterCommand("replay_spectate");
	}

	private void InitializeMovement()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		MoverHandler moverHandler = new MoverHandler(this, (DirectionFlag)4);
		MoverHandler moverHandler2 = new MoverHandler(this, (DirectionFlag)8);
		MoverHandler moverHandler3 = new MoverHandler(this, (DirectionFlag)2);
		MoverHandler moverHandler4 = new MoverHandler(this, (DirectionFlag)1);
		CommandBinds.Builder.Bind(EngineKeyFunctions.MoveUp, (InputCmdHandler)(object)moverHandler).Bind(EngineKeyFunctions.MoveLeft, (InputCmdHandler)(object)moverHandler2).Bind(EngineKeyFunctions.MoveRight, (InputCmdHandler)(object)moverHandler3)
			.Bind(EngineKeyFunctions.MoveDown, (InputCmdHandler)(object)moverHandler4)
			.Register<ReplaySpectatorSystem>();
	}

	private void ShutdownMovement()
	{
		CommandBinds.Unregister<ReplaySpectatorSystem>();
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		if (_replayPlayback.Replay == null)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		if ((int)Direction == 0)
		{
			InputMoverComponent mover = default(InputMoverComponent);
			if (((EntitySystem)this).TryComp<InputMoverComponent>(valueOrDefault, ref mover))
			{
				_mover.LerpRotation(valueOrDefault, mover, frameTime);
			}
		}
		else if (!((EntitySystem)this).IsClientSide(valueOrDefault, (MetaDataComponent)null) || !((EntitySystem)this).HasComp<ReplaySpectatorComponent>(valueOrDefault))
		{
			SpawnSpectatorGhost(new EntityCoordinates(valueOrDefault, default(Vector2)), gridAttach: true);
		}
		else
		{
			InputMoverComponent mover2 = default(InputMoverComponent);
			if (!((EntitySystem)this).TryComp<InputMoverComponent>(valueOrDefault, ref mover2))
			{
				return;
			}
			_mover.LerpRotation(valueOrDefault, mover2, frameTime);
			DirectionFlag val = Direction;
			if ((Direction & 4) != 0)
			{
				val = (DirectionFlag)(val & -2);
			}
			if ((Direction & 2) != 0)
			{
				val = (DirectionFlag)(val & -9);
			}
			TransformComponent component = ((EntitySystem)this).GetEntityQuery<TransformComponent>().GetComponent(valueOrDefault);
			Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(component);
			EntityUid parentUid = component.ParentUid;
			if (!((EntityUid)(ref parentUid)).IsValid())
			{
				SetSpectatorPosition(default(SpectatorData));
				return;
			}
			((SharedTransformSystem)_transform).SetGridId(valueOrDefault, component, (EntityUid?)null, (EntityQuery<TransformComponent>?)null);
			((SharedTransformSystem)_transform).AttachToGridOrMap(valueOrDefault, (TransformComponent)null);
			parentUid = component.ParentUid;
			if (((EntityUid)(ref parentUid)).IsValid())
			{
				((SharedTransformSystem)_transform).SetGridId(valueOrDefault, component, ((EntitySystem)this).Transform(component.ParentUid).GridUid, (EntityQuery<TransformComponent>?)null);
			}
			Angle parentGridAngle = _mover.GetParentGridAngle(mover2);
			Angle val2 = DirectionExtensions.ToAngle(DirectionExtensions.AsDir(val));
			Vector2 vector = ((Angle)(ref val2)).ToWorldVec();
			Vector2 vector2 = ((Angle)(ref parentGridAngle)).RotateVec(ref vector);
			float num = ((EntitySystem)this).CompOrNull<MovementSpeedModifierComponent>(valueOrDefault)?.BaseSprintSpeed ?? 12f;
			Vector2 vector3 = vector2 * frameTime * num;
			((SharedTransformSystem)_transform).SetWorldPositionRotation(valueOrDefault, worldPosition + vector3, DirectionExtensions.ToWorldAngle(vector3), component);
		}
	}

	public SpectatorData GetSpectatorData()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		SpectatorData result = default(SpectatorData);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			result.Controller = (NetUserId)(((_003F?)((ISharedPlayerManager)_player).LocalUser) ?? DefaultUser);
			TransformComponent val = default(TransformComponent);
			if (!((EntitySystem)this).TryComp(valueOrDefault, ref val) || !val.MapUid.HasValue)
			{
				return result;
			}
			result.Local = (val.Coordinates, val.LocalRotation);
			ValueTuple<Vector2, Angle> worldPositionRotation = ((SharedTransformSystem)_transform).GetWorldPositionRotation(valueOrDefault);
			Vector2 item = worldPositionRotation.Item1;
			Angle item2 = worldPositionRotation.Item2;
			result.World = (new EntityCoordinates(val.MapUid.Value, item), item2);
			InputMoverComponent inputMoverComponent = default(InputMoverComponent);
			if (((EntitySystem)this).TryComp<InputMoverComponent>(valueOrDefault, ref inputMoverComponent))
			{
				result.Eye = (inputMoverComponent.RelativeEntity, inputMoverComponent.TargetRelativeRotation);
			}
			result.Entity = valueOrDefault;
			return result;
		}
		return result;
	}

	private void OnBeforeSetTick()
	{
		_spectatorData = GetSpectatorData();
	}

	private void OnAfterSetTick()
	{
		if (_spectatorData.HasValue)
		{
			SetSpectatorPosition(_spectatorData.Value);
		}
		_spectatorData = null;
	}

	private void OnBeforeApplyState((GameState Current, GameState? Next) args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		NetUserId? localUser = ((ISharedPlayerManager)_player).LocalUser;
		NetUserId defaultUser = DefaultUser;
		if (!localUser.HasValue || localUser.GetValueOrDefault() != defaultUser)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		NetEntity netEntity = ((EntitySystem)this).GetNetEntity(valueOrDefault, (MetaDataComponent)null);
		if (((NetEntity)(ref netEntity)).IsClientSide())
		{
			return;
		}
		ICommonSession localSession = default(ICommonSession);
		foreach (SessionState item in args.Current.PlayerStates.Value)
		{
			NetEntity? controlledEntity = item.ControlledEntity;
			NetEntity val = netEntity;
			if (controlledEntity.HasValue && !(controlledEntity.GetValueOrDefault() != val))
			{
				if (!((ISharedPlayerManager)_player).TryGetSessionById((NetUserId?)item.UserId, ref localSession))
				{
					localSession = ((ISharedPlayerManager)_player).CreateAndAddSession(item.UserId, item.Name);
				}
				_player.SetLocalSession(localSession);
				break;
			}
		}
	}

	public void SetSpectatorPosition(SpectatorData data)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		if (((ISharedPlayerManager)_player).LocalSession == null)
		{
			return;
		}
		if (data.Controller != DefaultUser)
		{
			ICommonSession val = default(ICommonSession);
			if (((ISharedPlayerManager)_player).TryGetSessionById((NetUserId?)data.Controller, ref val) && ((EntitySystem)this).Exists(val.AttachedEntity))
			{
				_player.SetLocalSession(val);
				return;
			}
			_player.SetLocalSession(((ISharedPlayerManager)_player).GetSessionById(DefaultUser));
		}
		if (((EntitySystem)this).Exists(data.Entity) && ((EntitySystem)this).Transform(data.Entity).MapID != MapId.Nullspace)
		{
			((ISharedPlayerManager)_player).SetAttachedEntity(((ISharedPlayerManager)_player).LocalSession, (EntityUid?)data.Entity, false);
			return;
		}
		(EntityCoordinates, Angle) value;
		if (data.Local.HasValue)
		{
			value = data.Local.Value;
			if (((EntityCoordinates)(ref value.Item1)).IsValid((IEntityManager)(object)base.EntityManager))
			{
				SpawnSpectatorGhost(data.Local.Value.Coords, gridAttach: false).LocalRotation = data.Local.Value.Rot;
				goto IL_01b4;
			}
		}
		if (data.World.HasValue)
		{
			value = data.World.Value;
			if (((EntityCoordinates)(ref value.Item1)).IsValid((IEntityManager)(object)base.EntityManager))
			{
				SpawnSpectatorGhost(data.World.Value.Coords, gridAttach: true).LocalRotation = data.World.Value.Rot;
				goto IL_01b4;
			}
		}
		if (TryFindFallbackSpawn(out var coords))
		{
			SpawnSpectatorGhost(coords, gridAttach: true).LocalRotation = Angle.op_Implicit(0f);
			goto IL_01b4;
		}
		((EntitySystem)this).Log.Error("Failed to find a suitable observer spawn point");
		return;
		IL_01b4:
		InputMoverComponent inputMoverComponent = default(InputMoverComponent);
		if (data.Eye.HasValue && ((EntitySystem)this).TryComp<InputMoverComponent>(((ISharedPlayerManager)_player).LocalSession.AttachedEntity, ref inputMoverComponent))
		{
			inputMoverComponent.RelativeEntity = data.Eye.Value.Ent;
			inputMoverComponent.TargetRelativeRotation = (inputMoverComponent.RelativeRotation = data.Eye.Value.Rot);
		}
	}

	private bool TryFindFallbackSpawn(out EntityCoordinates coords)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = default(EntityUid?);
		if (_replayPlayback.TryGetRecorderEntity(ref val))
		{
			coords = new EntityCoordinates(val.Value, default(Vector2));
			return true;
		}
		Entity<MapGridComponent>? val2 = null;
		float? num = null;
		EntityQueryEnumerator<MapGridComponent> val3 = ((EntitySystem)this).EntityQueryEnumerator<MapGridComponent>();
		bool flag = false;
		EntityUid val4 = default(EntityUid);
		MapGridComponent val5 = default(MapGridComponent);
		while (val3.MoveNext(ref val4, ref val5))
		{
			Box2 localAABB = val5.LocalAABB;
			float num2 = ((Box2)(ref localAABB)).Size.LengthSquared();
			bool flag2 = ((EntitySystem)this).HasComp<StationMemberComponent>(val4);
			if ((!num.HasValue || !(num2 < num) || (!flag && flag2)) && !(!flag2 && flag))
			{
				val2 = Entity<MapGridComponent>.op_Implicit((val4, val5));
				num = num2;
				if (flag2)
				{
					flag = true;
				}
			}
		}
		coords = new EntityCoordinates(Entity<MapGridComponent>.op_Implicit(val2.GetValueOrDefault()), default(Vector2));
		return val2.HasValue;
	}

	private void OnTerminating(EntityUid uid, ReplaySpectatorComponent component, ref EntityTerminatingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && !(uid != localEntity.GetValueOrDefault()))
		{
			TransformComponent val = ((EntitySystem)this).Transform(uid);
			if (val.MapUid.HasValue && !((EntitySystem)this).Terminating(val.MapUid.Value, (MetaDataComponent)null))
			{
				SpawnSpectatorGhost(new EntityCoordinates(val.MapUid.Value, default(Vector2)), gridAttach: true);
			}
		}
	}

	private void OnParentChanged(EntityUid uid, ReplaySpectatorComponent component, ref EntParentChangedMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && !(uid != localEntity.GetValueOrDefault()) && !((EntParentChangedMessage)(ref args)).Transform.MapUid.HasValue && args.OldMapId.HasValue && !_spectatorData.HasValue)
		{
			SetSpectatorPosition(default(SpectatorData));
		}
	}

	private void OnDetached(EntityUid uid, ReplaySpectatorComponent component, LocalPlayerDetachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).IsClientSide(uid, (MetaDataComponent)null))
		{
			((EntitySystem)this).QueueDel((EntityUid?)uid);
		}
		else
		{
			((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)component);
		}
	}

	private void OnGetAlternativeVerbs(GetVerbsEvent<AlternativeVerb> ev)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		if (_replayPlayback.Replay != null)
		{
			ev.Verbs.Add(new AlternativeVerb
			{
				Priority = 100,
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					SpectateEntity(ev.Target);
				},
				Text = base.Loc.GetString("replay-verb-spectate"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/vv.svg.192dpi.png"))
			});
		}
	}

	public void SpectateEntity(EntityUid target)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (((ISharedPlayerManager)_player).LocalSession == null)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid? val = localEntity;
		if (val.HasValue && val.GetValueOrDefault() == target)
		{
			SpawnSpectatorGhost(((EntitySystem)this).Transform(target).Coordinates, gridAttach: true);
			return;
		}
		((EntitySystem)this).EnsureComp<ReplaySpectatorComponent>(target);
		ActorComponent val2 = default(ActorComponent);
		if (((EntitySystem)this).TryComp<ActorComponent>(target, ref val2))
		{
			_player.SetLocalSession(val2.PlayerSession);
		}
		else
		{
			((ISharedPlayerManager)_player).SetAttachedEntity(((ISharedPlayerManager)_player).LocalSession, (EntityUid?)target, false);
		}
		_stateMan.RequestStateChange<ReplaySpectateEntityState>();
		if (localEntity.HasValue)
		{
			if (((EntitySystem)this).IsClientSide(localEntity.Value, (MetaDataComponent)null))
			{
				((EntitySystem)this).Del((EntityUid?)localEntity.Value);
			}
			else
			{
				((EntitySystem)this).RemComp<ReplaySpectatorComponent>(localEntity.Value);
			}
		}
	}

	public TransformComponent SpawnSpectatorGhost(EntityCoordinates coords, bool gridAttach)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		ICommonSession sessionById = ((ISharedPlayerManager)_player).GetSessionById(DefaultUser);
		_player.SetLocalSession(sessionById);
		EntityUid val = ((EntitySystem)this).Spawn("ReplayObserver", coords);
		_eye.SetMaxZoom(val, Vector2.One * 5f);
		((EntitySystem)this).EnsureComp<ReplaySpectatorComponent>(val);
		TransformComponent result = ((EntitySystem)this).Transform(val);
		if (gridAttach)
		{
			((SharedTransformSystem)_transform).AttachToGridOrMap(val, (TransformComponent)null);
		}
		((ISharedPlayerManager)_player).SetAttachedEntity(sessionById, (EntityUid?)val, false);
		if (localEntity.HasValue)
		{
			if (((EntitySystem)this).IsClientSide(localEntity.Value, (MetaDataComponent)null))
			{
				((EntitySystem)this).QueueDel((EntityUid?)localEntity.Value);
			}
			else
			{
				((EntitySystem)this).RemComp<ReplaySpectatorComponent>(localEntity.Value);
			}
		}
		_stateMan.RequestStateChange<ReplayGhostState>();
		_spectatorData = GetSpectatorData();
		return result;
	}

	private void SpectateCommand(IConsoleShell shell, string argStr, string[] args)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		NetEntity val2 = default(NetEntity);
		if (args.Length == 0)
		{
			ICommonSession localSession = ((ISharedPlayerManager)_player).LocalSession;
			EntityUid? val = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				SpawnSpectatorGhost(new EntityCoordinates(valueOrDefault, default(Vector2)), gridAttach: true);
			}
			else
			{
				SpawnSpectatorGhost(default(EntityCoordinates), gridAttach: true);
			}
		}
		else if (!NetEntity.TryParse((ReadOnlySpan<char>)args[0], ref val2))
		{
			shell.WriteError(base.Loc.GetString("cmd-parse-failure-uid", (ValueTuple<string, object>)("arg", args[0])));
		}
		else
		{
			EntityUid entity = ((EntitySystem)this).GetEntity(val2);
			if (!((EntitySystem)this).Exists(entity))
			{
				shell.WriteError(base.Loc.GetString("cmd-parse-failure-entity-exist", (ValueTuple<string, object>)("arg", args[0])));
			}
			else
			{
				SpectateEntity(entity);
			}
		}
	}

	private CompletionResult SpectateCompletions(IConsoleShell shell, string[] args)
	{
		if (args.Length != 1)
		{
			return CompletionResult.Empty;
		}
		return CompletionResult.FromHintOptions(CompletionHelper.NetEntities(args[0], (IEntityManager)(object)base.EntityManager, 20), base.Loc.GetString("cmd-replay-spectate-hint"));
	}
}

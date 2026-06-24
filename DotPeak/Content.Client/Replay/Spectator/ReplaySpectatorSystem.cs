// Decompiled with JetBrains decompiler
// Type: Content.Client.Replay.Spectator.ReplaySpectatorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Replay.Spectator;

public sealed class ReplaySpectatorSystem : EntitySystem
{
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
  private ReplaySpectatorSystem.SpectatorData? _spectatorData;
  public const string SpectateCmd = "replay_spectate";
  public static readonly NetUserId DefaultUser;
  public DirectionFlag Direction;
  public const float DefaultSpeed = 12f;

  private void InitializeBlockers()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, UseAttemptEvent>(new ComponentEventHandler<ReplaySpectatorComponent, UseAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, PickupAttemptEvent>(new ComponentEventHandler<ReplaySpectatorComponent, PickupAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, ThrowAttemptEvent>(new ComponentEventHandler<ReplaySpectatorComponent, ThrowAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, InteractionAttemptEvent>(new EntityEventRefHandler<ReplaySpectatorComponent, InteractionAttemptEvent>((object) this, __methodptr(OnInteractAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, AttackAttemptEvent>(new ComponentEventHandler<ReplaySpectatorComponent, AttackAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, DropAttemptEvent>(new ComponentEventHandler<ReplaySpectatorComponent, DropAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, IsEquippingAttemptEvent>(new ComponentEventHandler<ReplaySpectatorComponent, IsEquippingAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, IsUnequippingAttemptEvent>(new ComponentEventHandler<ReplaySpectatorComponent, IsUnequippingAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, UpdateCanMoveEvent>(new ComponentEventHandler<ReplaySpectatorComponent, UpdateCanMoveEvent>((object) this, __methodptr(OnUpdateCanMove)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, ChangeDirectionAttemptEvent>(new ComponentEventHandler<ReplaySpectatorComponent, ChangeDirectionAttemptEvent>((object) this, __methodptr(OnUpdateCanMove)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, PullAttemptEvent>(new ComponentEventHandler<ReplaySpectatorComponent, PullAttemptEvent>((object) this, __methodptr(OnPullAttempt)), (Type[]) null, (Type[]) null);
  }

  private void OnInteractAttempt(
    Entity<ReplaySpectatorComponent> ent,
    ref InteractionAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnAttempt(
    EntityUid uid,
    ReplaySpectatorComponent component,
    CancellableEntityEventArgs args)
  {
    args.Cancel();
  }

  private void OnUpdateCanMove(
    EntityUid uid,
    ReplaySpectatorComponent component,
    CancellableEntityEventArgs args)
  {
    args.Cancel();
  }

  private void OnPullAttempt(
    EntityUid uid,
    ReplaySpectatorComponent component,
    PullAttemptEvent args)
  {
    args.Cancelled = true;
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GetVerbsEvent<AlternativeVerb>>(new EntityEventHandler<GetVerbsEvent<AlternativeVerb>>(this.OnGetAlternativeVerbs), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, EntityTerminatingEvent>(new ComponentEventRefHandler<ReplaySpectatorComponent, EntityTerminatingEvent>((object) this, __methodptr(OnTerminating)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<ReplaySpectatorComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnDetached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReplaySpectatorComponent, EntParentChangedMessage>(new ComponentEventRefHandler<ReplaySpectatorComponent, EntParentChangedMessage>((object) this, __methodptr(OnParentChanged)), (Type[]) null, (Type[]) null);
    this.InitializeBlockers();
    this._replayPlayback.BeforeSetTick += new Action(this.OnBeforeSetTick);
    this._replayPlayback.AfterSetTick += new Action(this.OnAfterSetTick);
    this._replayPlayback.ReplayPlaybackStarted += new Action<MappingDataNode, List<object>>(this.OnPlaybackStarted);
    this._replayPlayback.ReplayPlaybackStopped += new Action(this.OnPlaybackStopped);
    this._replayPlayback.BeforeApplyState += new Action<(GameState, GameState)>(this.OnBeforeApplyState);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._replayPlayback.BeforeSetTick -= new Action(this.OnBeforeSetTick);
    this._replayPlayback.AfterSetTick -= new Action(this.OnAfterSetTick);
    this._replayPlayback.ReplayPlaybackStarted -= new Action<MappingDataNode, List<object>>(this.OnPlaybackStarted);
    this._replayPlayback.ReplayPlaybackStopped -= new Action(this.OnPlaybackStopped);
    this._replayPlayback.BeforeApplyState -= new Action<(GameState, GameState)>(this.OnBeforeApplyState);
  }

  private void OnPlaybackStarted(MappingDataNode yamlMappingNode, List<object> objects)
  {
    this.InitializeMovement();
    this._conHost.RegisterCommand("replay_spectate", this.Loc.GetString("cmd-replay-spectate-desc"), this.Loc.GetString("cmd-replay-spectate-help"), new ConCommandCallback(this.SpectateCommand), new ConCommandCompletionCallback(this.SpectateCompletions), false);
    EntityUid? nullable;
    if (this._replayPlayback.TryGetRecorderEntity(ref nullable))
      this.SpectateEntity(nullable.Value);
    else
      this.SetSpectatorPosition(new ReplaySpectatorSystem.SpectatorData());
  }

  private void OnPlaybackStopped()
  {
    this.ShutdownMovement();
    this._conHost.UnregisterCommand("replay_spectate");
  }

  private void InitializeMovement()
  {
    ReplaySpectatorSystem.MoverHandler moverHandler1 = new ReplaySpectatorSystem.MoverHandler(this, (DirectionFlag) 4);
    ReplaySpectatorSystem.MoverHandler moverHandler2 = new ReplaySpectatorSystem.MoverHandler(this, (DirectionFlag) 8);
    ReplaySpectatorSystem.MoverHandler moverHandler3 = new ReplaySpectatorSystem.MoverHandler(this, (DirectionFlag) 2);
    ReplaySpectatorSystem.MoverHandler moverHandler4 = new ReplaySpectatorSystem.MoverHandler(this, (DirectionFlag) 1);
    CommandBinds.Builder.Bind(EngineKeyFunctions.MoveUp, (InputCmdHandler) moverHandler1).Bind(EngineKeyFunctions.MoveLeft, (InputCmdHandler) moverHandler2).Bind(EngineKeyFunctions.MoveRight, (InputCmdHandler) moverHandler3).Bind(EngineKeyFunctions.MoveDown, (InputCmdHandler) moverHandler4).Register<ReplaySpectatorSystem>();
  }

  private void ShutdownMovement() => CommandBinds.Unregister<ReplaySpectatorSystem>();

  public virtual void FrameUpdate(float frameTime)
  {
    if (this._replayPlayback.Replay == null)
      return;
    EntityUid? nullable1 = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!nullable1.HasValue)
      return;
    EntityUid valueOrDefault = nullable1.GetValueOrDefault();
    if (this.Direction == null)
    {
      InputMoverComponent mover;
      if (!this.TryComp<InputMoverComponent>(valueOrDefault, ref mover))
        return;
      this._mover.LerpRotation(valueOrDefault, mover, frameTime);
    }
    else if (!this.IsClientSide(valueOrDefault, (MetaDataComponent) null) || !this.HasComp<ReplaySpectatorComponent>(valueOrDefault))
    {
      this.SpawnSpectatorGhost(new EntityCoordinates(valueOrDefault, new Vector2()), true);
    }
    else
    {
      InputMoverComponent mover;
      if (!this.TryComp<InputMoverComponent>(valueOrDefault, ref mover))
        return;
      this._mover.LerpRotation(valueOrDefault, mover, frameTime);
      DirectionFlag directionFlag = this.Direction;
      if ((this.Direction & 4) != null)
        directionFlag = (DirectionFlag) (directionFlag & -2);
      if ((this.Direction & 2) != null)
        directionFlag = (DirectionFlag) (directionFlag & -9);
      TransformComponent component = this.GetEntityQuery<TransformComponent>().GetComponent(valueOrDefault);
      Vector2 worldPosition = ((SharedTransformSystem) this._transform).GetWorldPosition(component);
      EntityUid parentUid = component.ParentUid;
      if (!((EntityUid) ref parentUid).IsValid())
      {
        this.SetSpectatorPosition(new ReplaySpectatorSystem.SpectatorData());
      }
      else
      {
        TransformSystem transform = this._transform;
        EntityUid entityUid = valueOrDefault;
        TransformComponent transformComponent = component;
        nullable1 = new EntityUid?();
        EntityUid? nullable2 = nullable1;
        EntityQuery<TransformComponent>? nullable3 = new EntityQuery<TransformComponent>?();
        ((SharedTransformSystem) transform).SetGridId(entityUid, transformComponent, nullable2, nullable3);
        ((SharedTransformSystem) this._transform).AttachToGridOrMap(valueOrDefault, (TransformComponent) null);
        parentUid = component.ParentUid;
        if (((EntityUid) ref parentUid).IsValid())
          ((SharedTransformSystem) this._transform).SetGridId(valueOrDefault, component, this.Transform(component.ParentUid).GridUid, new EntityQuery<TransformComponent>?());
        Angle parentGridAngle = this._mover.GetParentGridAngle(mover);
        Angle angle = DirectionExtensions.ToAngle(DirectionExtensions.AsDir(directionFlag));
        Vector2 worldVec = ((Angle) ref angle).ToWorldVec();
        Vector2 vector2_1 = ((Angle) ref parentGridAngle).RotateVec(ref worldVec);
        MovementSpeedModifierComponent modifierComponent = this.CompOrNull<MovementSpeedModifierComponent>(valueOrDefault);
        float num1 = modifierComponent != null ? modifierComponent.BaseSprintSpeed : 12f;
        double num2 = (double) frameTime;
        Vector2 vector2_2 = vector2_1 * (float) num2 * num1;
        ((SharedTransformSystem) this._transform).SetWorldPositionRotation(valueOrDefault, worldPosition + vector2_2, DirectionExtensions.ToWorldAngle(vector2_2), component);
      }
    }
  }

  public ReplaySpectatorSystem.SpectatorData GetSpectatorData()
  {
    ReplaySpectatorSystem.SpectatorData spectatorData = new ReplaySpectatorSystem.SpectatorData();
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return spectatorData;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    spectatorData.Controller = ((ISharedPlayerManager) this._player).LocalUser ?? ReplaySpectatorSystem.DefaultUser;
    TransformComponent transformComponent;
    if (!this.TryComp(valueOrDefault, ref transformComponent) || !transformComponent.MapUid.HasValue)
      return spectatorData;
    spectatorData.Local = new (EntityCoordinates, Angle)?((transformComponent.Coordinates, transformComponent.LocalRotation));
    (Vector2 vector2, Angle angle) = ((SharedTransformSystem) this._transform).GetWorldPositionRotation(valueOrDefault);
    spectatorData.World = new (EntityCoordinates, Angle)?((new EntityCoordinates(transformComponent.MapUid.Value, vector2), angle));
    InputMoverComponent inputMoverComponent;
    if (this.TryComp<InputMoverComponent>(valueOrDefault, ref inputMoverComponent))
      spectatorData.Eye = new (EntityUid?, Angle)?((inputMoverComponent.RelativeEntity, inputMoverComponent.TargetRelativeRotation));
    spectatorData.Entity = valueOrDefault;
    return spectatorData;
  }

  private void OnBeforeSetTick()
  {
    this._spectatorData = new ReplaySpectatorSystem.SpectatorData?(this.GetSpectatorData());
  }

  private void OnAfterSetTick()
  {
    if (this._spectatorData.HasValue)
      this.SetSpectatorPosition(this._spectatorData.Value);
    this._spectatorData = new ReplaySpectatorSystem.SpectatorData?();
  }

  private void OnBeforeApplyState((GameState Current, GameState? Next) args)
  {
    NetUserId? localUser = ((ISharedPlayerManager) this._player).LocalUser;
    NetUserId defaultUser = ReplaySpectatorSystem.DefaultUser;
    if ((localUser.HasValue ? (NetUserId.op_Inequality(localUser.GetValueOrDefault(), defaultUser) ? 1 : 0) : 1) != 0)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    NetEntity netEntity1 = this.GetNetEntity(localEntity.GetValueOrDefault(), (MetaDataComponent) null);
    if (((NetEntity) ref netEntity1).IsClientSide())
      return;
    foreach (SessionState sessionState in (IEnumerable<SessionState>) args.Current.PlayerStates.Value)
    {
      NetEntity? controlledEntity = sessionState.ControlledEntity;
      NetEntity netEntity2 = netEntity1;
      if ((controlledEntity.HasValue ? (NetEntity.op_Inequality(controlledEntity.GetValueOrDefault(), netEntity2) ? 1 : 0) : 1) == 0)
      {
        ICommonSession andAddSession;
        if (!((ISharedPlayerManager) this._player).TryGetSessionById(new NetUserId?(sessionState.UserId), ref andAddSession))
          andAddSession = ((ISharedPlayerManager) this._player).CreateAndAddSession(sessionState.UserId, sessionState.Name);
        this._player.SetLocalSession(andAddSession);
        break;
      }
    }
  }

  public void SetSpectatorPosition(ReplaySpectatorSystem.SpectatorData data)
  {
    if (((ISharedPlayerManager) this._player).LocalSession == null)
      return;
    if (NetUserId.op_Inequality(data.Controller, ReplaySpectatorSystem.DefaultUser))
    {
      ICommonSession icommonSession;
      if (((ISharedPlayerManager) this._player).TryGetSessionById(new NetUserId?(data.Controller), ref icommonSession) && this.Exists(icommonSession.AttachedEntity))
      {
        this._player.SetLocalSession(icommonSession);
        return;
      }
      this._player.SetLocalSession(((ISharedPlayerManager) this._player).GetSessionById(ReplaySpectatorSystem.DefaultUser));
    }
    if (this.Exists(data.Entity) && MapId.op_Inequality(this.Transform(data.Entity).MapID, MapId.Nullspace))
    {
      ((ISharedPlayerManager) this._player).SetAttachedEntity(((ISharedPlayerManager) this._player).LocalSession, new EntityUid?(data.Entity), false);
    }
    else
    {
      (EntityCoordinates, Angle) valueTuple;
      if (data.Local.HasValue)
      {
        valueTuple = data.Local.Value;
        // ISSUE: explicit reference operation
        if (((EntityCoordinates) @valueTuple.Item1).IsValid((IEntityManager) this.EntityManager))
        {
          this.SpawnSpectatorGhost(data.Local.Value.Coords, false).LocalRotation = data.Local.Value.Rot;
          goto label_17;
        }
      }
      if (data.World.HasValue)
      {
        valueTuple = data.World.Value;
        // ISSUE: explicit reference operation
        if (((EntityCoordinates) @valueTuple.Item1).IsValid((IEntityManager) this.EntityManager))
        {
          this.SpawnSpectatorGhost(data.World.Value.Coords, true).LocalRotation = data.World.Value.Rot;
          goto label_17;
        }
      }
      EntityCoordinates coords;
      if (this.TryFindFallbackSpawn(out coords))
      {
        this.SpawnSpectatorGhost(coords, true).LocalRotation = Angle.op_Implicit(0.0f);
      }
      else
      {
        this.Log.Error("Failed to find a suitable observer spawn point");
        return;
      }
label_17:
      InputMoverComponent inputMoverComponent;
      if (!data.Eye.HasValue || !this.TryComp<InputMoverComponent>(((ISharedPlayerManager) this._player).LocalSession.AttachedEntity, ref inputMoverComponent))
        return;
      inputMoverComponent.RelativeEntity = data.Eye.Value.Ent;
      inputMoverComponent.TargetRelativeRotation = inputMoverComponent.RelativeRotation = data.Eye.Value.Rot;
    }
  }

  private bool TryFindFallbackSpawn(out EntityCoordinates coords)
  {
    EntityUid? nullable1;
    if (this._replayPlayback.TryGetRecorderEntity(ref nullable1))
    {
      coords = new EntityCoordinates(nullable1.Value, new Vector2());
      return true;
    }
    Entity<MapGridComponent>? nullable2 = new Entity<MapGridComponent>?();
    float? nullable3 = new float?();
    EntityQueryEnumerator<MapGridComponent> entityQueryEnumerator = this.EntityQueryEnumerator<MapGridComponent>();
    bool flag1 = false;
    EntityUid entityUid;
    MapGridComponent mapGridComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref mapGridComponent))
    {
      Box2 localAabb = mapGridComponent.LocalAABB;
      float num1 = ((Box2) ref localAabb).Size.LengthSquared();
      bool flag2 = this.HasComp<StationMemberComponent>(entityUid);
      if (nullable3.HasValue)
      {
        double num2 = (double) num1;
        float? nullable4 = nullable3;
        double valueOrDefault = (double) nullable4.GetValueOrDefault();
        if (num2 < valueOrDefault & nullable4.HasValue && !(!flag1 & flag2))
          continue;
      }
      if (!(!flag2 & flag1))
      {
        nullable2 = new Entity<MapGridComponent>?(Entity<MapGridComponent>.op_Implicit((entityUid, mapGridComponent)));
        nullable3 = new float?(num1);
        if (flag2)
          flag1 = true;
      }
    }
    coords = new EntityCoordinates(Entity<MapGridComponent>.op_Implicit(nullable2.GetValueOrDefault()), new Vector2());
    return nullable2.HasValue;
  }

  private void OnTerminating(
    EntityUid uid,
    ReplaySpectatorComponent component,
    ref EntityTerminatingEvent args)
  {
    EntityUid entityUid = uid;
    EntityUid? nullable = ((ISharedPlayerManager) this._player).LocalEntity;
    if ((nullable.HasValue ? (EntityUid.op_Inequality(entityUid, nullable.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    TransformComponent transformComponent = this.Transform(uid);
    nullable = transformComponent.MapUid;
    if (!nullable.HasValue)
      return;
    nullable = transformComponent.MapUid;
    if (this.Terminating(nullable.Value, (MetaDataComponent) null))
      return;
    nullable = transformComponent.MapUid;
    this.SpawnSpectatorGhost(new EntityCoordinates(nullable.Value, new Vector2()), true);
  }

  private void OnParentChanged(
    EntityUid uid,
    ReplaySpectatorComponent component,
    ref EntParentChangedMessage args)
  {
    EntityUid entityUid = uid;
    EntityUid? nullable = ((ISharedPlayerManager) this._player).LocalEntity;
    if ((nullable.HasValue ? (EntityUid.op_Inequality(entityUid, nullable.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    nullable = ((EntParentChangedMessage) ref args).Transform.MapUid;
    if (nullable.HasValue || !args.OldMapId.HasValue || this._spectatorData.HasValue)
      return;
    this.SetSpectatorPosition(new ReplaySpectatorSystem.SpectatorData());
  }

  private void OnDetached(
    EntityUid uid,
    ReplaySpectatorComponent component,
    LocalPlayerDetachedEvent args)
  {
    if (this.IsClientSide(uid, (MetaDataComponent) null))
      this.QueueDel(new EntityUid?(uid));
    else
      this.RemCompDeferred(uid, (IComponent) component);
  }

  private void OnGetAlternativeVerbs(GetVerbsEvent<AlternativeVerb> ev)
  {
    if (this._replayPlayback.Replay == null)
      return;
    SortedSet<AlternativeVerb> verbs = ev.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Priority = 100;
    alternativeVerb.Act = (Action) (() => this.SpectateEntity(ev.Target));
    alternativeVerb.Text = this.Loc.GetString("replay-verb-spectate");
    alternativeVerb.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/vv.svg.192dpi.png"));
    verbs.Add(alternativeVerb);
  }

  public void SpectateEntity(EntityUid target)
  {
    if (((ISharedPlayerManager) this._player).LocalSession == null)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid? nullable = localEntity;
    EntityUid entityUid = target;
    if ((nullable.HasValue ? (EntityUid.op_Equality(nullable.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) != 0)
    {
      this.SpawnSpectatorGhost(this.Transform(target).Coordinates, true);
    }
    else
    {
      this.EnsureComp<ReplaySpectatorComponent>(target);
      ActorComponent actorComponent;
      if (this.TryComp<ActorComponent>(target, ref actorComponent))
        this._player.SetLocalSession(actorComponent.PlayerSession);
      else
        ((ISharedPlayerManager) this._player).SetAttachedEntity(((ISharedPlayerManager) this._player).LocalSession, new EntityUid?(target), false);
      this._stateMan.RequestStateChange<ReplaySpectateEntityState>();
      if (!localEntity.HasValue)
        return;
      if (this.IsClientSide(localEntity.Value, (MetaDataComponent) null))
        this.Del(new EntityUid?(localEntity.Value));
      else
        this.RemComp<ReplaySpectatorComponent>(localEntity.Value);
    }
  }

  public TransformComponent SpawnSpectatorGhost(EntityCoordinates coords, bool gridAttach)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    ICommonSession sessionById = ((ISharedPlayerManager) this._player).GetSessionById(ReplaySpectatorSystem.DefaultUser);
    this._player.SetLocalSession(sessionById);
    EntityUid uid = this.Spawn("ReplayObserver", coords);
    this._eye.SetMaxZoom(uid, Vector2.One * 5f);
    this.EnsureComp<ReplaySpectatorComponent>(uid);
    TransformComponent transformComponent = this.Transform(uid);
    if (gridAttach)
      ((SharedTransformSystem) this._transform).AttachToGridOrMap(uid, (TransformComponent) null);
    ((ISharedPlayerManager) this._player).SetAttachedEntity(sessionById, new EntityUid?(uid), false);
    if (localEntity.HasValue)
    {
      if (this.IsClientSide(localEntity.Value, (MetaDataComponent) null))
        this.QueueDel(new EntityUid?(localEntity.Value));
      else
        this.RemComp<ReplaySpectatorComponent>(localEntity.Value);
    }
    this._stateMan.RequestStateChange<ReplayGhostState>();
    this._spectatorData = new ReplaySpectatorSystem.SpectatorData?(this.GetSpectatorData());
    return transformComponent;
  }

  private void SpectateCommand(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length == 0)
    {
      EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._player).LocalSession?.AttachedEntity;
      if (attachedEntity.HasValue)
        this.SpawnSpectatorGhost(new EntityCoordinates(attachedEntity.GetValueOrDefault(), new Vector2()), true);
      else
        this.SpawnSpectatorGhost(new EntityCoordinates(), true);
    }
    else
    {
      NetEntity netEntity;
      if (!NetEntity.TryParse((ReadOnlySpan<char>) args[0], ref netEntity))
      {
        shell.WriteError(this.Loc.GetString("cmd-parse-failure-uid", ("arg", (object) args[0])));
      }
      else
      {
        EntityUid entity = this.GetEntity(netEntity);
        if (!this.Exists(entity))
          shell.WriteError(this.Loc.GetString("cmd-parse-failure-entity-exist", ("arg", (object) args[0])));
        else
          this.SpectateEntity(entity);
      }
    }
  }

  private CompletionResult SpectateCompletions(IConsoleShell shell, string[] args)
  {
    return args.Length != 1 ? CompletionResult.Empty : CompletionResult.FromHintOptions(CompletionHelper.NetEntities(args[0], (IEntityManager) this.EntityManager, 20), this.Loc.GetString("cmd-replay-spectate-hint"));
  }

  private sealed class MoverHandler : InputCmdHandler
  {
    private readonly ReplaySpectatorSystem _sys;
    private readonly DirectionFlag _dir;

    public MoverHandler(ReplaySpectatorSystem sys, DirectionFlag dir)
    {
      this._sys = sys;
      this._dir = dir;
    }

    public virtual bool HandleCmdMessage(
      IEntityManager entManager,
      ICommonSession? session,
      IFullInputCmdMessage message)
    {
      if (message.State == 1)
      {
        ReplaySpectatorSystem sys = this._sys;
        sys.Direction = sys.Direction | this._dir;
      }
      else
      {
        ReplaySpectatorSystem sys = this._sys;
        sys.Direction = (DirectionFlag) (sys.Direction & (int) (sbyte) ~this._dir);
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
}

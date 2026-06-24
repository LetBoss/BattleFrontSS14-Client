// Decompiled with JetBrains decompiler
// Type: Content.Shared.Vehicle.Components.GridVehicleMoverComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Stun;
using Content.Shared.Maps;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Vehicle.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (GridVehicleMoverSystem)}, Other = AccessPermissions.ReadWrite)]
public sealed class GridVehicleMoverComponent : 
  Component,
  ISerializationGenerated<GridVehicleMoverComponent>,
  ISerializationGenerated
{
  [AutoNetworkedField]
  public Vector2i CurrentTile;
  [AutoNetworkedField]
  public Vector2i TargetTile;
  [AutoNetworkedField]
  public Vector2 Position;
  [AutoNetworkedField]
  public Vector2 TargetPosition;
  [AutoNetworkedField]
  public Vector2i CurrentDirection;
  [AutoNetworkedField]
  public Vector2i PushDirection;
  [AutoNetworkedField]
  public float CurrentSpeed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxSpeed = 11f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Acceleration = 7f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Deceleration = 12f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxReverseSpeed = 4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ReverseAcceleration = 4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FrontOffset;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TileOffsetLimit = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TileOffsetStep = 0.05f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TileOffsetLookahead = 3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float LaneCorrectionSpeed = 4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MovementProbeStep = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MovementCollisionInset = 0.05f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BlockingMobBypassNudgeLimit = 1.75f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BlockingMobBypassNudgeStep = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float PushCooldown = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float PushImpulseSpeed = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TurnDelay = 0.08f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool TurnInPlace;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TurnInPlaceMaxSpeed = 0.35f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TurnNudgeLimit = 0.45f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TurnNudgeStep = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TurnCollisionGraceDistance = 1f;
  [AutoNetworkedField]
  public TimeSpan NextPushTime;
  [AutoNetworkedField]
  public TimeSpan NextTurnTime;
  [AutoNetworkedField]
  public TimeSpan InPlaceTurnBlockUntil;
  [AutoNetworkedField]
  public bool IsCommittedToMove;
  [AutoNetworkedField]
  public bool IsPushMove;
  [AutoNetworkedField]
  public bool IsMoving;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCSizes? XenoBlockMinimumSize;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanXenosPush = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCSizes? XenoPushMinimumSize;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCSizes? XenoMoveMinimumSize;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ContentTileDefinition>? TrackTile;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<ContentTileDefinition>> TrackTileWhitelist = new List<ProtoId<ContentTileDefinition>>();
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<ContentTileDefinition>> TrackTileBlacklist = new List<ProtoId<ContentTileDefinition>>();
  [DataField(null, false, 1, false, false, null)]
  public float TrackTileChance = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float TrackLifetime;
  [DataField(null, false, 1, false, false, null)]
  public bool TrackOnlyDiggable = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanPushVehicles;
  [NonSerialized]
  public EntityUid? SyncedGrid;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TurnSpeedMultiplier = 0.93f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RapidTurnSpeedMultiplier = 0.55f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RapidTurnWindow = 1.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int RapidTurnGraceCount = 2;
  [AutoNetworkedField]
  public int TurnStreak;
  [AutoNetworkedField]
  public TimeSpan LastMovingTurnTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ScrapeDeceleration = 9f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ScrapeMinSpeed = 2.5f;
  [AutoNetworkedField]
  public float SmashSlowdownMultiplier = 1f;
  [AutoNetworkedField]
  public TimeSpan SmashSlowdownUntil;
  [AutoNetworkedField]
  public Angle Heading;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TurnRate = 120f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TurnFullSpeed = 4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float PivotTurnRate = 60f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AllowReverse = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GridVehicleMoverComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GridVehicleMoverComponent) target1;
    if (serialization.TryCustomCopy<GridVehicleMoverComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxSpeed, ref target2, hookCtx, false, context))
      target2 = this.MaxSpeed;
    target.MaxSpeed = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Acceleration, ref target3, hookCtx, false, context))
      target3 = this.Acceleration;
    target.Acceleration = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Deceleration, ref target4, hookCtx, false, context))
      target4 = this.Deceleration;
    target.Deceleration = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxReverseSpeed, ref target5, hookCtx, false, context))
      target5 = this.MaxReverseSpeed;
    target.MaxReverseSpeed = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReverseAcceleration, ref target6, hookCtx, false, context))
      target6 = this.ReverseAcceleration;
    target.ReverseAcceleration = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FrontOffset, ref target7, hookCtx, false, context))
      target7 = this.FrontOffset;
    target.FrontOffset = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TileOffsetLimit, ref target8, hookCtx, false, context))
      target8 = this.TileOffsetLimit;
    target.TileOffsetLimit = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TileOffsetStep, ref target9, hookCtx, false, context))
      target9 = this.TileOffsetStep;
    target.TileOffsetStep = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.TileOffsetLookahead, ref target10, hookCtx, false, context))
      target10 = this.TileOffsetLookahead;
    target.TileOffsetLookahead = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LaneCorrectionSpeed, ref target11, hookCtx, false, context))
      target11 = this.LaneCorrectionSpeed;
    target.LaneCorrectionSpeed = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MovementProbeStep, ref target12, hookCtx, false, context))
      target12 = this.MovementProbeStep;
    target.MovementProbeStep = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MovementCollisionInset, ref target13, hookCtx, false, context))
      target13 = this.MovementCollisionInset;
    target.MovementCollisionInset = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BlockingMobBypassNudgeLimit, ref target14, hookCtx, false, context))
      target14 = this.BlockingMobBypassNudgeLimit;
    target.BlockingMobBypassNudgeLimit = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BlockingMobBypassNudgeStep, ref target15, hookCtx, false, context))
      target15 = this.BlockingMobBypassNudgeStep;
    target.BlockingMobBypassNudgeStep = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PushCooldown, ref target16, hookCtx, false, context))
      target16 = this.PushCooldown;
    target.PushCooldown = target16;
    float target17 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PushImpulseSpeed, ref target17, hookCtx, false, context))
      target17 = this.PushImpulseSpeed;
    target.PushImpulseSpeed = target17;
    float target18 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TurnDelay, ref target18, hookCtx, false, context))
      target18 = this.TurnDelay;
    target.TurnDelay = target18;
    bool target19 = false;
    if (!serialization.TryCustomCopy<bool>(this.TurnInPlace, ref target19, hookCtx, false, context))
      target19 = this.TurnInPlace;
    target.TurnInPlace = target19;
    float target20 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TurnInPlaceMaxSpeed, ref target20, hookCtx, false, context))
      target20 = this.TurnInPlaceMaxSpeed;
    target.TurnInPlaceMaxSpeed = target20;
    float target21 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TurnNudgeLimit, ref target21, hookCtx, false, context))
      target21 = this.TurnNudgeLimit;
    target.TurnNudgeLimit = target21;
    float target22 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TurnNudgeStep, ref target22, hookCtx, false, context))
      target22 = this.TurnNudgeStep;
    target.TurnNudgeStep = target22;
    float target23 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TurnCollisionGraceDistance, ref target23, hookCtx, false, context))
      target23 = this.TurnCollisionGraceDistance;
    target.TurnCollisionGraceDistance = target23;
    RMCSizes? target24 = new RMCSizes?();
    if (!serialization.TryCustomCopy<RMCSizes?>(this.XenoBlockMinimumSize, ref target24, hookCtx, false, context))
      target24 = this.XenoBlockMinimumSize;
    target.XenoBlockMinimumSize = target24;
    bool target25 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanXenosPush, ref target25, hookCtx, false, context))
      target25 = this.CanXenosPush;
    target.CanXenosPush = target25;
    RMCSizes? target26 = new RMCSizes?();
    if (!serialization.TryCustomCopy<RMCSizes?>(this.XenoPushMinimumSize, ref target26, hookCtx, false, context))
      target26 = this.XenoPushMinimumSize;
    target.XenoPushMinimumSize = target26;
    RMCSizes? target27 = new RMCSizes?();
    if (!serialization.TryCustomCopy<RMCSizes?>(this.XenoMoveMinimumSize, ref target27, hookCtx, false, context))
      target27 = this.XenoMoveMinimumSize;
    target.XenoMoveMinimumSize = target27;
    ProtoId<ContentTileDefinition>? target28 = new ProtoId<ContentTileDefinition>?();
    if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>?>(this.TrackTile, ref target28, hookCtx, false, context))
      target28 = serialization.CreateCopy<ProtoId<ContentTileDefinition>?>(this.TrackTile, hookCtx, context);
    target.TrackTile = target28;
    List<ProtoId<ContentTileDefinition>> target29 = (List<ProtoId<ContentTileDefinition>>) null;
    if (this.TrackTileWhitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<ContentTileDefinition>>>(this.TrackTileWhitelist, ref target29, hookCtx, true, context))
      target29 = serialization.CreateCopy<List<ProtoId<ContentTileDefinition>>>(this.TrackTileWhitelist, hookCtx, context);
    target.TrackTileWhitelist = target29;
    List<ProtoId<ContentTileDefinition>> target30 = (List<ProtoId<ContentTileDefinition>>) null;
    if (this.TrackTileBlacklist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<ContentTileDefinition>>>(this.TrackTileBlacklist, ref target30, hookCtx, true, context))
      target30 = serialization.CreateCopy<List<ProtoId<ContentTileDefinition>>>(this.TrackTileBlacklist, hookCtx, context);
    target.TrackTileBlacklist = target30;
    float target31 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TrackTileChance, ref target31, hookCtx, false, context))
      target31 = this.TrackTileChance;
    target.TrackTileChance = target31;
    float target32 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TrackLifetime, ref target32, hookCtx, false, context))
      target32 = this.TrackLifetime;
    target.TrackLifetime = target32;
    bool target33 = false;
    if (!serialization.TryCustomCopy<bool>(this.TrackOnlyDiggable, ref target33, hookCtx, false, context))
      target33 = this.TrackOnlyDiggable;
    target.TrackOnlyDiggable = target33;
    bool target34 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanPushVehicles, ref target34, hookCtx, false, context))
      target34 = this.CanPushVehicles;
    target.CanPushVehicles = target34;
    float target35 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TurnSpeedMultiplier, ref target35, hookCtx, false, context))
      target35 = this.TurnSpeedMultiplier;
    target.TurnSpeedMultiplier = target35;
    float target36 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RapidTurnSpeedMultiplier, ref target36, hookCtx, false, context))
      target36 = this.RapidTurnSpeedMultiplier;
    target.RapidTurnSpeedMultiplier = target36;
    float target37 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RapidTurnWindow, ref target37, hookCtx, false, context))
      target37 = this.RapidTurnWindow;
    target.RapidTurnWindow = target37;
    int target38 = 0;
    if (!serialization.TryCustomCopy<int>(this.RapidTurnGraceCount, ref target38, hookCtx, false, context))
      target38 = this.RapidTurnGraceCount;
    target.RapidTurnGraceCount = target38;
    float target39 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ScrapeDeceleration, ref target39, hookCtx, false, context))
      target39 = this.ScrapeDeceleration;
    target.ScrapeDeceleration = target39;
    float target40 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ScrapeMinSpeed, ref target40, hookCtx, false, context))
      target40 = this.ScrapeMinSpeed;
    target.ScrapeMinSpeed = target40;
    float target41 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TurnRate, ref target41, hookCtx, false, context))
      target41 = this.TurnRate;
    target.TurnRate = target41;
    float target42 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TurnFullSpeed, ref target42, hookCtx, false, context))
      target42 = this.TurnFullSpeed;
    target.TurnFullSpeed = target42;
    float target43 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PivotTurnRate, ref target43, hookCtx, false, context))
      target43 = this.PivotTurnRate;
    target.PivotTurnRate = target43;
    bool target44 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowReverse, ref target44, hookCtx, false, context))
      target44 = this.AllowReverse;
    target.AllowReverse = target44;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GridVehicleMoverComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GridVehicleMoverComponent target1 = (GridVehicleMoverComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GridVehicleMoverComponent target1 = (GridVehicleMoverComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GridVehicleMoverComponent target1 = (GridVehicleMoverComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual GridVehicleMoverComponent Component.Instantiate() => new GridVehicleMoverComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GridVehicleMoverComponent_AutoState : IComponentState
  {
    public Vector2i CurrentTile;
    public Vector2i TargetTile;
    public Vector2 Position;
    public Vector2 TargetPosition;
    public Vector2i CurrentDirection;
    public Vector2i PushDirection;
    public float CurrentSpeed;
    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;
    public float MaxReverseSpeed;
    public float ReverseAcceleration;
    public float FrontOffset;
    public float TileOffsetLimit;
    public float TileOffsetStep;
    public int TileOffsetLookahead;
    public float LaneCorrectionSpeed;
    public float MovementProbeStep;
    public float MovementCollisionInset;
    public float BlockingMobBypassNudgeLimit;
    public float BlockingMobBypassNudgeStep;
    public float PushCooldown;
    public float PushImpulseSpeed;
    public float TurnDelay;
    public bool TurnInPlace;
    public float TurnInPlaceMaxSpeed;
    public float TurnNudgeLimit;
    public float TurnNudgeStep;
    public float TurnCollisionGraceDistance;
    public TimeSpan NextPushTime;
    public TimeSpan NextTurnTime;
    public TimeSpan InPlaceTurnBlockUntil;
    public bool IsCommittedToMove;
    public bool IsPushMove;
    public bool IsMoving;
    public RMCSizes? XenoBlockMinimumSize;
    public bool CanXenosPush;
    public RMCSizes? XenoPushMinimumSize;
    public RMCSizes? XenoMoveMinimumSize;
    public bool CanPushVehicles;
    public float TurnSpeedMultiplier;
    public float RapidTurnSpeedMultiplier;
    public float RapidTurnWindow;
    public int RapidTurnGraceCount;
    public int TurnStreak;
    public TimeSpan LastMovingTurnTime;
    public float ScrapeDeceleration;
    public float ScrapeMinSpeed;
    public float SmashSlowdownMultiplier;
    public TimeSpan SmashSlowdownUntil;
    public Angle Heading;
    public float TurnRate;
    public float TurnFullSpeed;
    public float PivotTurnRate;
    public bool AllowReverse;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GridVehicleMoverComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GridVehicleMoverComponent, ComponentGetState>(new ComponentEventRefHandler<GridVehicleMoverComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GridVehicleMoverComponent, ComponentHandleState>(new ComponentEventRefHandler<GridVehicleMoverComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GridVehicleMoverComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GridVehicleMoverComponent.GridVehicleMoverComponent_AutoState()
      {
        CurrentTile = component.CurrentTile,
        TargetTile = component.TargetTile,
        Position = component.Position,
        TargetPosition = component.TargetPosition,
        CurrentDirection = component.CurrentDirection,
        PushDirection = component.PushDirection,
        CurrentSpeed = component.CurrentSpeed,
        MaxSpeed = component.MaxSpeed,
        Acceleration = component.Acceleration,
        Deceleration = component.Deceleration,
        MaxReverseSpeed = component.MaxReverseSpeed,
        ReverseAcceleration = component.ReverseAcceleration,
        FrontOffset = component.FrontOffset,
        TileOffsetLimit = component.TileOffsetLimit,
        TileOffsetStep = component.TileOffsetStep,
        TileOffsetLookahead = component.TileOffsetLookahead,
        LaneCorrectionSpeed = component.LaneCorrectionSpeed,
        MovementProbeStep = component.MovementProbeStep,
        MovementCollisionInset = component.MovementCollisionInset,
        BlockingMobBypassNudgeLimit = component.BlockingMobBypassNudgeLimit,
        BlockingMobBypassNudgeStep = component.BlockingMobBypassNudgeStep,
        PushCooldown = component.PushCooldown,
        PushImpulseSpeed = component.PushImpulseSpeed,
        TurnDelay = component.TurnDelay,
        TurnInPlace = component.TurnInPlace,
        TurnInPlaceMaxSpeed = component.TurnInPlaceMaxSpeed,
        TurnNudgeLimit = component.TurnNudgeLimit,
        TurnNudgeStep = component.TurnNudgeStep,
        TurnCollisionGraceDistance = component.TurnCollisionGraceDistance,
        NextPushTime = component.NextPushTime,
        NextTurnTime = component.NextTurnTime,
        InPlaceTurnBlockUntil = component.InPlaceTurnBlockUntil,
        IsCommittedToMove = component.IsCommittedToMove,
        IsPushMove = component.IsPushMove,
        IsMoving = component.IsMoving,
        XenoBlockMinimumSize = component.XenoBlockMinimumSize,
        CanXenosPush = component.CanXenosPush,
        XenoPushMinimumSize = component.XenoPushMinimumSize,
        XenoMoveMinimumSize = component.XenoMoveMinimumSize,
        CanPushVehicles = component.CanPushVehicles,
        TurnSpeedMultiplier = component.TurnSpeedMultiplier,
        RapidTurnSpeedMultiplier = component.RapidTurnSpeedMultiplier,
        RapidTurnWindow = component.RapidTurnWindow,
        RapidTurnGraceCount = component.RapidTurnGraceCount,
        TurnStreak = component.TurnStreak,
        LastMovingTurnTime = component.LastMovingTurnTime,
        ScrapeDeceleration = component.ScrapeDeceleration,
        ScrapeMinSpeed = component.ScrapeMinSpeed,
        SmashSlowdownMultiplier = component.SmashSlowdownMultiplier,
        SmashSlowdownUntil = component.SmashSlowdownUntil,
        Heading = component.Heading,
        TurnRate = component.TurnRate,
        TurnFullSpeed = component.TurnFullSpeed,
        PivotTurnRate = component.PivotTurnRate,
        AllowReverse = component.AllowReverse
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GridVehicleMoverComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GridVehicleMoverComponent.GridVehicleMoverComponent_AutoState current))
        return;
      component.CurrentTile = current.CurrentTile;
      component.TargetTile = current.TargetTile;
      component.Position = current.Position;
      component.TargetPosition = current.TargetPosition;
      component.CurrentDirection = current.CurrentDirection;
      component.PushDirection = current.PushDirection;
      component.CurrentSpeed = current.CurrentSpeed;
      component.MaxSpeed = current.MaxSpeed;
      component.Acceleration = current.Acceleration;
      component.Deceleration = current.Deceleration;
      component.MaxReverseSpeed = current.MaxReverseSpeed;
      component.ReverseAcceleration = current.ReverseAcceleration;
      component.FrontOffset = current.FrontOffset;
      component.TileOffsetLimit = current.TileOffsetLimit;
      component.TileOffsetStep = current.TileOffsetStep;
      component.TileOffsetLookahead = current.TileOffsetLookahead;
      component.LaneCorrectionSpeed = current.LaneCorrectionSpeed;
      component.MovementProbeStep = current.MovementProbeStep;
      component.MovementCollisionInset = current.MovementCollisionInset;
      component.BlockingMobBypassNudgeLimit = current.BlockingMobBypassNudgeLimit;
      component.BlockingMobBypassNudgeStep = current.BlockingMobBypassNudgeStep;
      component.PushCooldown = current.PushCooldown;
      component.PushImpulseSpeed = current.PushImpulseSpeed;
      component.TurnDelay = current.TurnDelay;
      component.TurnInPlace = current.TurnInPlace;
      component.TurnInPlaceMaxSpeed = current.TurnInPlaceMaxSpeed;
      component.TurnNudgeLimit = current.TurnNudgeLimit;
      component.TurnNudgeStep = current.TurnNudgeStep;
      component.TurnCollisionGraceDistance = current.TurnCollisionGraceDistance;
      component.NextPushTime = current.NextPushTime;
      component.NextTurnTime = current.NextTurnTime;
      component.InPlaceTurnBlockUntil = current.InPlaceTurnBlockUntil;
      component.IsCommittedToMove = current.IsCommittedToMove;
      component.IsPushMove = current.IsPushMove;
      component.IsMoving = current.IsMoving;
      component.XenoBlockMinimumSize = current.XenoBlockMinimumSize;
      component.CanXenosPush = current.CanXenosPush;
      component.XenoPushMinimumSize = current.XenoPushMinimumSize;
      component.XenoMoveMinimumSize = current.XenoMoveMinimumSize;
      component.CanPushVehicles = current.CanPushVehicles;
      component.TurnSpeedMultiplier = current.TurnSpeedMultiplier;
      component.RapidTurnSpeedMultiplier = current.RapidTurnSpeedMultiplier;
      component.RapidTurnWindow = current.RapidTurnWindow;
      component.RapidTurnGraceCount = current.RapidTurnGraceCount;
      component.TurnStreak = current.TurnStreak;
      component.LastMovingTurnTime = current.LastMovingTurnTime;
      component.ScrapeDeceleration = current.ScrapeDeceleration;
      component.ScrapeMinSpeed = current.ScrapeMinSpeed;
      component.SmashSlowdownMultiplier = current.SmashSlowdownMultiplier;
      component.SmashSlowdownUntil = current.SmashSlowdownUntil;
      component.Heading = current.Heading;
      component.TurnRate = current.TurnRate;
      component.TurnFullSpeed = current.TurnFullSpeed;
      component.PivotTurnRate = current.PivotTurnRate;
      component.AllowReverse = current.AllowReverse;
    }
  }
}

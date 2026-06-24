// Decompiled with JetBrains decompiler
// Type: Content.Shared.Shuttles.Components.FTLComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Shuttles.Systems;
using Content.Shared.Tag;
using Content.Shared.Timing;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Shuttles.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class FTLComponent : 
  Component,
  ISerializationGenerated<FTLComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FTLState State;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public StartEndTime StateTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StartupTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TravelTime;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? VisualizerProto;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? VisualizerEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates TargetCoordinates;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle TargetAngle;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<TagPrototype>? PriorityTag;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundTravel", false, 1, false, false, null)]
  public SoundSpecifier? TravelSound;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? StartupStream;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? TravelStream;

  public FTLComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Effects/Shuttle/hyperspace_progress.ogg");
    soundPathSpecifier.Params = AudioParams.Default.WithVolume(-3f).WithLoop(true);
    this.TravelSound = (SoundSpecifier) soundPathSpecifier;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FTLComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FTLComponent) target1;
    if (serialization.TryCustomCopy<FTLComponent>(this, ref target, hookCtx, false, context))
      return;
    FTLState target2 = FTLState.Invalid;
    if (!serialization.TryCustomCopy<FTLState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    StartEndTime target3 = new StartEndTime();
    if (!serialization.TryCustomCopy<StartEndTime>(this.StateTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<StartEndTime>(this.StateTime, hookCtx, context);
    target.StateTime = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StartupTime, ref target4, hookCtx, false, context))
      target4 = this.StartupTime;
    target.StartupTime = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TravelTime, ref target5, hookCtx, false, context))
      target5 = this.TravelTime;
    target.TravelTime = target5;
    EntProtoId? target6 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.VisualizerProto, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId?>(this.VisualizerProto, hookCtx, context);
    target.VisualizerProto = target6;
    EntityUid? target7 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.VisualizerEntity, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityUid?>(this.VisualizerEntity, hookCtx, context);
    target.VisualizerEntity = target7;
    EntityCoordinates target8 = new EntityCoordinates();
    if (!serialization.TryCustomCopy<EntityCoordinates>(this.TargetCoordinates, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntityCoordinates>(this.TargetCoordinates, hookCtx, context);
    target.TargetCoordinates = target8;
    Angle target9 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.TargetAngle, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<Angle>(this.TargetAngle, hookCtx, context);
    target.TargetAngle = target9;
    ProtoId<TagPrototype>? target10 = new ProtoId<TagPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<TagPrototype>?>(this.PriorityTag, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<ProtoId<TagPrototype>?>(this.PriorityTag, hookCtx, context);
    target.PriorityTag = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TravelSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.TravelSound, hookCtx, context);
    target.TravelSound = target11;
    EntityUid? target12 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.StartupStream, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntityUid?>(this.StartupStream, hookCtx, context);
    target.StartupStream = target12;
    EntityUid? target13 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.TravelStream, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntityUid?>(this.TravelStream, hookCtx, context);
    target.TravelStream = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FTLComponent target,
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
    FTLComponent target1 = (FTLComponent) target;
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
    FTLComponent target1 = (FTLComponent) target;
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
    FTLComponent target1 = (FTLComponent) target;
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
  virtual FTLComponent Component.Instantiate() => new FTLComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FTLComponent_AutoState : IComponentState
  {
    public FTLState State;
    public StartEndTime StateTime;
    public float StartupTime;
    public float TravelTime;
    public NetEntity? VisualizerEntity;
    public NetCoordinates TargetCoordinates;
    public Angle TargetAngle;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FTLComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FTLComponent, ComponentGetState>(new ComponentEventRefHandler<FTLComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FTLComponent, ComponentHandleState>(new ComponentEventRefHandler<FTLComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, FTLComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new FTLComponent.FTLComponent_AutoState()
      {
        State = component.State,
        StateTime = component.StateTime,
        StartupTime = component.StartupTime,
        TravelTime = component.TravelTime,
        VisualizerEntity = this.GetNetEntity(component.VisualizerEntity),
        TargetCoordinates = this.GetNetCoordinates(component.TargetCoordinates),
        TargetAngle = component.TargetAngle
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FTLComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FTLComponent.FTLComponent_AutoState current))
        return;
      component.State = current.State;
      component.StateTime = current.StateTime;
      component.StartupTime = current.StartupTime;
      component.TravelTime = current.TravelTime;
      component.VisualizerEntity = this.EnsureEntity<FTLComponent>(current.VisualizerEntity, uid);
      component.TargetCoordinates = this.EnsureCoordinates<FTLComponent>(current.TargetCoordinates, uid);
      component.TargetAngle = current.TargetAngle;
    }
  }
}

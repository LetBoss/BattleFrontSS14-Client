// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sentry.TeslaCoil.RMCTeslaCoilComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Sentry.TeslaCoil;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (TeslaCoilSystem)})]
public sealed class RMCTeslaCoilComponent : 
  Component,
  ISerializationGenerated<RMCTeslaCoilComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  public TimeSpan LastFired;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FireDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxTargets = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId TeslaBeamProto = (EntProtoId) "EffectTeslaBeam";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunDuration = TimeSpan.FromSeconds(0L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DazeDuration = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowDuration = TimeSpan.FromSeconds(6L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCTeslaCoilComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCTeslaCoilComponent) target1;
    if (serialization.TryCustomCopy<RMCTeslaCoilComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastFired, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.LastFired, hookCtx, context);
    target.LastFired = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FireDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.FireDelay, hookCtx, context);
    target.FireDelay = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target4, hookCtx, false, context))
      target4 = this.Range;
    target.Range = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxTargets, ref target5, hookCtx, false, context))
      target5 = this.MaxTargets;
    target.MaxTargets = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.TeslaBeamProto, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.TeslaBeamProto, hookCtx, context);
    target.TeslaBeamProto = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunDuration, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.StunDuration, hookCtx, context);
    target.StunDuration = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DazeDuration, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.DazeDuration, hookCtx, context);
    target.DazeDuration = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowDuration, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.SlowDuration, hookCtx, context);
    target.SlowDuration = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCTeslaCoilComponent target,
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
    RMCTeslaCoilComponent target1 = (RMCTeslaCoilComponent) target;
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
    RMCTeslaCoilComponent target1 = (RMCTeslaCoilComponent) target;
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
    RMCTeslaCoilComponent target1 = (RMCTeslaCoilComponent) target;
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
  virtual RMCTeslaCoilComponent Component.Instantiate() => new RMCTeslaCoilComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCTeslaCoilComponent_AutoState : IComponentState
  {
    public TimeSpan LastFired;
    public TimeSpan FireDelay;
    public float Range;
    public int MaxTargets;
    public EntProtoId TeslaBeamProto;
    public TimeSpan StunDuration;
    public TimeSpan DazeDuration;
    public TimeSpan SlowDuration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCTeslaCoilComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCTeslaCoilComponent, ComponentGetState>(new ComponentEventRefHandler<RMCTeslaCoilComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCTeslaCoilComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCTeslaCoilComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCTeslaCoilComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCTeslaCoilComponent.RMCTeslaCoilComponent_AutoState()
      {
        LastFired = component.LastFired,
        FireDelay = component.FireDelay,
        Range = component.Range,
        MaxTargets = component.MaxTargets,
        TeslaBeamProto = component.TeslaBeamProto,
        StunDuration = component.StunDuration,
        DazeDuration = component.DazeDuration,
        SlowDuration = component.SlowDuration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCTeslaCoilComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCTeslaCoilComponent.RMCTeslaCoilComponent_AutoState current))
        return;
      component.LastFired = current.LastFired;
      component.FireDelay = current.FireDelay;
      component.Range = current.Range;
      component.MaxTargets = current.MaxTargets;
      component.TeslaBeamProto = current.TeslaBeamProto;
      component.StunDuration = current.StunDuration;
      component.DazeDuration = current.DazeDuration;
      component.SlowDuration = current.SlowDuration;
    }
  }
}

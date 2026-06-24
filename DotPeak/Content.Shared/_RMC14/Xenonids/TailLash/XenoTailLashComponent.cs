// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Tail_Lash.XenoTailLashComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Tail_Lash;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoTailLashComponent : 
  Component,
  ISerializationGenerated<XenoTailLashComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Cost = (FixedPoint2) 80 /*0x50*/;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Windup = TimeSpan.FromSeconds(0.2);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(13L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Width = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Height = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FlingDistance = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunTime = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowTime = TimeSpan.FromSeconds(2.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Effect = (EntProtoId) "RMCEffectXenoTelegraphLash";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId EffectEdge = (EntProtoId) "RMCEffectXenoTelegraphLashAnim";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Box2Rotated? Area;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoTailLashComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoTailLashComponent) target1;
    if (serialization.TryCustomCopy<XenoTailLashComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Cost, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Cost, hookCtx, context);
    target.Cost = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Windup, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Windup, hookCtx, context);
    target.Windup = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Width, ref target5, hookCtx, false, context))
      target5 = this.Width;
    target.Width = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Height, ref target6, hookCtx, false, context))
      target6 = this.Height;
    target.Height = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FlingDistance, ref target7, hookCtx, false, context))
      target7 = this.FlingDistance;
    target.FlingDistance = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunTime, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.StunTime, hookCtx, context);
    target.StunTime = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.SlowTime, hookCtx, context);
    target.SlowTime = target9;
    EntProtoId target10 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target10;
    EntProtoId target11 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.EffectEdge, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId>(this.EffectEdge, hookCtx, context);
    target.EffectEdge = target11;
    Box2Rotated? target12 = new Box2Rotated?();
    if (!serialization.TryCustomCopy<Box2Rotated?>(this.Area, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<Box2Rotated?>(this.Area, hookCtx, context);
    target.Area = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoTailLashComponent target,
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
    XenoTailLashComponent target1 = (XenoTailLashComponent) target;
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
    XenoTailLashComponent target1 = (XenoTailLashComponent) target;
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
    XenoTailLashComponent target1 = (XenoTailLashComponent) target;
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
  virtual XenoTailLashComponent Component.Instantiate() => new XenoTailLashComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoTailLashComponent_AutoState : IComponentState
  {
    public FixedPoint2 Cost;
    public TimeSpan Windup;
    public TimeSpan Cooldown;
    public float Width;
    public float Height;
    public float FlingDistance;
    public TimeSpan StunTime;
    public TimeSpan SlowTime;
    public EntProtoId Effect;
    public EntProtoId EffectEdge;
    public Box2Rotated? Area;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoTailLashComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoTailLashComponent, ComponentGetState>(new ComponentEventRefHandler<XenoTailLashComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoTailLashComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoTailLashComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoTailLashComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoTailLashComponent.XenoTailLashComponent_AutoState()
      {
        Cost = component.Cost,
        Windup = component.Windup,
        Cooldown = component.Cooldown,
        Width = component.Width,
        Height = component.Height,
        FlingDistance = component.FlingDistance,
        StunTime = component.StunTime,
        SlowTime = component.SlowTime,
        Effect = component.Effect,
        EffectEdge = component.EffectEdge,
        Area = component.Area
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoTailLashComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoTailLashComponent.XenoTailLashComponent_AutoState current))
        return;
      component.Cost = current.Cost;
      component.Windup = current.Windup;
      component.Cooldown = current.Cooldown;
      component.Width = current.Width;
      component.Height = current.Height;
      component.FlingDistance = current.FlingDistance;
      component.StunTime = current.StunTime;
      component.SlowTime = current.SlowTime;
      component.Effect = current.Effect;
      component.EffectEdge = current.EffectEdge;
      component.Area = current.Area;
    }
  }
}

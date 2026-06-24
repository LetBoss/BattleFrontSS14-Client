// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Invisibility.XenoTurnInvisibleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Invisibility;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoInvisibilitySystem)})]
public sealed class XenoTurnInvisibleComponent : 
  Component,
  ISerializationGenerated<XenoTurnInvisibleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = FixedPoint2.New(20);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FullCooldown = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ToggleLockoutTime = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Opacity = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ManualRefundMultiplier = 0.9f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RevealedRefundMultiplier = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 SpeedMultiplier = FixedPoint2.New(1.15);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoTurnInvisibleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoTurnInvisibleComponent) target1;
    if (serialization.TryCustomCopy<XenoTurnInvisibleComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FullCooldown, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.FullCooldown, hookCtx, context);
    target.FullCooldown = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ToggleLockoutTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.ToggleLockoutTime, hookCtx, context);
    target.ToggleLockoutTime = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Opacity, ref target6, hookCtx, false, context))
      target6 = this.Opacity;
    target.Opacity = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ManualRefundMultiplier, ref target7, hookCtx, false, context))
      target7 = this.ManualRefundMultiplier;
    target.ManualRefundMultiplier = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RevealedRefundMultiplier, ref target8, hookCtx, false, context))
      target8 = this.RevealedRefundMultiplier;
    target.RevealedRefundMultiplier = target8;
    FixedPoint2 target9 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.SpeedMultiplier, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<FixedPoint2>(this.SpeedMultiplier, hookCtx, context);
    target.SpeedMultiplier = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoTurnInvisibleComponent target,
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
    XenoTurnInvisibleComponent target1 = (XenoTurnInvisibleComponent) target;
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
    XenoTurnInvisibleComponent target1 = (XenoTurnInvisibleComponent) target;
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
    XenoTurnInvisibleComponent target1 = (XenoTurnInvisibleComponent) target;
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
  virtual XenoTurnInvisibleComponent Component.Instantiate() => new XenoTurnInvisibleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoTurnInvisibleComponent_AutoState : IComponentState
  {
    public FixedPoint2 PlasmaCost;
    public TimeSpan Duration;
    public TimeSpan FullCooldown;
    public TimeSpan ToggleLockoutTime;
    public float Opacity;
    public float ManualRefundMultiplier;
    public float RevealedRefundMultiplier;
    public FixedPoint2 SpeedMultiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoTurnInvisibleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoTurnInvisibleComponent, ComponentGetState>(new ComponentEventRefHandler<XenoTurnInvisibleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoTurnInvisibleComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoTurnInvisibleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoTurnInvisibleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoTurnInvisibleComponent.XenoTurnInvisibleComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        Duration = component.Duration,
        FullCooldown = component.FullCooldown,
        ToggleLockoutTime = component.ToggleLockoutTime,
        Opacity = component.Opacity,
        ManualRefundMultiplier = component.ManualRefundMultiplier,
        RevealedRefundMultiplier = component.RevealedRefundMultiplier,
        SpeedMultiplier = component.SpeedMultiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoTurnInvisibleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoTurnInvisibleComponent.XenoTurnInvisibleComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.Duration = current.Duration;
      component.FullCooldown = current.FullCooldown;
      component.ToggleLockoutTime = current.ToggleLockoutTime;
      component.Opacity = current.Opacity;
      component.ManualRefundMultiplier = current.ManualRefundMultiplier;
      component.RevealedRefundMultiplier = current.RevealedRefundMultiplier;
      component.SpeedMultiplier = current.SpeedMultiplier;
    }
  }
}

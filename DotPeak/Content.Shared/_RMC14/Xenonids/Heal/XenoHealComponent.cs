// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Heal.XenoHealComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
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
namespace Content.Shared._RMC14.Xenonids.Heal;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoHealSystem)})]
public sealed class XenoHealComponent : 
  Component,
  ISerializationGenerated<XenoHealComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? HealEffect = (EntProtoId?) "RMCEffectHealQueen";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Radius = 4;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Percentage = (FixedPoint2) 0.3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TimeBetweenHeals = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextHeal;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoHealComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoHealComponent) target1;
    if (serialization.TryCustomCopy<XenoHealComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target2 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.HealEffect, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId?>(this.HealEffect, hookCtx, context);
    target.HealEffect = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Radius, ref target3, hookCtx, false, context))
      target3 = this.Radius;
    target.Radius = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Percentage, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.Percentage, hookCtx, context);
    target.Percentage = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeBetweenHeals, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.TimeBetweenHeals, hookCtx, context);
    target.TimeBetweenHeals = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextHeal, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.NextHeal, hookCtx, context);
    target.NextHeal = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoHealComponent target,
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
    XenoHealComponent target1 = (XenoHealComponent) target;
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
    XenoHealComponent target1 = (XenoHealComponent) target;
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
    XenoHealComponent target1 = (XenoHealComponent) target;
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
  virtual XenoHealComponent Component.Instantiate() => new XenoHealComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoHealComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoHealComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoHealComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoHealComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextHeal += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoHealComponent_AutoState : IComponentState
  {
    public EntProtoId? HealEffect;
    public int Radius;
    public FixedPoint2 Percentage;
    public TimeSpan Duration;
    public TimeSpan TimeBetweenHeals;
    public TimeSpan NextHeal;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoHealComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoHealComponent, ComponentGetState>(new ComponentEventRefHandler<XenoHealComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoHealComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoHealComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, 
    #nullable enable
    XenoHealComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoHealComponent.XenoHealComponent_AutoState()
      {
        HealEffect = component.HealEffect,
        Radius = component.Radius,
        Percentage = component.Percentage,
        Duration = component.Duration,
        TimeBetweenHeals = component.TimeBetweenHeals,
        NextHeal = component.NextHeal
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoHealComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoHealComponent.XenoHealComponent_AutoState current))
        return;
      component.HealEffect = current.HealEffect;
      component.Radius = current.Radius;
      component.Percentage = current.Percentage;
      component.Duration = current.Duration;
      component.TimeBetweenHeals = current.TimeBetweenHeals;
      component.NextHeal = current.NextHeal;
    }
  }
}

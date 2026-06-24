// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fruit.Components.XenoFruitEffectRegenComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Fruit.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoFruitSystem)})]
public sealed class XenoFruitEffectRegenComponent : 
  Component,
  ISerializationGenerated<XenoFruitEffectRegenComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 RegenPerTick;
  [DataField(null, false, 1, false, false, null)]
  public int? TicksLeft;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? NextTickAt;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan TickPeriod = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public int TickCount;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFruitEffectRegenComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFruitEffectRegenComponent) target1;
    if (serialization.TryCustomCopy<XenoFruitEffectRegenComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.RegenPerTick, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.RegenPerTick, hookCtx, context);
    target.RegenPerTick = target2;
    int? target3 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.TicksLeft, ref target3, hookCtx, false, context))
      target3 = this.TicksLeft;
    target.TicksLeft = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextTickAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.NextTickAt, hookCtx, context);
    target.NextTickAt = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TickPeriod, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.TickPeriod, hookCtx, context);
    target.TickPeriod = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.TickCount, ref target6, hookCtx, false, context))
      target6 = this.TickCount;
    target.TickCount = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFruitEffectRegenComponent target,
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
    XenoFruitEffectRegenComponent target1 = (XenoFruitEffectRegenComponent) target;
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
    XenoFruitEffectRegenComponent target1 = (XenoFruitEffectRegenComponent) target;
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
    XenoFruitEffectRegenComponent target1 = (XenoFruitEffectRegenComponent) target;
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
  virtual XenoFruitEffectRegenComponent Component.Instantiate()
  {
    return new XenoFruitEffectRegenComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFruitEffectRegenComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFruitEffectRegenComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoFruitEffectRegenComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoFruitEffectRegenComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.NextTickAt.HasValue)
        component.NextTickAt = new TimeSpan?(component.NextTickAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFruitEffectRegenComponent_AutoState : IComponentState
  {
    public FixedPoint2 RegenPerTick;
    public TimeSpan? NextTickAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFruitEffectRegenComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFruitEffectRegenComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFruitEffectRegenComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFruitEffectRegenComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFruitEffectRegenComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      XenoFruitEffectRegenComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFruitEffectRegenComponent.XenoFruitEffectRegenComponent_AutoState()
      {
        RegenPerTick = component.RegenPerTick,
        NextTickAt = component.NextTickAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFruitEffectRegenComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFruitEffectRegenComponent.XenoFruitEffectRegenComponent_AutoState current))
        return;
      component.RegenPerTick = current.RegenPerTick;
      component.NextTickAt = current.NextTickAt;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, XenoFruitEffectRegenComponent>(uid, component, ref args1);
    }
  }
}

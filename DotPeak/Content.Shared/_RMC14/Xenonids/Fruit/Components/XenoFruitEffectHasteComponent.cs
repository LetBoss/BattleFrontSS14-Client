// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fruit.Components.XenoFruitEffectHasteComponent
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
public sealed class XenoFruitEffectHasteComponent : 
  Component,
  ISerializationGenerated<XenoFruitEffectHasteComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ReductionMax;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ReductionPerSlash;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ReductionCurrent;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? EndAt;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Duration = TimeSpan.FromSeconds(0L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFruitEffectHasteComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFruitEffectHasteComponent) target1;
    if (serialization.TryCustomCopy<XenoFruitEffectHasteComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ReductionMax, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.ReductionMax, hookCtx, context);
    target.ReductionMax = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ReductionPerSlash, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.ReductionPerSlash, hookCtx, context);
    target.ReductionPerSlash = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ReductionCurrent, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.ReductionCurrent, hookCtx, context);
    target.ReductionCurrent = target4;
    TimeSpan? target5 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EndAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan?>(this.EndAt, hookCtx, context);
    target.EndAt = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFruitEffectHasteComponent target,
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
    XenoFruitEffectHasteComponent target1 = (XenoFruitEffectHasteComponent) target;
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
    XenoFruitEffectHasteComponent target1 = (XenoFruitEffectHasteComponent) target;
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
    XenoFruitEffectHasteComponent target1 = (XenoFruitEffectHasteComponent) target;
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
  virtual XenoFruitEffectHasteComponent Component.Instantiate()
  {
    return new XenoFruitEffectHasteComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFruitEffectHasteComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFruitEffectHasteComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoFruitEffectHasteComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoFruitEffectHasteComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.EndAt.HasValue)
        component.EndAt = new TimeSpan?(component.EndAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFruitEffectHasteComponent_AutoState : IComponentState
  {
    public FixedPoint2 ReductionMax;
    public FixedPoint2 ReductionPerSlash;
    public FixedPoint2 ReductionCurrent;
    public TimeSpan? EndAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFruitEffectHasteComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFruitEffectHasteComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFruitEffectHasteComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFruitEffectHasteComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFruitEffectHasteComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      XenoFruitEffectHasteComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFruitEffectHasteComponent.XenoFruitEffectHasteComponent_AutoState()
      {
        ReductionMax = component.ReductionMax,
        ReductionPerSlash = component.ReductionPerSlash,
        ReductionCurrent = component.ReductionCurrent,
        EndAt = component.EndAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFruitEffectHasteComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFruitEffectHasteComponent.XenoFruitEffectHasteComponent_AutoState current))
        return;
      component.ReductionMax = current.ReductionMax;
      component.ReductionPerSlash = current.ReductionPerSlash;
      component.ReductionCurrent = current.ReductionCurrent;
      component.EndAt = current.EndAt;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, XenoFruitEffectHasteComponent>(uid, component, ref args1);
    }
  }
}

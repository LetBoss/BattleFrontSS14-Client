// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fruit.Components.XenoFruitHasteComponent
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
namespace Content.Shared._RMC14.Xenonids.Fruit.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedXenoFruitSystem)})]
public sealed class XenoFruitHasteComponent : 
  Component,
  ISerializationGenerated<XenoFruitHasteComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ReductionMax = (FixedPoint2) 0.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ReductionPerSlash = (FixedPoint2) 0.05f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Duration = TimeSpan.FromSeconds(0L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFruitHasteComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFruitHasteComponent) target1;
    if (serialization.TryCustomCopy<XenoFruitHasteComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ReductionMax, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.ReductionMax, hookCtx, context);
    target.ReductionMax = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ReductionPerSlash, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.ReductionPerSlash, hookCtx, context);
    target.ReductionPerSlash = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFruitHasteComponent target,
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
    XenoFruitHasteComponent target1 = (XenoFruitHasteComponent) target;
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
    XenoFruitHasteComponent target1 = (XenoFruitHasteComponent) target;
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
    XenoFruitHasteComponent target1 = (XenoFruitHasteComponent) target;
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
  virtual XenoFruitHasteComponent Component.Instantiate() => new XenoFruitHasteComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFruitHasteComponent_AutoState : IComponentState
  {
    public FixedPoint2 ReductionMax;
    public FixedPoint2 ReductionPerSlash;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFruitHasteComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFruitHasteComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFruitHasteComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFruitHasteComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFruitHasteComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoFruitHasteComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFruitHasteComponent.XenoFruitHasteComponent_AutoState()
      {
        ReductionMax = component.ReductionMax,
        ReductionPerSlash = component.ReductionPerSlash
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFruitHasteComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFruitHasteComponent.XenoFruitHasteComponent_AutoState current))
        return;
      component.ReductionMax = current.ReductionMax;
      component.ReductionPerSlash = current.ReductionPerSlash;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, XenoFruitHasteComponent>(uid, component, ref args1);
    }
  }
}

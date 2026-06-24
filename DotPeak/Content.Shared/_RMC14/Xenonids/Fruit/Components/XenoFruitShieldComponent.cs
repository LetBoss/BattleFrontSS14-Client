// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fruit.Components.XenoFruitShieldComponent
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
public sealed class XenoFruitShieldComponent : 
  Component,
  ISerializationGenerated<XenoFruitShieldComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ShieldAmount = (FixedPoint2) 200;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ShieldRatio = (FixedPoint2) 0.3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ShieldDecay = (FixedPoint2) 10f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Duration = TimeSpan.FromSeconds(0L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFruitShieldComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFruitShieldComponent) target1;
    if (serialization.TryCustomCopy<XenoFruitShieldComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ShieldAmount, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.ShieldAmount, hookCtx, context);
    target.ShieldAmount = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ShieldRatio, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.ShieldRatio, hookCtx, context);
    target.ShieldRatio = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ShieldDecay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.ShieldDecay, hookCtx, context);
    target.ShieldDecay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFruitShieldComponent target,
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
    XenoFruitShieldComponent target1 = (XenoFruitShieldComponent) target;
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
    XenoFruitShieldComponent target1 = (XenoFruitShieldComponent) target;
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
    XenoFruitShieldComponent target1 = (XenoFruitShieldComponent) target;
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
  virtual XenoFruitShieldComponent Component.Instantiate() => new XenoFruitShieldComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFruitShieldComponent_AutoState : IComponentState
  {
    public FixedPoint2 ShieldAmount;
    public FixedPoint2 ShieldRatio;
    public FixedPoint2 ShieldDecay;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFruitShieldComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFruitShieldComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFruitShieldComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFruitShieldComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFruitShieldComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoFruitShieldComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFruitShieldComponent.XenoFruitShieldComponent_AutoState()
      {
        ShieldAmount = component.ShieldAmount,
        ShieldRatio = component.ShieldRatio,
        ShieldDecay = component.ShieldDecay
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFruitShieldComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFruitShieldComponent.XenoFruitShieldComponent_AutoState current))
        return;
      component.ShieldAmount = current.ShieldAmount;
      component.ShieldRatio = current.ShieldRatio;
      component.ShieldDecay = current.ShieldDecay;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, XenoFruitShieldComponent>(uid, component, ref args1);
    }
  }
}

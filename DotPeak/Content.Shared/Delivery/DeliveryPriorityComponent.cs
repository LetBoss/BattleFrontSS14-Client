// Decompiled with JetBrains decompiler
// Type: Content.Shared.Delivery.DeliveryPriorityComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared.Delivery;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (DeliveryModifierSystem)})]
public sealed class DeliveryPriorityComponent : 
  Component,
  ISerializationGenerated<DeliveryPriorityComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float InTimeMultiplierOffset = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  public float ExpiredMultiplierOffset = -0.15f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Delivered;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Expired;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DeliveryTime = TimeSpan.FromMinutes(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DeliverUntilTime;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DeliveryPriorityComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DeliveryPriorityComponent) component;
    if (serialization.TryCustomCopy<DeliveryPriorityComponent>(this, ref target, hookCtx, false, context))
      return;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InTimeMultiplierOffset, ref num1, hookCtx, false, context))
      num1 = this.InTimeMultiplierOffset;
    target.InTimeMultiplierOffset = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExpiredMultiplierOffset, ref num2, hookCtx, false, context))
      num2 = this.ExpiredMultiplierOffset;
    target.ExpiredMultiplierOffset = num2;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Delivered, ref flag1, hookCtx, false, context))
      flag1 = this.Delivered;
    target.Delivered = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Expired, ref flag2, hookCtx, false, context))
      flag2 = this.Expired;
    target.Expired = flag2;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DeliveryTime, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.DeliveryTime, hookCtx, context, false);
    target.DeliveryTime = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DeliverUntilTime, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.DeliverUntilTime, hookCtx, context, false);
    target.DeliverUntilTime = timeSpan2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DeliveryPriorityComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DeliveryPriorityComponent target1 = (DeliveryPriorityComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DeliveryPriorityComponent target1 = (DeliveryPriorityComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DeliveryPriorityComponent target1 = (DeliveryPriorityComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DeliveryPriorityComponent Component.Instantiate() => new DeliveryPriorityComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DeliveryPriorityComponent_AutoState : IComponentState
  {
    public bool Delivered;
    public bool Expired;
    public TimeSpan DeliveryTime;
    public TimeSpan DeliverUntilTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeliveryPriorityComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DeliveryPriorityComponent, ComponentGetState>(new ComponentEventRefHandler<DeliveryPriorityComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DeliveryPriorityComponent, ComponentHandleState>(new ComponentEventRefHandler<DeliveryPriorityComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      DeliveryPriorityComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new DeliveryPriorityComponent.DeliveryPriorityComponent_AutoState()
      {
        Delivered = component.Delivered,
        Expired = component.Expired,
        DeliveryTime = component.DeliveryTime,
        DeliverUntilTime = component.DeliverUntilTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DeliveryPriorityComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is DeliveryPriorityComponent.DeliveryPriorityComponent_AutoState current))
        return;
      component.Delivered = current.Delivered;
      component.Expired = current.Expired;
      component.DeliveryTime = current.DeliveryTime;
      component.DeliverUntilTime = current.DeliverUntilTime;
    }
  }
}

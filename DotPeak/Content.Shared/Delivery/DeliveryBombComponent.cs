// Decompiled with JetBrains decompiler
// Type: Content.Shared.Delivery.DeliveryBombComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared.Delivery;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (DeliveryModifierSystem)})]
public sealed class DeliveryBombComponent : 
  Component,
  ISerializationGenerated<DeliveryBombComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ExplosionRetryDelay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextExplosionRetry;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ExplosionChance = 0.05f;
  [DataField(null, false, 1, false, false, null)]
  public float ExplosionChanceRetryIncrease = 0.01f;
  [DataField(null, false, 1, false, false, null)]
  public bool PrimeOnUnlock = true;
  [DataField(null, false, 1, false, false, null)]
  public bool PrimeOnBreakage = true;
  [DataField(null, false, 1, false, false, null)]
  public bool PrimeOnExpire = true;
  [DataField(null, false, 1, false, false, null)]
  public float SpesoMultiplier = 1.5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DeliveryBombComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DeliveryBombComponent) component;
    if (serialization.TryCustomCopy<DeliveryBombComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExplosionRetryDelay, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.ExplosionRetryDelay, hookCtx, context, false);
    target.ExplosionRetryDelay = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextExplosionRetry, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.NextExplosionRetry, hookCtx, context, false);
    target.NextExplosionRetry = timeSpan2;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExplosionChance, ref num1, hookCtx, false, context))
      num1 = this.ExplosionChance;
    target.ExplosionChance = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExplosionChanceRetryIncrease, ref num2, hookCtx, false, context))
      num2 = this.ExplosionChanceRetryIncrease;
    target.ExplosionChanceRetryIncrease = num2;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.PrimeOnUnlock, ref flag1, hookCtx, false, context))
      flag1 = this.PrimeOnUnlock;
    target.PrimeOnUnlock = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.PrimeOnBreakage, ref flag2, hookCtx, false, context))
      flag2 = this.PrimeOnBreakage;
    target.PrimeOnBreakage = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.PrimeOnExpire, ref flag3, hookCtx, false, context))
      flag3 = this.PrimeOnExpire;
    target.PrimeOnExpire = flag3;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpesoMultiplier, ref num3, hookCtx, false, context))
      num3 = this.SpesoMultiplier;
    target.SpesoMultiplier = num3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DeliveryBombComponent target,
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
    DeliveryBombComponent target1 = (DeliveryBombComponent) target;
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
    DeliveryBombComponent target1 = (DeliveryBombComponent) target;
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
    DeliveryBombComponent target1 = (DeliveryBombComponent) target;
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
  virtual DeliveryBombComponent Component.Instantiate() => new DeliveryBombComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeliveryBombComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DeliveryBombComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DeliveryBombComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      DeliveryBombComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextExplosionRetry += args.PausedTime;
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DeliveryBombComponent_AutoState : IComponentState
  {
    public TimeSpan NextExplosionRetry;
    public float ExplosionChance;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeliveryBombComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DeliveryBombComponent, ComponentGetState>(new ComponentEventRefHandler<DeliveryBombComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DeliveryBombComponent, ComponentHandleState>(new ComponentEventRefHandler<DeliveryBombComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      DeliveryBombComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new DeliveryBombComponent.DeliveryBombComponent_AutoState()
      {
        NextExplosionRetry = component.NextExplosionRetry,
        ExplosionChance = component.ExplosionChance
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DeliveryBombComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is DeliveryBombComponent.DeliveryBombComponent_AutoState current))
        return;
      component.NextExplosionRetry = current.NextExplosionRetry;
      component.ExplosionChance = current.ExplosionChance;
    }
  }
}

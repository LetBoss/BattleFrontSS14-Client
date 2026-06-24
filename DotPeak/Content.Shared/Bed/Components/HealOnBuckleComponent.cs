// Decompiled with JetBrains decompiler
// Type: Content.Shared.Bed.Components.HealOnBuckleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Bed.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentPause]
[AutoGenerateComponentState(false, false)]
public sealed class HealOnBuckleComponent : 
  Component,
  ISerializationGenerated<HealOnBuckleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public DamageSpecifier Damage;
  [DataField(null, false, 1, false, false, null)]
  public float HealTime = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float SleepMultiplier = 3f;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  [AutoNetworkedField]
  public TimeSpan NextHealTime = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? SleepAction;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HealOnBuckleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (HealOnBuckleComponent) component;
    if (serialization.TryCustomCopy<HealOnBuckleComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier damageSpecifier = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref damageSpecifier, hookCtx, false, context))
    {
      if (this.Damage == null)
        damageSpecifier = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref damageSpecifier, hookCtx, context, true);
    }
    target.Damage = damageSpecifier;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HealTime, ref num1, hookCtx, false, context))
      num1 = this.HealTime;
    target.HealTime = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SleepMultiplier, ref num2, hookCtx, false, context))
      num2 = this.SleepMultiplier;
    target.SleepMultiplier = num2;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextHealTime, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.NextHealTime, hookCtx, context, false);
    target.NextHealTime = timeSpan;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.SleepAction, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.SleepAction, hookCtx, context, false);
    target.SleepAction = nullable;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HealOnBuckleComponent target,
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
    HealOnBuckleComponent target1 = (HealOnBuckleComponent) target;
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
    HealOnBuckleComponent target1 = (HealOnBuckleComponent) target;
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
    HealOnBuckleComponent target1 = (HealOnBuckleComponent) target;
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
  virtual HealOnBuckleComponent Component.Instantiate() => new HealOnBuckleComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HealOnBuckleComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<HealOnBuckleComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<HealOnBuckleComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      HealOnBuckleComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextHealTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HealOnBuckleComponent_AutoState : IComponentState
  {
    public TimeSpan NextHealTime;
    public NetEntity? SleepAction;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HealOnBuckleComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<HealOnBuckleComponent, ComponentGetState>(new ComponentEventRefHandler<HealOnBuckleComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<HealOnBuckleComponent, ComponentHandleState>(new ComponentEventRefHandler<HealOnBuckleComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      HealOnBuckleComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new HealOnBuckleComponent.HealOnBuckleComponent_AutoState()
      {
        NextHealTime = component.NextHealTime,
        SleepAction = this.GetNetEntity(component.SleepAction, (MetaDataComponent) null)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HealOnBuckleComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is HealOnBuckleComponent.HealOnBuckleComponent_AutoState current))
        return;
      component.NextHealTime = current.NextHealTime;
      component.SleepAction = this.EnsureEntity<HealOnBuckleComponent>(current.SleepAction, uid);
    }
  }
}

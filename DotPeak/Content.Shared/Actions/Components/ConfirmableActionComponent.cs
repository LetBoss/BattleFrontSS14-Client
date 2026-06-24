// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.Components.ConfirmableActionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Actions.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (ConfirmableActionSystem)})]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[EntityCategory(new string[] {"Actions"})]
public sealed class ConfirmableActionComponent : 
  Component,
  ISerializationGenerated<ConfirmableActionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public LocId Popup = LocId.op_Implicit(string.Empty);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? NextConfirm;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? NextUnprime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ConfirmDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan PrimeTime = TimeSpan.FromSeconds(5L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ConfirmableActionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ConfirmableActionComponent) component;
    if (serialization.TryCustomCopy<ConfirmableActionComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId locId = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Popup, ref locId, hookCtx, false, context))
      locId = serialization.CreateCopy<LocId>(this.Popup, hookCtx, context, false);
    target.Popup = locId;
    TimeSpan? nullable1 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextConfirm, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<TimeSpan?>(this.NextConfirm, hookCtx, context, false);
    target.NextConfirm = nullable1;
    TimeSpan? nullable2 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextUnprime, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<TimeSpan?>(this.NextUnprime, hookCtx, context, false);
    target.NextUnprime = nullable2;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ConfirmDelay, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.ConfirmDelay, hookCtx, context, false);
    target.ConfirmDelay = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PrimeTime, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.PrimeTime, hookCtx, context, false);
    target.PrimeTime = timeSpan2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ConfirmableActionComponent target,
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
    ConfirmableActionComponent target1 = (ConfirmableActionComponent) target;
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
    ConfirmableActionComponent target1 = (ConfirmableActionComponent) target;
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
    ConfirmableActionComponent target1 = (ConfirmableActionComponent) target;
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
  virtual ConfirmableActionComponent Component.Instantiate() => new ConfirmableActionComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ConfirmableActionComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ConfirmableActionComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<ConfirmableActionComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      ConfirmableActionComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.NextConfirm.HasValue)
        component.NextConfirm = new TimeSpan?(component.NextConfirm.Value + args.PausedTime);
      if (component.NextUnprime.HasValue)
        component.NextUnprime = new TimeSpan?(component.NextUnprime.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ConfirmableActionComponent_AutoState : IComponentState
  {
    public TimeSpan? NextConfirm;
    public TimeSpan? NextUnprime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ConfirmableActionComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ConfirmableActionComponent, ComponentGetState>(new ComponentEventRefHandler<ConfirmableActionComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ConfirmableActionComponent, ComponentHandleState>(new ComponentEventRefHandler<ConfirmableActionComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      ConfirmableActionComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new ConfirmableActionComponent.ConfirmableActionComponent_AutoState()
      {
        NextConfirm = component.NextConfirm,
        NextUnprime = component.NextUnprime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ConfirmableActionComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is ConfirmableActionComponent.ConfirmableActionComponent_AutoState current))
        return;
      component.NextConfirm = current.NextConfirm;
      component.NextUnprime = current.NextUnprime;
    }
  }
}

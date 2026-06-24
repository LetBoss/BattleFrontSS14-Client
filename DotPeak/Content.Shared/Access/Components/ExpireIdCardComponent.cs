// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Components.ExpireIdCardComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Access.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedIdCardSystem)})]
public sealed class ExpireIdCardComponent : 
  Component,
  ISerializationGenerated<ExpireIdCardComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Expired;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Permanent;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  [AutoNetworkedField]
  public TimeSpan ExpireTime = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<AccessLevelPrototype>> ExpiredAccess = new HashSet<ProtoId<AccessLevelPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public LocId? ExpireMessage;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExpireIdCardComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ExpireIdCardComponent) component;
    if (serialization.TryCustomCopy<ExpireIdCardComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Expired, ref flag1, hookCtx, false, context))
      flag1 = this.Expired;
    target.Expired = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Permanent, ref flag2, hookCtx, false, context))
      flag2 = this.Permanent;
    target.Permanent = flag2;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExpireTime, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.ExpireTime, hookCtx, context, false);
    target.ExpireTime = timeSpan;
    HashSet<ProtoId<AccessLevelPrototype>> protoIdSet = (HashSet<ProtoId<AccessLevelPrototype>>) null;
    if (this.ExpiredAccess == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.ExpiredAccess, ref protoIdSet, hookCtx, true, context))
      protoIdSet = serialization.CreateCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.ExpiredAccess, hookCtx, context, false);
    target.ExpiredAccess = protoIdSet;
    LocId? nullable = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.ExpireMessage, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<LocId?>(this.ExpireMessage, hookCtx, context, false);
    target.ExpireMessage = nullable;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExpireIdCardComponent target,
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
    ExpireIdCardComponent target1 = (ExpireIdCardComponent) target;
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
    ExpireIdCardComponent target1 = (ExpireIdCardComponent) target;
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
    ExpireIdCardComponent target1 = (ExpireIdCardComponent) target;
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
  virtual ExpireIdCardComponent Component.Instantiate() => new ExpireIdCardComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ExpireIdCardComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ExpireIdCardComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<ExpireIdCardComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      ExpireIdCardComponent component,
      ref EntityUnpausedEvent args)
    {
      component.ExpireTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ExpireIdCardComponent_AutoState : IComponentState
  {
    public bool Expired;
    public bool Permanent;
    public TimeSpan ExpireTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ExpireIdCardComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ExpireIdCardComponent, ComponentGetState>(new ComponentEventRefHandler<ExpireIdCardComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ExpireIdCardComponent, ComponentHandleState>(new ComponentEventRefHandler<ExpireIdCardComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      ExpireIdCardComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new ExpireIdCardComponent.ExpireIdCardComponent_AutoState()
      {
        Expired = component.Expired,
        Permanent = component.Permanent,
        ExpireTime = component.ExpireTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ExpireIdCardComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is ExpireIdCardComponent.ExpireIdCardComponent_AutoState current))
        return;
      component.Expired = current.Expired;
      component.Permanent = current.Permanent;
      component.ExpireTime = current.ExpireTime;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Content.Shared.Bed.Cryostorage.CryostorageContainedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Bed.Cryostorage;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class CryostorageContainedComponent : 
  Component,
  ISerializationGenerated<CryostorageContainedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool AllowReEnteringBody;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? GracePeriodEndTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Cryostorage;
  [DataField(null, false, 1, false, false, null)]
  public NetUserId? UserId;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CryostorageContainedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CryostorageContainedComponent) component;
    if (serialization.TryCustomCopy<CryostorageContainedComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowReEnteringBody, ref flag, hookCtx, false, context))
      flag = this.AllowReEnteringBody;
    target.AllowReEnteringBody = flag;
    TimeSpan? nullable1 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.GracePeriodEndTime, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<TimeSpan?>(this.GracePeriodEndTime, hookCtx, context, false);
    target.GracePeriodEndTime = nullable1;
    EntityUid? nullable2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Cryostorage, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<EntityUid?>(this.Cryostorage, hookCtx, context, false);
    target.Cryostorage = nullable2;
    NetUserId? nullable3 = new NetUserId?();
    if (!serialization.TryCustomCopy<NetUserId?>(this.UserId, ref nullable3, hookCtx, false, context))
      nullable3 = serialization.CreateCopy<NetUserId?>(this.UserId, hookCtx, context, false);
    target.UserId = nullable3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CryostorageContainedComponent target,
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
    CryostorageContainedComponent target1 = (CryostorageContainedComponent) target;
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
    CryostorageContainedComponent target1 = (CryostorageContainedComponent) target;
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
    CryostorageContainedComponent target1 = (CryostorageContainedComponent) target;
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
  virtual CryostorageContainedComponent Component.Instantiate()
  {
    return new CryostorageContainedComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CryostorageContainedComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<CryostorageContainedComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<CryostorageContainedComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      CryostorageContainedComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.GracePeriodEndTime.HasValue)
        component.GracePeriodEndTime = new TimeSpan?(component.GracePeriodEndTime.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CryostorageContainedComponent_AutoState : IComponentState
  {
    public TimeSpan? GracePeriodEndTime;
    public NetEntity? Cryostorage;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CryostorageContainedComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<CryostorageContainedComponent, ComponentGetState>(new ComponentEventRefHandler<CryostorageContainedComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<CryostorageContainedComponent, ComponentHandleState>(new ComponentEventRefHandler<CryostorageContainedComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      CryostorageContainedComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new CryostorageContainedComponent.CryostorageContainedComponent_AutoState()
      {
        GracePeriodEndTime = component.GracePeriodEndTime,
        Cryostorage = this.GetNetEntity(component.Cryostorage, (MetaDataComponent) null)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CryostorageContainedComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is CryostorageContainedComponent.CryostorageContainedComponent_AutoState current))
        return;
      component.GracePeriodEndTime = current.GracePeriodEndTime;
      component.Cryostorage = this.EnsureEntity<CryostorageContainedComponent>(current.Cryostorage, uid);
    }
  }
}

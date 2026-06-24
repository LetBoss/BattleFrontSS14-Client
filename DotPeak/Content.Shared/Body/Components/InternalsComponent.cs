// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Components.InternalsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Body.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class InternalsComponent : 
  Component,
  ISerializationGenerated<InternalsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? GasTankEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> BreathTools = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Delay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> InternalsAlert = ProtoId<AlertPrototype>.op_Implicit("Internals");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InternalsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (InternalsComponent) component;
    if (serialization.TryCustomCopy<InternalsComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.GasTankEntity, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.GasTankEntity, hookCtx, context, false);
    target.GasTankEntity = nullable;
    HashSet<EntityUid> entityUidSet = (HashSet<EntityUid>) null;
    if (this.BreathTools == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.BreathTools, ref entityUidSet, hookCtx, true, context))
      entityUidSet = serialization.CreateCopy<HashSet<EntityUid>>(this.BreathTools, hookCtx, context, false);
    target.BreathTools = entityUidSet;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context, false);
    target.Delay = timeSpan;
    ProtoId<AlertPrototype> protoId = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.InternalsAlert, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.InternalsAlert, hookCtx, context, false);
    target.InternalsAlert = protoId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InternalsComponent target,
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
    InternalsComponent target1 = (InternalsComponent) target;
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
    InternalsComponent target1 = (InternalsComponent) target;
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
    InternalsComponent target1 = (InternalsComponent) target;
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
  virtual InternalsComponent Component.Instantiate() => new InternalsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class InternalsComponent_AutoState : IComponentState
  {
    public NetEntity? GasTankEntity;
    public HashSet<NetEntity> BreathTools;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class InternalsComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<InternalsComponent, ComponentGetState>(new ComponentEventRefHandler<InternalsComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<InternalsComponent, ComponentHandleState>(new ComponentEventRefHandler<InternalsComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      InternalsComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new InternalsComponent.InternalsComponent_AutoState()
      {
        GasTankEntity = this.GetNetEntity(component.GasTankEntity, (MetaDataComponent) null),
        BreathTools = this.GetNetEntitySet(component.BreathTools)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      InternalsComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is InternalsComponent.InternalsComponent_AutoState current))
        return;
      component.GasTankEntity = this.EnsureEntity<InternalsComponent>(current.GasTankEntity, uid);
      this.EnsureEntitySet<InternalsComponent>(current.BreathTools, uid, component.BreathTools);
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, InternalsComponent>(uid, component, ref handleStateEvent);
    }
  }
}

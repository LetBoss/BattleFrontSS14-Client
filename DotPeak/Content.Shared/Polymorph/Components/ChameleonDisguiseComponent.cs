// Decompiled with JetBrains decompiler
// Type: Content.Shared.Polymorph.Components.ChameleonDisguiseComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Polymorph.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Polymorph.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedChameleonProjectorSystem)})]
[AutoGenerateComponentState(true, false)]
public sealed class ChameleonDisguiseComponent : 
  Component,
  ISerializationGenerated<ChameleonDisguiseComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityUid User;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid Projector;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid SourceEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? SourceProto;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ChameleonDisguiseComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ChameleonDisguiseComponent) target1;
    if (serialization.TryCustomCopy<ChameleonDisguiseComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.User, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this.User, hookCtx, context);
    target.User = target2;
    EntityUid target3 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Projector, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid>(this.Projector, hookCtx, context);
    target.Projector = target3;
    EntityUid target4 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.SourceEntity, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid>(this.SourceEntity, hookCtx, context);
    target.SourceEntity = target4;
    EntProtoId? target5 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.SourceProto, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId?>(this.SourceProto, hookCtx, context);
    target.SourceProto = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ChameleonDisguiseComponent target,
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
    ChameleonDisguiseComponent target1 = (ChameleonDisguiseComponent) target;
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
    ChameleonDisguiseComponent target1 = (ChameleonDisguiseComponent) target;
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
    ChameleonDisguiseComponent target1 = (ChameleonDisguiseComponent) target;
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
  virtual ChameleonDisguiseComponent Component.Instantiate() => new ChameleonDisguiseComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ChameleonDisguiseComponent_AutoState : IComponentState
  {
    public NetEntity SourceEntity;
    public EntProtoId? SourceProto;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ChameleonDisguiseComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ChameleonDisguiseComponent, ComponentGetState>(new ComponentEventRefHandler<ChameleonDisguiseComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ChameleonDisguiseComponent, ComponentHandleState>(new ComponentEventRefHandler<ChameleonDisguiseComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ChameleonDisguiseComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ChameleonDisguiseComponent.ChameleonDisguiseComponent_AutoState()
      {
        SourceEntity = this.GetNetEntity(component.SourceEntity),
        SourceProto = component.SourceProto
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ChameleonDisguiseComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ChameleonDisguiseComponent.ChameleonDisguiseComponent_AutoState current))
        return;
      component.SourceEntity = this.EnsureEntity<ChameleonDisguiseComponent>(current.SourceEntity, uid);
      component.SourceProto = current.SourceProto;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, ChameleonDisguiseComponent>(uid, component, ref args1);
    }
  }
}

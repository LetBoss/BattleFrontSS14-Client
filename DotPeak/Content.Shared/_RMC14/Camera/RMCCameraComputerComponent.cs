// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Camera.RMCCameraComputerComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Camera;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedRMCCameraSystem)})]
public sealed class RMCCameraComputerComponent : 
  Component,
  ISerializationGenerated<RMCCameraComputerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public HashSet<EntProtoId> ProtoIds = new HashSet<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? CurrentCamera;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<NetEntity> CameraIds = new List<NetEntity>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<string> CameraNames = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> Watchers = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? Title;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCCameraComputerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCCameraComputerComponent) target1;
    if (serialization.TryCustomCopy<RMCCameraComputerComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<EntProtoId> target2 = (HashSet<EntProtoId>) null;
    if (this.ProtoIds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntProtoId>>(this.ProtoIds, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<EntProtoId>>(this.ProtoIds, hookCtx, context);
    target.ProtoIds = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.CurrentCamera, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.CurrentCamera, hookCtx, context);
    target.CurrentCamera = target3;
    List<NetEntity> target4 = (List<NetEntity>) null;
    if (this.CameraIds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<NetEntity>>(this.CameraIds, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<NetEntity>>(this.CameraIds, hookCtx, context);
    target.CameraIds = target4;
    List<string> target5 = (List<string>) null;
    if (this.CameraNames == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.CameraNames, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<string>>(this.CameraNames, hookCtx, context);
    target.CameraNames = target5;
    List<EntityUid> target6 = (List<EntityUid>) null;
    if (this.Watchers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Watchers, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<EntityUid>>(this.Watchers, hookCtx, context);
    target.Watchers = target6;
    LocId? target7 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.Title, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId?>(this.Title, hookCtx, context);
    target.Title = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCCameraComputerComponent target,
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
    RMCCameraComputerComponent target1 = (RMCCameraComputerComponent) target;
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
    RMCCameraComputerComponent target1 = (RMCCameraComputerComponent) target;
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
    RMCCameraComputerComponent target1 = (RMCCameraComputerComponent) target;
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
  virtual RMCCameraComputerComponent Component.Instantiate() => new RMCCameraComputerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCCameraComputerComponent_AutoState : IComponentState
  {
    public HashSet<EntProtoId> ProtoIds;
    public NetEntity? CurrentCamera;
    public List<NetEntity> CameraIds;
    public List<string> CameraNames;
    public List<NetEntity> Watchers;
    public LocId? Title;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCCameraComputerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCCameraComputerComponent, ComponentGetState>(new ComponentEventRefHandler<RMCCameraComputerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCCameraComputerComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCCameraComputerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCCameraComputerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCCameraComputerComponent.RMCCameraComputerComponent_AutoState()
      {
        ProtoIds = component.ProtoIds,
        CurrentCamera = this.GetNetEntity(component.CurrentCamera),
        CameraIds = component.CameraIds,
        CameraNames = component.CameraNames,
        Watchers = this.GetNetEntityList(component.Watchers),
        Title = component.Title
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCCameraComputerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCCameraComputerComponent.RMCCameraComputerComponent_AutoState current))
        return;
      component.ProtoIds = current.ProtoIds == null ? (HashSet<EntProtoId>) null : new HashSet<EntProtoId>((IEnumerable<EntProtoId>) current.ProtoIds);
      component.CurrentCamera = this.EnsureEntity<RMCCameraComputerComponent>(current.CurrentCamera, uid);
      component.CameraIds = current.CameraIds == null ? (List<NetEntity>) null : new List<NetEntity>((IEnumerable<NetEntity>) current.CameraIds);
      component.CameraNames = current.CameraNames == null ? (List<string>) null : new List<string>((IEnumerable<string>) current.CameraNames);
      this.EnsureEntityList<RMCCameraComputerComponent>(current.Watchers, uid, component.Watchers);
      component.Title = current.Title;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, RMCCameraComputerComponent>(uid, component, ref args1);
    }
  }
}

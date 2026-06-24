// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mobs.CMGhostComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Ghost;
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
namespace Content.Shared._RMC14.Mobs;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedGhostSystem)})]
[AutoGenerateComponentState(true, false)]
public sealed class CMGhostComponent : 
  Component,
  ISerializationGenerated<CMGhostComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId ToggleMarineHud = (EntProtoId) "ActionToggleMarineHud";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ToggleMarineHudEntity;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId ToggleXenoHud = (EntProtoId) "ActionToggleXenoHud";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ToggleXenoHudEntity;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId FindParasite = (EntProtoId) "ActionFindParasite";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? FindParasiteEntity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMGhostComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMGhostComponent) target1;
    if (serialization.TryCustomCopy<CMGhostComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ToggleMarineHud, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.ToggleMarineHud, hookCtx, context);
    target.ToggleMarineHud = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ToggleMarineHudEntity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.ToggleMarineHudEntity, hookCtx, context);
    target.ToggleMarineHudEntity = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ToggleXenoHud, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.ToggleXenoHud, hookCtx, context);
    target.ToggleXenoHud = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ToggleXenoHudEntity, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.ToggleXenoHudEntity, hookCtx, context);
    target.ToggleXenoHudEntity = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.FindParasite, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.FindParasite, hookCtx, context);
    target.FindParasite = target6;
    EntityUid? target7 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.FindParasiteEntity, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityUid?>(this.FindParasiteEntity, hookCtx, context);
    target.FindParasiteEntity = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMGhostComponent target,
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
    CMGhostComponent target1 = (CMGhostComponent) target;
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
    CMGhostComponent target1 = (CMGhostComponent) target;
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
    CMGhostComponent target1 = (CMGhostComponent) target;
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
  virtual CMGhostComponent Component.Instantiate() => new CMGhostComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMGhostComponent_AutoState : IComponentState
  {
    public NetEntity? ToggleMarineHudEntity;
    public NetEntity? ToggleXenoHudEntity;
    public NetEntity? FindParasiteEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMGhostComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMGhostComponent, ComponentGetState>(new ComponentEventRefHandler<CMGhostComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMGhostComponent, ComponentHandleState>(new ComponentEventRefHandler<CMGhostComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, CMGhostComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMGhostComponent.CMGhostComponent_AutoState()
      {
        ToggleMarineHudEntity = this.GetNetEntity(component.ToggleMarineHudEntity),
        ToggleXenoHudEntity = this.GetNetEntity(component.ToggleXenoHudEntity),
        FindParasiteEntity = this.GetNetEntity(component.FindParasiteEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMGhostComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMGhostComponent.CMGhostComponent_AutoState current))
        return;
      component.ToggleMarineHudEntity = this.EnsureEntity<CMGhostComponent>(current.ToggleMarineHudEntity, uid);
      component.ToggleXenoHudEntity = this.EnsureEntity<CMGhostComponent>(current.ToggleXenoHudEntity, uid);
      component.FindParasiteEntity = this.EnsureEntity<CMGhostComponent>(current.FindParasiteEntity, uid);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, CMGhostComponent>(uid, component, ref args1);
    }
  }
}

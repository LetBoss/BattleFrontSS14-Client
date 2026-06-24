// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Explosion.ProjectileLimitHitsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Explosion;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ProjectileLimitHitsComponent : 
  Component,
  ISerializationGenerated<ProjectileLimitHitsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> IgnoredEntities = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid OriginEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Limit = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? ExtraId;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ProjectileLimitHitsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ProjectileLimitHitsComponent) target1;
    if (serialization.TryCustomCopy<ProjectileLimitHitsComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntityUid> target2 = (List<EntityUid>) null;
    if (this.IgnoredEntities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.IgnoredEntities, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntityUid>>(this.IgnoredEntities, hookCtx, context);
    target.IgnoredEntities = target2;
    EntityUid target3 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.OriginEntity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid>(this.OriginEntity, hookCtx, context);
    target.OriginEntity = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Limit, ref target4, hookCtx, false, context))
      target4 = this.Limit;
    target.Limit = target4;
    int? target5 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.ExtraId, ref target5, hookCtx, false, context))
      target5 = this.ExtraId;
    target.ExtraId = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ProjectileLimitHitsComponent target,
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
    ProjectileLimitHitsComponent target1 = (ProjectileLimitHitsComponent) target;
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
    ProjectileLimitHitsComponent target1 = (ProjectileLimitHitsComponent) target;
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
    ProjectileLimitHitsComponent target1 = (ProjectileLimitHitsComponent) target;
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
  virtual ProjectileLimitHitsComponent Component.Instantiate()
  {
    return new ProjectileLimitHitsComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ProjectileLimitHitsComponent_AutoState : IComponentState
  {
    public List<NetEntity> IgnoredEntities;
    public NetEntity OriginEntity;
    public int Limit;
    public int? ExtraId;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ProjectileLimitHitsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ProjectileLimitHitsComponent, ComponentGetState>(new ComponentEventRefHandler<ProjectileLimitHitsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ProjectileLimitHitsComponent, ComponentHandleState>(new ComponentEventRefHandler<ProjectileLimitHitsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ProjectileLimitHitsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ProjectileLimitHitsComponent.ProjectileLimitHitsComponent_AutoState()
      {
        IgnoredEntities = this.GetNetEntityList(component.IgnoredEntities),
        OriginEntity = this.GetNetEntity(component.OriginEntity),
        Limit = component.Limit,
        ExtraId = component.ExtraId
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ProjectileLimitHitsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ProjectileLimitHitsComponent.ProjectileLimitHitsComponent_AutoState current))
        return;
      this.EnsureEntityList<ProjectileLimitHitsComponent>(current.IgnoredEntities, uid, component.IgnoredEntities);
      component.OriginEntity = this.EnsureEntity<ProjectileLimitHitsComponent>(current.OriginEntity, uid);
      component.Limit = current.Limit;
      component.ExtraId = current.ExtraId;
    }
  }
}

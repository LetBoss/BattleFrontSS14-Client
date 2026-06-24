// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mind.MindComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Mind;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class MindComponent : 
  Component,
  ISerializationGenerated<MindComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> Objectives = new List<EntityUid>();
  [AutoNetworkedField]
  public NetEntity? OriginalOwnedEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> MindRoles = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<RoleTypePrototype> RoleType = (ProtoId<RoleTypePrototype>) "Neutral";
  [DataField(null, false, 1, false, false, null)]
  public LocId? Subtype;

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (SharedMindSystem)})]
  public NetUserId? UserId { get; set; }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (SharedMindSystem)})]
  public NetUserId? OriginalOwnerUserId { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsVisitingEntity => this.VisitingEntity.HasValue;

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (SharedMindSystem)})]
  public EntityUid? VisitingEntity { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? CurrentEntity => this.VisitingEntity ?? this.OwnedEntity;

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string? CharacterName { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? TimeOfDeath { get; set; }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (SharedMindSystem)})]
  public EntityUid? OwnedEntity { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [Obsolete("Use Objectives field")]
  public IEnumerable<EntityUid> AllObjectives => (IEnumerable<EntityUid>) this.Objectives;

  [DataField(null, false, 1, false, false, null)]
  public bool PreventGhosting { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public bool PreventSuicide { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MindComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MindComponent) target1;
    if (serialization.TryCustomCopy<MindComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntityUid> target2 = (List<EntityUid>) null;
    if (this.Objectives == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Objectives, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntityUid>>(this.Objectives, hookCtx, context);
    target.Objectives = target2;
    NetUserId? target3 = new NetUserId?();
    if (!serialization.TryCustomCopy<NetUserId?>(this.UserId, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<NetUserId?>(this.UserId, hookCtx, context);
    target.UserId = target3;
    NetUserId? target4 = new NetUserId?();
    if (!serialization.TryCustomCopy<NetUserId?>(this.OriginalOwnerUserId, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<NetUserId?>(this.OriginalOwnerUserId, hookCtx, context);
    target.OriginalOwnerUserId = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.VisitingEntity, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.VisitingEntity, hookCtx, context);
    target.VisitingEntity = target5;
    string target6 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.CharacterName, ref target6, hookCtx, false, context))
      target6 = this.CharacterName;
    target.CharacterName = target6;
    TimeSpan? target7 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.TimeOfDeath, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan?>(this.TimeOfDeath, hookCtx, context);
    target.TimeOfDeath = target7;
    EntityUid? target8 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.OwnedEntity, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntityUid?>(this.OwnedEntity, hookCtx, context);
    target.OwnedEntity = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.PreventGhosting, ref target9, hookCtx, false, context))
      target9 = this.PreventGhosting;
    target.PreventGhosting = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.PreventSuicide, ref target10, hookCtx, false, context))
      target10 = this.PreventSuicide;
    target.PreventSuicide = target10;
    List<EntityUid> target11 = (List<EntityUid>) null;
    if (this.MindRoles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.MindRoles, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<List<EntityUid>>(this.MindRoles, hookCtx, context);
    target.MindRoles = target11;
    ProtoId<RoleTypePrototype> target12 = new ProtoId<RoleTypePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<RoleTypePrototype>>(this.RoleType, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<ProtoId<RoleTypePrototype>>(this.RoleType, hookCtx, context);
    target.RoleType = target12;
    LocId? target13 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.Subtype, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<LocId?>(this.Subtype, hookCtx, context);
    target.Subtype = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MindComponent target,
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
    MindComponent target1 = (MindComponent) target;
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
    MindComponent target1 = (MindComponent) target;
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
    MindComponent target1 = (MindComponent) target;
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
  virtual MindComponent Component.Instantiate() => new MindComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MindComponent_AutoState : IComponentState
  {
    public List<NetEntity> Objectives;
    public NetUserId? UserId;
    public NetUserId? OriginalOwnerUserId;
    public NetEntity? OriginalOwnedEntity;
    public NetEntity? VisitingEntity;
    public string? CharacterName;
    public NetEntity? OwnedEntity;
    public List<NetEntity> MindRoles;
    public ProtoId<RoleTypePrototype> RoleType;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MindComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MindComponent, ComponentGetState>(new ComponentEventRefHandler<MindComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MindComponent, ComponentHandleState>(new ComponentEventRefHandler<MindComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, MindComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new MindComponent.MindComponent_AutoState()
      {
        Objectives = this.GetNetEntityList(component.Objectives),
        UserId = component.UserId,
        OriginalOwnerUserId = component.OriginalOwnerUserId,
        OriginalOwnedEntity = component.OriginalOwnedEntity,
        VisitingEntity = this.GetNetEntity(component.VisitingEntity),
        CharacterName = component.CharacterName,
        OwnedEntity = this.GetNetEntity(component.OwnedEntity),
        MindRoles = this.GetNetEntityList(component.MindRoles),
        RoleType = component.RoleType
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MindComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MindComponent.MindComponent_AutoState current))
        return;
      this.EnsureEntityList<MindComponent>(current.Objectives, uid, component.Objectives);
      component.UserId = current.UserId;
      component.OriginalOwnerUserId = current.OriginalOwnerUserId;
      component.OriginalOwnedEntity = current.OriginalOwnedEntity;
      component.VisitingEntity = this.EnsureEntity<MindComponent>(current.VisitingEntity, uid);
      component.CharacterName = current.CharacterName;
      component.OwnedEntity = this.EnsureEntity<MindComponent>(current.OwnedEntity, uid);
      this.EnsureEntityList<MindComponent>(current.MindRoles, uid, component.MindRoles);
      component.RoleType = current.RoleType;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, MindComponent>(uid, component, ref args1);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Components.IdCardComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Systems;
using Content.Shared.PDA;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
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
namespace Content.Shared.Access.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedIdCardSystem), typeof (SharedPdaSystem), typeof (SharedAgentIdCardSystem)})]
public sealed class IdCardComponent : 
  Component,
  ISerializationGenerated<IdCardComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? FullName;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedIdCardSystem), typeof (SharedPdaSystem), typeof (SharedAgentIdCardSystem)})]
  public LocId? JobTitle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? _jobTitle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<JobIconPrototype> JobIcon = ProtoId<JobIconPrototype>.op_Implicit("JobIconUnknown");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<AccessLevelPrototype>? JobPrototype;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<DepartmentPrototype>> JobDepartments = new List<ProtoId<DepartmentPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public bool BypassLogging;
  [DataField(null, false, 1, false, false, null)]
  public LocId NameLocId = LocId.op_Implicit("access-id-card-component-owner-name-job-title-text");
  [DataField(null, false, 1, false, false, null)]
  public LocId FullNameLocId = LocId.op_Implicit("access-id-card-component-owner-full-name-job-title-text");
  [DataField(null, false, 1, false, false, null)]
  public bool CanMicrowave = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? OriginalOwner;

  [Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedIdCardSystem), typeof (SharedPdaSystem), typeof (SharedAgentIdCardSystem)})]
  public string? LocalizedJobTitle
  {
    set => this._jobTitle = value;
    get
    {
      return this._jobTitle ?? Loc.GetString(LocId.op_Implicit(this.JobTitle ?? LocId.op_Implicit(string.Empty)));
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IdCardComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (IdCardComponent) component;
    if (serialization.TryCustomCopy<IdCardComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.FullName, ref str1, hookCtx, false, context))
      str1 = this.FullName;
    target.FullName = str1;
    LocId? nullable1 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.JobTitle, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<LocId?>(this.JobTitle, hookCtx, context, false);
    target.JobTitle = nullable1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this._jobTitle, ref str2, hookCtx, false, context))
      str2 = this._jobTitle;
    target._jobTitle = str2;
    ProtoId<JobIconPrototype> protoId = new ProtoId<JobIconPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<JobIconPrototype>>(this.JobIcon, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<JobIconPrototype>>(this.JobIcon, hookCtx, context, false);
    target.JobIcon = protoId;
    ProtoId<AccessLevelPrototype>? nullable2 = new ProtoId<AccessLevelPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<AccessLevelPrototype>?>(this.JobPrototype, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<ProtoId<AccessLevelPrototype>?>(this.JobPrototype, hookCtx, context, false);
    target.JobPrototype = nullable2;
    List<ProtoId<DepartmentPrototype>> protoIdList = (List<ProtoId<DepartmentPrototype>>) null;
    if (this.JobDepartments == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<DepartmentPrototype>>>(this.JobDepartments, ref protoIdList, hookCtx, true, context))
      protoIdList = serialization.CreateCopy<List<ProtoId<DepartmentPrototype>>>(this.JobDepartments, hookCtx, context, false);
    target.JobDepartments = protoIdList;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.BypassLogging, ref flag1, hookCtx, false, context))
      flag1 = this.BypassLogging;
    target.BypassLogging = flag1;
    LocId locId1 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.NameLocId, ref locId1, hookCtx, false, context))
      locId1 = serialization.CreateCopy<LocId>(this.NameLocId, hookCtx, context, false);
    target.NameLocId = locId1;
    LocId locId2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.FullNameLocId, ref locId2, hookCtx, false, context))
      locId2 = serialization.CreateCopy<LocId>(this.FullNameLocId, hookCtx, context, false);
    target.FullNameLocId = locId2;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanMicrowave, ref flag2, hookCtx, false, context))
      flag2 = this.CanMicrowave;
    target.CanMicrowave = flag2;
    EntityUid? nullable3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.OriginalOwner, ref nullable3, hookCtx, false, context))
      nullable3 = serialization.CreateCopy<EntityUid?>(this.OriginalOwner, hookCtx, context, false);
    target.OriginalOwner = nullable3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IdCardComponent target,
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
    IdCardComponent target1 = (IdCardComponent) target;
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
    IdCardComponent target1 = (IdCardComponent) target;
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
    IdCardComponent target1 = (IdCardComponent) target;
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
  virtual IdCardComponent Component.Instantiate() => new IdCardComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class IdCardComponent_AutoState : IComponentState
  {
    public string? FullName;
    public LocId? JobTitle;
    public string? _jobTitle;
    public ProtoId<JobIconPrototype> JobIcon;
    public ProtoId<AccessLevelPrototype>? JobPrototype;
    public List<ProtoId<DepartmentPrototype>> JobDepartments;
    public NetEntity? OriginalOwner;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IdCardComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<IdCardComponent, ComponentGetState>(new ComponentEventRefHandler<IdCardComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<IdCardComponent, ComponentHandleState>(new ComponentEventRefHandler<IdCardComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, IdCardComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new IdCardComponent.IdCardComponent_AutoState()
      {
        FullName = component.FullName,
        JobTitle = component.JobTitle,
        _jobTitle = component._jobTitle,
        JobIcon = component.JobIcon,
        JobPrototype = component.JobPrototype,
        JobDepartments = component.JobDepartments,
        OriginalOwner = this.GetNetEntity(component.OriginalOwner, (MetaDataComponent) null)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      IdCardComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is IdCardComponent.IdCardComponent_AutoState current))
        return;
      component.FullName = current.FullName;
      component.JobTitle = current.JobTitle;
      component._jobTitle = current._jobTitle;
      component.JobIcon = current.JobIcon;
      component.JobPrototype = current.JobPrototype;
      component.JobDepartments = current.JobDepartments == null ? (List<ProtoId<DepartmentPrototype>>) null : new List<ProtoId<DepartmentPrototype>>((IEnumerable<ProtoId<DepartmentPrototype>>) current.JobDepartments);
      component.OriginalOwner = this.EnsureEntity<IdCardComponent>(current.OriginalOwner, uid);
    }
  }
}

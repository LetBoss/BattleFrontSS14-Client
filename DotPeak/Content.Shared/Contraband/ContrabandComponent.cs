// Decompiled with JetBrains decompiler
// Type: Content.Shared.Contraband.ContrabandComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Roles;
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
namespace Content.Shared.Contraband;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (ContrabandSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class ContrabandComponent : 
  Component,
  ISerializationGenerated<ContrabandComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ContrabandSeverityPrototype> Severity = ProtoId<ContrabandSeverityPrototype>.op_Implicit("Restricted");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<DepartmentPrototype>> AllowedDepartments = new HashSet<ProtoId<DepartmentPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<JobPrototype>> AllowedJobs = new HashSet<ProtoId<JobPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ContrabandComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ContrabandComponent) component;
    if (serialization.TryCustomCopy<ContrabandComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ContrabandSeverityPrototype> protoId = new ProtoId<ContrabandSeverityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ContrabandSeverityPrototype>>(this.Severity, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<ContrabandSeverityPrototype>>(this.Severity, hookCtx, context, false);
    target.Severity = protoId;
    HashSet<ProtoId<DepartmentPrototype>> protoIdSet1 = (HashSet<ProtoId<DepartmentPrototype>>) null;
    if (this.AllowedDepartments == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<DepartmentPrototype>>>(this.AllowedDepartments, ref protoIdSet1, hookCtx, true, context))
      protoIdSet1 = serialization.CreateCopy<HashSet<ProtoId<DepartmentPrototype>>>(this.AllowedDepartments, hookCtx, context, false);
    target.AllowedDepartments = protoIdSet1;
    HashSet<ProtoId<JobPrototype>> protoIdSet2 = (HashSet<ProtoId<JobPrototype>>) null;
    if (this.AllowedJobs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<JobPrototype>>>(this.AllowedJobs, ref protoIdSet2, hookCtx, true, context))
      protoIdSet2 = serialization.CreateCopy<HashSet<ProtoId<JobPrototype>>>(this.AllowedJobs, hookCtx, context, false);
    target.AllowedJobs = protoIdSet2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ContrabandComponent target,
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
    ContrabandComponent target1 = (ContrabandComponent) target;
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
    ContrabandComponent target1 = (ContrabandComponent) target;
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
    ContrabandComponent target1 = (ContrabandComponent) target;
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
  virtual ContrabandComponent Component.Instantiate() => new ContrabandComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ContrabandComponent_AutoState : IComponentState
  {
    public ProtoId<ContrabandSeverityPrototype> Severity;
    public HashSet<ProtoId<DepartmentPrototype>> AllowedDepartments;
    public HashSet<ProtoId<JobPrototype>> AllowedJobs;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ContrabandComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ContrabandComponent, ComponentGetState>(new ComponentEventRefHandler<ContrabandComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ContrabandComponent, ComponentHandleState>(new ComponentEventRefHandler<ContrabandComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      ContrabandComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new ContrabandComponent.ContrabandComponent_AutoState()
      {
        Severity = component.Severity,
        AllowedDepartments = component.AllowedDepartments,
        AllowedJobs = component.AllowedJobs
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ContrabandComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is ContrabandComponent.ContrabandComponent_AutoState current))
        return;
      component.Severity = current.Severity;
      component.AllowedDepartments = current.AllowedDepartments == null ? (HashSet<ProtoId<DepartmentPrototype>>) null : new HashSet<ProtoId<DepartmentPrototype>>((IEnumerable<ProtoId<DepartmentPrototype>>) current.AllowedDepartments);
      component.AllowedJobs = current.AllowedJobs == null ? (HashSet<ProtoId<JobPrototype>>) null : new HashSet<ProtoId<JobPrototype>>((IEnumerable<ProtoId<JobPrototype>>) current.AllowedJobs);
    }
  }
}

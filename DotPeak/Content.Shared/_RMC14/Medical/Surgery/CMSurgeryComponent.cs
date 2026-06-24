// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Surgery.CMSurgeryComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Medical.Surgery;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedCMSurgerySystem)})]
[EntityCategory(new string[] {"Surgeries"})]
public sealed class CMSurgeryComponent : 
  Component,
  ISerializationGenerated<CMSurgeryComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (SharedCMSurgerySystem)}, Other = AccessPermissions.ReadWriteExecute)]
  public int Priority;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? Requirement;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> Steps = new List<EntProtoId>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMSurgeryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMSurgeryComponent) target1;
    if (serialization.TryCustomCopy<CMSurgeryComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Priority, ref target2, hookCtx, false, context))
      target2 = this.Priority;
    target.Priority = target2;
    EntProtoId? target3 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Requirement, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId?>(this.Requirement, hookCtx, context);
    target.Requirement = target3;
    List<EntProtoId> target4 = (List<EntProtoId>) null;
    if (this.Steps == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.Steps, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntProtoId>>(this.Steps, hookCtx, context);
    target.Steps = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMSurgeryComponent target,
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
    CMSurgeryComponent target1 = (CMSurgeryComponent) target;
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
    CMSurgeryComponent target1 = (CMSurgeryComponent) target;
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
    CMSurgeryComponent target1 = (CMSurgeryComponent) target;
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
  virtual CMSurgeryComponent Component.Instantiate() => new CMSurgeryComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMSurgeryComponent_AutoState : IComponentState
  {
    public int Priority;
    public EntProtoId? Requirement;
    public List<EntProtoId> Steps;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMSurgeryComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMSurgeryComponent, ComponentGetState>(new ComponentEventRefHandler<CMSurgeryComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMSurgeryComponent, ComponentHandleState>(new ComponentEventRefHandler<CMSurgeryComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CMSurgeryComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMSurgeryComponent.CMSurgeryComponent_AutoState()
      {
        Priority = component.Priority,
        Requirement = component.Requirement,
        Steps = component.Steps
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMSurgeryComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMSurgeryComponent.CMSurgeryComponent_AutoState current))
        return;
      component.Priority = current.Priority;
      component.Requirement = current.Requirement;
      component.Steps = current.Steps == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.Steps);
    }
  }
}

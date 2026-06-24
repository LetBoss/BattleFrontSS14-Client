// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Skills.ReagentExaminationRequiresSkillComponent
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
namespace Content.Shared._RMC14.Marines.Skills;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SkillsSystem)})]
public sealed class ReagentExaminationRequiresSkillComponent : 
  Component,
  ISerializationGenerated<ReagentExaminationRequiresSkillComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? UnskilledExamine;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public LocId SkilledExamineContains;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public LocId SkilledExamineNone;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? ContainerId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? NoContainerExamine;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReagentExaminationRequiresSkillComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ReagentExaminationRequiresSkillComponent) target1;
    if (serialization.TryCustomCopy<ReagentExaminationRequiresSkillComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId? target2 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.UnskilledExamine, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId?>(this.UnskilledExamine, hookCtx, context);
    target.UnskilledExamine = target2;
    LocId target3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.SkilledExamineContains, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId>(this.SkilledExamineContains, hookCtx, context);
    target.SkilledExamineContains = target3;
    LocId target4 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.SkilledExamineNone, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId>(this.SkilledExamineNone, hookCtx, context);
    target.SkilledExamineNone = target4;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target5 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (this.Skills == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, hookCtx, context);
    target.Skills = target5;
    string target6 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target6, hookCtx, false, context))
      target6 = this.ContainerId;
    target.ContainerId = target6;
    LocId? target7 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.NoContainerExamine, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId?>(this.NoContainerExamine, hookCtx, context);
    target.NoContainerExamine = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReagentExaminationRequiresSkillComponent target,
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
    ReagentExaminationRequiresSkillComponent target1 = (ReagentExaminationRequiresSkillComponent) target;
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
    ReagentExaminationRequiresSkillComponent target1 = (ReagentExaminationRequiresSkillComponent) target;
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
    ReagentExaminationRequiresSkillComponent target1 = (ReagentExaminationRequiresSkillComponent) target;
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
  virtual ReagentExaminationRequiresSkillComponent Component.Instantiate()
  {
    return new ReagentExaminationRequiresSkillComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ReagentExaminationRequiresSkillComponent_AutoState : IComponentState
  {
    public LocId? UnskilledExamine;
    public LocId SkilledExamineContains;
    public LocId SkilledExamineNone;
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills;
    public string? ContainerId;
    public LocId? NoContainerExamine;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ReagentExaminationRequiresSkillComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ReagentExaminationRequiresSkillComponent, ComponentGetState>(new ComponentEventRefHandler<ReagentExaminationRequiresSkillComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ReagentExaminationRequiresSkillComponent, ComponentHandleState>(new ComponentEventRefHandler<ReagentExaminationRequiresSkillComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ReagentExaminationRequiresSkillComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ReagentExaminationRequiresSkillComponent.ReagentExaminationRequiresSkillComponent_AutoState()
      {
        UnskilledExamine = component.UnskilledExamine,
        SkilledExamineContains = component.SkilledExamineContains,
        SkilledExamineNone = component.SkilledExamineNone,
        Skills = component.Skills,
        ContainerId = component.ContainerId,
        NoContainerExamine = component.NoContainerExamine
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ReagentExaminationRequiresSkillComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ReagentExaminationRequiresSkillComponent.ReagentExaminationRequiresSkillComponent_AutoState current))
        return;
      component.UnskilledExamine = current.UnskilledExamine;
      component.SkilledExamineContains = current.SkilledExamineContains;
      component.SkilledExamineNone = current.SkilledExamineNone;
      component.Skills = current.Skills == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) current.Skills);
      component.ContainerId = current.ContainerId;
      component.NoContainerExamine = current.NoContainerExamine;
    }
  }
}

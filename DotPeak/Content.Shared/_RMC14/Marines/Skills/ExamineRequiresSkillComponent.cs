// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Skills.ExamineRequiresSkillComponent
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
public sealed class ExamineRequiresSkillComponent : 
  Component,
  ISerializationGenerated<ExamineRequiresSkillComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? UnskilledExamine;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public LocId SkilledExamine;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ExaminePriority = 1000;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExamineRequiresSkillComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ExamineRequiresSkillComponent) target1;
    if (serialization.TryCustomCopy<ExamineRequiresSkillComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId? target2 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.UnskilledExamine, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId?>(this.UnskilledExamine, hookCtx, context);
    target.UnskilledExamine = target2;
    LocId target3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.SkilledExamine, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId>(this.SkilledExamine, hookCtx, context);
    target.SkilledExamine = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.ExaminePriority, ref target4, hookCtx, false, context))
      target4 = this.ExaminePriority;
    target.ExaminePriority = target4;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target5 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (this.Skills == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, hookCtx, context);
    target.Skills = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExamineRequiresSkillComponent target,
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
    ExamineRequiresSkillComponent target1 = (ExamineRequiresSkillComponent) target;
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
    ExamineRequiresSkillComponent target1 = (ExamineRequiresSkillComponent) target;
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
    ExamineRequiresSkillComponent target1 = (ExamineRequiresSkillComponent) target;
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
  virtual ExamineRequiresSkillComponent Component.Instantiate()
  {
    return new ExamineRequiresSkillComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ExamineRequiresSkillComponent_AutoState : IComponentState
  {
    public LocId? UnskilledExamine;
    public LocId SkilledExamine;
    public int ExaminePriority;
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ExamineRequiresSkillComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ExamineRequiresSkillComponent, ComponentGetState>(new ComponentEventRefHandler<ExamineRequiresSkillComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ExamineRequiresSkillComponent, ComponentHandleState>(new ComponentEventRefHandler<ExamineRequiresSkillComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ExamineRequiresSkillComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ExamineRequiresSkillComponent.ExamineRequiresSkillComponent_AutoState()
      {
        UnskilledExamine = component.UnskilledExamine,
        SkilledExamine = component.SkilledExamine,
        ExaminePriority = component.ExaminePriority,
        Skills = component.Skills
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ExamineRequiresSkillComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ExamineRequiresSkillComponent.ExamineRequiresSkillComponent_AutoState current))
        return;
      component.UnskilledExamine = current.UnskilledExamine;
      component.SkilledExamine = current.SkilledExamine;
      component.ExaminePriority = current.ExaminePriority;
      component.Skills = current.Skills == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) current.Skills);
    }
  }
}

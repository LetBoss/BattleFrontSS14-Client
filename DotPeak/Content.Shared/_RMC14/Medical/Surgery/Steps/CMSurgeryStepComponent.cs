// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Surgery.Steps.CMSurgeryStepComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
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
namespace Content.Shared._RMC14.Medical.Surgery.Steps;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedCMSurgerySystem)})]
[EntityCategory(new string[] {"SurgerySteps"})]
public sealed class CMSurgeryStepComponent : 
  Component,
  ISerializationGenerated<CMSurgeryStepComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> SkillType = (EntProtoId<SkillDefinitionComponent>) "RMCSkillSurgery";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Skill = 1;
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry? Tool;
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry? Add;
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry? Remove;
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry? BodyRemove;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMSurgeryStepComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMSurgeryStepComponent) target1;
    if (serialization.TryCustomCopy<CMSurgeryStepComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId<SkillDefinitionComponent> target2 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.SkillType, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.SkillType, hookCtx, context);
    target.SkillType = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Skill, ref target3, hookCtx, false, context))
      target3 = this.Skill;
    target.Skill = target3;
    ComponentRegistry target4 = (ComponentRegistry) null;
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.Tool, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ComponentRegistry>(this.Tool, hookCtx, context);
    target.Tool = target4;
    ComponentRegistry target5 = (ComponentRegistry) null;
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.Add, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ComponentRegistry>(this.Add, hookCtx, context);
    target.Add = target5;
    ComponentRegistry target6 = (ComponentRegistry) null;
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.Remove, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ComponentRegistry>(this.Remove, hookCtx, context);
    target.Remove = target6;
    ComponentRegistry target7 = (ComponentRegistry) null;
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.BodyRemove, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ComponentRegistry>(this.BodyRemove, hookCtx, context);
    target.BodyRemove = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMSurgeryStepComponent target,
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
    CMSurgeryStepComponent target1 = (CMSurgeryStepComponent) target;
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
    CMSurgeryStepComponent target1 = (CMSurgeryStepComponent) target;
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
    CMSurgeryStepComponent target1 = (CMSurgeryStepComponent) target;
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
  virtual CMSurgeryStepComponent Component.Instantiate() => new CMSurgeryStepComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMSurgeryStepComponent_AutoState : IComponentState
  {
    public EntProtoId<SkillDefinitionComponent> SkillType;
    public int Skill;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMSurgeryStepComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMSurgeryStepComponent, ComponentGetState>(new ComponentEventRefHandler<CMSurgeryStepComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMSurgeryStepComponent, ComponentHandleState>(new ComponentEventRefHandler<CMSurgeryStepComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CMSurgeryStepComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMSurgeryStepComponent.CMSurgeryStepComponent_AutoState()
      {
        SkillType = component.SkillType,
        Skill = component.Skill
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMSurgeryStepComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMSurgeryStepComponent.CMSurgeryStepComponent_AutoState current))
        return;
      component.SkillType = current.SkillType;
      component.Skill = current.Skill;
    }
  }
}

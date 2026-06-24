// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Construction.Upgrades.RMCConstructionUpgradeTargetComponent
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
namespace Content.Shared._RMC14.Construction.Upgrades;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCUpgradeSystem)})]
public sealed class RMCConstructionUpgradeTargetComponent : 
  Component,
  ISerializationGenerated<RMCConstructionUpgradeTargetComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId[]? Upgrades;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillConstruction";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SkillAmountRequired = 1;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCConstructionUpgradeTargetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCConstructionUpgradeTargetComponent) target1;
    if (serialization.TryCustomCopy<RMCConstructionUpgradeTargetComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId[] target2 = (EntProtoId[]) null;
    if (!serialization.TryCustomCopy<EntProtoId[]>(this.Upgrades, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<EntProtoId[]>(this.Upgrades, hookCtx, context);
    target.Upgrades = target2;
    EntProtoId<SkillDefinitionComponent> target3 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.SkillAmountRequired, ref target4, hookCtx, false, context))
      target4 = this.SkillAmountRequired;
    target.SkillAmountRequired = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCConstructionUpgradeTargetComponent target,
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
    RMCConstructionUpgradeTargetComponent target1 = (RMCConstructionUpgradeTargetComponent) target;
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
    RMCConstructionUpgradeTargetComponent target1 = (RMCConstructionUpgradeTargetComponent) target;
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
    RMCConstructionUpgradeTargetComponent target1 = (RMCConstructionUpgradeTargetComponent) target;
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
  virtual RMCConstructionUpgradeTargetComponent Component.Instantiate()
  {
    return new RMCConstructionUpgradeTargetComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCConstructionUpgradeTargetComponent_AutoState : IComponentState
  {
    public EntProtoId[]? Upgrades;
    public EntProtoId<SkillDefinitionComponent> Skill;
    public int SkillAmountRequired;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCConstructionUpgradeTargetComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCConstructionUpgradeTargetComponent, ComponentGetState>(new ComponentEventRefHandler<RMCConstructionUpgradeTargetComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCConstructionUpgradeTargetComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCConstructionUpgradeTargetComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCConstructionUpgradeTargetComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCConstructionUpgradeTargetComponent.RMCConstructionUpgradeTargetComponent_AutoState()
      {
        Upgrades = component.Upgrades,
        Skill = component.Skill,
        SkillAmountRequired = component.SkillAmountRequired
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCConstructionUpgradeTargetComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCConstructionUpgradeTargetComponent.RMCConstructionUpgradeTargetComponent_AutoState current))
        return;
      component.Upgrades = current.Upgrades;
      component.Skill = current.Skill;
      component.SkillAmountRequired = current.SkillAmountRequired;
    }
  }
}

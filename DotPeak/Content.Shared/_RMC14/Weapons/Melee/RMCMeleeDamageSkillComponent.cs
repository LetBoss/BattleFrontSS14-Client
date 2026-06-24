// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Melee.RMCMeleeDamageSkillComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Damage.Prototypes;
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
namespace Content.Shared._RMC14.Weapons.Melee;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCMeleeWeaponSystem)})]
public sealed class RMCMeleeDamageSkillComponent : 
  Component,
  ISerializationGenerated<RMCMeleeDamageSkillComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillCqc";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<DamageTypePrototype> BonusDamageType = (ProtoId<DamageTypePrototype>) "Blunt";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCMeleeDamageSkillComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCMeleeDamageSkillComponent) target1;
    if (serialization.TryCustomCopy<RMCMeleeDamageSkillComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId<SkillDefinitionComponent> target2 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target2;
    ProtoId<DamageTypePrototype> target3 = new ProtoId<DamageTypePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageTypePrototype>>(this.BonusDamageType, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<DamageTypePrototype>>(this.BonusDamageType, hookCtx, context);
    target.BonusDamageType = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCMeleeDamageSkillComponent target,
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
    RMCMeleeDamageSkillComponent target1 = (RMCMeleeDamageSkillComponent) target;
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
    RMCMeleeDamageSkillComponent target1 = (RMCMeleeDamageSkillComponent) target;
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
    RMCMeleeDamageSkillComponent target1 = (RMCMeleeDamageSkillComponent) target;
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
  virtual RMCMeleeDamageSkillComponent Component.Instantiate()
  {
    return new RMCMeleeDamageSkillComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCMeleeDamageSkillComponent_AutoState : IComponentState
  {
    public EntProtoId<SkillDefinitionComponent> Skill;
    public ProtoId<DamageTypePrototype> BonusDamageType;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCMeleeDamageSkillComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCMeleeDamageSkillComponent, ComponentGetState>(new ComponentEventRefHandler<RMCMeleeDamageSkillComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCMeleeDamageSkillComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCMeleeDamageSkillComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCMeleeDamageSkillComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCMeleeDamageSkillComponent.RMCMeleeDamageSkillComponent_AutoState()
      {
        Skill = component.Skill,
        BonusDamageType = component.BonusDamageType
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCMeleeDamageSkillComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCMeleeDamageSkillComponent.RMCMeleeDamageSkillComponent_AutoState current))
        return;
      component.Skill = current.Skill;
      component.BonusDamageType = current.BonusDamageType;
    }
  }
}

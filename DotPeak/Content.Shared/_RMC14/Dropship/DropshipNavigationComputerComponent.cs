// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.DropshipNavigationComputerComponent
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
namespace Content.Shared._RMC14.Dropship;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedDropshipSystem)})]
public sealed class DropshipNavigationComputerComponent : 
  Component,
  ISerializationGenerated<DropshipNavigationComputerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillPilot";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MultiplierSkillLevel = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FlyBySkillLevel = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SkillFlyByMultiplier = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SkillTravelMultiplier = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SkillRechargeMultiplier = 0.75f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Hijackable = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipNavigationComputerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipNavigationComputerComponent) target1;
    if (serialization.TryCustomCopy<DropshipNavigationComputerComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId<SkillDefinitionComponent> target2 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MultiplierSkillLevel, ref target3, hookCtx, false, context))
      target3 = this.MultiplierSkillLevel;
    target.MultiplierSkillLevel = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.FlyBySkillLevel, ref target4, hookCtx, false, context))
      target4 = this.FlyBySkillLevel;
    target.FlyBySkillLevel = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SkillFlyByMultiplier, ref target5, hookCtx, false, context))
      target5 = this.SkillFlyByMultiplier;
    target.SkillFlyByMultiplier = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SkillTravelMultiplier, ref target6, hookCtx, false, context))
      target6 = this.SkillTravelMultiplier;
    target.SkillTravelMultiplier = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SkillRechargeMultiplier, ref target7, hookCtx, false, context))
      target7 = this.SkillRechargeMultiplier;
    target.SkillRechargeMultiplier = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Hijackable, ref target8, hookCtx, false, context))
      target8 = this.Hijackable;
    target.Hijackable = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipNavigationComputerComponent target,
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
    DropshipNavigationComputerComponent target1 = (DropshipNavigationComputerComponent) target;
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
    DropshipNavigationComputerComponent target1 = (DropshipNavigationComputerComponent) target;
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
    DropshipNavigationComputerComponent target1 = (DropshipNavigationComputerComponent) target;
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
  virtual DropshipNavigationComputerComponent Component.Instantiate()
  {
    return new DropshipNavigationComputerComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipNavigationComputerComponent_AutoState : IComponentState
  {
    public EntProtoId<SkillDefinitionComponent> Skill;
    public int MultiplierSkillLevel;
    public int FlyBySkillLevel;
    public float SkillFlyByMultiplier;
    public float SkillTravelMultiplier;
    public float SkillRechargeMultiplier;
    public bool Hijackable;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipNavigationComputerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipNavigationComputerComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipNavigationComputerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipNavigationComputerComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipNavigationComputerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DropshipNavigationComputerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipNavigationComputerComponent.DropshipNavigationComputerComponent_AutoState()
      {
        Skill = component.Skill,
        MultiplierSkillLevel = component.MultiplierSkillLevel,
        FlyBySkillLevel = component.FlyBySkillLevel,
        SkillFlyByMultiplier = component.SkillFlyByMultiplier,
        SkillTravelMultiplier = component.SkillTravelMultiplier,
        SkillRechargeMultiplier = component.SkillRechargeMultiplier,
        Hijackable = component.Hijackable
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipNavigationComputerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipNavigationComputerComponent.DropshipNavigationComputerComponent_AutoState current))
        return;
      component.Skill = current.Skill;
      component.MultiplierSkillLevel = current.MultiplierSkillLevel;
      component.FlyBySkillLevel = current.FlyBySkillLevel;
      component.SkillFlyByMultiplier = current.SkillFlyByMultiplier;
      component.SkillTravelMultiplier = current.SkillTravelMultiplier;
      component.SkillRechargeMultiplier = current.SkillRechargeMultiplier;
      component.Hijackable = current.Hijackable;
    }
  }
}

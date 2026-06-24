// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tackle.RMCDisarmComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
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
namespace Content.Shared._RMC14.Tackle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (TackleSystem)})]
public sealed class RMCDisarmComponent : 
  Component,
  ISerializationGenerated<RMCDisarmComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillCqc";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int AccidentalDischargeSkillAmount = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AccidentalDischargeChance = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BaseStunTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<LocId> RandomShoveTexts = new List<LocId>()
  {
    (LocId) "rmc-disarm-text-1",
    (LocId) "rmc-disarm-text-2"
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCDisarmComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCDisarmComponent) target1;
    if (serialization.TryCustomCopy<RMCDisarmComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId<SkillDefinitionComponent> target2 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.AccidentalDischargeSkillAmount, ref target3, hookCtx, false, context))
      target3 = this.AccidentalDischargeSkillAmount;
    target.AccidentalDischargeSkillAmount = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AccidentalDischargeChance, ref target4, hookCtx, false, context))
      target4 = this.AccidentalDischargeChance;
    target.AccidentalDischargeChance = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BaseStunTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.BaseStunTime, hookCtx, context);
    target.BaseStunTime = target5;
    List<LocId> target6 = (List<LocId>) null;
    if (this.RandomShoveTexts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<LocId>>(this.RandomShoveTexts, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<LocId>>(this.RandomShoveTexts, hookCtx, context);
    target.RandomShoveTexts = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCDisarmComponent target,
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
    RMCDisarmComponent target1 = (RMCDisarmComponent) target;
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
    RMCDisarmComponent target1 = (RMCDisarmComponent) target;
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
    RMCDisarmComponent target1 = (RMCDisarmComponent) target;
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
  virtual RMCDisarmComponent Component.Instantiate() => new RMCDisarmComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCDisarmComponent_AutoState : IComponentState
  {
    public EntProtoId<SkillDefinitionComponent> Skill;
    public int AccidentalDischargeSkillAmount;
    public float AccidentalDischargeChance;
    public TimeSpan BaseStunTime;
    public List<LocId> RandomShoveTexts;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCDisarmComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCDisarmComponent, ComponentGetState>(new ComponentEventRefHandler<RMCDisarmComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCDisarmComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCDisarmComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCDisarmComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCDisarmComponent.RMCDisarmComponent_AutoState()
      {
        Skill = component.Skill,
        AccidentalDischargeSkillAmount = component.AccidentalDischargeSkillAmount,
        AccidentalDischargeChance = component.AccidentalDischargeChance,
        BaseStunTime = component.BaseStunTime,
        RandomShoveTexts = component.RandomShoveTexts
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCDisarmComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCDisarmComponent.RMCDisarmComponent_AutoState current))
        return;
      component.Skill = current.Skill;
      component.AccidentalDischargeSkillAmount = current.AccidentalDischargeSkillAmount;
      component.AccidentalDischargeChance = current.AccidentalDischargeChance;
      component.BaseStunTime = current.BaseStunTime;
      component.RandomShoveTexts = current.RandomShoveTexts == null ? (List<LocId>) null : new List<LocId>((IEnumerable<LocId>) current.RandomShoveTexts);
    }
  }
}

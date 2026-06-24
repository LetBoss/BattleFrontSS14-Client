// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Medicine.PubgMedicalComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Medicine;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PubgMedicalComponent : 
  Component,
  ISerializationGenerated<PubgMedicalComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 HealAmount = (FixedPoint2) 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MaxHealThreshold = (FixedPoint2) 0;
  [DataField(null, false, 1, false, false, null)]
  public float Delay = 2f;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<SkillDefinitionComponent> DelaySkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillMedical";
  [DataField(null, false, 1, false, false, null)]
  public float[] SkillDelayMultipliers = new float[2]
  {
    1f,
    0.667f
  };
  [DataField(null, false, 1, false, false, null)]
  public bool BreakOnMove = true;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? HealingBeginSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? HealingLoopSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? HealingEndSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public PubgMedicalType HealingType;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RepeatUse;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 BloodRestore = (FixedPoint2) 0;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? HealingLoopStream;
  [DataField(null, false, 1, false, false, null)]
  public float HealOverTimeSeconds;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgMedicalComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgMedicalComponent) target1;
    if (serialization.TryCustomCopy<PubgMedicalComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.HealAmount, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.HealAmount, hookCtx, context);
    target.HealAmount = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxHealThreshold, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.MaxHealThreshold, hookCtx, context);
    target.MaxHealThreshold = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Delay, ref target4, hookCtx, false, context))
      target4 = this.Delay;
    target.Delay = target4;
    EntProtoId<SkillDefinitionComponent> target5 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.DelaySkill, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.DelaySkill, hookCtx, context);
    target.DelaySkill = target5;
    float[] target6 = (float[]) null;
    if (this.SkillDelayMultipliers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<float[]>(this.SkillDelayMultipliers, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<float[]>(this.SkillDelayMultipliers, hookCtx, context);
    target.SkillDelayMultipliers = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnMove, ref target7, hookCtx, false, context))
      target7 = this.BreakOnMove;
    target.BreakOnMove = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HealingBeginSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.HealingBeginSound, hookCtx, context);
    target.HealingBeginSound = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HealingLoopSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.HealingLoopSound, hookCtx, context);
    target.HealingLoopSound = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HealingEndSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.HealingEndSound, hookCtx, context);
    target.HealingEndSound = target10;
    PubgMedicalType target11 = PubgMedicalType.Bandage;
    if (!serialization.TryCustomCopy<PubgMedicalType>(this.HealingType, ref target11, hookCtx, false, context))
      target11 = this.HealingType;
    target.HealingType = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.RepeatUse, ref target12, hookCtx, false, context))
      target12 = this.RepeatUse;
    target.RepeatUse = target12;
    FixedPoint2 target13 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BloodRestore, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<FixedPoint2>(this.BloodRestore, hookCtx, context);
    target.BloodRestore = target13;
    EntityUid? target14 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.HealingLoopStream, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<EntityUid?>(this.HealingLoopStream, hookCtx, context);
    target.HealingLoopStream = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HealOverTimeSeconds, ref target15, hookCtx, false, context))
      target15 = this.HealOverTimeSeconds;
    target.HealOverTimeSeconds = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgMedicalComponent target,
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
    PubgMedicalComponent target1 = (PubgMedicalComponent) target;
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
    PubgMedicalComponent target1 = (PubgMedicalComponent) target;
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
    PubgMedicalComponent target1 = (PubgMedicalComponent) target;
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
  virtual PubgMedicalComponent Component.Instantiate() => new PubgMedicalComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgMedicalComponent_AutoState : IComponentState
  {
    public FixedPoint2 HealAmount;
    public FixedPoint2 MaxHealThreshold;
    public PubgMedicalType HealingType;
    public bool RepeatUse;
    public FixedPoint2 BloodRestore;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgMedicalComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgMedicalComponent, ComponentGetState>(new ComponentEventRefHandler<PubgMedicalComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgMedicalComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgMedicalComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgMedicalComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgMedicalComponent.PubgMedicalComponent_AutoState()
      {
        HealAmount = component.HealAmount,
        MaxHealThreshold = component.MaxHealThreshold,
        HealingType = component.HealingType,
        RepeatUse = component.RepeatUse,
        BloodRestore = component.BloodRestore
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgMedicalComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgMedicalComponent.PubgMedicalComponent_AutoState current))
        return;
      component.HealAmount = current.HealAmount;
      component.MaxHealThreshold = current.MaxHealThreshold;
      component.HealingType = current.HealingType;
      component.RepeatUse = current.RepeatUse;
      component.BloodRestore = current.BloodRestore;
    }
  }
}

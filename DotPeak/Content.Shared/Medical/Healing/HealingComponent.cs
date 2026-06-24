// Decompiled with JetBrains decompiler
// Type: Content.Shared.Medical.Healing.HealingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Medical.Healing;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class HealingComponent : 
  Component,
  ISerializationGenerated<HealingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BloodlossModifier;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ModifyBloodLevel;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<DamageContainerPrototype>>? DamageContainers;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Delay = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SelfHealPenaltyMultiplier = 3f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? HealingBeginSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? HealingEndSound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HealingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HealingComponent) target1;
    if (serialization.TryCustomCopy<HealingComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target2, hookCtx, false, context))
    {
      if (this.Damage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target2, hookCtx, context, true);
    }
    target.Damage = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BloodlossModifier, ref target3, hookCtx, false, context))
      target3 = this.BloodlossModifier;
    target.BloodlossModifier = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ModifyBloodLevel, ref target4, hookCtx, false, context))
      target4 = this.ModifyBloodLevel;
    target.ModifyBloodLevel = target4;
    List<ProtoId<DamageContainerPrototype>> target5 = (List<ProtoId<DamageContainerPrototype>>) null;
    if (!serialization.TryCustomCopy<List<ProtoId<DamageContainerPrototype>>>(this.DamageContainers, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<ProtoId<DamageContainerPrototype>>>(this.DamageContainers, hookCtx, context);
    target.DamageContainers = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Delay, ref target6, hookCtx, false, context))
      target6 = this.Delay;
    target.Delay = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SelfHealPenaltyMultiplier, ref target7, hookCtx, false, context))
      target7 = this.SelfHealPenaltyMultiplier;
    target.SelfHealPenaltyMultiplier = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HealingBeginSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.HealingBeginSound, hookCtx, context);
    target.HealingBeginSound = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HealingEndSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.HealingEndSound, hookCtx, context);
    target.HealingEndSound = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HealingComponent target,
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
    HealingComponent target1 = (HealingComponent) target;
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
    HealingComponent target1 = (HealingComponent) target;
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
    HealingComponent target1 = (HealingComponent) target;
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
  virtual HealingComponent Component.Instantiate() => new HealingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HealingComponent_AutoState : IComponentState
  {
    public DamageSpecifier Damage;
    public float BloodlossModifier;
    public float ModifyBloodLevel;
    public List<ProtoId<DamageContainerPrototype>>? DamageContainers;
    public float Delay;
    public float SelfHealPenaltyMultiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HealingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HealingComponent, ComponentGetState>(new ComponentEventRefHandler<HealingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HealingComponent, ComponentHandleState>(new ComponentEventRefHandler<HealingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, HealingComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new HealingComponent.HealingComponent_AutoState()
      {
        Damage = component.Damage,
        BloodlossModifier = component.BloodlossModifier,
        ModifyBloodLevel = component.ModifyBloodLevel,
        DamageContainers = component.DamageContainers,
        Delay = component.Delay,
        SelfHealPenaltyMultiplier = component.SelfHealPenaltyMultiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HealingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HealingComponent.HealingComponent_AutoState current))
        return;
      component.Damage = current.Damage;
      component.BloodlossModifier = current.BloodlossModifier;
      component.ModifyBloodLevel = current.ModifyBloodLevel;
      component.DamageContainers = current.DamageContainers == null ? (List<ProtoId<DamageContainerPrototype>>) null : new List<ProtoId<DamageContainerPrototype>>((IEnumerable<ProtoId<DamageContainerPrototype>>) current.DamageContainers);
      component.Delay = current.Delay;
      component.SelfHealPenaltyMultiplier = current.SelfHealPenaltyMultiplier;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Charge.XenoToggleChargingDamageComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Charge;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoChargeSystem)})]
public sealed class XenoToggleChargingDamageComponent : 
  Component,
  ISerializationGenerated<XenoToggleChargingDamageComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int StageLoss;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StageLossProbability = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Destroy;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Stop;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Unanchor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MinimumStage = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PercentageDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? ArmorPiercingDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ArmorPiercing;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (SharedRMCDamageableSystem)})]
  public FixedPoint2 DestroyDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int DefaultMultiplier;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<int, int>? StageMultipliers;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoToggleChargingDamageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoToggleChargingDamageComponent) target1;
    if (serialization.TryCustomCopy<XenoToggleChargingDamageComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.StageLoss, ref target2, hookCtx, false, context))
      target2 = this.StageLoss;
    target.StageLoss = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StageLossProbability, ref target3, hookCtx, false, context))
      target3 = this.StageLossProbability;
    target.StageLossProbability = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Destroy, ref target4, hookCtx, false, context))
      target4 = this.Destroy;
    target.Destroy = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Stop, ref target5, hookCtx, false, context))
      target5 = this.Stop;
    target.Stop = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Unanchor, ref target6, hookCtx, false, context))
      target6 = this.Unanchor;
    target.Unanchor = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinimumStage, ref target7, hookCtx, false, context))
      target7 = this.MinimumStage;
    target.MinimumStage = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target8;
    FixedPoint2 target9 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PercentageDamage, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<FixedPoint2>(this.PercentageDamage, hookCtx, context);
    target.PercentageDamage = target9;
    DamageSpecifier target10 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target10, hookCtx, false, context))
    {
      if (this.Damage == null)
        target10 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target10, hookCtx, context);
    }
    target.Damage = target10;
    DamageSpecifier target11 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.ArmorPiercingDamage, ref target11, hookCtx, false, context))
    {
      if (this.ArmorPiercingDamage == null)
        target11 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.ArmorPiercingDamage, ref target11, hookCtx, context);
    }
    target.ArmorPiercingDamage = target11;
    int target12 = 0;
    if (!serialization.TryCustomCopy<int>(this.ArmorPiercing, ref target12, hookCtx, false, context))
      target12 = this.ArmorPiercing;
    target.ArmorPiercing = target12;
    FixedPoint2 target13 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.DestroyDamage, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<FixedPoint2>(this.DestroyDamage, hookCtx, context);
    target.DestroyDamage = target13;
    int target14 = 0;
    if (!serialization.TryCustomCopy<int>(this.DefaultMultiplier, ref target14, hookCtx, false, context))
      target14 = this.DefaultMultiplier;
    target.DefaultMultiplier = target14;
    Dictionary<int, int> target15 = (Dictionary<int, int>) null;
    if (!serialization.TryCustomCopy<Dictionary<int, int>>(this.StageMultipliers, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<Dictionary<int, int>>(this.StageMultipliers, hookCtx, context);
    target.StageMultipliers = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoToggleChargingDamageComponent target,
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
    XenoToggleChargingDamageComponent target1 = (XenoToggleChargingDamageComponent) target;
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
    XenoToggleChargingDamageComponent target1 = (XenoToggleChargingDamageComponent) target;
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
    XenoToggleChargingDamageComponent target1 = (XenoToggleChargingDamageComponent) target;
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
  virtual XenoToggleChargingDamageComponent Component.Instantiate()
  {
    return new XenoToggleChargingDamageComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoToggleChargingDamageComponent_AutoState : IComponentState
  {
    public int StageLoss;
    public float StageLossProbability;
    public bool Destroy;
    public bool Stop;
    public bool Unanchor;
    public int MinimumStage;
    public SoundSpecifier? Sound;
    public FixedPoint2 PercentageDamage;
    public DamageSpecifier? Damage;
    public DamageSpecifier? ArmorPiercingDamage;
    public int ArmorPiercing;
    public FixedPoint2 DestroyDamage;
    public int DefaultMultiplier;
    public Dictionary<int, int>? StageMultipliers;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoToggleChargingDamageComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoToggleChargingDamageComponent, ComponentGetState>(new ComponentEventRefHandler<XenoToggleChargingDamageComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoToggleChargingDamageComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoToggleChargingDamageComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoToggleChargingDamageComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoToggleChargingDamageComponent.XenoToggleChargingDamageComponent_AutoState()
      {
        StageLoss = component.StageLoss,
        StageLossProbability = component.StageLossProbability,
        Destroy = component.Destroy,
        Stop = component.Stop,
        Unanchor = component.Unanchor,
        MinimumStage = component.MinimumStage,
        Sound = component.Sound,
        PercentageDamage = component.PercentageDamage,
        Damage = component.Damage,
        ArmorPiercingDamage = component.ArmorPiercingDamage,
        ArmorPiercing = component.ArmorPiercing,
        DestroyDamage = component.DestroyDamage,
        DefaultMultiplier = component.DefaultMultiplier,
        StageMultipliers = component.StageMultipliers
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoToggleChargingDamageComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoToggleChargingDamageComponent.XenoToggleChargingDamageComponent_AutoState current))
        return;
      component.StageLoss = current.StageLoss;
      component.StageLossProbability = current.StageLossProbability;
      component.Destroy = current.Destroy;
      component.Stop = current.Stop;
      component.Unanchor = current.Unanchor;
      component.MinimumStage = current.MinimumStage;
      component.Sound = current.Sound;
      component.PercentageDamage = current.PercentageDamage;
      component.Damage = current.Damage;
      component.ArmorPiercingDamage = current.ArmorPiercingDamage;
      component.ArmorPiercing = current.ArmorPiercing;
      component.DestroyDamage = current.DestroyDamage;
      component.DefaultMultiplier = current.DefaultMultiplier;
      component.StageMultipliers = current.StageMultipliers == null ? (Dictionary<int, int>) null : new Dictionary<int, int>((IDictionary<int, int>) current.StageMultipliers);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mobs.Components.MobThresholdsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Mobs.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (MobThresholdSystem)})]
public sealed class MobThresholdsComponent : 
  Component,
  ISerializationGenerated<MobThresholdsComponent>,
  ISerializationGenerated
{
  [DataField("thresholds", false, 1, true, false, null)]
  public SortedDictionary<FixedPoint2, MobState> Thresholds = new SortedDictionary<FixedPoint2, MobState>();
  [DataField("triggersAlerts", false, 1, false, false, null)]
  public bool TriggersAlerts;
  [DataField("currentThresholdState", false, 1, false, false, null)]
  public MobState CurrentThresholdState;
  [DataField("stateAlertDict", false, 1, false, false, null)]
  public Dictionary<MobState, ProtoId<AlertPrototype>> StateAlertDict = new Dictionary<MobState, ProtoId<AlertPrototype>>()
  {
    {
      MobState.Alive,
      (ProtoId<AlertPrototype>) "HumanHealth"
    },
    {
      MobState.Critical,
      (ProtoId<AlertPrototype>) "HumanCrit"
    },
    {
      MobState.Dead,
      (ProtoId<AlertPrototype>) "HumanDead"
    }
  };
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertCategoryPrototype> HealthAlertCategory = (ProtoId<AlertCategoryPrototype>) "Health";
  [DataField("showOverlays", false, 1, false, false, null)]
  public bool ShowOverlays = true;
  [DataField("allowRevives", false, 1, false, false, null)]
  public bool AllowRevives;
  [DataField("displayDamageInAlert", false, 1, false, false, null)]
  public bool DisplayDamageInAlert;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MobThresholdsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MobThresholdsComponent) target1;
    if (serialization.TryCustomCopy<MobThresholdsComponent>(this, ref target, hookCtx, false, context))
      return;
    SortedDictionary<FixedPoint2, MobState> target2 = (SortedDictionary<FixedPoint2, MobState>) null;
    if (this.Thresholds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SortedDictionary<FixedPoint2, MobState>>(this.Thresholds, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SortedDictionary<FixedPoint2, MobState>>(this.Thresholds, hookCtx, context);
    target.Thresholds = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.TriggersAlerts, ref target3, hookCtx, false, context))
      target3 = this.TriggersAlerts;
    target.TriggersAlerts = target3;
    MobState target4 = MobState.Invalid;
    if (!serialization.TryCustomCopy<MobState>(this.CurrentThresholdState, ref target4, hookCtx, false, context))
      target4 = this.CurrentThresholdState;
    target.CurrentThresholdState = target4;
    Dictionary<MobState, ProtoId<AlertPrototype>> target5 = (Dictionary<MobState, ProtoId<AlertPrototype>>) null;
    if (this.StateAlertDict == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<MobState, ProtoId<AlertPrototype>>>(this.StateAlertDict, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<MobState, ProtoId<AlertPrototype>>>(this.StateAlertDict, hookCtx, context);
    target.StateAlertDict = target5;
    ProtoId<AlertCategoryPrototype> target6 = new ProtoId<AlertCategoryPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertCategoryPrototype>>(this.HealthAlertCategory, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<AlertCategoryPrototype>>(this.HealthAlertCategory, hookCtx, context);
    target.HealthAlertCategory = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowOverlays, ref target7, hookCtx, false, context))
      target7 = this.ShowOverlays;
    target.ShowOverlays = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowRevives, ref target8, hookCtx, false, context))
      target8 = this.AllowRevives;
    target.AllowRevives = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.DisplayDamageInAlert, ref target9, hookCtx, false, context))
      target9 = this.DisplayDamageInAlert;
    target.DisplayDamageInAlert = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MobThresholdsComponent target,
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
    MobThresholdsComponent target1 = (MobThresholdsComponent) target;
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
    MobThresholdsComponent target1 = (MobThresholdsComponent) target;
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
    MobThresholdsComponent target1 = (MobThresholdsComponent) target;
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
  virtual MobThresholdsComponent Component.Instantiate() => new MobThresholdsComponent();
}

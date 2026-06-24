// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Laws.Components.IonStormTargetComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Random;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.Laws.Components;

[RegisterComponent]
public sealed class IonStormTargetComponent : 
  Component,
  ISerializationGenerated<IonStormTargetComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public ProtoId<WeightedRandomPrototype> RandomLawsets = (ProtoId<WeightedRandomPrototype>) "IonStormLawsets";
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float Chance = 0.8f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float RandomLawsetChance = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float RemoveChance = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float ReplaceChance = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float ShuffleChance = 0.2f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IonStormTargetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IonStormTargetComponent) target1;
    if (serialization.TryCustomCopy<IonStormTargetComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<WeightedRandomPrototype> target2 = new ProtoId<WeightedRandomPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<WeightedRandomPrototype>>(this.RandomLawsets, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<WeightedRandomPrototype>>(this.RandomLawsets, hookCtx, context);
    target.RandomLawsets = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Chance, ref target3, hookCtx, false, context))
      target3 = this.Chance;
    target.Chance = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RandomLawsetChance, ref target4, hookCtx, false, context))
      target4 = this.RandomLawsetChance;
    target.RandomLawsetChance = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RemoveChance, ref target5, hookCtx, false, context))
      target5 = this.RemoveChance;
    target.RemoveChance = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReplaceChance, ref target6, hookCtx, false, context))
      target6 = this.ReplaceChance;
    target.ReplaceChance = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShuffleChance, ref target7, hookCtx, false, context))
      target7 = this.ShuffleChance;
    target.ShuffleChance = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IonStormTargetComponent target,
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
    IonStormTargetComponent target1 = (IonStormTargetComponent) target;
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
    IonStormTargetComponent target1 = (IonStormTargetComponent) target;
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
    IonStormTargetComponent target1 = (IonStormTargetComponent) target;
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
  virtual IonStormTargetComponent Component.Instantiate() => new IonStormTargetComponent();
}

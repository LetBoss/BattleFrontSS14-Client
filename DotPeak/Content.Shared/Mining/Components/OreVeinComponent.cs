// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mining.Components.OreVeinComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Random;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Mining.Components;

[RegisterComponent]
public sealed class OreVeinComponent : 
  Component,
  ISerializationGenerated<OreVeinComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float OreChance = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<WeightedRandomOrePrototype>? OreRarityPrototypeId;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<OrePrototype>? CurrentOre;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OreVeinComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OreVeinComponent) target1;
    if (serialization.TryCustomCopy<OreVeinComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OreChance, ref target2, hookCtx, false, context))
      target2 = this.OreChance;
    target.OreChance = target2;
    ProtoId<WeightedRandomOrePrototype>? target3 = new ProtoId<WeightedRandomOrePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<WeightedRandomOrePrototype>?>(this.OreRarityPrototypeId, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<WeightedRandomOrePrototype>?>(this.OreRarityPrototypeId, hookCtx, context);
    target.OreRarityPrototypeId = target3;
    ProtoId<OrePrototype>? target4 = new ProtoId<OrePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<OrePrototype>?>(this.CurrentOre, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<OrePrototype>?>(this.CurrentOre, hookCtx, context);
    target.CurrentOre = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OreVeinComponent target,
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
    OreVeinComponent target1 = (OreVeinComponent) target;
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
    OreVeinComponent target1 = (OreVeinComponent) target;
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
    OreVeinComponent target1 = (OreVeinComponent) target;
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
  virtual OreVeinComponent Component.Instantiate() => new OreVeinComponent();
}

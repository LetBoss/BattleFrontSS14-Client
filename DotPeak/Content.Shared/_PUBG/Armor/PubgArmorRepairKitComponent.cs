// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Armor.PubgArmorRepairKitComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Armor;

[RegisterComponent]
public sealed class PubgArmorRepairKitComponent : 
  Component,
  ISerializationGenerated<PubgArmorRepairKitComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float RepairAmount = 30f;
  [DataField(null, false, 1, false, false, null)]
  public float Delay = 2f;
  [DataField(null, false, 1, false, false, null)]
  public bool DeleteOnEmpty = true;
  [DataField(null, false, 1, false, false, null)]
  public SlotFlags BlockedSlots = SlotFlags.HEAD | SlotFlags.OUTERCLOTHING;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? RepairSound;
  [DataField(null, false, 1, false, false, null)]
  public float RepairSoundVolume;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgArmorRepairKitComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgArmorRepairKitComponent) target1;
    if (serialization.TryCustomCopy<PubgArmorRepairKitComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepairAmount, ref target2, hookCtx, false, context))
      target2 = this.RepairAmount;
    target.RepairAmount = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Delay, ref target3, hookCtx, false, context))
      target3 = this.Delay;
    target.Delay = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeleteOnEmpty, ref target4, hookCtx, false, context))
      target4 = this.DeleteOnEmpty;
    target.DeleteOnEmpty = target4;
    SlotFlags target5 = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.BlockedSlots, ref target5, hookCtx, false, context))
      target5 = this.BlockedSlots;
    target.BlockedSlots = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RepairSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.RepairSound, hookCtx, context);
    target.RepairSound = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepairSoundVolume, ref target7, hookCtx, false, context))
      target7 = this.RepairSoundVolume;
    target.RepairSoundVolume = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgArmorRepairKitComponent target,
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
    PubgArmorRepairKitComponent target1 = (PubgArmorRepairKitComponent) target;
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
    PubgArmorRepairKitComponent target1 = (PubgArmorRepairKitComponent) target;
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
    PubgArmorRepairKitComponent target1 = (PubgArmorRepairKitComponent) target;
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
  virtual PubgArmorRepairKitComponent Component.Instantiate() => new PubgArmorRepairKitComponent();
}

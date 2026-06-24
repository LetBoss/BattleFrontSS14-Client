// Decompiled with JetBrains decompiler
// Type: Content.Shared.Payload.Components.ChemicalPayloadComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Payload.Components;

[RegisterComponent]
public sealed class ChemicalPayloadComponent : 
  Component,
  ISerializationGenerated<ChemicalPayloadComponent>,
  ISerializationGenerated
{
  [DataField("beakerSlotA", false, 1, true, false, null)]
  public ItemSlot BeakerSlotA = new ItemSlot();
  [DataField("beakerSlotB", false, 1, true, false, null)]
  public ItemSlot BeakerSlotB = new ItemSlot();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ChemicalPayloadComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ChemicalPayloadComponent) target1;
    if (serialization.TryCustomCopy<ChemicalPayloadComponent>(this, ref target, hookCtx, false, context))
      return;
    ItemSlot target2 = (ItemSlot) null;
    if (this.BeakerSlotA == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.BeakerSlotA, ref target2, hookCtx, false, context))
    {
      if (this.BeakerSlotA == null)
        target2 = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.BeakerSlotA, ref target2, hookCtx, context, true);
    }
    target.BeakerSlotA = target2;
    ItemSlot target3 = (ItemSlot) null;
    if (this.BeakerSlotB == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.BeakerSlotB, ref target3, hookCtx, false, context))
    {
      if (this.BeakerSlotB == null)
        target3 = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.BeakerSlotB, ref target3, hookCtx, context, true);
    }
    target.BeakerSlotB = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ChemicalPayloadComponent target,
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
    ChemicalPayloadComponent target1 = (ChemicalPayloadComponent) target;
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
    ChemicalPayloadComponent target1 = (ChemicalPayloadComponent) target;
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
    ChemicalPayloadComponent target1 = (ChemicalPayloadComponent) target;
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
  virtual ChemicalPayloadComponent Component.Instantiate() => new ChemicalPayloadComponent();
}

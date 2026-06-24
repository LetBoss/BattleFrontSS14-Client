// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.Components.FoldableClothingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class FoldableClothingComponent : 
  Component,
  ISerializationGenerated<FoldableClothingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SlotFlags? FoldedSlots;
  [DataField(null, false, 1, false, false, null)]
  public SlotFlags? UnfoldedSlots;
  [DataField(null, false, 1, false, false, null)]
  public string? FoldedEquippedPrefix;
  [DataField(null, false, 1, false, false, null)]
  public string? FoldedHeldPrefix;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<HumanoidVisualLayers> UnfoldedHideLayers = new HashSet<HumanoidVisualLayers>();
  [DataField(null, false, 1, false, false, null)]
  public HashSet<HumanoidVisualLayers> FoldedHideLayers = new HashSet<HumanoidVisualLayers>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FoldableClothingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (FoldableClothingComponent) component;
    if (serialization.TryCustomCopy<FoldableClothingComponent>(this, ref target, hookCtx, false, context))
      return;
    SlotFlags? nullable1 = new SlotFlags?();
    if (!serialization.TryCustomCopy<SlotFlags?>(this.FoldedSlots, ref nullable1, hookCtx, false, context))
      nullable1 = this.FoldedSlots;
    target.FoldedSlots = nullable1;
    SlotFlags? nullable2 = new SlotFlags?();
    if (!serialization.TryCustomCopy<SlotFlags?>(this.UnfoldedSlots, ref nullable2, hookCtx, false, context))
      nullable2 = this.UnfoldedSlots;
    target.UnfoldedSlots = nullable2;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.FoldedEquippedPrefix, ref str1, hookCtx, false, context))
      str1 = this.FoldedEquippedPrefix;
    target.FoldedEquippedPrefix = str1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.FoldedHeldPrefix, ref str2, hookCtx, false, context))
      str2 = this.FoldedHeldPrefix;
    target.FoldedHeldPrefix = str2;
    HashSet<HumanoidVisualLayers> humanoidVisualLayersSet1 = (HashSet<HumanoidVisualLayers>) null;
    if (this.UnfoldedHideLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(this.UnfoldedHideLayers, ref humanoidVisualLayersSet1, hookCtx, true, context))
      humanoidVisualLayersSet1 = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(this.UnfoldedHideLayers, hookCtx, context, false);
    target.UnfoldedHideLayers = humanoidVisualLayersSet1;
    HashSet<HumanoidVisualLayers> humanoidVisualLayersSet2 = (HashSet<HumanoidVisualLayers>) null;
    if (this.FoldedHideLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(this.FoldedHideLayers, ref humanoidVisualLayersSet2, hookCtx, true, context))
      humanoidVisualLayersSet2 = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(this.FoldedHideLayers, hookCtx, context, false);
    target.FoldedHideLayers = humanoidVisualLayersSet2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FoldableClothingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoldableClothingComponent target1 = (FoldableClothingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoldableClothingComponent target1 = (FoldableClothingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoldableClothingComponent target1 = (FoldableClothingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual FoldableClothingComponent Component.Instantiate() => new FoldableClothingComponent();
}

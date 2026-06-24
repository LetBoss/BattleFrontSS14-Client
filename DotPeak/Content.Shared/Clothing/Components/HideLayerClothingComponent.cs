// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.Components.HideLayerClothingComponent
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
public sealed class HideLayerClothingComponent : 
  Component,
  ISerializationGenerated<HideLayerClothingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Obsolete("This attribute is deprecated, please use Layers instead.")]
  public HashSet<HumanoidVisualLayers>? Slots = new HashSet<HumanoidVisualLayers>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<HumanoidVisualLayers, SlotFlags> Layers = new Dictionary<HumanoidVisualLayers, SlotFlags>();
  [DataField(null, false, 1, false, false, null)]
  public bool HideOnToggle;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HideLayerClothingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (HideLayerClothingComponent) component;
    if (serialization.TryCustomCopy<HideLayerClothingComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<HumanoidVisualLayers> humanoidVisualLayersSet = (HashSet<HumanoidVisualLayers>) null;
    if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(this.Slots, ref humanoidVisualLayersSet, hookCtx, true, context))
      humanoidVisualLayersSet = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(this.Slots, hookCtx, context, false);
    target.Slots = humanoidVisualLayersSet;
    Dictionary<HumanoidVisualLayers, SlotFlags> dictionary = (Dictionary<HumanoidVisualLayers, SlotFlags>) null;
    if (this.Layers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, SlotFlags>>(this.Layers, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, SlotFlags>>(this.Layers, hookCtx, context, false);
    target.Layers = dictionary;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.HideOnToggle, ref flag, hookCtx, false, context))
      flag = this.HideOnToggle;
    target.HideOnToggle = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HideLayerClothingComponent target,
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
    HideLayerClothingComponent target1 = (HideLayerClothingComponent) target;
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
    HideLayerClothingComponent target1 = (HideLayerClothingComponent) target;
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
    HideLayerClothingComponent target1 = (HideLayerClothingComponent) target;
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
  virtual HideLayerClothingComponent Component.Instantiate() => new HideLayerClothingComponent();
}

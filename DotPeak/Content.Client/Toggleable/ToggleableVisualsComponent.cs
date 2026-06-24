// Decompiled with JetBrains decompiler
// Type: Content.Client.Toggleable.ToggleableVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Toggleable;

[RegisterComponent]
public sealed class ToggleableVisualsComponent : 
  Component,
  ISerializationGenerated<ToggleableVisualsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string? SpriteLayer;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<HandLocation, List<PrototypeLayerData>> InhandVisuals = new Dictionary<HandLocation, List<PrototypeLayerData>>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, List<PrototypeLayerData>> ClothingVisuals = new Dictionary<string, List<PrototypeLayerData>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ToggleableVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ToggleableVisualsComponent) component;
    if (serialization.TryCustomCopy<ToggleableVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.SpriteLayer, ref str, hookCtx, false, context))
      str = this.SpriteLayer;
    target.SpriteLayer = str;
    Dictionary<HandLocation, List<PrototypeLayerData>> dictionary1 = (Dictionary<HandLocation, List<PrototypeLayerData>>) null;
    if (this.InhandVisuals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HandLocation, List<PrototypeLayerData>>>(this.InhandVisuals, ref dictionary1, hookCtx, true, context))
      dictionary1 = serialization.CreateCopy<Dictionary<HandLocation, List<PrototypeLayerData>>>(this.InhandVisuals, hookCtx, context, false);
    target.InhandVisuals = dictionary1;
    Dictionary<string, List<PrototypeLayerData>> dictionary2 = (Dictionary<string, List<PrototypeLayerData>>) null;
    if (this.ClothingVisuals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, List<PrototypeLayerData>>>(this.ClothingVisuals, ref dictionary2, hookCtx, true, context))
      dictionary2 = serialization.CreateCopy<Dictionary<string, List<PrototypeLayerData>>>(this.ClothingVisuals, hookCtx, context, false);
    target.ClothingVisuals = dictionary2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ToggleableVisualsComponent target,
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
    ToggleableVisualsComponent target1 = (ToggleableVisualsComponent) target;
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
    ToggleableVisualsComponent target1 = (ToggleableVisualsComponent) target;
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
    ToggleableVisualsComponent target1 = (ToggleableVisualsComponent) target;
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
  virtual ToggleableVisualsComponent Component.Instantiate() => new ToggleableVisualsComponent();
}

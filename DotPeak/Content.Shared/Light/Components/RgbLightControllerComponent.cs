// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.Components.RgbLightControllerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Light.Components;

[NetworkedComponent]
[RegisterComponent]
[Access(new Type[] {typeof (SharedRgbLightControllerSystem)})]
public sealed class RgbLightControllerComponent : 
  Component,
  ISerializationGenerated<RgbLightControllerComponent>,
  ISerializationGenerated
{
  [DataField("layers", false, 1, false, false, null)]
  public List<int>? Layers;
  public Color OriginalLightColor;
  public Dictionary<int, Color>? OriginalLayerColors;
  public EntityUid? Holder;
  public List<string>? HolderLayers;

  [DataField("cycleRate", false, 1, false, false, null)]
  public float CycleRate { get; set; } = 0.1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RgbLightControllerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RgbLightControllerComponent) target1;
    if (serialization.TryCustomCopy<RgbLightControllerComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CycleRate, ref target2, hookCtx, false, context))
      target2 = this.CycleRate;
    target.CycleRate = target2;
    List<int> target3 = (List<int>) null;
    if (!serialization.TryCustomCopy<List<int>>(this.Layers, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<int>>(this.Layers, hookCtx, context);
    target.Layers = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RgbLightControllerComponent target,
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
    RgbLightControllerComponent target1 = (RgbLightControllerComponent) target;
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
    RgbLightControllerComponent target1 = (RgbLightControllerComponent) target;
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
    RgbLightControllerComponent target1 = (RgbLightControllerComponent) target;
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
  virtual RgbLightControllerComponent Component.Instantiate() => new RgbLightControllerComponent();
}

// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Components.LayerChangeOnWeldComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Physics;
using Content.Shared.Tools.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Tools.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (WeldableSystem)})]
public sealed class LayerChangeOnWeldComponent : 
  Component,
  ISerializationGenerated<LayerChangeOnWeldComponent>,
  ISerializationGenerated
{
  [DataField("unWeldedLayer", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public CollisionGroup UnWeldedLayer = CollisionGroup.AirlockLayer;
  [DataField("weldedLayer", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public CollisionGroup WeldedLayer = CollisionGroup.WallLayer;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LayerChangeOnWeldComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LayerChangeOnWeldComponent) target1;
    if (serialization.TryCustomCopy<LayerChangeOnWeldComponent>(this, ref target, hookCtx, false, context))
      return;
    CollisionGroup target2 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.UnWeldedLayer, ref target2, hookCtx, false, context))
      target2 = this.UnWeldedLayer;
    target.UnWeldedLayer = target2;
    CollisionGroup target3 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.WeldedLayer, ref target3, hookCtx, false, context))
      target3 = this.WeldedLayer;
    target.WeldedLayer = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LayerChangeOnWeldComponent target,
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
    LayerChangeOnWeldComponent target1 = (LayerChangeOnWeldComponent) target;
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
    LayerChangeOnWeldComponent target1 = (LayerChangeOnWeldComponent) target;
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
    LayerChangeOnWeldComponent target1 = (LayerChangeOnWeldComponent) target;
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
  virtual LayerChangeOnWeldComponent Component.Instantiate() => new LayerChangeOnWeldComponent();
}

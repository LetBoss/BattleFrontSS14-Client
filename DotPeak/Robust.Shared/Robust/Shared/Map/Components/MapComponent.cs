// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Components.MapComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Map.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class MapComponent : 
  Component,
  ISerializationGenerated<MapComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedMapSystem), typeof (MapManager)})]
  public bool MapPaused;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedMapSystem), typeof (MapManager)})]
  public bool MapInitialized;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool LightingEnabled { get; set; } = true;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [Access(new Type[] {typeof (SharedMapSystem)}, Other = AccessPermissions.ReadExecute)]
  public MapId MapId { get; internal set; } = MapId.Nullspace;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MapComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MapComponent) target1;
    if (serialization.TryCustomCopy<MapComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.LightingEnabled, ref target2, hookCtx, false, context))
      target2 = this.LightingEnabled;
    target.LightingEnabled = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.MapPaused, ref target3, hookCtx, false, context))
      target3 = this.MapPaused;
    target.MapPaused = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.MapInitialized, ref target4, hookCtx, false, context))
      target4 = this.MapInitialized;
    target.MapInitialized = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MapComponent target,
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
    MapComponent target1 = (MapComponent) target;
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
    MapComponent target1 = (MapComponent) target;
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
    MapComponent target1 = (MapComponent) target;
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
  virtual MapComponent Component.Instantiate() => new MapComponent();
}

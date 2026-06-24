// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.AtmosMonitoringConsoleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Consoles;
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
namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedAtmosMonitoringConsoleSystem)})]
public sealed class AtmosMonitoringConsoleComponent : 
  Component,
  ISerializationGenerated<AtmosMonitoringConsoleComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<Vector2i, AtmosPipeChunk> AtmosPipeChunks = new Dictionary<Vector2i, AtmosPipeChunk>();
  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<NetEntity, AtmosDeviceNavMapData> AtmosDevices = new Dictionary<NetEntity, AtmosDeviceNavMapData>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public Color NavMapTileColor;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public Color NavMapWallColor;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool ForceFullUpdate;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AtmosMonitoringConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AtmosMonitoringConsoleComponent) component;
    if (serialization.TryCustomCopy<AtmosMonitoringConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    Color color1 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.NavMapTileColor, ref color1, hookCtx, false, context))
      color1 = serialization.CreateCopy<Color>(this.NavMapTileColor, hookCtx, context, false);
    target.NavMapTileColor = color1;
    Color color2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.NavMapWallColor, ref color2, hookCtx, false, context))
      color2 = serialization.CreateCopy<Color>(this.NavMapWallColor, hookCtx, context, false);
    target.NavMapWallColor = color2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AtmosMonitoringConsoleComponent target,
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
    AtmosMonitoringConsoleComponent target1 = (AtmosMonitoringConsoleComponent) target;
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
    AtmosMonitoringConsoleComponent target1 = (AtmosMonitoringConsoleComponent) target;
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
    AtmosMonitoringConsoleComponent target1 = (AtmosMonitoringConsoleComponent) target;
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
  virtual AtmosMonitoringConsoleComponent Component.Instantiate()
  {
    return new AtmosMonitoringConsoleComponent();
  }
}

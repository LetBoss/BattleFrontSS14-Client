// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.RMCVehicleAutopilotMarkerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
public sealed class RMCVehicleAutopilotMarkerComponent : 
  Component,
  ISerializationGenerated<RMCVehicleAutopilotMarkerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string MarkerName = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public List<string> Aliases = new List<string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCVehicleAutopilotMarkerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCVehicleAutopilotMarkerComponent) target1;
    if (serialization.TryCustomCopy<RMCVehicleAutopilotMarkerComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.MarkerName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.MarkerName, ref target2, hookCtx, false, context))
      target2 = this.MarkerName;
    target.MarkerName = target2;
    List<string> target3 = (List<string>) null;
    if (this.Aliases == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.Aliases, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<string>>(this.Aliases, hookCtx, context);
    target.Aliases = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCVehicleAutopilotMarkerComponent target,
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
    RMCVehicleAutopilotMarkerComponent target1 = (RMCVehicleAutopilotMarkerComponent) target;
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
    RMCVehicleAutopilotMarkerComponent target1 = (RMCVehicleAutopilotMarkerComponent) target;
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
    RMCVehicleAutopilotMarkerComponent target1 = (RMCVehicleAutopilotMarkerComponent) target;
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
  virtual RMCVehicleAutopilotMarkerComponent Component.Instantiate()
  {
    return new RMCVehicleAutopilotMarkerComponent();
  }
}

// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.MapGen.BuildingZoneGroupComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.MapGen;

[RegisterComponent]
public sealed class BuildingZoneGroupComponent : 
  Component,
  ISerializationGenerated<BuildingZoneGroupComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string GroupPath { get; set; } = "";

  [DataField(null, false, 1, false, false, null)]
  public bool CanRotate { get; set; } = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BuildingZoneGroupComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BuildingZoneGroupComponent) target1;
    if (serialization.TryCustomCopy<BuildingZoneGroupComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.GroupPath == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.GroupPath, ref target2, hookCtx, false, context))
      target2 = this.GroupPath;
    target.GroupPath = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanRotate, ref target3, hookCtx, false, context))
      target3 = this.CanRotate;
    target.CanRotate = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BuildingZoneGroupComponent target,
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
    BuildingZoneGroupComponent target1 = (BuildingZoneGroupComponent) target;
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
    BuildingZoneGroupComponent target1 = (BuildingZoneGroupComponent) target;
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
    BuildingZoneGroupComponent target1 = (BuildingZoneGroupComponent) target;
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
  virtual BuildingZoneGroupComponent Component.Instantiate() => new BuildingZoneGroupComponent();
}

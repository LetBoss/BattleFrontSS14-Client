// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Aircraft.AircraftPilotActionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Aircraft;

[RegisterComponent]
public sealed class AircraftPilotActionComponent : 
  Component,
  ISerializationGenerated<AircraftPilotActionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Vehicle;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? AscendAction;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? DescendAction;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? BombAction;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AircraftPilotActionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AircraftPilotActionComponent) target1;
    if (serialization.TryCustomCopy<AircraftPilotActionComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Vehicle, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Vehicle, hookCtx, context);
    target.Vehicle = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.AscendAction, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.AscendAction, hookCtx, context);
    target.AscendAction = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.DescendAction, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.DescendAction, hookCtx, context);
    target.DescendAction = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.BombAction, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.BombAction, hookCtx, context);
    target.BombAction = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AircraftPilotActionComponent target,
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
    AircraftPilotActionComponent target1 = (AircraftPilotActionComponent) target;
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
    AircraftPilotActionComponent target1 = (AircraftPilotActionComponent) target;
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
    AircraftPilotActionComponent target1 = (AircraftPilotActionComponent) target;
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
  virtual AircraftPilotActionComponent Component.Instantiate()
  {
    return new AircraftPilotActionComponent();
  }
}

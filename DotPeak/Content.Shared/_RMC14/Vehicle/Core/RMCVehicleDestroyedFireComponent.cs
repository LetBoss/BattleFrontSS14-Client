// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.RMCVehicleDestroyedFireComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (RMCVehicleDestroyedFireSystem)})]
public sealed class RMCVehicleDestroyedFireComponent : 
  Component,
  ISerializationGenerated<RMCVehicleDestroyedFireComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId InteriorFire = (EntProtoId) "RMCTileFireForeverWeak";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId ExteriorFire = (EntProtoId) "RMCTileFire";
  [DataField(null, false, 1, false, false, null)]
  public float ExteriorFireChance = 0.3f;
  [DataField(null, false, 1, false, false, null)]
  public int ExteriorPaddingTiles = 1;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCVehicleDestroyedFireComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCVehicleDestroyedFireComponent) target1;
    if (serialization.TryCustomCopy<RMCVehicleDestroyedFireComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.InteriorFire, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.InteriorFire, hookCtx, context);
    target.InteriorFire = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ExteriorFire, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.ExteriorFire, hookCtx, context);
    target.ExteriorFire = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExteriorFireChance, ref target4, hookCtx, false, context))
      target4 = this.ExteriorFireChance;
    target.ExteriorFireChance = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.ExteriorPaddingTiles, ref target5, hookCtx, false, context))
      target5 = this.ExteriorPaddingTiles;
    target.ExteriorPaddingTiles = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCVehicleDestroyedFireComponent target,
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
    RMCVehicleDestroyedFireComponent target1 = (RMCVehicleDestroyedFireComponent) target;
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
    RMCVehicleDestroyedFireComponent target1 = (RMCVehicleDestroyedFireComponent) target;
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
    RMCVehicleDestroyedFireComponent target1 = (RMCVehicleDestroyedFireComponent) target;
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
  virtual RMCVehicleDestroyedFireComponent Component.Instantiate()
  {
    return new RMCVehicleDestroyedFireComponent();
  }
}

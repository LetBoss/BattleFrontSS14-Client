// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Prediction.PredictedProjectileClientComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Prediction;

[RegisterComponent]
public sealed class PredictedProjectileClientComponent : 
  Component,
  ISerializationGenerated<PredictedProjectileClientComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Hit;
  [DataField(null, false, 1, false, false, null)]
  public EntityCoordinates? Coordinates;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PredictedProjectileClientComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PredictedProjectileClientComponent) target1;
    if (serialization.TryCustomCopy<PredictedProjectileClientComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Hit, ref target2, hookCtx, false, context))
      target2 = this.Hit;
    target.Hit = target2;
    EntityCoordinates? target3 = new EntityCoordinates?();
    if (!serialization.TryCustomCopy<EntityCoordinates?>(this.Coordinates, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityCoordinates?>(this.Coordinates, hookCtx, context);
    target.Coordinates = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PredictedProjectileClientComponent target,
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
    PredictedProjectileClientComponent target1 = (PredictedProjectileClientComponent) target;
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
    PredictedProjectileClientComponent target1 = (PredictedProjectileClientComponent) target;
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
    PredictedProjectileClientComponent target1 = (PredictedProjectileClientComponent) target;
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
  virtual PredictedProjectileClientComponent Component.Instantiate()
  {
    return new PredictedProjectileClientComponent();
  }
}

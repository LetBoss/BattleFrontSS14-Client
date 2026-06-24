// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Weapons.CivSuppressionPendingShotComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Weapons;

[RegisterComponent]
[Access(new Type[] {typeof (SharedCivSuppressionSystem)})]
public sealed class CivSuppressionPendingShotComponent : 
  Component,
  ISerializationGenerated<CivSuppressionPendingShotComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Shooter;
  [DataField(null, false, 1, false, false, null)]
  public float Intensity;
  [DataField(null, false, 1, false, false, null)]
  public float ShotPenaltyDegrees;
  [DataField(null, false, 1, false, false, null)]
  public float HighStressThreshold;
  [DataField(null, false, 1, false, false, null)]
  public float HighStressShotPenaltyMultiplier;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivSuppressionPendingShotComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivSuppressionPendingShotComponent) target1;
    if (serialization.TryCustomCopy<CivSuppressionPendingShotComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Shooter, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Shooter, hookCtx, context);
    target.Shooter = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Intensity, ref target3, hookCtx, false, context))
      target3 = this.Intensity;
    target.Intensity = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShotPenaltyDegrees, ref target4, hookCtx, false, context))
      target4 = this.ShotPenaltyDegrees;
    target.ShotPenaltyDegrees = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HighStressThreshold, ref target5, hookCtx, false, context))
      target5 = this.HighStressThreshold;
    target.HighStressThreshold = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HighStressShotPenaltyMultiplier, ref target6, hookCtx, false, context))
      target6 = this.HighStressShotPenaltyMultiplier;
    target.HighStressShotPenaltyMultiplier = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivSuppressionPendingShotComponent target,
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
    CivSuppressionPendingShotComponent target1 = (CivSuppressionPendingShotComponent) target;
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
    CivSuppressionPendingShotComponent target1 = (CivSuppressionPendingShotComponent) target;
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
    CivSuppressionPendingShotComponent target1 = (CivSuppressionPendingShotComponent) target;
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
  virtual CivSuppressionPendingShotComponent Component.Instantiate()
  {
    return new CivSuppressionPendingShotComponent();
  }
}

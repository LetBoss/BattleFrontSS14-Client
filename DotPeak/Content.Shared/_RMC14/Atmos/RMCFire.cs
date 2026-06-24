// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Atmos.RMCFire
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Atmos;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class RMCFire : ISerializationGenerated<RMCFire>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Type = (EntProtoId) "RMCTileFire";
  [DataField(null, false, 1, false, false, null)]
  public int Range;
  [DataField(null, false, 1, false, false, null)]
  public int CardinalRange;
  [DataField(null, false, 1, false, false, null)]
  public int OrdinalRange;
  [DataField(null, false, 1, false, false, null)]
  public int? Intensity;
  [DataField(null, false, 1, false, false, null)]
  public int? Duration;
  [DataField(null, false, 1, false, false, null)]
  public int? Total;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCFire target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RMCFire>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target1 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Type, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<EntProtoId>(this.Type, hookCtx, context);
    target.Type = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.CardinalRange, ref target3, hookCtx, false, context))
      target3 = this.CardinalRange;
    target.CardinalRange = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.OrdinalRange, ref target4, hookCtx, false, context))
      target4 = this.OrdinalRange;
    target.OrdinalRange = target4;
    int? target5 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Intensity, ref target5, hookCtx, false, context))
      target5 = this.Intensity;
    target.Intensity = target5;
    int? target6 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Duration, ref target6, hookCtx, false, context))
      target6 = this.Duration;
    target.Duration = target6;
    int? target7 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Total, ref target7, hookCtx, false, context))
      target7 = this.Total;
    target.Total = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCFire target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCFire target1 = (RMCFire) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RMCFire Instantiate() => new RMCFire();
}

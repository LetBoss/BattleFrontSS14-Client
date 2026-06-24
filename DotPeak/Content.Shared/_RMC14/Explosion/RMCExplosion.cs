// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Explosion.RMCExplosion
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Explosion;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Explosion;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class RMCExplosion : ISerializationGenerated<RMCExplosion>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ExplosionPrototype> Type = (ProtoId<ExplosionPrototype>) "RMC";
  [DataField(null, false, 1, false, false, null)]
  public float Total;
  [DataField(null, false, 1, false, false, null)]
  public float Slope;
  [DataField(null, false, 1, false, false, null)]
  public float Max;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCExplosion target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RMCExplosion>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ExplosionPrototype> target1 = new ProtoId<ExplosionPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ExplosionPrototype>>(this.Type, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<ProtoId<ExplosionPrototype>>(this.Type, hookCtx, context);
    target.Type = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Total, ref target2, hookCtx, false, context))
      target2 = this.Total;
    target.Total = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Slope, ref target3, hookCtx, false, context))
      target3 = this.Slope;
    target.Slope = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Max, ref target4, hookCtx, false, context))
      target4 = this.Max;
    target.Max = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCExplosion target,
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
    RMCExplosion target1 = (RMCExplosion) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RMCExplosion Instantiate() => new RMCExplosion();
}

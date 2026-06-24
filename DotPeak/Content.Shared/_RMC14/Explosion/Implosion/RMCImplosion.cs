// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Explosion.Implosion.RMCImplosion
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Explosion.Implosion;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class RMCImplosion : ISerializationGenerated<RMCImplosion>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float PullRange;
  [DataField(null, false, 1, false, false, null)]
  public float PullDistance;
  [DataField(null, false, 1, false, false, null)]
  public float PullSpeed;
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreSize = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCImplosion target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RMCImplosion>(this, ref target, hookCtx, false, context))
      return;
    float target1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PullRange, ref target1, hookCtx, false, context))
      target1 = this.PullRange;
    target.PullRange = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PullDistance, ref target2, hookCtx, false, context))
      target2 = this.PullDistance;
    target.PullDistance = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PullSpeed, ref target3, hookCtx, false, context))
      target3 = this.PullSpeed;
    target.PullSpeed = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreSize, ref target4, hookCtx, false, context))
      target4 = this.IgnoreSize;
    target.IgnoreSize = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCImplosion target,
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
    RMCImplosion target1 = (RMCImplosion) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RMCImplosion Instantiate() => new RMCImplosion();
}

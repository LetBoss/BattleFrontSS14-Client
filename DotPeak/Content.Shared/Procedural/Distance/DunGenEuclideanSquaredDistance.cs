// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.Distance.DunGenEuclideanSquaredDistance
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Procedural.Distance;

public sealed class DunGenEuclideanSquaredDistance : 
  IDunGenDistance,
  ISerializationGenerated<IDunGenDistance>,
  ISerializationGenerated,
  ISerializationGenerated<DunGenEuclideanSquaredDistance>
{
  [DataField(null, false, 1, false, false, null)]
  public float BlendWeight { get; set; } = 0.5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DunGenEuclideanSquaredDistance target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<DunGenEuclideanSquaredDistance>(this, ref target, hookCtx, false, context))
      return;
    float target1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BlendWeight, ref target1, hookCtx, false, context))
      target1 = this.BlendWeight;
    target.BlendWeight = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DunGenEuclideanSquaredDistance target,
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
    DunGenEuclideanSquaredDistance target1 = (DunGenEuclideanSquaredDistance) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IDunGenDistance target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DunGenEuclideanSquaredDistance target1 = (DunGenEuclideanSquaredDistance) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IDunGenDistance) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IDunGenDistance target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public DunGenEuclideanSquaredDistance Instantiate() => new DunGenEuclideanSquaredDistance();

  IDunGenDistance IDunGenDistance.Instantiate() => (IDunGenDistance) this.Instantiate();

  IDunGenDistance ISerializationGenerated<IDunGenDistance>.Instantiate()
  {
    return (IDunGenDistance) this.Instantiate();
  }
}

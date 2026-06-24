// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonGenerators.NoiseDistanceDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Procedural.Distance;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Procedural.DungeonGenerators;

public sealed class NoiseDistanceDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<NoiseDistanceDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public IDunGenDistance? DistanceConfig;
  [DataField(null, false, 1, false, false, null)]
  public Vector2i Size;
  [DataField(null, false, 1, true, false, null)]
  public List<NoiseDunGenLayer> Layers = new List<NoiseDunGenLayer>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NoiseDistanceDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<NoiseDistanceDunGen>(this, ref target, hookCtx, false, context))
      return;
    IDunGenDistance target1 = (IDunGenDistance) null;
    if (!serialization.TryCustomCopy<IDunGenDistance>(this.DistanceConfig, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<IDunGenDistance>(this.DistanceConfig, hookCtx, context);
    target.DistanceConfig = target1;
    Vector2i target2 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Size, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2i>(this.Size, hookCtx, context);
    target.Size = target2;
    List<NoiseDunGenLayer> target3 = (List<NoiseDunGenLayer>) null;
    if (this.Layers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<NoiseDunGenLayer>>(this.Layers, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<NoiseDunGenLayer>>(this.Layers, hookCtx, context);
    target.Layers = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NoiseDistanceDunGen target,
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
    NoiseDistanceDunGen target1 = (NoiseDistanceDunGen) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IDunGenLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NoiseDistanceDunGen target1 = (NoiseDistanceDunGen) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IDunGenLayer) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IDunGenLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public NoiseDistanceDunGen Instantiate() => new NoiseDistanceDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}

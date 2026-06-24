// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonGenerators.NoiseDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Procedural.DungeonGenerators;

public sealed class NoiseDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<NoiseDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public int Iterations = int.MaxValue;
  [DataField(null, false, 1, false, false, null)]
  public int TileCap = 128 /*0x80*/;
  [DataField(null, false, 1, false, false, null)]
  public float CapStd = 8f;
  [DataField(null, false, 1, true, false, null)]
  public List<NoiseDunGenLayer> Layers = new List<NoiseDunGenLayer>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NoiseDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<NoiseDunGen>(this, ref target, hookCtx, false, context))
      return;
    int target1 = 0;
    if (!serialization.TryCustomCopy<int>(this.Iterations, ref target1, hookCtx, false, context))
      target1 = this.Iterations;
    target.Iterations = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.TileCap, ref target2, hookCtx, false, context))
      target2 = this.TileCap;
    target.TileCap = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CapStd, ref target3, hookCtx, false, context))
      target3 = this.CapStd;
    target.CapStd = target3;
    List<NoiseDunGenLayer> target4 = (List<NoiseDunGenLayer>) null;
    if (this.Layers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<NoiseDunGenLayer>>(this.Layers, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<NoiseDunGenLayer>>(this.Layers, hookCtx, context);
    target.Layers = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NoiseDunGen target,
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
    NoiseDunGen target1 = (NoiseDunGen) target;
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
    NoiseDunGen target1 = (NoiseDunGen) target;
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
  public NoiseDunGen Instantiate() => new NoiseDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}

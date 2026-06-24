// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonGenerators.ReplaceTileDunGen
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

public sealed class ReplaceTileDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<ReplaceTileDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public float VariantWeight = 0.1f;
  [DataField(null, false, 1, true, false, null)]
  public List<ReplaceTileLayer> Layers = new List<ReplaceTileLayer>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReplaceTileDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ReplaceTileDunGen>(this, ref target, hookCtx, false, context))
      return;
    float target1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VariantWeight, ref target1, hookCtx, false, context))
      target1 = this.VariantWeight;
    target.VariantWeight = target1;
    List<ReplaceTileLayer> target2 = (List<ReplaceTileLayer>) null;
    if (this.Layers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ReplaceTileLayer>>(this.Layers, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ReplaceTileLayer>>(this.Layers, hookCtx, context);
    target.Layers = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReplaceTileDunGen target,
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
    ReplaceTileDunGen target1 = (ReplaceTileDunGen) target;
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
    ReplaceTileDunGen target1 = (ReplaceTileDunGen) target;
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
  public ReplaceTileDunGen Instantiate() => new ReplaceTileDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}

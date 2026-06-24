// Decompiled with JetBrains decompiler
// Type: Content.Shared.Parallax.Biomes.Layers.IBiomeWorldLayer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Parallax.Biomes.Layers;

public interface IBiomeWorldLayer : 
  IBiomeLayer,
  ISerializationGenerated<IBiomeLayer>,
  ISerializationGenerated,
  ISerializationGenerated<IBiomeWorldLayer>
{
  List<string> AllowedTiles { get; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void InternalCopy(
    ref IBiomeWorldLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<IBiomeWorldLayer>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void Copy(
    ref IBiomeWorldLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    IBiomeWorldLayer target1 = (IBiomeWorldLayer) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void InternalCopy(
    ref IBiomeLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    IBiomeWorldLayer target1 = (IBiomeWorldLayer) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IBiomeLayer) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void Copy(
    ref IBiomeLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  new IBiomeWorldLayer Instantiate() => throw new NotImplementedException();

  IBiomeLayer IBiomeLayer.Instantiate() => (IBiomeLayer) this.Instantiate();

  IBiomeLayer ISerializationGenerated<IBiomeLayer>.Instantiate()
  {
    return (IBiomeLayer) this.Instantiate();
  }
}

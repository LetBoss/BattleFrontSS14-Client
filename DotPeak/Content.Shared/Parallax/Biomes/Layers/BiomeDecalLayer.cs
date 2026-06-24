// Decompiled with JetBrains decompiler
// Type: Content.Shared.Parallax.Biomes.Layers.BiomeDecalLayer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Decals;
using Content.Shared.Maps;
using Robust.Shared.Noise;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using System;

#nullable enable
namespace Content.Shared.Parallax.Biomes.Layers;

[NetSerializable]
[Serializable]
public sealed class BiomeDecalLayer : 
  IBiomeWorldLayer,
  IBiomeLayer,
  ISerializationGenerated<IBiomeLayer>,
  ISerializationGenerated,
  ISerializationGenerated<IBiomeWorldLayer>,
  ISerializationGenerated<BiomeDecalLayer>
{
  [DataField("divisions", false, 1, false, false, null)]
  public float Divisions = 1f;
  [DataField("decals", false, 1, true, false, typeof (PrototypeIdListSerializer<DecalPrototype>))]
  public System.Collections.Generic.List<string> Decals = new System.Collections.Generic.List<string>();

  [DataField("allowedTiles", false, 1, false, false, typeof (PrototypeIdListSerializer<ContentTileDefinition>))]
  public System.Collections.Generic.List<string> AllowedTiles { get; private set; } = new System.Collections.Generic.List<string>();

  [DataField("noise", false, 1, false, false, null)]
  public FastNoiseLite Noise { get; private set; } = new FastNoiseLite(0);

  [DataField("threshold", false, 1, false, false, null)]
  public float Threshold { get; private set; } = 0.8f;

  [DataField("invert", false, 1, false, false, null)]
  public bool Invert { get; private set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BiomeDecalLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<BiomeDecalLayer>(this, ref target, hookCtx, false, context))
      return;
    System.Collections.Generic.List<string> target1 = (System.Collections.Generic.List<string>) null;
    if (this.AllowedTiles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.List<string>>(this.AllowedTiles, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<System.Collections.Generic.List<string>>(this.AllowedTiles, hookCtx, context);
    target.AllowedTiles = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Divisions, ref target2, hookCtx, false, context))
      target2 = this.Divisions;
    target.Divisions = target2;
    FastNoiseLite target3 = (FastNoiseLite) null;
    if (this.Noise == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<FastNoiseLite>(this.Noise, ref target3, hookCtx, true, context))
    {
      if (this.Noise == null)
        target3 = (FastNoiseLite) null;
      else
        serialization.CopyTo<FastNoiseLite>(this.Noise, ref target3, hookCtx, context, true);
    }
    target.Noise = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Threshold, ref target4, hookCtx, false, context))
      target4 = this.Threshold;
    target.Threshold = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Invert, ref target5, hookCtx, false, context))
      target5 = this.Invert;
    target.Invert = target5;
    System.Collections.Generic.List<string> target6 = (System.Collections.Generic.List<string>) null;
    if (this.Decals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.List<string>>(this.Decals, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<System.Collections.Generic.List<string>>(this.Decals, hookCtx, context);
    target.Decals = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BiomeDecalLayer target,
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
    BiomeDecalLayer target1 = (BiomeDecalLayer) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IBiomeWorldLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BiomeDecalLayer target1 = (BiomeDecalLayer) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IBiomeWorldLayer) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IBiomeWorldLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IBiomeLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BiomeDecalLayer target1 = (BiomeDecalLayer) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IBiomeLayer) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IBiomeLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public BiomeDecalLayer Instantiate() => new BiomeDecalLayer();

  IBiomeWorldLayer IBiomeWorldLayer.Instantiate() => (IBiomeWorldLayer) this.Instantiate();

  IBiomeWorldLayer ISerializationGenerated<IBiomeWorldLayer>.Instantiate()
  {
    return (IBiomeWorldLayer) this.Instantiate();
  }
}

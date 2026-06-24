// Decompiled with JetBrains decompiler
// Type: Content.Shared.Parallax.Biomes.Layers.BiomeEntityLayer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
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
public sealed class BiomeEntityLayer : 
  IBiomeWorldLayer,
  IBiomeLayer,
  ISerializationGenerated<IBiomeLayer>,
  ISerializationGenerated,
  ISerializationGenerated<IBiomeWorldLayer>,
  ISerializationGenerated<BiomeEntityLayer>
{
  [DataField("entities", false, 1, true, false, typeof (PrototypeIdListSerializer<EntityPrototype>))]
  public System.Collections.Generic.List<string> Entities = new System.Collections.Generic.List<string>();

  [DataField("allowedTiles", false, 1, false, false, typeof (PrototypeIdListSerializer<ContentTileDefinition>))]
  public System.Collections.Generic.List<string> AllowedTiles { get; private set; } = new System.Collections.Generic.List<string>();

  [DataField("noise", false, 1, false, false, null)]
  public FastNoiseLite Noise { get; private set; } = new FastNoiseLite(0);

  [DataField("threshold", false, 1, false, false, null)]
  public float Threshold { get; private set; } = 0.5f;

  [DataField("invert", false, 1, false, false, null)]
  public bool Invert { get; private set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BiomeEntityLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<BiomeEntityLayer>(this, ref target, hookCtx, false, context))
      return;
    System.Collections.Generic.List<string> target1 = (System.Collections.Generic.List<string>) null;
    if (this.AllowedTiles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.List<string>>(this.AllowedTiles, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<System.Collections.Generic.List<string>>(this.AllowedTiles, hookCtx, context);
    target.AllowedTiles = target1;
    FastNoiseLite target2 = (FastNoiseLite) null;
    if (this.Noise == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<FastNoiseLite>(this.Noise, ref target2, hookCtx, true, context))
    {
      if (this.Noise == null)
        target2 = (FastNoiseLite) null;
      else
        serialization.CopyTo<FastNoiseLite>(this.Noise, ref target2, hookCtx, context, true);
    }
    target.Noise = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Threshold, ref target3, hookCtx, false, context))
      target3 = this.Threshold;
    target.Threshold = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Invert, ref target4, hookCtx, false, context))
      target4 = this.Invert;
    target.Invert = target4;
    System.Collections.Generic.List<string> target5 = (System.Collections.Generic.List<string>) null;
    if (this.Entities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.List<string>>(this.Entities, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<System.Collections.Generic.List<string>>(this.Entities, hookCtx, context);
    target.Entities = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BiomeEntityLayer target,
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
    BiomeEntityLayer target1 = (BiomeEntityLayer) target;
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
    BiomeEntityLayer target1 = (BiomeEntityLayer) target;
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
    BiomeEntityLayer target1 = (BiomeEntityLayer) target;
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
  public BiomeEntityLayer Instantiate() => new BiomeEntityLayer();

  IBiomeWorldLayer IBiomeWorldLayer.Instantiate() => (IBiomeWorldLayer) this.Instantiate();

  IBiomeWorldLayer ISerializationGenerated<IBiomeWorldLayer>.Instantiate()
  {
    return (IBiomeWorldLayer) this.Instantiate();
  }
}

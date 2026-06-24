// Decompiled with JetBrains decompiler
// Type: Content.Shared.Parallax.Biomes.Layers.BiomeMetaLayer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Noise;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;

#nullable enable
namespace Content.Shared.Parallax.Biomes.Layers;

[NetSerializable]
[Serializable]
public sealed class BiomeMetaLayer : 
  IBiomeLayer,
  ISerializationGenerated<IBiomeLayer>,
  ISerializationGenerated,
  ISerializationGenerated<BiomeMetaLayer>
{
  [DataField("template", false, 1, true, false, typeof (PrototypeIdSerializer<BiomeTemplatePrototype>))]
  public string Template = string.Empty;

  [DataField("noise", false, 1, false, false, null)]
  public FastNoiseLite Noise { get; private set; } = new FastNoiseLite(0);

  [DataField("threshold", false, 1, false, false, null)]
  public float Threshold { get; private set; } = -1f;

  [DataField("invert", false, 1, false, false, null)]
  public bool Invert { get; private set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BiomeMetaLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<BiomeMetaLayer>(this, ref target, hookCtx, false, context))
      return;
    FastNoiseLite target1 = (FastNoiseLite) null;
    if (this.Noise == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<FastNoiseLite>(this.Noise, ref target1, hookCtx, true, context))
    {
      if (this.Noise == null)
        target1 = (FastNoiseLite) null;
      else
        serialization.CopyTo<FastNoiseLite>(this.Noise, ref target1, hookCtx, context, true);
    }
    target.Noise = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Threshold, ref target2, hookCtx, false, context))
      target2 = this.Threshold;
    target.Threshold = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Invert, ref target3, hookCtx, false, context))
      target3 = this.Invert;
    target.Invert = target3;
    string target4 = (string) null;
    if (this.Template == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Template, ref target4, hookCtx, false, context))
      target4 = this.Template;
    target.Template = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BiomeMetaLayer target,
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
    BiomeMetaLayer target1 = (BiomeMetaLayer) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IBiomeLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BiomeMetaLayer target1 = (BiomeMetaLayer) target;
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
  public BiomeMetaLayer Instantiate() => new BiomeMetaLayer();

  IBiomeLayer IBiomeLayer.Instantiate() => (IBiomeLayer) this.Instantiate();

  IBiomeLayer ISerializationGenerated<IBiomeLayer>.Instantiate()
  {
    return (IBiomeLayer) this.Instantiate();
  }
}

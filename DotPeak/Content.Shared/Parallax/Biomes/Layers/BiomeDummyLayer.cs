// Decompiled with JetBrains decompiler
// Type: Content.Shared.Parallax.Biomes.Layers.BiomeDummyLayer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Noise;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.Parallax.Biomes.Layers;

[NetSerializable]
[Serializable]
public sealed class BiomeDummyLayer : 
  IBiomeLayer,
  ISerializationGenerated<IBiomeLayer>,
  ISerializationGenerated,
  ISerializationGenerated<BiomeDummyLayer>
{
  [DataField("id", false, 1, true, false, null)]
  public string ID = string.Empty;

  public FastNoiseLite Noise { get; } = new FastNoiseLite();

  public float Threshold { get; }

  public bool Invert { get; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BiomeDummyLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<BiomeDummyLayer>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.ID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ID, ref target1, hookCtx, false, context))
      target1 = this.ID;
    target.ID = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BiomeDummyLayer target,
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
    BiomeDummyLayer target1 = (BiomeDummyLayer) target;
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
    BiomeDummyLayer target1 = (BiomeDummyLayer) target;
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
  public BiomeDummyLayer Instantiate() => new BiomeDummyLayer();

  IBiomeLayer IBiomeLayer.Instantiate() => (IBiomeLayer) this.Instantiate();

  IBiomeLayer ISerializationGenerated<IBiomeLayer>.Instantiate()
  {
    return (IBiomeLayer) this.Instantiate();
  }
}

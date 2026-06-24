// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.Loot.BiomeMarkerLoot
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Parallax.Biomes.Markers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Procedural.Loot;

public sealed class BiomeMarkerLoot : 
  IDungeonLoot,
  ISerializationGenerated<IDungeonLoot>,
  ISerializationGenerated,
  ISerializationGenerated<BiomeMarkerLoot>
{
  [DataField("proto", false, 1, true, false, null)]
  public ProtoId<BiomeMarkerLayerPrototype> Prototype;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BiomeMarkerLoot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<BiomeMarkerLoot>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<BiomeMarkerLayerPrototype> target1 = new ProtoId<BiomeMarkerLayerPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<BiomeMarkerLayerPrototype>>(this.Prototype, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<ProtoId<BiomeMarkerLayerPrototype>>(this.Prototype, hookCtx, context);
    target.Prototype = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BiomeMarkerLoot target,
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
    BiomeMarkerLoot target1 = (BiomeMarkerLoot) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IDungeonLoot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BiomeMarkerLoot target1 = (BiomeMarkerLoot) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IDungeonLoot) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IDungeonLoot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public BiomeMarkerLoot Instantiate() => new BiomeMarkerLoot();

  IDungeonLoot IDungeonLoot.Instantiate() => (IDungeonLoot) this.Instantiate();

  IDungeonLoot ISerializationGenerated<IDungeonLoot>.Instantiate()
  {
    return (IDungeonLoot) this.Instantiate();
  }
}

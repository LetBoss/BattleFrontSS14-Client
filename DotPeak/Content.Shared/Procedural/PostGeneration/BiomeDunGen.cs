// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.PostGeneration.BiomeDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Content.Shared.Parallax.Biomes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Procedural.PostGeneration;

public sealed class BiomeDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<BiomeDunGen>
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<BiomeTemplatePrototype> BiomeTemplate;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<ContentTileDefinition>>? TileMask;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BiomeDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<BiomeDunGen>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<BiomeTemplatePrototype> target1 = new ProtoId<BiomeTemplatePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<BiomeTemplatePrototype>>(this.BiomeTemplate, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<ProtoId<BiomeTemplatePrototype>>(this.BiomeTemplate, hookCtx, context);
    target.BiomeTemplate = target1;
    HashSet<ProtoId<ContentTileDefinition>> target2 = (HashSet<ProtoId<ContentTileDefinition>>) null;
    if (!serialization.TryCustomCopy<HashSet<ProtoId<ContentTileDefinition>>>(this.TileMask, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<ProtoId<ContentTileDefinition>>>(this.TileMask, hookCtx, context);
    target.TileMask = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BiomeDunGen target,
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
    BiomeDunGen target1 = (BiomeDunGen) target;
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
    BiomeDunGen target1 = (BiomeDunGen) target;
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
  public BiomeDunGen Instantiate() => new BiomeDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}

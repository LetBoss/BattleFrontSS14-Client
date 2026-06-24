// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.PostGeneration.CorridorDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Procedural.PostGeneration;

public sealed class CorridorDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<CorridorDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public int PathLimit = 2048 /*0x0800*/;
  [DataField(null, false, 1, false, false, null)]
  public float Width = 3f;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<ContentTileDefinition> Tile;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CorridorDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CorridorDunGen>(this, ref target, hookCtx, false, context))
      return;
    int target1 = 0;
    if (!serialization.TryCustomCopy<int>(this.PathLimit, ref target1, hookCtx, false, context))
      target1 = this.PathLimit;
    target.PathLimit = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Width, ref target2, hookCtx, false, context))
      target2 = this.Width;
    target.Width = target2;
    ProtoId<ContentTileDefinition> target3 = new ProtoId<ContentTileDefinition>();
    if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>>(this.Tile, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<ContentTileDefinition>>(this.Tile, hookCtx, context);
    target.Tile = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CorridorDunGen target,
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
    CorridorDunGen target1 = (CorridorDunGen) target;
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
    CorridorDunGen target1 = (CorridorDunGen) target;
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
  public CorridorDunGen Instantiate() => new CorridorDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}

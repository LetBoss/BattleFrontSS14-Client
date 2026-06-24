// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.PostGeneration.WormCorridorDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Procedural.PostGeneration;

public sealed class WormCorridorDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<WormCorridorDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public int PathLimit = 2048 /*0x0800*/;
  [DataField(null, false, 1, false, false, null)]
  public int Count = 20;
  [DataField(null, false, 1, false, false, null)]
  public int Length = 20;
  [DataField(null, false, 1, false, false, null)]
  public Angle MaxAngleChange = Angle.FromDegrees(45.0);
  [DataField(null, false, 1, false, false, null)]
  public float Width = 3f;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<ContentTileDefinition> Tile;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WormCorridorDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<WormCorridorDunGen>(this, ref target, hookCtx, false, context))
      return;
    int target1 = 0;
    if (!serialization.TryCustomCopy<int>(this.PathLimit, ref target1, hookCtx, false, context))
      target1 = this.PathLimit;
    target.PathLimit = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Count, ref target2, hookCtx, false, context))
      target2 = this.Count;
    target.Count = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Length, ref target3, hookCtx, false, context))
      target3 = this.Length;
    target.Length = target3;
    Angle target4 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.MaxAngleChange, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Angle>(this.MaxAngleChange, hookCtx, context);
    target.MaxAngleChange = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Width, ref target5, hookCtx, false, context))
      target5 = this.Width;
    target.Width = target5;
    ProtoId<ContentTileDefinition> target6 = new ProtoId<ContentTileDefinition>();
    if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>>(this.Tile, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<ContentTileDefinition>>(this.Tile, hookCtx, context);
    target.Tile = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WormCorridorDunGen target,
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
    WormCorridorDunGen target1 = (WormCorridorDunGen) target;
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
    WormCorridorDunGen target1 = (WormCorridorDunGen) target;
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
  public WormCorridorDunGen Instantiate() => new WormCorridorDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}

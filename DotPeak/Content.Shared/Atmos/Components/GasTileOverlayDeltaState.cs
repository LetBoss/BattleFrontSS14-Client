// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.GasTileOverlayDeltaState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Atmos.Components;

[NetSerializable]
[Serializable]
public sealed class GasTileOverlayDeltaState : 
  ComponentState,
  IComponentDeltaState<GasTileOverlayState>,
  IComponentDeltaState,
  IComponentState
{
  public readonly Dictionary<Vector2i, GasOverlayChunk> ModifiedChunks;
  public readonly HashSet<Vector2i> AllChunks;

  public GasTileOverlayDeltaState(
    Dictionary<Vector2i, GasOverlayChunk> modifiedChunks,
    HashSet<Vector2i> allChunks)
  {
    this.ModifiedChunks = modifiedChunks;
    this.AllChunks = allChunks;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public void ApplyToFullState(GasTileOverlayState state)
  {
    foreach (Vector2i key in state.Chunks.Keys)
    {
      if (!this.AllChunks.Contains(key))
        state.Chunks.Remove(key);
    }
    foreach ((Vector2i key, GasOverlayChunk data) in this.ModifiedChunks)
      state.Chunks[key] = new GasOverlayChunk(data);
  }

  public GasTileOverlayState CreateNewFullState(GasTileOverlayState state)
  {
    Dictionary<Vector2i, GasOverlayChunk> chunks = new Dictionary<Vector2i, GasOverlayChunk>(this.AllChunks.Count);
    foreach ((Vector2i key3, GasOverlayChunk gasOverlayChunk) in this.ModifiedChunks)
    {
      Vector2i key2 = key3;
      GasOverlayChunk data = gasOverlayChunk;
      chunks[key2] = new GasOverlayChunk(data);
    }
    foreach ((key3, gasOverlayChunk) in state.Chunks)
    {
      Vector2i key4 = key3;
      GasOverlayChunk data = gasOverlayChunk;
      if (this.AllChunks.Contains(key4))
        chunks.TryAdd(key4, new GasOverlayChunk(data));
    }
    return new GasTileOverlayState(chunks);
  }
}

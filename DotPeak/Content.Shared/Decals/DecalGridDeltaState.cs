// Decompiled with JetBrains decompiler
// Type: Content.Shared.Decals.DecalGridDeltaState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Decals;

[NetSerializable]
[Serializable]
public sealed class DecalGridDeltaState : 
  ComponentState,
  IComponentDeltaState<DecalGridState>,
  IComponentDeltaState,
  IComponentState
{
  public Dictionary<Vector2i, DecalGridComponent.DecalChunk> ModifiedChunks;
  public HashSet<Vector2i> AllChunks;

  public DecalGridDeltaState(
    Dictionary<Vector2i, DecalGridComponent.DecalChunk> modifiedChunks,
    HashSet<Vector2i> allChunks)
  {
    this.ModifiedChunks = modifiedChunks;
    this.AllChunks = allChunks;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public void ApplyToFullState(DecalGridState state)
  {
    foreach (Vector2i key in state.Chunks.Keys)
    {
      if (!this.AllChunks.Contains(key))
        state.Chunks.Remove(key);
    }
    foreach ((Vector2i key, DecalGridComponent.DecalChunk chunk) in this.ModifiedChunks)
      state.Chunks[key] = new DecalGridComponent.DecalChunk(chunk);
  }

  public DecalGridState CreateNewFullState(DecalGridState state)
  {
    Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunks = new Dictionary<Vector2i, DecalGridComponent.DecalChunk>(state.Chunks.Count);
    foreach ((Vector2i key3, DecalGridComponent.DecalChunk decalChunk) in this.ModifiedChunks)
    {
      Vector2i key2 = key3;
      DecalGridComponent.DecalChunk chunk = decalChunk;
      chunks[key2] = new DecalGridComponent.DecalChunk(chunk);
    }
    foreach ((key3, decalChunk) in state.Chunks)
    {
      Vector2i key4 = key3;
      DecalGridComponent.DecalChunk chunk = decalChunk;
      if (this.AllChunks.Contains(key4))
        chunks.TryAdd(key4, new DecalGridComponent.DecalChunk(chunk));
    }
    return new DecalGridState(chunks);
  }
}

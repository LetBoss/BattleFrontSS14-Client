// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Components.MapGridComponentDeltaState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Map.Components;

[NetSerializable]
[Serializable]
internal sealed class MapGridComponentDeltaState(
  ushort chunkSize,
  Dictionary<Vector2i, ChunkDatum>? chunkData,
  GameTick lastTileModifiedTick) : 
  ComponentState,
  IComponentDeltaState<MapGridComponentState>,
  IComponentDeltaState,
  IComponentState
{
  public readonly ushort ChunkSize = chunkSize;
  public readonly Dictionary<Vector2i, ChunkDatum>? ChunkData = chunkData;
  public GameTick LastTileModifiedTick = lastTileModifiedTick;

  public void ApplyToFullState(MapGridComponentState state)
  {
    state.ChunkSize = this.ChunkSize;
    if (this.ChunkData == null)
      return;
    foreach ((Vector2i key, ChunkDatum chunkDatum) in this.ChunkData)
    {
      if (chunkDatum.IsDeleted())
        state.FullGridData.Remove(key);
      else
        state.FullGridData[key] = chunkDatum;
    }
    state.LastTileModifiedTick = this.LastTileModifiedTick;
  }

  public MapGridComponentState CreateNewFullState(MapGridComponentState state)
  {
    if (this.ChunkData == null)
      return new MapGridComponentState(this.ChunkSize, state.FullGridData, state.LastTileModifiedTick);
    MapGridComponentState state1 = new MapGridComponentState(this.ChunkSize, state.FullGridData.ShallowClone<Vector2i, ChunkDatum>(), this.LastTileModifiedTick);
    this.ApplyToFullState(state1);
    return state1;
  }
}

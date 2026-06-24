// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Components.MapGridComponentState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Map.Components;

[NetSerializable]
[Serializable]
internal sealed class MapGridComponentState(
  ushort chunkSize,
  Dictionary<Vector2i, ChunkDatum> fullGridData,
  GameTick lastTileModifiedTick) : ComponentState
{
  public ushort ChunkSize = chunkSize;
  public Dictionary<Vector2i, ChunkDatum> FullGridData = fullGridData;
  public GameTick LastTileModifiedTick = lastTileModifiedTick;
}

// Decompiled with JetBrains decompiler
// Type: Content.Shared.Explosion.Components.ExplosionVisualsState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.Explosion.Components;

[NetSerializable]
[Serializable]
public sealed class ExplosionVisualsState : ComponentState
{
  public MapCoordinates Epicenter;
  public Dictionary<int, List<Vector2i>>? SpaceTiles;
  public Dictionary<NetEntity, Dictionary<int, List<Vector2i>>> Tiles;
  public List<float> Intensity;
  public string ExplosionType = string.Empty;
  public Matrix3x2 SpaceMatrix;
  public ushort SpaceTileSize;

  public ExplosionVisualsState(
    MapCoordinates epicenter,
    string typeID,
    List<float> intensity,
    Dictionary<int, List<Vector2i>>? spaceTiles,
    Dictionary<NetEntity, Dictionary<int, List<Vector2i>>> tiles,
    Matrix3x2 spaceMatrix,
    ushort spaceTileSize)
  {
    this.Epicenter = epicenter;
    this.SpaceTiles = spaceTiles;
    this.Tiles = tiles;
    this.Intensity = intensity;
    this.ExplosionType = typeID;
    this.SpaceMatrix = spaceMatrix;
    this.SpaceTileSize = spaceTileSize;
  }
}

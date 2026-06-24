// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.IMapManagerInternal
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

#nullable enable
namespace Robust.Shared.Map;

internal interface IMapManagerInternal : IMapManager
{
  void RaiseOnTileChanged(
    Entity<MapGridComponent> entity,
    TileRef tileRef,
    Tile oldTile,
    Vector2i chunk);
}

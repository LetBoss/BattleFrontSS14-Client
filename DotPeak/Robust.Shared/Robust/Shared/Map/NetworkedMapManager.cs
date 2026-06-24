// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.NetworkedMapManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Robust.Shared.Map;

internal sealed class NetworkedMapManager : 
  MapManager,
  INetworkedMapManager,
  IMapManagerInternal,
  IMapManager
{
  public void CullDeletionHistory(GameTick upToTick)
  {
    AllEntityQueryEnumerator<MapGridComponent> entityQueryEnumerator = this.EntityManager.AllEntityQueryEnumerator<MapGridComponent>();
    MapGridComponent comp1;
    while (entityQueryEnumerator.MoveNext(out comp1))
      comp1.ChunkDeletionHistory.RemoveAll((Predicate<(GameTick, Vector2i)>) (t => t.tick < upToTick));
  }

  MapGridComponent IMapManager.CreateGrid(MapId currentMapId, in GridCreateOptions options)
  {
    return this.CreateGrid(currentMapId, in options);
  }
}

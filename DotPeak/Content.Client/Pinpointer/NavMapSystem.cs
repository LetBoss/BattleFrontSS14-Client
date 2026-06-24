// Decompiled with JetBrains decompiler
// Type: Content.Client.Pinpointer.NavMapSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos;
using Content.Shared.Pinpointer;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Pinpointer;

public sealed class NavMapSystem : SharedNavMapSystem
{
  private (AtmosDirection, Vector2i, AtmosDirection)[] _regionPropagationTable = new (AtmosDirection, Vector2i, AtmosDirection)[4]
  {
    (AtmosDirection.East, new Vector2i(1, 0), AtmosDirection.West),
    (AtmosDirection.West, new Vector2i(-1, 0), AtmosDirection.East),
    (AtmosDirection.North, new Vector2i(0, 1), AtmosDirection.South),
    (AtmosDirection.South, new Vector2i(0, -1), AtmosDirection.North)
  };

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<NavMapComponent, ComponentHandleState>(new ComponentEventRefHandler<NavMapComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    EntityUid uid,
    NavMapComponent component,
    ref ComponentHandleState args)
  {
    Dictionary<Vector2i, int[]> dictionary;
    Dictionary<NetEntity, SharedNavMapSystem.NavMapBeacon> beacons;
    Dictionary<NetEntity, SharedNavMapSystem.NavMapRegionProperties> regions;
    switch (((ComponentHandleState) ref args).Current)
    {
      case SharedNavMapSystem.NavMapDeltaState navMapDeltaState:
        dictionary = navMapDeltaState.ModifiedChunks;
        beacons = navMapDeltaState.Beacons;
        regions = navMapDeltaState.Regions;
        using (Dictionary<Vector2i, NavMapChunk>.KeyCollection.Enumerator enumerator = component.Chunks.Keys.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Vector2i current = enumerator.Current;
            if (!navMapDeltaState.AllChunks.Contains(current))
              component.Chunks.Remove(current);
          }
          break;
        }
      case SharedNavMapSystem.NavMapState navMapState:
        dictionary = navMapState.Chunks;
        beacons = navMapState.Beacons;
        regions = navMapState.Regions;
        using (Dictionary<Vector2i, NavMapChunk>.KeyCollection.Enumerator enumerator = component.Chunks.Keys.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Vector2i current = enumerator.Current;
            if (!navMapState.Chunks.ContainsKey(current))
              component.Chunks.Remove(current);
          }
          break;
        }
      default:
        return;
    }
    List<NetEntity> list = component.RegionProperties.Keys.ToList<NetEntity>();
    List<NetEntity> second = new List<NetEntity>();
    component.RegionProperties.Clear();
    foreach ((NetEntity key3, SharedNavMapSystem.NavMapRegionProperties regionProperties1) in regions)
    {
      NetEntity key2 = key3;
      SharedNavMapSystem.NavMapRegionProperties regionProperties2 = regionProperties1;
      if (regionProperties2.Seeds.Any<Vector2i>())
      {
        component.RegionProperties[key2] = regionProperties2;
        second.Add(key2);
        if (!component.RegionOverlays.ContainsKey(key2) && !component.QueuedRegionsToFlood.Contains(key2))
          component.QueuedRegionsToFlood.Enqueue(key2);
      }
    }
    foreach (NetEntity regionOwner in list.Except<NetEntity>((IEnumerable<NetEntity>) second))
      this.RemoveNavMapRegion(uid, component, regionOwner);
    foreach ((Vector2i vector2i, int[] sourceArray) in dictionary)
    {
      NavMapChunk navMapChunk = new NavMapChunk(vector2i);
      Array.Copy((Array) sourceArray, (Array) navMapChunk.TileData, sourceArray.Length);
      component.Chunks[vector2i] = navMapChunk;
      HashSet<NetEntity> netEntitySet;
      if (component.ChunkToRegionOwnerTable.TryGetValue(vector2i, out netEntitySet))
      {
        foreach (NetEntity netEntity in netEntitySet)
        {
          if (!component.QueuedRegionsToFlood.Contains(netEntity))
            component.QueuedRegionsToFlood.Enqueue(netEntity);
        }
      }
    }
    component.Beacons.Clear();
    SharedNavMapSystem.NavMapBeacon navMapBeacon2;
    foreach ((key3, navMapBeacon2) in beacons)
    {
      NetEntity key4 = key3;
      SharedNavMapSystem.NavMapBeacon navMapBeacon3 = navMapBeacon2;
      component.Beacons[key4] = navMapBeacon3;
    }
  }

  public virtual void Update(float frameTime)
  {
    AllEntityQueryEnumerator<NavMapComponent> entityQueryEnumerator = this.AllEntityQuery<NavMapComponent>();
    EntityUid uid;
    NavMapComponent component;
    while (entityQueryEnumerator.MoveNext(ref uid, ref component))
      this.FloodFillNextEnqueuedRegion(uid, component);
  }

  private void FloodFillNextEnqueuedRegion(EntityUid uid, NavMapComponent component)
  {
    if (!component.QueuedRegionsToFlood.Any<NetEntity>())
      return;
    NetEntity key1 = component.QueuedRegionsToFlood.Dequeue();
    SharedNavMapSystem.NavMapRegionProperties regionProperties;
    if (!component.RegionProperties.TryGetValue(key1, out regionProperties) || !regionProperties.Seeds.Any<Vector2i>())
    {
      this.FloodFillNextEnqueuedRegion(uid, component);
    }
    else
    {
      (HashSet<Vector2i> tiles, HashSet<Vector2i> vector2iSet1) = this.FloodFillRegion(uid, component, regionProperties);
      List<(Vector2i, Vector2i)> mergedRegionTiles = this.GetMergedRegionTiles(tiles);
      NavMapRegionOverlay mapRegionOverlay = new NavMapRegionOverlay(regionProperties.UiKey, mergedRegionTiles)
      {
        Color = regionProperties.Color
      };
      component.RegionOverlays[key1] = mapRegionOverlay;
      HashSet<Vector2i> vector2iSet2;
      if (component.RegionOwnerToChunkTable.TryGetValue(key1, out vector2iSet2))
      {
        foreach (Vector2i key2 in vector2iSet2)
        {
          HashSet<NetEntity> netEntitySet;
          if (component.ChunkToRegionOwnerTable.TryGetValue(key2, out netEntitySet))
          {
            netEntitySet.Remove(key1);
            component.ChunkToRegionOwnerTable[key2] = netEntitySet;
          }
        }
      }
      component.RegionOwnerToChunkTable[key1] = vector2iSet1;
      foreach (Vector2i key3 in vector2iSet1)
      {
        HashSet<NetEntity> netEntitySet;
        if (!component.ChunkToRegionOwnerTable.TryGetValue(key3, out netEntitySet))
          netEntitySet = new HashSet<NetEntity>();
        netEntitySet.Add(key1);
        component.ChunkToRegionOwnerTable[key3] = netEntitySet;
      }
    }
  }

  private (HashSet<Vector2i>, HashSet<Vector2i>) FloodFillRegion(
    EntityUid uid,
    NavMapComponent component,
    SharedNavMapSystem.NavMapRegionProperties regionProperties)
  {
    if (!regionProperties.Seeds.Any<Vector2i>())
      return (new HashSet<Vector2i>(), new HashSet<Vector2i>());
    HashSet<Vector2i> vector2iSet1 = new HashSet<Vector2i>();
    HashSet<Vector2i> vector2iSet2 = new HashSet<Vector2i>();
    Stack<Vector2i> vector2iStack = new Stack<Vector2i>();
    foreach (Vector2i seed in regionProperties.Seeds)
    {
      vector2iStack.Push(seed);
      while (vector2iStack.Count > 0)
      {
        if (vector2iSet2.Count > regionProperties.MaxArea)
          return (new HashSet<Vector2i>(), new HashSet<Vector2i>());
        Vector2i tile1 = vector2iStack.Pop();
        Vector2i vector2i1 = Vector2i.op_Subtraction(seed, tile1);
        if ((double) ((Vector2i) ref vector2i1).Length <= (double) regionProperties.MaxRadius && !vector2iSet2.Contains(tile1))
        {
          Vector2i chunkIndices1 = SharedMapSystem.GetChunkIndices(tile1, 8);
          int tileIndex = SharedNavMapSystem.GetTileIndex(SharedMapSystem.GetChunkRelative(tile1, 8));
          NavMapChunk chunk1;
          if (component.Chunks.TryGetValue(chunkIndices1, out chunk1))
          {
            int num = chunk1.TileData[tileIndex];
            if ((15 & num) != 0 && (240 /*0xF0*/ & num) != 240 /*0xF0*/ && (3840 /*0x0F00*/ & num) != 3840 /*0x0F00*/)
            {
              vector2iSet2.Add(tile1);
              vector2iSet1.Add(chunkIndices1);
              foreach ((AtmosDirection direction1, Vector2i vector2i2, AtmosDirection direction2) in this._regionPropagationTable)
              {
                if (this.RegionCanPropagateInDirection(chunk1, tile1, direction1))
                {
                  Vector2i tile2 = Vector2i.op_Addition(tile1, vector2i2);
                  Vector2i chunkIndices2 = SharedMapSystem.GetChunkIndices(tile2, 8);
                  NavMapChunk chunk2;
                  if (component.Chunks.TryGetValue(chunkIndices2, out chunk2))
                  {
                    vector2iSet1.Add(chunkIndices2);
                    if (this.RegionCanPropagateInDirection(chunk2, tile2, direction2))
                      vector2iStack.Push(tile2);
                  }
                }
              }
            }
          }
        }
      }
    }
    return (vector2iSet2, vector2iSet1);
  }

  private bool RegionCanPropagateInDirection(
    NavMapChunk chunk,
    Vector2i tile,
    AtmosDirection direction)
  {
    int tileIndex = SharedNavMapSystem.GetTileIndex(SharedMapSystem.GetChunkRelative(tile, 8));
    int num1 = chunk.TileData[tileIndex];
    if ((15 & num1) == 0)
      return false;
    int num2 = (int) direction << 4;
    int num3 = (int) direction << 8;
    int num4 = num1;
    return (num2 & num4) <= 0 && (num3 & num1) <= 0;
  }

  private List<(Vector2i, Vector2i)> GetMergedRegionTiles(HashSet<Vector2i> tiles)
  {
    if (!tiles.Any<Vector2i>())
      return new List<(Vector2i, Vector2i)>();
    IEnumerable<int> source1 = tiles.Select<Vector2i, int>((Func<Vector2i, int>) (t => t.X));
    int num1 = source1.Min();
    int num2 = source1.Max();
    IEnumerable<int> source2 = tiles.Select<Vector2i, int>((Func<Vector2i, int>) (t => t.Y));
    int num3 = source2.Min();
    int num4 = source2.Max();
    int num5 = num1;
    int[,] matrix = new int[num2 - num5 + 1, num4 - num3 + 1];
    foreach (Vector2i tile in tiles)
    {
      int index1 = tile.X - num1;
      int index2 = tile.Y - num3;
      matrix[index1, index2] = 1;
    }
    return this.GetMergedRegionTiles(matrix, new Vector2i(num1, num3));
  }

  private List<(Vector2i, Vector2i)> GetMergedRegionTiles(int[,] matrix, Vector2i offset)
  {
    List<(Vector2i, Vector2i)> mergedRegionTiles = new List<(Vector2i, Vector2i)>();
    int length1 = matrix.GetLength(0);
    int length2 = matrix.GetLength(1);
    int[,] numArray1 = new int[length1, length2];
    (Vector2i, Vector2i) valueTuple = (new Vector2i(), new Vector2i());
    int num1 = 0;
    while (!this.IsArrayEmpty(matrix))
    {
      ++num1;
      if (num1 <= length1 * length2)
      {
        int[,] numArray2 = new int[length1, length2];
        valueTuple = (new Vector2i(), new Vector2i());
        int val1_1 = 0;
        for (int index = 0; index < length2; ++index)
          numArray2[0, index] = matrix[0, index];
        for (int index1 = 1; index1 < length1; ++index1)
        {
          for (int index2 = 0; index2 < length2; ++index2)
            numArray2[index1, index2] = matrix[index1, index2] == 1 ? numArray2[index1 - 1, index2] + 1 : 0;
        }
        for (int index3 = 0; index3 < length1; ++index3)
        {
          for (int index4 = 0; index4 < length2; ++index4)
          {
            int val1_2 = numArray2[index3, index4];
            for (int index5 = index4; index5 >= 0 && numArray2[index3, index5] > 0; --index5)
            {
              val1_2 = Math.Min(val1_2, numArray2[index3, index5]);
              int num2 = Math.Max(val1_1, val1_2 * (index4 - index5 + 1));
              if (num2 > val1_1)
              {
                val1_1 = num2;
                valueTuple = (new Vector2i(index3 - val1_2 + 1, index5), new Vector2i(index3, index4));
              }
            }
          }
        }
        mergedRegionTiles.Add((Vector2i.op_Addition(valueTuple.Item1, offset), Vector2i.op_Addition(valueTuple.Item2, offset)));
        for (int x = valueTuple.Item1.X; x <= valueTuple.Item2.X; ++x)
        {
          for (int y = valueTuple.Item1.Y; y <= valueTuple.Item2.Y; ++y)
            matrix[x, y] = 0;
        }
      }
      else
        break;
    }
    return mergedRegionTiles;
  }

  private bool IsArrayEmpty(int[,] matrix)
  {
    for (int index1 = 0; index1 < matrix.GetLength(0); ++index1)
    {
      for (int index2 = 0; index2 < matrix.GetLength(1); ++index2)
      {
        if (matrix[index1, index2] == 1)
          return false;
      }
    }
    return true;
  }
}

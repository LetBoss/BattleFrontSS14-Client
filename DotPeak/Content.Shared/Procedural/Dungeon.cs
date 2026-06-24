// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.Dungeon
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Procedural;

public sealed class Dungeon
{
  public static Dungeon Empty = new Dungeon();
  private List<DungeonRoom> _rooms;
  private HashSet<Vector2i> _allTiles = new HashSet<Vector2i>();
  public readonly HashSet<Vector2i> RoomTiles = new HashSet<Vector2i>();
  public readonly HashSet<Vector2i> RoomExteriorTiles = new HashSet<Vector2i>();
  public readonly HashSet<Vector2i> CorridorTiles = new HashSet<Vector2i>();
  public readonly HashSet<Vector2i> CorridorExteriorTiles = new HashSet<Vector2i>();
  public readonly HashSet<Vector2i> Entrances = new HashSet<Vector2i>();

  public IReadOnlyList<DungeonRoom> Rooms => (IReadOnlyList<DungeonRoom>) this._rooms;

  public IReadOnlySet<Vector2i> AllTiles => (IReadOnlySet<Vector2i>) this._allTiles;

  public Dungeon()
    : this(new List<DungeonRoom>())
  {
  }

  public Dungeon(List<DungeonRoom> rooms)
  {
    this._rooms = rooms;
    foreach (DungeonRoom room in this._rooms)
      this.InternalAddRoom(room);
    this.RefreshAllTiles();
  }

  public void RefreshAllTiles()
  {
    this._allTiles.Clear();
    this._allTiles.UnionWith((IEnumerable<Vector2i>) this.RoomTiles);
    this._allTiles.UnionWith((IEnumerable<Vector2i>) this.RoomExteriorTiles);
    this._allTiles.UnionWith((IEnumerable<Vector2i>) this.CorridorTiles);
    this._allTiles.UnionWith((IEnumerable<Vector2i>) this.CorridorExteriorTiles);
    this._allTiles.UnionWith((IEnumerable<Vector2i>) this.Entrances);
  }

  public void Rebuild()
  {
    this._allTiles.Clear();
    this.RoomTiles.Clear();
    this.RoomExteriorTiles.Clear();
    this.Entrances.Clear();
    foreach (DungeonRoom room in this._rooms)
      this.InternalAddRoom(room, false);
    this.RefreshAllTiles();
  }

  public void AddRoom(DungeonRoom room)
  {
    this._rooms.Add(room);
    this.InternalAddRoom(room);
  }

  private void InternalAddRoom(DungeonRoom room, bool refreshAll = true)
  {
    this.Entrances.UnionWith((IEnumerable<Vector2i>) room.Entrances);
    this.RoomTiles.UnionWith((IEnumerable<Vector2i>) room.Tiles);
    this.RoomExteriorTiles.UnionWith((IEnumerable<Vector2i>) room.Exterior);
    if (!refreshAll)
      return;
    this.RefreshAllTiles();
  }
}

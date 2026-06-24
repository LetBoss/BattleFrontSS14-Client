using System.Collections.Generic;
using Robust.Shared.Maths;

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

	public IReadOnlyList<DungeonRoom> Rooms => _rooms;

	public IReadOnlySet<Vector2i> AllTiles => _allTiles;

	public Dungeon()
		: this(new List<DungeonRoom>())
	{
	}

	public Dungeon(List<DungeonRoom> rooms)
	{
		_rooms = rooms;
		foreach (DungeonRoom room in _rooms)
		{
			InternalAddRoom(room);
		}
		RefreshAllTiles();
	}

	public void RefreshAllTiles()
	{
		_allTiles.Clear();
		_allTiles.UnionWith(RoomTiles);
		_allTiles.UnionWith(RoomExteriorTiles);
		_allTiles.UnionWith(CorridorTiles);
		_allTiles.UnionWith(CorridorExteriorTiles);
		_allTiles.UnionWith(Entrances);
	}

	public void Rebuild()
	{
		_allTiles.Clear();
		RoomTiles.Clear();
		RoomExteriorTiles.Clear();
		Entrances.Clear();
		foreach (DungeonRoom room in _rooms)
		{
			InternalAddRoom(room, refreshAll: false);
		}
		RefreshAllTiles();
	}

	public void AddRoom(DungeonRoom room)
	{
		_rooms.Add(room);
		InternalAddRoom(room);
	}

	private void InternalAddRoom(DungeonRoom room, bool refreshAll = true)
	{
		Entrances.UnionWith(room.Entrances);
		RoomTiles.UnionWith(room.Tiles);
		RoomExteriorTiles.UnionWith(room.Exterior);
		if (refreshAll)
		{
			RefreshAllTiles();
		}
	}
}

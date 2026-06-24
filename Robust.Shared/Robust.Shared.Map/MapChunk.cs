using System;
using System.Collections.Generic;
using System.Diagnostics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Map;

internal sealed class MapChunk
{
	private struct SnapGridCell
	{
		public List<EntityUid>? Center;
	}

	private const int SnapCellStartingCapacity = 1;

	private readonly Vector2i _gridIndices;

	[ViewVariables]
	internal readonly Tile[,] Tiles;

	private readonly SnapGridCell[,] _snapGrid;

	[ViewVariables]
	internal HashSet<string> Fixtures = new HashSet<string>();

	[ViewVariables]
	internal int FilledTiles { get; set; }

	[ViewVariables]
	public Box2i CachedBounds { get; set; }

	[ViewVariables]
	public GameTick LastTileModifiedTick { get; set; }

	public bool SuppressCollisionRegeneration { get; set; }

	public ushort ChunkSize { get; }

	public int X => _gridIndices.X;

	public int Y => _gridIndices.Y;

	public Vector2i Indices => _gridIndices;

	public MapChunk(int x, int y, ushort chunkSize)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		_gridIndices = new Vector2i(x, y);
		ChunkSize = chunkSize;
		Tiles = new Tile[ChunkSize, ChunkSize];
		_snapGrid = new SnapGridCell[ChunkSize, ChunkSize];
	}

	public Tile GetTile(ushort xIndex, ushort yIndex)
	{
		if (xIndex >= ChunkSize)
		{
			throw new ArgumentOutOfRangeException("xIndex", "Tile indices out of bounds.");
		}
		if (yIndex >= ChunkSize)
		{
			throw new ArgumentOutOfRangeException("yIndex", "Tile indices out of bounds.");
		}
		return Tiles[xIndex, yIndex];
	}

	public Tile GetTile(Vector2i indices)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Tiles[indices.X, indices.Y];
	}

	public Vector2i GridTileToChunkTile(Vector2i gridTile)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		int num = MathHelper.Mod(gridTile.X, (int)ChunkSize);
		int num2 = MathHelper.Mod(gridTile.Y, (int)ChunkSize);
		return new Vector2i(num, num2);
	}

	public Vector2i ChunkTileToGridTile(Vector2i chunkTile)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return chunkTile + _gridIndices * (int)ChunkSize;
	}

	public IEnumerable<EntityUid> GetSnapGridCell(ushort xCell, ushort yCell)
	{
		if (xCell >= ChunkSize)
		{
			throw new ArgumentOutOfRangeException("xCell", "Tile indices out of bounds.");
		}
		if (yCell >= ChunkSize)
		{
			throw new ArgumentOutOfRangeException("yCell", "Tile indices out of bounds.");
		}
		List<EntityUid> center = _snapGrid[xCell, yCell].Center;
		if (center == null)
		{
			return Array.Empty<EntityUid>();
		}
		return center;
	}

	internal List<EntityUid>? GetSnapGrid(ushort xCell, ushort yCell)
	{
		if (xCell >= ChunkSize)
		{
			throw new ArgumentOutOfRangeException("xCell", "Tile indices out of bounds.");
		}
		if (yCell >= ChunkSize)
		{
			throw new ArgumentOutOfRangeException("yCell", "Tile indices out of bounds.");
		}
		return _snapGrid[xCell, yCell].Center;
	}

	public void AddToSnapGridCell(ushort xCell, ushort yCell, EntityUid euid)
	{
		if (xCell >= ChunkSize)
		{
			throw new ArgumentOutOfRangeException("xCell", "Tile indices out of bounds.");
		}
		if (yCell >= ChunkSize)
		{
			throw new ArgumentOutOfRangeException("yCell", "Tile indices out of bounds.");
		}
		ref SnapGridCell reference = ref _snapGrid[xCell, yCell];
		ref List<EntityUid> center = ref reference.Center;
		if (center == null)
		{
			center = new List<EntityUid>(1);
		}
		reference.Center.Add(euid);
	}

	public void RemoveFromSnapGridCell(ushort xCell, ushort yCell, EntityUid euid)
	{
		if (xCell >= ChunkSize)
		{
			throw new ArgumentOutOfRangeException("xCell", "Tile indices out of bounds.");
		}
		if (yCell >= ChunkSize)
		{
			throw new ArgumentOutOfRangeException("yCell", "Tile indices out of bounds.");
		}
		_snapGrid[xCell, yCell].Center?.Remove(euid);
	}

	public override string ToString()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return $"Chunk {_gridIndices}";
	}

	internal bool TrySetTile(ushort xIndex, ushort yIndex, Tile tile, out Tile oldTile, out bool shapeChanged)
	{
		if (xIndex >= Tiles.Length)
		{
			throw new ArgumentOutOfRangeException("xIndex", "Tile indices out of bounds.");
		}
		if (yIndex >= Tiles.Length)
		{
			throw new ArgumentOutOfRangeException("yIndex", "Tile indices out of bounds.");
		}
		shapeChanged = false;
		ref Tile reference = ref Tiles[xIndex, yIndex];
		if (reference == tile)
		{
			oldTile = default(Tile);
			return false;
		}
		if (reference.IsEmpty)
		{
			if (!tile.IsEmpty)
			{
				FilledTiles++;
				shapeChanged = true;
			}
		}
		else if (tile.IsEmpty)
		{
			FilledTiles--;
			shapeChanged = true;
		}
		oldTile = reference;
		reference = tile;
		return true;
	}

	[Conditional("DEBUG")]
	public void ValidateChunk()
	{
		int num = 0;
		Tile[,] tiles = Tiles;
		int upperBound = tiles.GetUpperBound(0);
		int upperBound2 = tiles.GetUpperBound(1);
		for (int i = tiles.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = tiles.GetLowerBound(1); j <= upperBound2; j++)
			{
				Tile tile = tiles[i, j];
				if (!tile.IsEmpty)
				{
					num++;
				}
			}
		}
	}
}

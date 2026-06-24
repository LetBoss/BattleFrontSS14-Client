using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Map.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class MapGridComponent : Component, ISerializationGenerated<MapGridComponent>, ISerializationGenerated
{
	[Dependency]
	private readonly IEntityManager _entManager;

	[DataField("index", false, 1, false, false, null)]
	internal int GridIndex;

	[DataField(null, false, 1, false, false, null)]
	internal ushort ChunkSize = 16;

	[ViewVariables]
	internal readonly List<(GameTick tick, Vector2i indices)> ChunkDeletionHistory = new List<(GameTick, Vector2i)>();

	internal DynamicTree.Proxy MapProxy = DynamicTree.Proxy.Free;

	[DataField("chunks", false, 1, false, false, null)]
	internal Dictionary<Vector2i, MapChunk> Chunks = new Dictionary<Vector2i, MapChunk>();

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("canSplit", false, 1, false, false, null)]
	public bool CanSplit = true;

	private SharedMapSystem MapSystem => _entManager.System<SharedMapSystem>();

	[ViewVariables]
	public int ChunkCount => Chunks.Count;

	[DataField("tileSize", false, 1, false, false, null)]
	public ushort TileSize { get; internal set; } = 1;

	public Vector2 TileSizeVector => new Vector2((int)TileSize, (int)TileSize);

	public Vector2 TileSizeHalfVector => new Vector2((float)(int)TileSize / 2f, (float)(int)TileSize / 2f);

	[ViewVariables]
	public GameTick LastTileModifiedTick { get; internal set; }

	[ViewVariables]
	public Box2 LocalAABB { get; internal set; }

	[Obsolete("Use the MapSystem method")]
	public TileRef GetTileRef(EntityCoordinates coords)
	{
		return MapSystem.GetTileRef(base.Owner, this, coords);
	}

	[Obsolete("Use the MapSystem method")]
	public TileRef GetTileRef(Vector2i tileCoordinates)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.GetTileRef(base.Owner, this, tileCoordinates);
	}

	[Obsolete("Use the MapSystem method")]
	public IEnumerable<TileRef> GetAllTiles(bool ignoreEmpty = true)
	{
		return MapSystem.GetAllTiles(base.Owner, this, ignoreEmpty);
	}

	[Obsolete("Use the MapSystem method")]
	public GridTileEnumerator GetAllTilesEnumerator(bool ignoreEmpty = true)
	{
		return MapSystem.GetAllTilesEnumerator(base.Owner, this, ignoreEmpty);
	}

	[Obsolete("Use the MapSystem method")]
	public void SetTile(EntityCoordinates coords, Tile tile)
	{
		MapSystem.SetTile(base.Owner, this, coords, tile);
	}

	[Obsolete("Use the MapSystem method")]
	public void SetTile(Vector2i gridIndices, Tile tile)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		MapSystem.SetTile(base.Owner, this, gridIndices, tile);
	}

	[Obsolete("Use the MapSystem method")]
	public void SetTiles(List<(Vector2i GridIndices, Tile Tile)> tiles)
	{
		MapSystem.SetTiles(base.Owner, this, tiles);
	}

	[Obsolete("Use the MapSystem method")]
	public IEnumerable<TileRef> GetTilesIntersecting(Box2 worldArea, bool ignoreEmpty = true, Predicate<TileRef>? predicate = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.GetTilesIntersecting(base.Owner, this, worldArea, ignoreEmpty, predicate);
	}

	[Obsolete("Use the MapSystem method")]
	public IEnumerable<TileRef> GetLocalTilesIntersecting(Box2 localArea, bool ignoreEmpty = true, Predicate<TileRef>? predicate = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.GetLocalTilesIntersecting(base.Owner, this, localArea, ignoreEmpty, predicate);
	}

	[Obsolete("Use the MapSystem method")]
	public IEnumerable<TileRef> GetTilesIntersecting(Circle worldArea, bool ignoreEmpty = true, Predicate<TileRef>? predicate = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.GetTilesIntersecting(base.Owner, this, worldArea, ignoreEmpty, predicate);
	}

	[Obsolete("Use the MapSystem method")]
	internal bool TryGetChunk(Vector2i chunkIndices, [NotNullWhen(true)] out MapChunk? chunk)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.TryGetChunk(base.Owner, this, chunkIndices, out chunk);
	}

	[Obsolete("Use the MapSystem method")]
	internal IReadOnlyDictionary<Vector2i, MapChunk> GetMapChunks()
	{
		return MapSystem.GetMapChunks(base.Owner, this);
	}

	[Obsolete("Use the MapSystem method")]
	internal ChunkEnumerator GetMapChunks(Box2Rotated worldArea)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.GetMapChunks(base.Owner, this, worldArea);
	}

	[Obsolete("Use the MapSystem method")]
	public IEnumerable<EntityUid> GetAnchoredEntities(MapCoordinates coords)
	{
		return MapSystem.GetAnchoredEntities(base.Owner, this, coords);
	}

	[Obsolete("Use the MapSystem method")]
	public IEnumerable<EntityUid> GetAnchoredEntities(Vector2i pos)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.GetAnchoredEntities(base.Owner, this, pos);
	}

	[Obsolete("Use the MapSystem method")]
	public AnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(Vector2i pos)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.GetAnchoredEntitiesEnumerator(base.Owner, this, pos);
	}

	[Obsolete("Use the MapSystem method")]
	public Vector2i TileIndicesFor(EntityCoordinates coords)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.TileIndicesFor(base.Owner, this, coords);
	}

	[Obsolete("Use the MapSystem method")]
	public Vector2i TileIndicesFor(MapCoordinates worldPos)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.TileIndicesFor(base.Owner, this, worldPos);
	}

	[Obsolete("Use the MapSystem method")]
	public IEnumerable<EntityUid> GetCellsInSquareArea(EntityCoordinates coords, int n)
	{
		return MapSystem.GetCellsInSquareArea(base.Owner, this, coords, n);
	}

	[Obsolete("Use the MapSystem method")]
	public Vector2 WorldToLocal(Vector2 posWorld)
	{
		return MapSystem.WorldToLocal(base.Owner, this, posWorld);
	}

	[Obsolete("Use the MapSystem method")]
	public EntityCoordinates MapToGrid(MapCoordinates posWorld)
	{
		return MapSystem.MapToGrid(base.Owner, posWorld);
	}

	[Obsolete("Use the MapSystem method")]
	public Vector2 LocalToWorld(Vector2 posLocal)
	{
		return MapSystem.LocalToWorld(base.Owner, this, posLocal);
	}

	[Obsolete("Use the MapSystem method")]
	public Vector2i WorldToTile(Vector2 posWorld)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.WorldToTile(base.Owner, this, posWorld);
	}

	[Obsolete("Use the MapSystem method")]
	public Vector2i LocalToTile(EntityCoordinates coordinates)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.LocalToTile(base.Owner, this, coordinates);
	}

	[Obsolete("Use the MapSystem method")]
	public Vector2i CoordinatesToTile(EntityCoordinates coords)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.CoordinatesToTile(base.Owner, this, coords);
	}

	[Obsolete("Use the MapSystem method")]
	public EntityCoordinates GridTileToLocal(Vector2i gridTile)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.GridTileToLocal(base.Owner, this, gridTile);
	}

	[Obsolete("Use the MapSystem method")]
	public Vector2 GridTileToWorldPos(Vector2i gridTile)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.GridTileToWorldPos(base.Owner, this, gridTile);
	}

	[Obsolete("Use the MapSystem method")]
	public bool TryGetTileRef(Vector2i indices, out TileRef tile)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return MapSystem.TryGetTileRef(base.Owner, this, indices, out tile);
	}

	[Obsolete("Use the MapSystem method")]
	public bool TryGetTileRef(EntityCoordinates coords, out TileRef tile)
	{
		return MapSystem.TryGetTileRef(base.Owner, this, coords, out tile);
	}

	public bool HasChunk(Vector2i indices)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return Chunks.ContainsKey(indices);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MapGridComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (MapGridComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			int target3 = 0;
			if (!serialization.TryCustomCopy(GridIndex, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = GridIndex;
			}
			target.GridIndex = target3;
			ushort target4 = 0;
			if (!serialization.TryCustomCopy(ChunkSize, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = ChunkSize;
			}
			target.ChunkSize = target4;
			ushort target5 = 0;
			if (!serialization.TryCustomCopy(TileSize, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = TileSize;
			}
			target.TileSize = target5;
			Dictionary<Vector2i, MapChunk> target6 = null;
			if (Chunks == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(Chunks, ref target6, hookCtx, hasHooks: true, context))
			{
				target6 = serialization.CreateCopy(Chunks, hookCtx, context);
			}
			target.Chunks = target6;
			bool target7 = false;
			if (!serialization.TryCustomCopy(CanSplit, ref target7, hookCtx, hasHooks: false, context))
			{
				target7 = CanSplit;
			}
			target.CanSplit = target7;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MapGridComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MapGridComponent target2 = (MapGridComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MapGridComponent target2 = (MapGridComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MapGridComponent target2 = (MapGridComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MapGridComponent Instantiate()
	{
		return new MapGridComponent();
	}
}

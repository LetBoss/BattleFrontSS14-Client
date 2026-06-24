using System;
using System.Linq;
using System.Numerics;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Decals;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Shared.Maps;

public sealed class TileSystem : EntitySystem
{
	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IRobustRandom _robustRandom;

	[Dependency]
	private ITileDefinitionManager _tileDefinitionManager;

	[Dependency]
	private SharedDecalSystem _decal;

	[Dependency]
	private SharedMapSystem _maps;

	[Dependency]
	private TurfSystem _turf;

	public byte PickVariant(ContentTileDefinition tile)
	{
		return PickVariant(tile, _robustRandom.Next());
	}

	public byte PickVariant(ContentTileDefinition tile, int seed)
	{
		System.Random rand = new System.Random(seed);
		return PickVariant(tile, rand);
	}

	public byte PickVariant(ContentTileDefinition tile, System.Random random)
	{
		float[] variants = tile.PlacementVariants;
		float sum = variants.Sum();
		float accumulated = 0f;
		float rand = random.NextSingle() * sum;
		for (byte i = 0; i < variants.Length; i++)
		{
			accumulated += variants[i];
			if (accumulated >= rand)
			{
				return i;
			}
		}
		throw new InvalidOperationException("Invalid weighted variantize tile pick for " + tile.ID + "!");
	}

	public Tile GetVariantTile(ContentTileDefinition tile, System.Random random)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return new Tile((int)tile.TileId, (byte)0, PickVariant(tile, random), (byte)0);
	}

	public Tile GetVariantTile(ContentTileDefinition tile, int seed)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		System.Random rand = new System.Random(seed);
		return new Tile((int)tile.TileId, (byte)0, PickVariant(tile, rand), (byte)0);
	}

	public bool PryTile(Vector2i indices, EntityUid gridId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		MapGridComponent grid = ((EntitySystem)this).Comp<MapGridComponent>(gridId);
		TileRef tileRef = _maps.GetTileRef(gridId, grid, indices);
		return PryTile(tileRef);
	}

	public bool PryTile(TileRef tileRef)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return PryTile(tileRef, pryPlating: false);
	}

	public bool PryTile(TileRef tileRef, bool pryPlating)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Tile tile = tileRef.Tile;
		if (((Tile)(ref tile)).IsEmpty)
		{
			return false;
		}
		if (!((ContentTileDefinition)(object)_tileDefinitionManager[tile.TypeId]).CanCrowbar)
		{
			return false;
		}
		return DeconstructTile(tileRef);
	}

	public bool ReplaceTile(TileRef tileref, ContentTileDefinition replacementTile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		MapGridComponent grid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(tileref.GridUid, ref grid))
		{
			return false;
		}
		return ReplaceTile(tileref, replacementTile, tileref.GridUid, grid);
	}

	public bool ReplaceTile(TileRef tileref, ContentTileDefinition replacementTile, EntityUid grid, MapGridComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MapGridComponent>(grid, ref component, true))
		{
			return false;
		}
		byte variant = PickVariant(replacementTile);
		foreach (var item in _decal.GetDecalsInRange(tileref.GridUid, _turf.GetTileCenter(tileref).Position, 0.5f))
		{
			uint id = item.Index;
			_decal.RemoveDecal(tileref.GridUid, id);
		}
		_maps.SetTile(grid, component, tileref.GridIndices, new Tile((int)replacementTile.TileId, (byte)0, variant, (byte)0));
		return true;
	}

	public bool DeconstructTile(TileRef tileRef)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		if (((Tile)(ref tileRef.Tile)).IsEmpty)
		{
			return false;
		}
		ContentTileDefinition tileDef = (ContentTileDefinition)(object)_tileDefinitionManager[tileRef.Tile.TypeId];
		if (string.IsNullOrEmpty(tileDef.BaseTurf))
		{
			return false;
		}
		EntityUid gridUid = tileRef.GridUid;
		MapGridComponent mapGrid = ((EntitySystem)this).Comp<MapGridComponent>(gridUid);
		float bounds = (float)(int)mapGrid.TileSize - 0.2f;
		Vector2i indices = tileRef.GridIndices;
		EntityCoordinates val = _maps.GridTileToLocal(gridUid, mapGrid, indices);
		EntityCoordinates coordinates = ((EntityCoordinates)(ref val)).Offset(new Vector2((_robustRandom.NextFloat() - 0.5f) * bounds, (_robustRandom.NextFloat() - 0.5f) * bounds));
		EntityUid tileItem = ((EntitySystem)this).Spawn(tileDef.ItemDropPrototypeName, coordinates);
		((EntitySystem)this).Transform(tileItem).LocalRotation = Angle.op_Implicit(_robustRandom.NextDouble() * (Math.PI * 2.0));
		foreach (var item in _decal.GetDecalsInRange(gridUid, coordinates.SnapToGrid((IEntityManager?)(object)base.EntityManager, _mapManager).Position, 0.5f))
		{
			uint id = item.Index;
			_decal.RemoveDecal(tileRef.GridUid, id);
		}
		ITileDefinition plating = _tileDefinitionManager[tileDef.BaseTurf];
		_maps.SetTile(gridUid, mapGrid, tileRef.GridIndices, new Tile((int)plating.TileId, (byte)0, (byte)0, (byte)0));
		return true;
	}
}

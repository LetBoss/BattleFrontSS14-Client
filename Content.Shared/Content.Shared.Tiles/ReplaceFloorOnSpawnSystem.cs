using System;
using System.Collections.Generic;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Tiles;

public sealed class ReplaceFloorOnSpawnSystem : EntitySystem
{
	[Dependency]
	private ITileDefinitionManager _tile;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedMapSystem _map;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<ReplaceFloorOnSpawnComponent, MapInitEvent>((EntityEventRefHandler<ReplaceFloorOnSpawnComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<ReplaceFloorOnSpawnComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = ((EntitySystem)this).Transform(Entity<ReplaceFloorOnSpawnComponent>.op_Implicit(ent));
		EntityUid? gridUid = xform.GridUid;
		if (!gridUid.HasValue)
		{
			return;
		}
		EntityUid grid = gridUid.GetValueOrDefault();
		MapGridComponent gridComp = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(grid, ref gridComp) || (ent.Comp.ReplaceableTiles != null && ent.Comp.ReplaceableTiles.Count == 0))
		{
			return;
		}
		Vector2i tileIndices = _map.LocalToTile(grid, gridComp, xform.Coordinates);
		TileRef tile = default(TileRef);
		foreach (Vector2i offset in ent.Comp.Offsets)
		{
			Vector2i actualIndices = tileIndices + offset;
			if (_map.TryGetTileRef(grid, gridComp, actualIndices, ref tile) && (ent.Comp.ReplaceableTiles == null || ((Tile)(ref tile.Tile)).IsEmpty || ent.Comp.ReplaceableTiles.Contains(ProtoId<ContentTileDefinition>.op_Implicit(((IPrototype)_tile[tile.Tile.TypeId]).ID))))
			{
				ProtoId<ContentTileDefinition> tileToSet = RandomExtensions.Pick<ProtoId<ContentTileDefinition>>(_random, (IReadOnlyList<ProtoId<ContentTileDefinition>>)ent.Comp.ReplacementTiles);
				_map.SetTile(grid, gridComp, tile.GridIndices, new Tile((int)_prototype.Index<ContentTileDefinition>(tileToSet).TileId, (byte)0, (byte)0, (byte)0));
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;

namespace Content.Shared.Maps;

public sealed class TurfSystem : EntitySystem
{
	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private ITileDefinitionManager _tileDefinitions;

	public TileRef? GetTileRef(EntityCoordinates coordinates)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityCoordinates)(ref coordinates)).IsValid((IEntityManager)(object)base.EntityManager))
		{
			return null;
		}
		MapCoordinates pos = _transform.ToMapCoordinates(coordinates, true);
		EntityUid gridUid = default(EntityUid);
		MapGridComponent gridComp = default(MapGridComponent);
		if (!_mapManager.TryFindGridAt(pos, ref gridUid, ref gridComp))
		{
			return null;
		}
		TileRef tile = default(TileRef);
		if (!_mapSystem.TryGetTileRef(gridUid, gridComp, coordinates, ref tile))
		{
			return null;
		}
		return tile;
	}

	public bool TryGetTileRef(EntityCoordinates coordinates, [NotNullWhen(true)] out TileRef? tile)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TileRef? val = (tile = GetTileRef(coordinates));
		return val.HasValue;
	}

	public bool IsTileBlocked(TileRef turf, CollisionGroup mask, float minIntersectionArea = 0.1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return IsTileBlocked(turf.GridUid, turf.GridIndices, mask, null, null, minIntersectionArea);
	}

	public bool IsTileBlocked(EntityUid gridUid, Vector2i indices, CollisionGroup mask, MapGridComponent? grid = null, TransformComponent? gridXform = null, float minIntersectionArea = 0.1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MapGridComponent, TransformComponent>(gridUid, ref grid, ref gridXform, true))
		{
			return false;
		}
		EntityQuery<TransformComponent> xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		ValueTuple<Vector2, Angle, Matrix3x2> worldPositionRotationMatrix = _transform.GetWorldPositionRotationMatrix(gridXform, xformQuery);
		Vector2 gridPos = worldPositionRotationMatrix.Item1;
		Angle gridRot = worldPositionRotationMatrix.Item2;
		Matrix3x2 matrix = worldPositionRotationMatrix.Item3;
		ushort size = grid.TileSize;
		Vector2 localPos = new Vector2((float)(indices.X * size) + (float)(int)size / 2f, (float)(indices.Y * size) + (float)(int)size / 2f);
		Vector2 worldPos = Vector2.Transform(localPos, matrix);
		Box2 tileAabb = ((Box2)(ref Box2.UnitCentered)).Scale(0.95f * (float)(int)size);
		Box2Rotated worldBox = default(Box2Rotated);
		((Box2Rotated)(ref worldBox))._002Ector(((Box2)(ref tileAabb)).Translated(worldPos), gridRot, worldPos);
		tileAabb = ((Box2)(ref tileAabb)).Translated(localPos);
		float intersectionArea = 0f;
		EntityQuery<FixturesComponent> fixtureQuery = ((EntitySystem)this).GetEntityQuery<FixturesComponent>();
		FixturesComponent fixtures = default(FixturesComponent);
		Transform xform = default(Transform);
		foreach (EntityUid ent in _entityLookup.GetEntitiesIntersecting(gridUid, worldBox, (LookupFlags)6))
		{
			if (!fixtureQuery.TryGetComponent(ent, ref fixtures))
			{
				continue;
			}
			ValueTuple<Vector2, Angle> worldPositionRotation = _transform.GetWorldPositionRotation(xformQuery.GetComponent(ent), xformQuery);
			Vector2 pos = worldPositionRotation.Item1;
			Angle rot = worldPositionRotation.Item2;
			rot -= gridRot;
			Angle val = -gridRot;
			Vector2 vector = pos - gridPos;
			pos = ((Angle)(ref val)).RotateVec(ref vector);
			((Transform)(ref xform))._002Ector(pos, (float)rot.Theta);
			foreach (Fixture fixture in fixtures.Fixtures.Values)
			{
				if (!fixture.Hard || ((uint)fixture.CollisionLayer & (uint)mask) == 0)
				{
					continue;
				}
				for (int i = 0; i < fixture.Shape.ChildCount; i++)
				{
					Box2 val2 = fixture.Shape.ComputeAABB(xform, i);
					Box2 intersection = ((Box2)(ref val2)).Intersect(ref tileAabb);
					intersectionArea += ((Box2)(ref intersection)).Width * ((Box2)(ref intersection)).Height;
					if (intersectionArea > minIntersectionArea)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool IsSpace(Tile tile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetContentTileDefinition(tile).MapAtmosphere;
	}

	public bool IsSpace(TileRef tile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return IsSpace(tile.Tile);
	}

	public EntityCoordinates GetTileCenter(TileRef turf)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		MapGridComponent grid = ((EntitySystem)this).Comp<MapGridComponent>(turf.GridUid);
		Vector2 center = (Vector2i.op_Implicit(turf.GridIndices) + new Vector2(0.5f, 0.5f)) * (int)grid.TileSize;
		return new EntityCoordinates(turf.GridUid, center);
	}

	public ContentTileDefinition GetContentTileDefinition(Tile tile)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return (ContentTileDefinition)(object)_tileDefinitions[tile.TypeId];
	}

	public ContentTileDefinition GetContentTileDefinition(TileRef tile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetContentTileDefinition(tile.Tile);
	}

	public void GetEntitiesInTile(EntityCoordinates coords, HashSet<EntityUid> intersecting, LookupFlags flags = (LookupFlags)4)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetTileRef(coords, out var tileRef))
		{
			_entityLookup.GetEntitiesInTile(tileRef.Value, intersecting, flags);
		}
	}

	public HashSet<EntityUid> GetEntitiesInTile(EntityCoordinates coords, LookupFlags flags = (LookupFlags)4)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetTileRef(coords, out var tileRef))
		{
			return new HashSet<EntityUid>();
		}
		return _entityLookup.GetEntitiesInTile(tileRef.Value, flags);
	}
}

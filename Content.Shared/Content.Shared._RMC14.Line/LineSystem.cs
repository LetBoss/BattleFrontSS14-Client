using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Map;
using Content.Shared.Beam.Components;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Doors.Components;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Line;

public sealed class LineSystem : EntitySystem
{
	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private INetManager _net;

	private static readonly ProtoId<TagPrototype> StructureTag = ProtoId<TagPrototype>.op_Implicit("Structure");

	private static readonly ProtoId<TagPrototype> WallTag = ProtoId<TagPrototype>.op_Implicit("Wall");

	private static readonly float MaxBeamDistance = 500f;

	private EntityQuery<BarricadeComponent> _barricadeQuery;

	private EntityQuery<DoorComponent> _doorQuery;

	private EntityQuery<MapGridComponent> _mapGridQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		_barricadeQuery = ((EntitySystem)this).GetEntityQuery<BarricadeComponent>();
		_doorQuery = ((EntitySystem)this).GetEntityQuery<DoorComponent>();
		_mapGridQuery = ((EntitySystem)this).GetEntityQuery<MapGridComponent>();
	}

	public List<LineTile> DrawLine(EntityCoordinates start, EntityCoordinates end, TimeSpan delayPer, float? range, out EntityUid? blocker, bool hitBlocker = false, bool thick = false)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		blocker = null;
		start = _mapSystem.AlignToGrid(_transform.GetMoverCoordinates(start));
		end = _mapSystem.AlignToGrid(_transform.GetMoverCoordinates(end));
		List<LineTile> tiles = new List<LineTile>();
		float distance = default(float);
		if (!((EntityCoordinates)(ref start)).TryDistance((IEntityManager)(object)base.EntityManager, _transform, end, ref distance))
		{
			return tiles;
		}
		if (range.HasValue)
		{
			distance = Math.Min(range.Value, distance);
		}
		float distanceX = ((EntityCoordinates)(ref end)).X - ((EntityCoordinates)(ref start)).X;
		float num = ((EntityCoordinates)(ref end)).Y - ((EntityCoordinates)(ref start)).Y;
		float x = ((EntityCoordinates)(ref start)).X;
		float y = ((EntityCoordinates)(ref start)).Y;
		float xOffset = distanceX / distance;
		float yOffset = num / distance;
		TimeSpan time = _timing.CurTime;
		EntityUid? gridId = _transform.GetGrid(Entity<TransformComponent>.op_Implicit(start.EntityId));
		MapGridComponent gridComp = ((!gridId.HasValue) ? null : _mapGridQuery.CompOrNull(gridId.Value));
		Entity<MapGridComponent>? grid = ((gridComp == null) ? ((Entity<MapGridComponent>?)null) : new Entity<MapGridComponent>?(new Entity<MapGridComponent>(gridId.Value, gridComp)));
		EntityCoordinates lastCoords = start;
		for (int i = 0; (float)i < distance; i++)
		{
			x += xOffset;
			y += yOffset;
			EntityCoordinates center = SnapgridHelper.SnapToGrid(new EntityCoordinates(start.EntityId, x, y), (IEntityManager?)(object)base.EntityManager, _mapManager);
			if (center == lastCoords)
			{
				continue;
			}
			List<EntityCoordinates> coords = new List<EntityCoordinates>(9);
			coords.Add(center);
			if (thick && i > 1)
			{
				for (int xo = -1; xo < 2; xo++)
				{
					for (int yo = -1; yo < 2; yo++)
					{
						if (xo != 0 || yo != 0)
						{
							EntityCoordinates point = SnapgridHelper.SnapToGrid(new EntityCoordinates(start.EntityId, x + (float)xo, y + (float)yo), (IEntityManager?)(object)base.EntityManager, _mapManager);
							coords.Add(point);
						}
					}
				}
			}
			bool centerBlocked = false;
			for (int j = 0; j < coords.Count; j++)
			{
				EntityCoordinates entityCoords = coords[j];
				Angle direction = DirectionExtensions.ToWorldAngle(entityCoords.Position - lastCoords.Position);
				bool blocked = IsTileBlocked(grid, entityCoords, direction, out blocker);
				if (blocked && !hitBlocker)
				{
					continue;
				}
				MapCoordinates mapCoords = _transform.ToMapCoordinates(entityCoords, true);
				bool isDuplicate = false;
				foreach (LineTile item in tiles)
				{
					if (item.Coordinates.Position == mapCoords.Position)
					{
						isDuplicate = true;
						break;
					}
				}
				if (!isDuplicate)
				{
					float delay = Vector2.Distance(entityCoords.Position, start.Position) - 1f;
					tiles.Add(new LineTile(mapCoords, time + delayPer * delay));
				}
				if (blocked && j == 0)
				{
					centerBlocked = true;
					break;
				}
			}
			if (centerBlocked)
			{
				break;
			}
			lastCoords = center;
		}
		return tiles;
	}

	private bool IsTileBlocked(Entity<MapGridComponent>? grid, EntityCoordinates coords, Angle angle, [NotNullWhen(true)] out EntityUid? blocker)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected I4, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Invalid comparison between Unknown and I4
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Invalid comparison between Unknown and I4
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Invalid comparison between Unknown and I4
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Invalid comparison between Unknown and I4
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Invalid comparison between Unknown and I4
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Invalid comparison between Unknown and I4
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		blocker = null;
		if (!grid.HasValue)
		{
			return false;
		}
		SharedMapSystem mapSystem = _mapSystem;
		EntityUid val = Entity<MapGridComponent>.op_Implicit(grid.Value);
		Entity<MapGridComponent>? val2 = grid;
		Vector2i indices = mapSystem.TileIndicesFor(val, val2.HasValue ? Entity<MapGridComponent>.op_Implicit(val2.GetValueOrDefault()) : null, coords);
		SharedMapSystem mapSystem2 = _mapSystem;
		EntityUid val3 = Entity<MapGridComponent>.op_Implicit(grid.Value);
		val2 = grid;
		AnchoredEntitiesEnumerator anchored = mapSystem2.GetAnchoredEntitiesEnumerator(val3, val2.HasValue ? Entity<MapGridComponent>.op_Implicit(val2.GetValueOrDefault()) : null, indices);
		EntityUid? uid = default(EntityUid?);
		DoorComponent door = default(DoorComponent);
		DoorComponent door2 = default(DoorComponent);
		while (((AnchoredEntitiesEnumerator)(ref anchored)).MoveNext(ref uid))
		{
			if (_barricadeQuery.HasComp(uid))
			{
				if (_doorQuery.TryComp(uid, ref door) && door.State != DoorState.Closed)
				{
					continue;
				}
				Angle worldRotation = _transform.GetWorldRotation(uid.Value);
				Direction barricadeDir = ((Angle)(ref worldRotation)).GetCardinalDir();
				Direction direction = ((Angle)(ref angle)).GetDir();
				if (barricadeDir == direction || barricadeDir == DirectionExtensions.GetOpposite(direction))
				{
					blocker = uid.Value;
					return true;
				}
				if (!direction.IsCardinal())
				{
					bool flag;
					switch (direction - 1)
					{
					case 0:
					{
						bool flag2 = (((int)barricadeDir == 4 || (int)barricadeDir == 6) ? true : false);
						flag = flag2;
						break;
					}
					case 2:
					{
						bool flag2 = (((int)barricadeDir == 0 || (int)barricadeDir == 6) ? true : false);
						flag = flag2;
						break;
					}
					case 4:
					{
						bool flag2 = (((int)barricadeDir == 0 || (int)barricadeDir == 2) ? true : false);
						flag = flag2;
						break;
					}
					case 6:
					{
						bool flag2 = (((int)barricadeDir == 2 || (int)barricadeDir == 4) ? true : false);
						flag = flag2;
						break;
					}
					default:
						flag = false;
						break;
					}
					if (flag)
					{
						blocker = uid.Value;
						return true;
					}
				}
			}
			else if (_doorQuery.TryComp(uid, ref door2))
			{
				if (door2.State == DoorState.Closed)
				{
					blocker = uid.Value;
					return true;
				}
			}
			else if (_tag.HasAnyTag(uid.Value, StructureTag, WallTag))
			{
				blocker = uid.Value;
				return true;
			}
		}
		return false;
	}

	public bool TryCreateLine(EntityUid source, EntityUid target, string proto, out List<EntityUid> lines)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		lines = new List<EntityUid>();
		if (_net.IsClient)
		{
			return false;
		}
		if (((EntitySystem)this).Deleted(source, (MetaDataComponent)null) || ((EntitySystem)this).Deleted(target, (MetaDataComponent)null))
		{
			return false;
		}
		if (_transform.GetMapId(Entity<TransformComponent>.op_Implicit(source)) != _transform.GetMapId(Entity<TransformComponent>.op_Implicit(target)))
		{
			return false;
		}
		MapCoordinates sourceMapPos = _transform.GetMapCoordinates(source, (TransformComponent)null);
		MapCoordinates targetMapPos = _transform.GetMapCoordinates(target, (TransformComponent)null);
		Vector2 calculatedDistance = targetMapPos.Position - sourceMapPos.Position;
		Angle sourceAngle = DirectionExtensions.ToWorldAngle(calculatedDistance);
		if (sourceMapPos.MapId != targetMapPos.MapId)
		{
			return false;
		}
		MapCoordinates beamStartPos = ((MapCoordinates)(ref sourceMapPos)).Offset(Vector2Helpers.Normalized(calculatedDistance));
		if (calculatedDistance.Length() == 0f || calculatedDistance.Length() > MaxBeamDistance)
		{
			return false;
		}
		Vector2 distanceCorrection = calculatedDistance - Vector2Helpers.Normalized(calculatedDistance);
		EntityUid beam = ((EntitySystem)this).Spawn(proto, beamStartPos, (ComponentRegistry)null, default(Angle));
		lines.Add(beam);
		float distanceLength = distanceCorrection.Length();
		BeamVisualizerEvent beamVisualizerEvent = new BeamVisualizerEvent(((EntitySystem)this).GetNetEntity(beam, (MetaDataComponent)null), distanceLength, sourceAngle, null, "shaded");
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)beamVisualizerEvent);
		for (int i = 0; (float)i < distanceLength - 1f; i++)
		{
			beamStartPos = ((MapCoordinates)(ref beamStartPos)).Offset(Vector2Helpers.Normalized(calculatedDistance));
			EntityUid newEnt = ((EntitySystem)this).Spawn(proto, beamStartPos, (ComponentRegistry)null, default(Angle));
			lines.Add(newEnt);
			BeamVisualizerEvent ev = new BeamVisualizerEvent(((EntitySystem)this).GetNetEntity(newEnt, (MetaDataComponent)null), distanceLength, sourceAngle, null, "shaded");
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev);
		}
		return true;
	}

	public void DeleteBeam(List<EntityUid> beam)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		foreach (EntityUid ent in beam)
		{
			((EntitySystem)this).QueueDel((EntityUid?)ent);
		}
		beam.Clear();
	}
}

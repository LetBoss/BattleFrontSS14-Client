using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.Tiles;

public sealed class FloorTileSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private ITileDefinitionManager _tileDefinitionManager;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedStackSystem _stackSystem;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TileSystem _tile;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private TurfSystem _turf;

	private static readonly Vector2 CheckRange = new Vector2(1f, 1f);

	private readonly HashSet<EntityUid> _turfCheck = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FloorTileComponent, AfterInteractEvent>((ComponentEventHandler<FloorTileComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
	}

	private void OnAfterInteract(EntityUid uid, FloorTileComponent component, AfterInteractEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Invalid comparison between Unknown and I4
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		StackComponent stack = default(StackComponent);
		if (!args.CanReach || ((HandledEntityEventArgs)args).Handled || !((EntitySystem)this).TryComp<StackComponent>(uid, ref stack) || component.OutputTiles == null)
		{
			return;
		}
		EntityCoordinates location = CoordinatesExtensions.AlignWithClosestGridTile(args.ClickLocation, 1.5f, (IEntityManager)null, (IMapManager)null);
		MapCoordinates locationMap = _transform.ToMapCoordinates(location, true);
		if (locationMap.MapId == MapId.Nullspace)
		{
			return;
		}
		EntityQuery<PhysicsComponent> physicQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		EntityQuery<TransformComponent> transformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		MapCoordinates map = _transform.ToMapCoordinates(location, true);
		(bool, EntityUid) state = (true, location.EntityId);
		_mapManager.FindGridsIntersecting<(bool, EntityUid)>(map.MapId, new Box2(map.Position - CheckRange, map.Position + CheckRange), ref state, (GridCallback<(bool, EntityUid)>)delegate(EntityUid entityUid, MapGridComponent val, ref (bool weh, EntityUid EntityId) tuple)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if (tuple.EntityId == entityUid)
			{
				return true;
			}
			tuple.weh = false;
			return false;
		}, false, true);
		if (!state.Item1)
		{
			if (_netManager.IsClient && _timing.IsFirstTimePredicted)
			{
				_popup.PopupEntity(base.Loc.GetString("invalid-floor-placement"), args.User);
			}
			return;
		}
		Vector2 dir = _transform.ToMapCoordinates(transformQuery.GetComponent(args.User).Coordinates, true).Position - map.Position;
		bool canAccessCenter = false;
		if ((double)dir.LengthSquared() > 0.01)
		{
			CollisionRay ray = default(CollisionRay);
			((CollisionRay)(ref ray))._002Ector(map.Position, Vector2Helpers.Normalized(dir), 2);
			canAccessCenter = !_physics.IntersectRay(locationMap.MapId, ray, dir.Length(), (EntityUid?)null, true).Any();
		}
		if (!canAccessCenter && _turf.TryGetTileRef(location, out var tileRef))
		{
			_turfCheck.Clear();
			_lookup.GetEntitiesInTile(tileRef.Value, _turfCheck, (LookupFlags)4);
			PhysicsComponent phys = default(PhysicsComponent);
			foreach (EntityUid ent in _turfCheck)
			{
				if (physicQuery.TryGetComponent(ent, ref phys) && (int)phys.BodyType == 4 && phys.Hard && (phys.CollisionLayer & 2) != 0)
				{
					return;
				}
			}
		}
		MapGridComponent mapGrid = default(MapGridComponent);
		((EntitySystem)this).TryComp<MapGridComponent>(location.EntityId, ref mapGrid);
		foreach (string currentTile in component.OutputTiles)
		{
			ContentTileDefinition currentTileDefinition = (ContentTileDefinition)(object)_tileDefinitionManager[currentTile];
			if (mapGrid != null)
			{
				EntityUid gridUid = location.EntityId;
				TileRef tile = _map.GetTileRef(gridUid, mapGrid, location);
				if (!CanPlaceTile(gridUid, mapGrid, tile.GridIndices, out string reason))
				{
					_popup.PopupClient(reason, args.User, args.User);
					break;
				}
				ContentTileDefinition baseTurf = (ContentTileDefinition)(object)_tileDefinitionManager[tile.Tile.TypeId];
				if (HasBaseTurf(currentTileDefinition, baseTurf.ID) && _stackSystem.Use(uid, 1, stack))
				{
					PlaceAt(args.User, gridUid, mapGrid, location, currentTileDefinition.TileId, component.PlaceTileSound);
					((HandledEntityEventArgs)args).Handled = true;
					break;
				}
			}
			else if (HasBaseTurf(currentTileDefinition, "Space") && _stackSystem.Use(uid, 1, stack))
			{
				((HandledEntityEventArgs)args).Handled = true;
				if (!_netManager.IsClient)
				{
					Entity<MapGridComponent> grid = _mapManager.CreateGridEntity(locationMap.MapId, (GridCreateOptions?)null);
					TransformComponent gridXform = ((EntitySystem)this).Transform(Entity<MapGridComponent>.op_Implicit(grid));
					_transform.SetWorldPosition(Entity<TransformComponent>.op_Implicit((Entity<MapGridComponent>.op_Implicit(grid), gridXform)), locationMap.Position);
					location = new EntityCoordinates(Entity<MapGridComponent>.op_Implicit(grid), Vector2.Zero);
					PlaceAt(args.User, Entity<MapGridComponent>.op_Implicit(grid), grid.Comp, location, _tileDefinitionManager[component.OutputTiles[0]].TileId, component.PlaceTileSound, (float)(int)grid.Comp.TileSize / 2f);
				}
				break;
			}
		}
	}

	public bool HasBaseTurf(ContentTileDefinition tileDef, string baseTurf)
	{
		return tileDef.BaseTurf == baseTurf;
	}

	private void PlaceAt(EntityUid user, EntityUid gridUid, MapGridComponent mapGrid, EntityCoordinates location, ushort tileId, SoundSpecifier placeSound, float offset = 0f)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(18, 4);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "actor", "ToPrettyString(user)");
		handler.AppendLiteral(" placed tile ");
		handler.AppendFormatted(_tileDefinitionManager[(int)tileId].Name);
		handler.AppendLiteral(" at ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(gridUid)), "ToPrettyString(gridUid)");
		handler.AppendLiteral(" ");
		handler.AppendFormatted<EntityCoordinates>(location, "location");
		adminLogger.Add(LogType.Tile, LogImpact.Low, ref handler);
		System.Random random = new System.Random((int)_timing.CurTick.Value);
		byte variant = _tile.PickVariant((ContentTileDefinition)(object)_tileDefinitionManager[(int)tileId], random);
		_map.SetTile(gridUid, mapGrid, ((EntityCoordinates)(ref location)).Offset(new Vector2(offset, offset)), new Tile((int)tileId, (byte)0, variant, (byte)0));
		_audio.PlayPredicted(placeSound, location, (EntityUid?)user, (AudioParams?)null);
	}

	public bool CanPlaceTile(EntityUid gridUid, MapGridComponent component, Vector2i gridIndices, [NotNullWhen(false)] out string? reason)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		FloorTileAttemptEvent ev = new FloorTileAttemptEvent(gridIndices);
		((EntitySystem)this).RaiseLocalEvent<FloorTileAttemptEvent>(gridUid, ref ev, false);
		if (ev.Cancelled)
		{
			reason = base.Loc.GetString("invalid-floor-placement");
			return false;
		}
		reason = null;
		return true;
	}
}

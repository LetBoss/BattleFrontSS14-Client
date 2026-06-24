using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Robust.Shared.GameObjects;

public abstract class SharedTransformSystem : EntitySystem
{
	public delegate void MoveEventHandler(ref MoveEvent ev);

	[Robust.Shared.IoC.Dependency]
	private readonly IGameTiming _gameTiming;

	[Robust.Shared.IoC.Dependency]
	private readonly IMapManager _mapManager;

	[Robust.Shared.IoC.Dependency]
	private readonly EntityLookupSystem _lookup;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedMapSystem _map;

	[Robust.Shared.IoC.Dependency]
	private readonly MetaDataSystem _metaData;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedPhysicsSystem _physics;

	[Robust.Shared.IoC.Dependency]
	private readonly INetManager _netMan;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedContainerSystem _container;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedGridTraversalSystem _traversal;

	private EntityQuery<MapComponent> _mapQuery;

	private EntityQuery<MapGridComponent> _gridQuery;

	private EntityQuery<MetaDataComponent> _metaQuery;

	protected EntityQuery<TransformComponent> XformQuery;

	public event MoveEventHandler? OnGlobalMoveEvent;

	internal event MoveEventHandler? OnBeforeMoveEvent;

	internal void ReAnchor(EntityUid uid, TransformComponent xform, MapGridComponent oldGrid, MapGridComponent newGrid, Vector2i oldTilePos, Vector2i tilePos, EntityUid oldGridUid, EntityUid newGridUid, TransformComponent oldGridXform, TransformComponent newGridXform, Angle rotation)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		_map.RemoveFromSnapGridCell(oldGridUid, oldGrid, oldTilePos, uid);
		_map.AddToSnapGridCell(newGridUid, newGrid, tilePos, uid);
		xform._anchored = false;
		oldGridXform._children.Remove(uid);
		newGridXform._children.Add(uid);
		xform._parent = newGridUid;
		xform._anchored = true;
		Vector2 localPosition = xform._localPosition;
		Angle localRotation = xform._localRotation;
		EntityUid? mapUid = xform.MapUid;
		xform._localPosition = Vector2i.op_Implicit(tilePos) + newGrid.TileSizeHalfVector;
		xform._localRotation += rotation;
		MetaDataComponent metaDataComponent = MetaData(uid);
		SetGridId((Owner: uid, Comp1: xform, Comp2: metaDataComponent), newGridUid);
		RaiseMoveEvent((Owner: uid, Comp1: xform, Comp2: metaDataComponent), oldGridUid, localPosition, localRotation, mapUid);
		Dirty(uid, xform, metaDataComponent);
		ReAnchorEvent args = new ReAnchorEvent(uid, oldGridUid, newGridUid, tilePos, xform);
		RaiseLocalEvent(uid, ref args);
	}

	[Obsolete("Use Entity<T> variant")]
	public bool AnchorEntity(EntityUid uid, TransformComponent xform, EntityUid gridUid, MapGridComponent grid, Vector2i tileIndices)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return AnchorEntity((Owner: uid, Comp: xform), (Owner: gridUid, Comp: grid), tileIndices);
	}

	public bool AnchorEntity(Entity<TransformComponent> entity, Entity<MapGridComponent> grid, Vector2i tileIndices)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		Entity<TransformComponent> entity2 = entity;
		var (entityUid2, transformComponent2) = (Entity<TransformComponent>)(ref entity2);
		if (!_map.AddToSnapGridCell(grid, grid, tileIndices, entityUid2))
		{
			return false;
		}
		bool anchored = entity.Comp._anchored;
		transformComponent2._anchored = true;
		MetaDataComponent metaDataComponent = MetaData(entityUid2);
		Dirty(entity, metaDataComponent);
		_physics.TrySetBodyType(entityUid2, BodyType.Static, null, null, transformComponent2);
		if (!anchored && transformComponent2.Running)
		{
			AnchorStateChangedEvent args = new AnchorStateChangedEvent(entityUid2, transformComponent2);
			RaiseLocalEvent(entityUid2, ref args, broadcast: true);
		}
		SetCoordinates(value: new EntityCoordinates(grid, _map.GridTileToLocal(grid, grid, tileIndices).Position), entity: (Owner: entityUid2, Comp1: transformComponent2, Comp2: metaDataComponent), rotation: null, unanchor: false);
		return true;
	}

	[Obsolete("Use Entity<T> variants")]
	public bool AnchorEntity(EntityUid uid, TransformComponent xform, MapGridComponent grid)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Vector2i tileIndices = _map.TileIndicesFor(grid.Owner, grid, xform.Coordinates);
		return AnchorEntity(uid, xform, grid.Owner, grid, tileIndices);
	}

	public bool AnchorEntity(EntityUid uid)
	{
		return AnchorEntity(uid, XformQuery.GetComponent(uid));
	}

	public bool AnchorEntity(EntityUid uid, TransformComponent xform)
	{
		return AnchorEntity((Owner: uid, Comp: xform));
	}

	public bool AnchorEntity(Entity<TransformComponent> entity, Entity<MapGridComponent>? grid = null)
	{
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		if (grid.HasValue)
		{
			EntityUid owner = grid.Value.Owner;
			EntityUid? gridUid = entity.Comp.GridUid;
			if (owner != gridUid)
			{
				base.Log.Error($"Tried to anchor entity {Name(entity)} to a grid ({grid.Value.Owner}) different from its GridUid ({entity.Comp.GridUid})");
				return false;
			}
		}
		if (!grid.HasValue)
		{
			if (!TryComp(entity.Comp.GridUid, out MapGridComponent comp))
			{
				return false;
			}
			grid = (Owner: entity.Comp.GridUid.Value, Comp: comp);
		}
		Vector2i tileIndices = _map.TileIndicesFor(grid.Value, grid.Value, entity.Comp.Coordinates);
		return AnchorEntity(entity, grid.Value, tileIndices);
	}

	public void Unanchor(EntityUid uid)
	{
		Unanchor(uid, XformQuery.GetComponent(uid));
	}

	public void Unanchor(EntityUid uid, TransformComponent xform, bool setPhysics = true)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!xform._anchored)
		{
			return;
		}
		Dirty(uid, xform);
		xform._anchored = false;
		if (setPhysics)
		{
			_physics.TrySetBodyType(uid, BodyType.Dynamic, null, null, xform);
		}
		if ((int)xform.LifeStage >= 4)
		{
			if (_gridQuery.TryGetComponent(xform.GridUid, out MapGridComponent component))
			{
				Vector2i pos = _map.TileIndicesFor(xform.GridUid.Value, component, xform.Coordinates);
				_map.RemoveFromSnapGridCell(xform.GridUid.Value, component, pos, uid);
			}
			if (xform.Running)
			{
				AnchorStateChangedEvent args = new AnchorStateChangedEvent(uid, xform);
				RaiseLocalEvent(uid, ref args, broadcast: true);
			}
		}
	}

	public bool ContainsEntity(EntityUid parent, Entity<TransformComponent?> child)
	{
		if (!Resolve(child.Owner, ref child.Comp))
		{
			return false;
		}
		if (!child.Comp.ParentUid.IsValid())
		{
			return false;
		}
		if (parent == child.Comp.ParentUid)
		{
			return true;
		}
		if (!XformQuery.TryGetComponent(child.Comp.ParentUid, out TransformComponent component))
		{
			return false;
		}
		return ContainsEntity(parent, (Owner: child.Comp.ParentUid, Comp: component));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsParentOf(TransformComponent parent, EntityUid child)
	{
		return parent._children.Contains(child);
	}

	internal (EntityUid?, MapId) InitializeMapUid(EntityUid uid, TransformComponent xform)
	{
		if (xform._mapIdInitialized)
		{
			return (xform.MapUid, xform.MapID);
		}
		MapComponent component;
		if (xform.ParentUid.IsValid())
		{
			(xform.MapUid, xform.MapID) = InitializeMapUid(xform.ParentUid, Transform(xform.ParentUid));
		}
		else if (_mapQuery.TryComp(uid, out component))
		{
			if (component.MapId == MapId.Nullspace)
			{
				base.Log.Error("Transform is initialising before map ids have been assigned?");
				_map.AssignMapId((Owner: uid, Comp: component));
			}
			xform.MapUid = uid;
			xform.MapID = component.MapId;
		}
		else
		{
			xform.MapUid = null;
			xform.MapID = MapId.Nullspace;
		}
		xform._mapIdInitialized = true;
		return (xform.MapUid, xform.MapID);
	}

	private void OnCompInit(EntityUid uid, TransformComponent component, ComponentInit args)
	{
		InitializeMapUid(uid, component);
		if (component.ParentUid.IsValid())
		{
			TransformComponent component2 = XformQuery.GetComponent(component.ParentUid);
			if ((int)component2.LifeStage > 6 || (int)LifeStage(component.ParentUid) > 3)
			{
				string message = $"Attempted to re-parent to a terminating object. Entity: {ToPrettyString(component.ParentUid)}, new parent: {ToPrettyString(uid)}";
				base.Log.Error(message);
				Del(uid);
			}
			component2._children.Add(uid);
		}
		InitializeGridUid(uid, component);
		component.MatricesDirty = true;
		if (!component._anchored)
		{
			return;
		}
		Entity<MapGridComponent>? grid = null;
		EntityUid? gridUid = component.GridUid;
		EntityUid parentUid = component.ParentUid;
		if (gridUid.HasValue && gridUid.GetValueOrDefault() == parentUid && TryComp(component.ParentUid, out MapGridComponent grid2))
		{
			grid = (Owner: component.ParentUid, Comp: grid2);
		}
		else
		{
			MapCoordinates mapCoordinates = new MapCoordinates(GetWorldPosition(component), component.MapID);
			if (_mapManager.TryFindGridAt(mapCoordinates, out EntityUid uid2, out grid2))
			{
				grid = (Owner: uid2, Comp: grid2);
			}
		}
		if (!grid.HasValue)
		{
			Unanchor(uid, component);
		}
		else if (!AnchorEntity((Owner: uid, Comp: component), grid))
		{
			component._anchored = false;
		}
	}

	internal void InitializeGridUid(EntityUid uid, TransformComponent xform)
	{
		if (xform._gridInitialized)
		{
			return;
		}
		if (_gridQuery.HasComponent(uid))
		{
			xform._gridUid = uid;
			xform._gridInitialized = true;
			return;
		}
		if ((int)xform.LifeStage >= 3)
		{
			xform._gridInitialized = true;
		}
		if (xform._parent.IsValid())
		{
			TransformComponent component = XformQuery.GetComponent(xform._parent);
			InitializeGridUid(xform._parent, component);
			xform._gridUid = component._gridUid;
		}
	}

	private void OnCompStartup(EntityUid uid, TransformComponent xform, ComponentStartup args)
	{
		if (xform.Anchored)
		{
			AnchorStateChangedEvent args2 = new AnchorStateChangedEvent(uid, xform);
			RaiseLocalEvent(uid, ref args2, broadcast: true);
		}
		EntParentChangedMessage args3 = new EntParentChangedMessage(uid, null, null, xform);
		RaiseLocalEvent(uid, ref args3, broadcast: true);
		TransformStartupEvent args4 = new TransformStartupEvent((Owner: uid, Comp: xform));
		RaiseLocalEvent(uid, ref args4, broadcast: true);
	}

	public void SetGridId(EntityUid uid, TransformComponent xform, EntityUid? gridId, EntityQuery<TransformComponent>? xformQuery = null)
	{
		SetGridId((Owner: uid, Comp1: xform, Comp2: MetaData(uid)), gridId);
	}

	public void SetGridId(Entity<TransformComponent, MetaDataComponent?> ent, EntityUid? gridId)
	{
		if (!ent.Comp1._gridInitialized || ent.Comp1._gridUid == gridId || ent.Comp1.GridUid == ent.Owner)
		{
			return;
		}
		_metaQuery.ResolveInternal(ent.Owner, ref ent.Comp2);
		if ((ent.Comp2.Flags & MetaDataFlags.ExtraTransformEvents) != MetaDataFlags.None)
		{
			GridUidChangedEvent args = new GridUidChangedEvent((Owner: ent.Owner, Comp1: ent.Comp1, Comp2: ent.Comp2), ent.Comp1._gridUid);
			ent.Comp1._gridUid = gridId;
			RaiseLocalEvent(ent, ref args);
		}
		ent.Comp1._gridUid = gridId;
		foreach (EntityUid child in ent.Comp1._children)
		{
			SetGridId((Owner: child, Comp1: XformQuery.GetComponent(child), Comp2: (MetaDataComponent)null), gridId);
		}
	}

	[Obsolete("use override with EntityUid")]
	public void SetLocalPosition(TransformComponent xform, Vector2 value)
	{
		SetLocalPosition(xform.Owner, value, xform);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void SetLocalPosition(EntityUid uid, Vector2 value, TransformComponent? xform = null)
	{
		SetLocalPositionNoLerp(uid, value, xform);
	}

	[Obsolete("use override with EntityUid")]
	public void SetLocalPositionNoLerp(TransformComponent xform, Vector2 value)
	{
		SetLocalPositionNoLerp(xform.Owner, value, xform);
	}

	public void SetLocalPositionNoLerp(EntityUid uid, Vector2 value, TransformComponent? xform = null)
	{
		if (XformQuery.Resolve(uid, ref xform))
		{
			xform.LocalPosition = value;
		}
	}

	public void SetLocalRotationNoLerp(EntityUid uid, Angle value, TransformComponent? xform = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (XformQuery.Resolve(uid, ref xform))
		{
			xform.LocalRotation = value;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void SetLocalRotation(EntityUid uid, Angle value, TransformComponent? xform = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SetLocalRotationNoLerp(uid, value, xform);
	}

	[Obsolete("use override with EntityUid")]
	public void SetLocalRotation(TransformComponent xform, Angle value)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SetLocalRotation(xform.Owner, value, xform);
	}

	public void SetCoordinates(EntityUid uid, EntityCoordinates value)
	{
		SetCoordinates((Owner: uid, Comp1: Transform(uid), Comp2: MetaData(uid)), value);
	}

	public void SetCoordinates(Entity<TransformComponent, MetaDataComponent> entity, EntityCoordinates value, Angle? rotation = null, bool unanchor = true, TransformComponent? newParent = null, TransformComponent? oldParent = null)
	{
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		Entity<TransformComponent, MetaDataComponent> entity2 = entity;
		var (entityUid2, transformComponent2, metaDataComponent2) = (Entity<TransformComponent, MetaDataComponent>)(ref entity2);
		if (transformComponent2.ParentUid == value.EntityId && Vector2Helpers.EqualsApprox(transformComponent2._localPosition, value.Position) && (!rotation.HasValue || MathHelper.CloseTo(rotation.Value.Theta, transformComponent2._localRotation.Theta, 1E-07)))
		{
			return;
		}
		if (transformComponent2.Anchored && unanchor)
		{
			Unanchor(entityUid2, transformComponent2);
		}
		if (value.EntityId != transformComponent2.ParentUid && value.EntityId.IsValid())
		{
			if ((int)metaDataComponent2.EntityLifeStage >= 4)
			{
				base.Log.Error($"{ToPrettyString(entityUid2)} is attempting to move while terminating. New parent: {ToPrettyString(value.EntityId)}. Trace: {Environment.StackTrace}");
				return;
			}
			if (TerminatingOrDeleted(value.EntityId))
			{
				base.Log.Error($"{ToPrettyString(entityUid2)} is attempting to attach itself to a terminating entity {ToPrettyString(value.EntityId)}. Trace: {Environment.StackTrace}");
				return;
			}
		}
		EntityUid parent = transformComponent2._parent;
		Vector2 localPosition = transformComponent2._localPosition;
		Angle localRotation = transformComponent2._localRotation;
		EntityUid? mapUid = transformComponent2.MapUid;
		Dirty(entityUid2, transformComponent2, metaDataComponent2);
		transformComponent2.MatricesDirty = true;
		transformComponent2._localPosition = value.Position;
		if (rotation.HasValue && !transformComponent2.NoLocalRotation)
		{
			transformComponent2._localRotation = rotation.Value;
		}
		if (value.EntityId != transformComponent2._parent)
		{
			if (value.EntityId == entityUid2)
			{
				DetachEntity(entityUid2, transformComponent2);
				if (_netMan.IsServer || IsClientSide(entityUid2))
				{
					QueueDel(entityUid2);
				}
				throw new InvalidOperationException($"Attempted to parent an entity to itself: {ToPrettyString(entityUid2)}");
			}
			if (value.EntityId.IsValid())
			{
				if (!XformQuery.Resolve(value.EntityId, ref newParent, logMissing: false))
				{
					DetachEntity(entityUid2, transformComponent2);
					if (_netMan.IsServer || IsClientSide(entityUid2))
					{
						QueueDel(entityUid2);
					}
					throw new InvalidOperationException($"Attempted to parent entity {ToPrettyString(entityUid2)} to non-existent entity {value.EntityId}");
				}
				if ((int)newParent.LifeStage >= 7 || (int)LifeStage(value.EntityId) >= 4)
				{
					DetachEntity(entityUid2, transformComponent2);
					if (_netMan.IsServer || IsClientSide(entityUid2))
					{
						QueueDel(entityUid2);
					}
					throw new InvalidOperationException($"Attempted to re-parent to a terminating object. Entity: {ToPrettyString(entityUid2)}, new parent: {ToPrettyString(value.EntityId)}");
				}
				InitializeMapUid(value.EntityId, newParent);
				if (transformComponent2.MapUid == newParent.MapUid)
				{
					EntityUid entityUid3 = value.EntityId;
					TransformComponent transformComponent3 = newParent;
					while (transformComponent3.ParentUid.IsValid())
					{
						if (transformComponent3.ParentUid == entityUid2)
						{
							if (!_gameTiming.ApplyingState)
							{
								throw new InvalidOperationException($"Attempted to parent an entity to one of its descendants! {ToPrettyString(entityUid2)}. new parent: {ToPrettyString(value.EntityId)}");
							}
							base.Log.Warning($"Encountered circular transform hierarchy while applying state for entity: {ToPrettyString(entityUid2)}. Detaching child to null: {ToPrettyString(entityUid3)}");
							DetachEntity(entityUid3, transformComponent3);
							break;
						}
						entityUid3 = transformComponent3.ParentUid;
						transformComponent3 = XformQuery.GetComponent(entityUid3);
					}
				}
			}
			if (transformComponent2._parent.IsValid())
			{
				XformQuery.Resolve(transformComponent2._parent, ref oldParent);
			}
			oldParent?._children.Remove(entityUid2);
			newParent?._children.Add(entityUid2);
			transformComponent2._parent = value.EntityId;
			if (newParent != null)
			{
				ChangeMapId(entity, newParent.MapID);
				if (!transformComponent2._gridInitialized)
				{
					InitializeGridUid(entityUid2, transformComponent2);
				}
				else
				{
					if (!newParent._gridInitialized)
					{
						InitializeGridUid(value.EntityId, newParent);
					}
					SetGridId(entity, newParent.GridUid);
				}
			}
			else
			{
				ChangeMapId(entity, MapId.Nullspace);
				if (!transformComponent2._gridInitialized)
				{
					InitializeGridUid(entityUid2, transformComponent2);
				}
				else
				{
					SetGridId(entity, null);
				}
			}
			if (transformComponent2.Initialized && !rotation.HasValue && oldParent != null && newParent != null && !transformComponent2.NoLocalRotation)
			{
				transformComponent2._localRotation += GetWorldRotation(oldParent) - GetWorldRotation(newParent);
			}
		}
		if (transformComponent2.Initialized)
		{
			RaiseMoveEvent(entity, parent, localPosition, localRotation, mapUid);
		}
	}

	public void SetCoordinates(EntityUid uid, TransformComponent xform, EntityCoordinates value, Angle? rotation = null, bool unanchor = true, TransformComponent? newParent = null, TransformComponent? oldParent = null)
	{
		SetCoordinates((Owner: uid, Comp1: xform, Comp2: _metaQuery.GetComponent(uid)), value, rotation, unanchor, newParent, oldParent);
	}

	private void ChangeMapId(Entity<TransformComponent, MetaDataComponent> ent, MapId newMapId)
	{
		if (!(newMapId == ent.Comp1.MapID))
		{
			EntityUid? newMap = ((newMapId == MapId.Nullspace) ? ((EntityUid?)null) : new EntityUid?(_map.GetMap(newMapId)));
			bool? paused = null;
			if (!_gameTiming.ApplyingState)
			{
				paused = _map.IsPaused(newMapId);
				_metaData.SetEntityPaused(ent.Owner, paused.Value, ent.Comp2);
			}
			ChangeMapIdRecursive(ent, newMap, newMapId, paused);
		}
	}

	private void ChangeMapIdRecursive(Entity<TransformComponent, MetaDataComponent> ent, EntityUid? newMap, MapId newMapId, bool? paused)
	{
		if (paused.HasValue)
		{
			bool valueOrDefault = paused == true;
			_metaData.SetEntityPaused(ent.Owner, valueOrDefault, ent.Comp2);
		}
		if ((ent.Comp2.Flags & MetaDataFlags.ExtraTransformEvents) != MetaDataFlags.None)
		{
			MapUidChangedEvent args = new MapUidChangedEvent(ent, ent.Comp1.MapUid, ent.Comp1.MapID);
			ent.Comp1.MapUid = newMap;
			ent.Comp1.MapID = newMapId;
			RaiseLocalEvent(ent.Owner, ref args);
		}
		ent.Comp1.MapUid = newMap;
		ent.Comp1.MapID = newMapId;
		foreach (EntityUid child in ent.Comp1._children)
		{
			Entity<TransformComponent, MetaDataComponent> ent2 = new Entity<TransformComponent, MetaDataComponent>(child, Transform(child), MetaData(child));
			ChangeMapIdRecursive(ent2, newMap, newMapId, paused);
		}
	}

	public void ReparentChildren(EntityUid oldUid, EntityUid uid)
	{
		ReparentChildren(oldUid, uid, XformQuery);
	}

	public void ReparentChildren(EntityUid oldUid, EntityUid uid, EntityQuery<TransformComponent> xformQuery)
	{
		if (oldUid == uid)
		{
			base.Log.Error($"Tried to reparent entities from the same entity, {ToPrettyString(oldUid)}");
			return;
		}
		TransformComponent component = xformQuery.GetComponent(oldUid);
		TransformComponent component2 = xformQuery.GetComponent(uid);
		EntityUid[] array = component._children.ToArray();
		foreach (EntityUid uid2 in array)
		{
			SetParent(uid2, xformQuery.GetComponent(uid2), uid, xformQuery, component2);
		}
	}

	public TransformComponent? GetParent(EntityUid uid)
	{
		return GetParent(XformQuery.GetComponent(uid));
	}

	public TransformComponent? GetParent(TransformComponent xform)
	{
		if (!xform.ParentUid.IsValid())
		{
			return null;
		}
		return XformQuery.GetComponent(xform.ParentUid);
	}

	public EntityUid GetParentUid(EntityUid uid)
	{
		return XformQuery.GetComponent(uid).ParentUid;
	}

	public void SetParent(EntityUid uid, EntityUid parent)
	{
		SetParent(uid, XformQuery.GetComponent(uid), parent, XformQuery);
	}

	public void SetParent(EntityUid uid, TransformComponent xform, EntityUid parent, TransformComponent? parentXform = null)
	{
		SetParent(uid, xform, parent, XformQuery, parentXform);
	}

	public void SetParent(EntityUid uid, TransformComponent xform, EntityUid parent, EntityQuery<TransformComponent> xformQuery, TransformComponent? parentXform = null)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!(xform.ParentUid == parent))
		{
			if (!parent.IsValid())
			{
				DetachEntity(uid, xform);
			}
			else if (xformQuery.Resolve(parent, ref parentXform))
			{
				(Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) worldPositionRotationInvMatrix = GetWorldPositionRotationInvMatrix(parentXform, xformQuery);
				Angle item = worldPositionRotationInvMatrix.WorldRotation;
				Matrix3x2 item2 = worldPositionRotationInvMatrix.InvWorldMatrix;
				(Vector2 WorldPosition, Angle WorldRotation) worldPositionRotation = GetWorldPositionRotation(xform, xformQuery);
				Vector2 item3 = worldPositionRotation.WorldPosition;
				Angle item4 = worldPositionRotation.WorldRotation;
				Vector2 position = Vector2.Transform(item3, item2);
				Angle value = item4 - item;
				SetCoordinates(uid, xform, new EntityCoordinates(parent, position), value, unanchor: true, parentXform);
			}
		}
	}

	public virtual void ActivateLerp(EntityUid uid, TransformComponent xform)
	{
	}

	internal void OnGetState(EntityUid uid, TransformComponent component, ref ComponentGetState args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetEntity netEntity = GetNetEntity(component.ParentUid);
		args.State = new TransformComponentState(component.LocalPosition, component.LocalRotation, netEntity, component.NoLocalRotation, component.Anchored);
	}

	internal void OnHandleState(EntityUid uid, TransformComponent xform, ref ComponentHandleState args)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		bool anchored;
		if (args.Current is TransformComponentState transformComponentState)
		{
			EntityUid entityUid = EnsureEntity<TransformComponent>(transformComponentState.ParentID, uid);
			anchored = xform.Anchored;
			if (Vector2Helpers.EqualsApprox(xform.LocalPosition, transformComponentState.LocalPosition))
			{
				Angle localRotation = xform.LocalRotation;
				if (((Angle)(ref localRotation)).EqualsApprox(transformComponentState.Rotation) && !(xform.ParentUid != entityUid))
				{
					xform.Anchored = transformComponentState.Anchored;
					goto IL_019f;
				}
			}
			if (xform.Anchored && TryComp(xform.ParentUid, out MapGridComponent comp))
			{
				Vector2i pos = _map.TileIndicesFor(xform.ParentUid, comp, xform.Coordinates);
				_map.RemoveFromSnapGridCell(xform.ParentUid, comp, pos, uid);
			}
			xform._anchored |= transformComponentState.Anchored;
			SetCoordinates(uid, xform, new EntityCoordinates(entityUid, transformComponentState.LocalPosition), transformComponentState.Rotation, unanchor: false);
			xform._anchored = transformComponentState.Anchored;
			if (xform._anchored && xform.Initialized)
			{
				EntityUid parentUid = xform.ParentUid;
				EntityUid? gridUid = xform.GridUid;
				if (gridUid.HasValue && parentUid == gridUid.GetValueOrDefault() && TryComp(xform.GridUid, out MapGridComponent comp2))
				{
					Vector2i pos2 = _map.TileIndicesFor(xform.GridUid.Value, comp2, xform.Coordinates);
					_map.AddToSnapGridCell(xform.GridUid.Value, comp2, pos2, uid);
				}
				else
				{
					xform._anchored = false;
				}
			}
			goto IL_019f;
		}
		goto IL_01cf;
		IL_01cf:
		if (args.Next is TransformComponentState transformComponentState2 && transformComponentState2.ParentID == GetNetEntity(xform.ParentUid))
		{
			xform.NextPosition = transformComponentState2.LocalPosition;
			xform.NextRotation = transformComponentState2.Rotation;
			ActivateLerp(uid, xform);
		}
		return;
		IL_019f:
		if (anchored != transformComponentState.Anchored && xform.Initialized)
		{
			AnchorStateChangedEvent args2 = new AnchorStateChangedEvent(uid, xform);
			RaiseLocalEvent(uid, ref args2, broadcast: true);
		}
		xform._noLocalRotation = transformComponentState.NoLocalRotation;
		goto IL_01cf;
	}

	public Matrix3x2 GetWorldMatrix(EntityUid uid)
	{
		return GetWorldMatrix(XformQuery.GetComponent(uid), XformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Matrix3x2 GetWorldMatrix(TransformComponent component)
	{
		return GetWorldMatrix(component, XformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Matrix3x2 GetWorldMatrix(EntityUid uid, EntityQuery<TransformComponent> xformQuery)
	{
		return GetWorldMatrix(xformQuery.GetComponent(uid), xformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Matrix3x2 GetWorldMatrix(TransformComponent component, EntityQuery<TransformComponent> xformQuery)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		var (vector, val) = GetWorldPositionRotation(component, xformQuery);
		return Matrix3Helpers.CreateTransform(ref vector, ref val);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2 GetWorldPosition(EntityUid uid)
	{
		return GetWorldPosition(XformQuery.GetComponent(uid));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2 GetWorldPosition(TransformComponent component)
	{
		Vector2 result = component._localPosition;
		while (true)
		{
			EntityUid parentUid = component.ParentUid;
			EntityUid? mapUid = component.MapUid;
			if (!(parentUid != mapUid) || !component.ParentUid.IsValid())
			{
				break;
			}
			component = XformQuery.GetComponent(component.ParentUid);
			result = ((Angle)(ref component._localRotation)).RotateVec(ref result) + component._localPosition;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2 GetWorldPosition(EntityUid uid, EntityQuery<TransformComponent> xformQuery)
	{
		return GetWorldPosition(xformQuery.GetComponent(uid));
	}

	public Vector2 GetWorldPosition(TransformComponent component, EntityQuery<TransformComponent> xformQuery)
	{
		return GetWorldPosition(component);
	}

	public MapCoordinates GetMapCoordinates(EntityUid entity, TransformComponent? xform = null)
	{
		if (!XformQuery.Resolve(entity, ref xform))
		{
			return MapCoordinates.Nullspace;
		}
		return GetMapCoordinates(xform);
	}

	public MapCoordinates GetMapCoordinates(TransformComponent xform)
	{
		return new MapCoordinates(GetWorldPosition(xform), xform.MapID);
	}

	public MapCoordinates GetMapCoordinates(Entity<TransformComponent> entity)
	{
		return GetMapCoordinates(entity.Comp);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMapCoordinates(EntityUid entity, MapCoordinates coordinates)
	{
		TransformComponent component = XformQuery.GetComponent(entity);
		SetMapCoordinates((Owner: entity, Comp: component), coordinates);
	}

	public void SetMapCoordinates(Entity<TransformComponent> entity, MapCoordinates coordinates)
	{
		EntityUid map = _map.GetMap(coordinates.MapId);
		if (!_gridQuery.HasComponent(entity) && _mapManager.TryFindGridAt(map, coordinates.Position, out EntityUid uid, out MapGridComponent _))
		{
			Matrix3x2 invWorldMatrix = GetInvWorldMatrix(uid);
			SetCoordinates((Owner: entity.Owner, Comp1: entity.Comp, Comp2: MetaData(entity.Owner)), new EntityCoordinates(uid, Vector2.Transform(coordinates.Position, invWorldMatrix)));
		}
		else
		{
			SetCoordinates((Owner: entity.Owner, Comp1: entity.Comp, Comp2: MetaData(entity.Owner)), new EntityCoordinates(map, coordinates.Position));
		}
	}

	public (Vector2 WorldPosition, Angle WorldRotation) GetWorldPositionRotation(EntityUid uid)
	{
		return GetWorldPositionRotation(XformQuery.GetComponent(uid));
	}

	public (Vector2 WorldPosition, Angle WorldRotation) GetWorldPositionRotation(TransformComponent component)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Vector2 item = component._localPosition;
		Angle val = component._localRotation;
		while (true)
		{
			EntityUid parentUid = component.ParentUid;
			EntityUid? mapUid = component.MapUid;
			if (!(parentUid != mapUid) || !component.ParentUid.IsValid())
			{
				break;
			}
			component = XformQuery.GetComponent(component.ParentUid);
			item = ((Angle)(ref component._localRotation)).RotateVec(ref item) + component._localPosition;
			val += component._localRotation;
		}
		return (WorldPosition: item, WorldRotation: val);
	}

	public (Vector2 WorldPosition, Angle WorldRotation) GetWorldPositionRotation(TransformComponent component, EntityQuery<TransformComponent> xformQuery)
	{
		return GetWorldPositionRotation(component);
	}

	[Obsolete("Use variant without entity query")]
	public (Vector2 Position, Angle Rotation) GetRelativePositionRotation(TransformComponent component, EntityUid relative, EntityQuery<TransformComponent> query)
	{
		return GetRelativePositionRotation(component, relative);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 Position, Angle Rotation) GetRelativePositionRotation(TransformComponent component, EntityUid relative)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Angle val = component._localRotation;
		Vector2 vector = component._localPosition;
		TransformComponent comp = component;
		while (comp.ParentUid != relative)
		{
			if (comp.ParentUid.IsValid() && TryComp(comp.ParentUid, out comp))
			{
				val += comp._localRotation;
				vector = ((Angle)(ref comp._localRotation)).RotateVec(ref vector) + comp._localPosition;
				continue;
			}
			base.Log.Warning($"Target entity ({ToPrettyString(relative)}) not in transform hierarchy while calling {"GetRelativePositionRotation"}.");
			TransformComponent component2 = Transform(relative);
			vector = Vector2.Transform(vector, GetInvWorldMatrix(component2));
			val -= GetWorldRotation(component2);
			break;
		}
		return (Position: vector, Rotation: val);
	}

	[Obsolete("Use variant without entity query")]
	public Vector2 GetRelativePosition(TransformComponent component, EntityUid relative, EntityQuery<TransformComponent> query)
	{
		return GetRelativePosition(component, relative);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2 GetRelativePosition(TransformComponent component, EntityUid relative)
	{
		Vector2 vector = component._localPosition;
		TransformComponent comp = component;
		while (comp.ParentUid != relative)
		{
			if (comp.ParentUid.IsValid() && TryComp(comp.ParentUid, out comp))
			{
				vector = ((Angle)(ref comp._localRotation)).RotateVec(ref vector) + comp._localPosition;
				continue;
			}
			base.Log.Warning($"Target entity ({ToPrettyString(relative)}) not in transform hierarchy while calling {"GetRelativePositionRotation"}.");
			TransformComponent component2 = Transform(relative);
			vector = Vector2.Transform(vector, GetInvWorldMatrix(component2));
			break;
		}
		return vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetWorldPosition(EntityUid uid, Vector2 worldPos)
	{
		TransformComponent component = XformQuery.GetComponent(uid);
		SetWorldPosition((Owner: uid, Comp: component), worldPos);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use overload that takes Entity<T> instead")]
	public void SetWorldPosition(TransformComponent component, Vector2 worldPos)
	{
		SetWorldPosition((Owner: component.Owner, Comp: component), worldPos);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetWorldPosition(Entity<TransformComponent> entity, Vector2 worldPos)
	{
		SetWorldPositionRotationInternal(entity.Owner, worldPos, null, entity.Comp);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Angle GetWorldRotation(EntityUid uid)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return GetWorldRotation(XformQuery.GetComponent(uid), XformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Angle GetWorldRotation(TransformComponent component)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return GetWorldRotation(component, XformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Angle GetWorldRotation(EntityUid uid, EntityQuery<TransformComponent> xformQuery)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return GetWorldRotation(xformQuery.GetComponent(uid), xformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Angle GetWorldRotation(TransformComponent component, EntityQuery<TransformComponent> xformQuery)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Angle val = component._localRotation;
		while (true)
		{
			EntityUid parentUid = component.ParentUid;
			EntityUid? mapUid = component.MapUid;
			if (!(parentUid != mapUid) || !component.ParentUid.IsValid())
			{
				break;
			}
			component = xformQuery.GetComponent(component.ParentUid);
			val += component._localRotation;
		}
		return val;
	}

	public void SetWorldRotationNoLerp(Entity<TransformComponent?> entity, Angle angle)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (XformQuery.Resolve(entity.Owner, ref entity.Comp))
		{
			Angle worldRotation = GetWorldRotation(entity.Comp);
			Angle val = angle - worldRotation;
			SetLocalRotationNoLerp(entity, entity.Comp.LocalRotation + val);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetWorldRotation(EntityUid uid, Angle angle)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent component = Transform(uid);
		SetWorldRotation(component, angle);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetWorldRotation(TransformComponent component, Angle angle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Angle worldRotation = GetWorldRotation(component);
		Angle val = angle - worldRotation;
		SetLocalRotation(component, component.LocalRotation + val);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetWorldRotation(EntityUid uid, Angle angle, EntityQuery<TransformComponent> xformQuery)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		SetWorldRotation(xformQuery.GetComponent(uid), angle, xformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetWorldRotation(TransformComponent component, Angle angle, EntityQuery<TransformComponent> xformQuery)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Angle worldRotation = GetWorldRotation(component, xformQuery);
		Angle val = angle - worldRotation;
		SetLocalRotation(component, component.LocalRotation + val);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetWorldPositionRotation(EntityUid uid, Vector2 worldPos, Angle worldRot, TransformComponent? component = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		SetWorldPositionRotationInternal(uid, worldPos, worldRot, component);
	}

	private void SetWorldPositionRotationInternal(EntityUid uid, Vector2 worldPos, Angle? worldRot = null, TransformComponent? component = null)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (XformQuery.Resolve(uid, ref component) && component._parent.IsValid() && component.MapUid.HasValue)
		{
			if (component.GridUid != uid && _mapManager.TryFindGridAt(component.MapUid.Value, worldPos, out EntityUid uid2, out MapGridComponent _))
			{
				TransformComponent component2 = XformQuery.GetComponent(uid2);
				Matrix3x2 invLocalMatrix = component2.InvLocalMatrix;
				Angle localRotation = component2.LocalRotation;
				Angle? val = worldRot;
				Angle val2 = localRotation;
				Angle? rotation = (val.HasValue ? new Angle?(val.GetValueOrDefault() - val2) : ((Angle?)null));
				SetCoordinates(uid, component, new EntityCoordinates(uid2, Vector2.Transform(worldPos, invLocalMatrix)), rotation);
			}
			else
			{
				SetCoordinates(uid, component, new EntityCoordinates(component.MapUid.Value, worldPos), worldRot);
			}
		}
	}

	[Obsolete("Use override with EntityUid")]
	public void SetLocalPositionRotation(TransformComponent xform, Vector2 pos, Angle rot)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SetLocalPositionRotation(xform.Owner, pos, rot, xform);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void SetLocalPositionRotation(EntityUid uid, Vector2 pos, Angle rot, TransformComponent? xform = null)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (!XformQuery.Resolve(uid, ref xform) || !xform._parent.IsValid())
		{
			return;
		}
		if (Vector2Helpers.EqualsApprox(xform._localPosition, pos))
		{
			Angle localRotation = xform.LocalRotation;
			if (((Angle)(ref localRotation)).EqualsApprox(rot))
			{
				return;
			}
		}
		EntityUid parent = xform._parent;
		Vector2 localPosition = xform._localPosition;
		Angle localRotation2 = xform.LocalRotation;
		if (!xform.Anchored)
		{
			xform._localPosition = pos;
		}
		if (!xform.NoLocalRotation)
		{
			xform._localRotation = rot;
		}
		MetaDataComponent metaDataComponent = MetaData(uid);
		Dirty(uid, xform, metaDataComponent);
		xform.MatricesDirty = true;
		if (xform.Initialized)
		{
			RaiseMoveEvent((Owner: uid, Comp1: xform, Comp2: metaDataComponent), parent, localPosition, localRotation2, xform.MapUid, !localPosition.Equals(pos));
		}
	}

	public Matrix3x2 GetInvWorldMatrix(EntityUid uid)
	{
		return GetInvWorldMatrix(XformQuery.GetComponent(uid), XformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Matrix3x2 GetInvWorldMatrix(TransformComponent component)
	{
		return GetInvWorldMatrix(component, XformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Matrix3x2 GetInvWorldMatrix(EntityUid uid, EntityQuery<TransformComponent> xformQuery)
	{
		return GetInvWorldMatrix(xformQuery.GetComponent(uid), xformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Matrix3x2 GetInvWorldMatrix(TransformComponent component, EntityQuery<TransformComponent> xformQuery)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		var (vector, val) = GetWorldPositionRotation(component, xformQuery);
		return Matrix3Helpers.CreateInverseTransform(ref vector, ref val);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) GetWorldPositionRotationMatrix(EntityUid uid)
	{
		return GetWorldPositionRotationMatrix(XformQuery.GetComponent(uid), XformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) GetWorldPositionRotationMatrix(TransformComponent xform)
	{
		return GetWorldPositionRotationMatrix(xform, XformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) GetWorldPositionRotationMatrix(EntityUid uid, EntityQuery<TransformComponent> xforms)
	{
		return GetWorldPositionRotationMatrix(xforms.GetComponent(uid), xforms);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) GetWorldPositionRotationMatrix(TransformComponent component, EntityQuery<TransformComponent> xforms)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		var (item, item2) = GetWorldPositionRotation(component, xforms);
		return (WorldPosition: item, WorldRotation: item2, WorldMatrix: Matrix3Helpers.CreateTransform(ref item, ref item2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationInvMatrix(EntityUid uid)
	{
		return GetWorldPositionRotationInvMatrix(XformQuery.GetComponent(uid));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationInvMatrix(TransformComponent xform)
	{
		return GetWorldPositionRotationInvMatrix(xform, XformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationInvMatrix(EntityUid uid, EntityQuery<TransformComponent> xforms)
	{
		return GetWorldPositionRotationInvMatrix(xforms.GetComponent(uid), xforms);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationInvMatrix(TransformComponent component, EntityQuery<TransformComponent> xforms)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		var (item, item2) = GetWorldPositionRotation(component, xforms);
		return (WorldPosition: item, WorldRotation: item2, InvWorldMatrix: Matrix3Helpers.CreateInverseTransform(ref item, ref item2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationMatrixWithInv(EntityUid uid)
	{
		return GetWorldPositionRotationMatrixWithInv(XformQuery.GetComponent(uid), XformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationMatrixWithInv(TransformComponent xform)
	{
		return GetWorldPositionRotationMatrixWithInv(xform, XformQuery);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationMatrixWithInv(EntityUid uid, EntityQuery<TransformComponent> xforms)
	{
		return GetWorldPositionRotationMatrixWithInv(xforms.GetComponent(uid), xforms);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationMatrixWithInv(TransformComponent component, EntityQuery<TransformComponent> xforms)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		var (item, item2) = GetWorldPositionRotation(component, xforms);
		return (WorldPosition: item, WorldRotation: item2, WorldMatrix: Matrix3Helpers.CreateTransform(ref item, ref item2), InvWorldMatrix: Matrix3Helpers.CreateInverseTransform(ref item, ref item2));
	}

	public void AttachToGridOrMap(EntityUid uid, TransformComponent? xform = null)
	{
		if (TerminatingOrDeleted(uid) || !XformQuery.Resolve(uid, ref xform, logMissing: false) || !xform.ParentUid.IsValid())
		{
			return;
		}
		EntityUid parentUid = xform.ParentUid;
		EntityUid? gridUid = xform.GridUid;
		if (parentUid == gridUid)
		{
			return;
		}
		if (!TryGetMapOrGridCoordinates(uid, out var coordinates, xform))
		{
			if (!_mapQuery.HasComp(uid))
			{
				base.Log.Warning($"Failed to attach entity to map or grid. Entity: ({ToPrettyString(uid)}). Trace: {Environment.StackTrace}");
			}
			DetachEntity(uid, xform);
		}
		else if (!(coordinates.Value.EntityId == xform.ParentUid) && !(coordinates.Value.EntityId == uid))
		{
			SetCoordinates(uid, xform, coordinates.Value);
		}
	}

	public bool TryGetMapOrGridCoordinates(EntityUid uid, [NotNullWhen(true)] out EntityCoordinates? coordinates, TransformComponent? xform = null)
	{
		coordinates = null;
		if (!XformQuery.Resolve(uid, ref xform, logMissing: false))
		{
			return false;
		}
		if (!xform.ParentUid.IsValid())
		{
			return false;
		}
		EntityUid? mapUid = xform.MapUid;
		if (mapUid.HasValue)
		{
			EntityUid valueOrDefault = mapUid.GetValueOrDefault();
			if (!TerminatingOrDeleted(valueOrDefault))
			{
				Vector2 worldPosition = GetWorldPosition(xform);
				if (_mapManager.TryFindGridAt(valueOrDefault, worldPosition, out EntityUid uid2, out MapGridComponent _) && !TerminatingOrDeleted(uid2))
				{
					coordinates = ((uid2 == xform.ParentUid) ? new EntityCoordinates(uid2, xform.LocalPosition) : new EntityCoordinates(uid2, Vector2.Transform(worldPosition, GetInvWorldMatrix(uid2))));
				}
				else
				{
					coordinates = new EntityCoordinates(valueOrDefault, worldPosition);
				}
				return true;
			}
		}
		return false;
	}

	[Obsolete("Use DetachEntity")]
	public void DetachParentToNull(EntityUid uid, TransformComponent xform)
	{
		DetachEntity(uid, xform);
	}

	public void DetachEntity(EntityUid uid, TransformComponent? xform = null)
	{
		if (XformQuery.Resolve(uid, ref xform))
		{
			XformQuery.TryGetComponent(xform.ParentUid, out TransformComponent component);
			DetachEntity(uid, xform, MetaData(uid), component);
		}
	}

	public void DetachEntity(Entity<TransformComponent?> ent)
	{
		if (XformQuery.Resolve(ent.Owner, ref ent.Comp))
		{
			XformQuery.TryGetComponent(ent.Comp.ParentUid, out TransformComponent component);
			DetachEntity(ent.Owner, ent.Comp, MetaData(ent.Owner), component);
		}
	}

	public void DetachEntity(EntityUid uid, TransformComponent xform, MetaDataComponent meta, TransformComponent? oldXform, bool terminating = false)
	{
		try
		{
			DetachEntityInternal(uid, xform, meta, oldXform, terminating);
		}
		catch (Exception value)
		{
			base.Log.Error($"Caught exception while attempting to detach an entity to nullspace. Entity: {ToPrettyString(uid, meta)}. Exception: {value}");
		}
	}

	internal void DetachEntityInternal(EntityUid uid, TransformComponent xform, MetaDataComponent meta, TransformComponent? oldXform, bool terminating = false)
	{
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		if (!terminating && (int)meta.EntityLifeStage >= 4)
		{
			base.Log.Error($"Attempting to detach a terminating entity: {ToPrettyString(uid, meta)}. Trace: {Environment.StackTrace}");
			return;
		}
		EntityUid parent = xform._parent;
		if (parent.IsValid())
		{
			_lookup.RemoveFromEntityTree(uid, xform);
			xform.NextPosition = null;
			xform.NextRotation = null;
			xform.LerpParent = EntityUid.Invalid;
			if (xform.Anchored && _metaQuery.TryGetComponent(xform.GridUid, out MetaDataComponent component) && (int)component.EntityLifeStage <= 3)
			{
				MapGridComponent grid = Comp<MapGridComponent>(xform.GridUid.Value);
				Vector2i pos = _map.TileIndicesFor(xform.GridUid.Value, grid, xform.Coordinates);
				_map.RemoveFromSnapGridCell(xform.GridUid.Value, grid, pos, uid);
				xform._anchored = false;
				AnchorStateChangedEvent args = new AnchorStateChangedEvent(uid, xform, detaching: true);
				RaiseLocalEvent(uid, ref args, broadcast: true);
			}
			SetCoordinates((Owner: uid, Comp1: xform, Comp2: meta), default(EntityCoordinates), Angle.Zero, unanchor: true, null, oldXform);
		}
	}

	private void OnGridAdd(EntityUid uid, TransformComponent component, GridAddEvent args)
	{
		MetaDataComponent metaDataComponent = MetaData(uid);
		if ((int)metaDataComponent.EntityLifeStage > 2)
		{
			SetGridId((Owner: uid, Comp1: component, Comp2: metaDataComponent), uid);
			return;
		}
		component._gridInitialized = true;
		component._gridUid = uid;
	}

	public void DropNextTo(Entity<TransformComponent?> entity, Entity<TransformComponent?> target)
	{
		TransformComponent component = entity.Comp;
		if (!XformQuery.Resolve(entity, ref component))
		{
			return;
		}
		TransformComponent component2 = target.Comp;
		if (!XformQuery.Resolve(target, ref component2) || !component2.ParentUid.IsValid())
		{
			DetachEntity(entity, component);
			return;
		}
		EntityCoordinates coordinates = component2.Coordinates;
		EntityUid entityUid = target.Owner;
		while (component2.ParentUid.IsValid())
		{
			if (_container.IsEntityInContainer(entityUid) && _container.TryGetContainingContainer(component2.ParentUid, entityUid, out BaseContainer container) && _container.Insert((Owner: (EntityUid)entity, Comp1: component, Comp2: (MetaDataComponent)null, Comp3: (PhysicsComponent)null), container))
			{
				return;
			}
			entityUid = component2.ParentUid;
			component2 = XformQuery.GetComponent(entityUid);
		}
		SetCoordinates(entity, component, coordinates);
		AttachToGridOrMap(entity, component);
	}

	public void PlaceNextTo(Entity<TransformComponent?> entity, Entity<TransformComponent?> target)
	{
		TransformComponent component = entity.Comp;
		if (!XformQuery.Resolve(entity, ref component))
		{
			return;
		}
		TransformComponent component2 = target.Comp;
		if (!XformQuery.Resolve(target, ref component2) || !component2.ParentUid.IsValid())
		{
			DetachEntity(entity, component);
			return;
		}
		if (!_container.IsEntityInContainer(target))
		{
			SetCoordinates(entity, component, component2.Coordinates);
			return;
		}
		foreach (BaseContainer value in Comp<ContainerManagerComponent>(component2.ParentUid).Containers.Values)
		{
			if (value.Contains(target) && !_container.Insert((Owner: (EntityUid)entity, Comp1: component, Comp2: (MetaDataComponent)null, Comp3: (PhysicsComponent)null), value))
			{
				PlaceNextTo((Owner: entity, Comp: component), component2.ParentUid);
			}
		}
	}

	public bool SwapPositions(Entity<TransformComponent?> entity1, Entity<TransformComponent?> entity2)
	{
		if (!XformQuery.Resolve(entity1, ref entity1.Comp) || !XformQuery.Resolve(entity2, ref entity2.Comp))
		{
			return false;
		}
		if (entity1 == entity2)
		{
			return true;
		}
		if (IsParentOf(entity1.Comp, entity2) || IsParentOf(entity2.Comp, entity1))
		{
			return false;
		}
		MapCoordinates? mapCoordinates = null;
		MapCoordinates? mapCoordinates2 = null;
		if (_container.TryGetContainingContainer(entity1, out BaseContainer container))
		{
			_container.Remove(entity1, container, reparent: true, force: true);
		}
		else
		{
			mapCoordinates = GetMapCoordinates(entity1.Comp);
		}
		if (_container.TryGetContainingContainer(entity2, out BaseContainer container2))
		{
			_container.Remove(entity2, container2, reparent: true, force: true);
		}
		else
		{
			mapCoordinates2 = GetMapCoordinates(entity2.Comp);
		}
		if (container != null && container.Owner == entity2.Owner)
		{
			return false;
		}
		if (container2 != null && container2.Owner == entity1.Owner)
		{
			return false;
		}
		MapGridComponent grid;
		if (container2 != null)
		{
			_container.Insert(entity1, container2);
		}
		else
		{
			if (!mapCoordinates2.HasValue)
			{
				throw new InvalidOperationException();
			}
			EntityUid mapOrInvalid = _map.GetMapOrInvalid(mapCoordinates2.Value.MapId);
			if (!_gridQuery.HasComponent(entity1) && _mapManager.TryFindGridAt(mapOrInvalid, mapCoordinates2.Value.Position, out EntityUid uid, out grid))
			{
				Matrix3x2 invWorldMatrix = GetInvWorldMatrix(uid);
				SetCoordinates((EntityUid)entity1, new EntityCoordinates(uid, Vector2.Transform(mapCoordinates2.Value.Position, invWorldMatrix)));
			}
			else
			{
				SetCoordinates((EntityUid)entity1, new EntityCoordinates(mapOrInvalid, mapCoordinates2.Value.Position));
			}
		}
		if (container != null)
		{
			_container.Insert(entity2, container);
		}
		else
		{
			if (!mapCoordinates.HasValue)
			{
				throw new InvalidOperationException();
			}
			EntityUid mapOrInvalid2 = _map.GetMapOrInvalid(mapCoordinates.Value.MapId);
			if (!_gridQuery.HasComponent(entity1) && _mapManager.TryFindGridAt(mapOrInvalid2, mapCoordinates.Value.Position, out EntityUid uid2, out grid))
			{
				Matrix3x2 invWorldMatrix2 = GetInvWorldMatrix(uid2);
				SetCoordinates((EntityUid)entity2, new EntityCoordinates(uid2, Vector2.Transform(mapCoordinates.Value.Position, invWorldMatrix2)));
			}
			else
			{
				SetCoordinates((EntityUid)entity2, new EntityCoordinates(mapOrInvalid2, mapCoordinates.Value.Position));
			}
		}
		return true;
	}

	public bool IsValid(EntityCoordinates coordinates)
	{
		EntityUid entityId = coordinates.EntityId;
		if (!entityId.IsValid() || !Exists(entityId))
		{
			return false;
		}
		if (!float.IsFinite(coordinates.Position.X) || !float.IsFinite(coordinates.Position.Y))
		{
			return false;
		}
		return true;
	}

	public EntityCoordinates WithEntityId(EntityCoordinates coordinates, EntityUid entity)
	{
		if (!(entity == coordinates.EntityId))
		{
			return ToCoordinates(entity, ToMapCoordinates(coordinates));
		}
		return coordinates;
	}

	public MapCoordinates ToMapCoordinates(EntityCoordinates coordinates, bool logError = true)
	{
		if (!TryComp(coordinates.EntityId, out TransformComponent comp))
		{
			if (logError)
			{
				base.Log.Error($"Attempted to convert coordinates with invalid entity: {coordinates}. Trace: {Environment.StackTrace}");
			}
			return MapCoordinates.Nullspace;
		}
		Vector2 position = ((Angle)(ref comp._localRotation)).RotateVec(ref coordinates.Position) + comp._localPosition;
		while (true)
		{
			EntityUid parentUid = comp.ParentUid;
			EntityUid? mapUid = comp.MapUid;
			if (!(parentUid != mapUid) || !comp.ParentUid.IsValid())
			{
				break;
			}
			comp = XformQuery.GetComponent(comp.ParentUid);
			position = ((Angle)(ref comp._localRotation)).RotateVec(ref position) + comp._localPosition;
		}
		return new MapCoordinates(position, comp.MapID);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MapCoordinates ToMapCoordinates(NetCoordinates coordinates)
	{
		EntityCoordinates coordinates2 = GetCoordinates(coordinates);
		return ToMapCoordinates(coordinates2);
	}

	public Vector2 ToWorldPosition(EntityCoordinates coordinates, bool logError = true)
	{
		if (!TryComp(coordinates.EntityId, out TransformComponent comp))
		{
			if (logError)
			{
				base.Log.Error($"Attempted to convert coordinates with invalid entity: {coordinates}. Trace: {Environment.StackTrace}");
			}
			return Vector2.Zero;
		}
		Vector2 result = ((Angle)(ref comp._localRotation)).RotateVec(ref coordinates.Position) + comp._localPosition;
		while (true)
		{
			EntityUid parentUid = comp.ParentUid;
			EntityUid? mapUid = comp.MapUid;
			if (!(parentUid != mapUid) || !comp.ParentUid.IsValid())
			{
				break;
			}
			comp = XformQuery.GetComponent(comp.ParentUid);
			result = ((Angle)(ref comp._localRotation)).RotateVec(ref result) + comp._localPosition;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2 ToWorldPosition(NetCoordinates coordinates)
	{
		EntityCoordinates coordinates2 = GetCoordinates(coordinates);
		return ToWorldPosition(coordinates2);
	}

	public EntityCoordinates ToCoordinates(Entity<TransformComponent?> entity, MapCoordinates coordinates)
	{
		if (!Resolve(entity, ref entity.Comp, logMissing: false))
		{
			base.Log.Error($"Attempted to convert coordinates with invalid entity: {coordinates}. Trace: {Environment.StackTrace}");
			return default(EntityCoordinates);
		}
		if (entity.Comp.MapID != coordinates.MapId)
		{
			base.Log.Error($"Attempted to convert map coordinates {coordinates} to entity coordinates on a different map. Entity: {ToPrettyString(entity)}. Trace: {Environment.StackTrace}");
			return default(EntityCoordinates);
		}
		Vector2 position = Vector2.Transform(coordinates.Position, GetInvWorldMatrix(entity.Comp));
		return new EntityCoordinates(entity, position);
	}

	public EntityCoordinates ToCoordinates(MapCoordinates coordinates)
	{
		if (_map.TryGetMap(coordinates.MapId, out var uid))
		{
			return ToCoordinates(uid.Value, coordinates);
		}
		base.Log.Error($"Attempted to convert map coordinates with unknown map id: {coordinates}. Trace: {Environment.StackTrace}");
		return default(EntityCoordinates);
	}

	public EntityUid? GetGrid(EntityCoordinates coordinates)
	{
		return GetGrid(coordinates.EntityId);
	}

	public EntityUid? GetGrid(Entity<TransformComponent?> entity)
	{
		if (Resolve(entity, ref entity.Comp, logMissing: false))
		{
			return entity.Comp.GridUid;
		}
		return null;
	}

	public MapId GetMapId(EntityCoordinates coordinates)
	{
		return GetMapId(coordinates.EntityId);
	}

	public MapId GetMapId(Entity<TransformComponent?> entity)
	{
		if (Resolve(entity, ref entity.Comp, logMissing: false))
		{
			return entity.Comp.MapID;
		}
		return MapId.Nullspace;
	}

	public EntityUid? GetMap(EntityCoordinates coordinates)
	{
		return GetMap(coordinates.EntityId);
	}

	public EntityUid? GetMap(Entity<TransformComponent?> entity)
	{
		if (Resolve(entity, ref entity.Comp, logMissing: false))
		{
			return entity.Comp.MapUid;
		}
		return null;
	}

	public bool InRange(EntityCoordinates coordA, EntityCoordinates coordB, float range)
	{
		if (!coordA.EntityId.IsValid() || !coordB.EntityId.IsValid())
		{
			return false;
		}
		if (coordA.EntityId == coordB.EntityId)
		{
			return (coordA.Position - coordB.Position).LengthSquared() < range * range;
		}
		MapCoordinates mapCoordinates = ToMapCoordinates(coordA, logError: false);
		MapCoordinates otherCoords = ToMapCoordinates(coordB, logError: false);
		if (mapCoordinates.MapId != otherCoords.MapId || mapCoordinates.MapId == MapId.Nullspace)
		{
			return false;
		}
		return mapCoordinates.InRange(otherCoords, range);
	}

	public bool InRange(Entity<TransformComponent?> entA, Entity<TransformComponent?> entB, float range)
	{
		if (!Resolve(entA, ref entA.Comp, logMissing: false))
		{
			return false;
		}
		if (!Resolve(entB, ref entB.Comp, logMissing: false))
		{
			return false;
		}
		if (!entA.Comp.ParentUid.IsValid() || !entB.Comp.ParentUid.IsValid())
		{
			return false;
		}
		if (entA.Comp.ParentUid == entB.Comp.ParentUid)
		{
			return (entA.Comp.LocalPosition - entB.Comp.LocalPosition).LengthSquared() < range * range;
		}
		if (entA.Comp.ParentUid == entB.Owner)
		{
			return entA.Comp.LocalPosition.LengthSquared() < range * range;
		}
		if (entB.Comp.ParentUid == entA.Owner)
		{
			return entB.Comp.LocalPosition.LengthSquared() < range * range;
		}
		MapCoordinates mapCoordinates = GetMapCoordinates(entA);
		MapCoordinates mapCoordinates2 = GetMapCoordinates(entB);
		if (mapCoordinates.MapId != mapCoordinates2.MapId || mapCoordinates.MapId == MapId.Nullspace)
		{
			return false;
		}
		return mapCoordinates.InRange(mapCoordinates2, range);
	}

	public override void Initialize()
	{
		base.Initialize();
		base.UpdatesOutsidePrediction = true;
		_mapQuery = GetEntityQuery<MapComponent>();
		_gridQuery = GetEntityQuery<MapGridComponent>();
		_metaQuery = GetEntityQuery<MetaDataComponent>();
		XformQuery = GetEntityQuery<TransformComponent>();
		SubscribeLocalEvent<TileChangedEvent>(MapManagerOnTileChanged);
		SubscribeLocalEvent<TransformComponent, ComponentInit>(OnCompInit);
		SubscribeLocalEvent<TransformComponent, ComponentStartup>(OnCompStartup);
		SubscribeLocalEvent<TransformComponent, ComponentGetState>(OnGetState);
		SubscribeLocalEvent<TransformComponent, ComponentHandleState>(OnHandleState);
		SubscribeLocalEvent<TransformComponent, GridAddEvent>(OnGridAdd);
	}

	private void MapManagerOnTileChanged(ref TileChangedEvent e)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		TileChangedEntry[] changes = e.Changes;
		for (int i = 0; i < changes.Length; i++)
		{
			TileChangedEntry tileChangedEntry = changes[i];
			if (!(tileChangedEntry.NewTile != Tile.Empty))
			{
				DeparentAllEntsOnTile(e.Entity, tileChangedEntry.GridIndices);
			}
		}
	}

	private void DeparentAllEntsOnTile(EntityUid gridId, Vector2i tileIndices)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!TryComp(gridId, out BroadphaseComponent comp) || !TryComp(gridId, out MapGridComponent comp2) || !XformQuery.TryGetComponent(gridId, out TransformComponent component) || !XformQuery.TryGetComponent(component.MapUid, out TransformComponent component2))
		{
			return;
		}
		Box2 localBounds = _lookup.GetLocalBounds(tileIndices, comp2.TileSize);
		foreach (EntityUid item in _lookup.GetLocalEntitiesIntersecting(comp, localBounds, LookupFlags.Uncontained | LookupFlags.Approximate))
		{
			if (XformQuery.TryGetComponent(item, out TransformComponent component3) && !(component3.ParentUid != gridId) && ((Box2)(ref localBounds)).Contains(component3.LocalPosition, true))
			{
				if (EntityManager.IsQueuedForDeletion(item))
				{
					DetachEntity(item, component3, MetaData(item), component);
				}
				else
				{
					SetParent(item, component3, component.MapUid.Value, component2);
				}
			}
		}
	}

	public EntityCoordinates GetMoverCoordinates(EntityUid uid)
	{
		return GetMoverCoordinates(uid, XformQuery.GetComponent(uid));
	}

	public EntityCoordinates GetMoverCoordinates(EntityUid uid, TransformComponent xform)
	{
		if (!xform.ParentUid.IsValid())
		{
			return xform.Coordinates;
		}
		if (!xform._gridInitialized)
		{
			InitializeGridUid(uid, xform);
		}
		if (xform.GridUid == xform.ParentUid)
		{
			return xform.Coordinates;
		}
		Vector2 worldPosition = GetWorldPosition(xform, XformQuery);
		if (xform.GridUid.HasValue)
		{
			return new EntityCoordinates(xform.GridUid.Value, Vector2.Transform(worldPosition, XformQuery.GetComponent(xform.GridUid.Value).InvLocalMatrix));
		}
		return new EntityCoordinates(xform.MapUid ?? xform.ParentUid, worldPosition);
	}

	public EntityCoordinates GetMoverCoordinates(EntityCoordinates coordinates, EntityQuery<TransformComponent> xformQuery)
	{
		return GetMoverCoordinates(coordinates);
	}

	public EntityCoordinates GetMoverCoordinates(EntityCoordinates coordinates)
	{
		EntityUid entityId = coordinates.EntityId;
		if (!entityId.IsValid())
		{
			return coordinates;
		}
		TransformComponent component = XformQuery.GetComponent(entityId);
		if (!component._gridInitialized)
		{
			InitializeGridUid(entityId, component);
		}
		if (component.GridUid == entityId)
		{
			return coordinates;
		}
		EntityUid? mapUid = component.MapUid;
		if (mapUid == entityId)
		{
			return coordinates;
		}
		Vector2 position = Vector2.Transform(coordinates.Position, GetWorldMatrix(component, XformQuery));
		if (component.GridUid.HasValue)
		{
			return new EntityCoordinates(component.GridUid.Value, Vector2.Transform(position, XformQuery.GetComponent(component.GridUid.Value).InvLocalMatrix));
		}
		return new EntityCoordinates(mapUid ?? entityId, position);
	}

	public (EntityCoordinates Coords, Angle worldRot) GetMoverCoordinateRotation(EntityUid uid, TransformComponent xform)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if (!xform.ParentUid.IsValid())
		{
			return (Coords: xform.Coordinates, worldRot: xform.LocalRotation);
		}
		if (!xform._gridInitialized)
		{
			InitializeGridUid(uid, xform);
		}
		if (xform.GridUid == xform.ParentUid)
		{
			return (Coords: xform.Coordinates, worldRot: GetWorldRotation(xform, XformQuery));
		}
		var (position, item) = GetWorldPositionRotation(xform, XformQuery);
		return (Coords: (!xform.GridUid.HasValue) ? new EntityCoordinates(xform.MapUid ?? xform.ParentUid, position) : new EntityCoordinates(xform.GridUid.Value, Vector2.Transform(position, XformQuery.GetComponent(xform.GridUid.Value).InvLocalMatrix)), worldRot: item);
	}

	public Vector2i GetGridOrMapTilePosition(EntityUid uid, TransformComponent? xform = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!Resolve(uid, ref xform, logMissing: false))
		{
			return Vector2i.Zero;
		}
		if (!xform.GridUid.HasValue)
		{
			return Vector2Helpers.Floored(GetWorldPosition(xform));
		}
		return _map.CoordinatesToTile(xform.GridUid.Value, Comp<MapGridComponent>(xform.GridUid.Value), xform.Coordinates);
	}

	public Vector2i GetGridTilePositionOrDefault(Entity<TransformComponent?> entity, MapGridComponent? grid = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent component = entity.Comp;
		if (!Resolve(entity.Owner, ref component) || !component.GridUid.HasValue)
		{
			return Vector2i.Zero;
		}
		if (!Resolve(component.GridUid.Value, ref grid))
		{
			return Vector2i.Zero;
		}
		return _map.CoordinatesToTile(component.GridUid.Value, grid, component.Coordinates);
	}

	public bool TryGetGridTilePosition(Entity<TransformComponent?> entity, out Vector2i indices, MapGridComponent? grid = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		indices = default(Vector2i);
		TransformComponent component = entity.Comp;
		if (!Resolve(entity.Owner, ref component) || !component.GridUid.HasValue)
		{
			return false;
		}
		if (!Resolve(component.GridUid.Value, ref grid))
		{
			return false;
		}
		indices = _map.CoordinatesToTile(component.GridUid.Value, grid, component.Coordinates);
		return true;
	}

	internal void RaiseMoveEvent(Entity<TransformComponent, MetaDataComponent> ent, EntityUid oldParent, Vector2 oldPosition, Angle oldRotation, EntityUid? oldMap, bool checkTraversal = true)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates newPos = ((ent.Comp1._parent == EntityUid.Invalid) ? default(EntityCoordinates) : new EntityCoordinates(ent.Comp1._parent, ent.Comp1._localPosition));
		EntityCoordinates oldPos = ((oldParent == EntityUid.Invalid) ? default(EntityCoordinates) : new EntityCoordinates(oldParent, oldPosition));
		MoveEvent ev = new MoveEvent(ent, oldPos, newPos, oldRotation, ent.Comp1._localRotation);
		if (oldParent != ent.Comp1._parent)
		{
			_physics.OnParentChange(ent, oldParent, oldMap);
			this.OnBeforeMoveEvent?.Invoke(ref ev);
			EntParentChangedMessage args = new EntParentChangedMessage(ev.Sender, oldParent, oldMap, ev.Component);
			RaiseLocalEvent(ev.Sender, ref args, broadcast: true);
		}
		else
		{
			this.OnBeforeMoveEvent?.Invoke(ref ev);
		}
		RaiseLocalEvent(ev.Sender, ref ev);
		this.OnGlobalMoveEvent?.Invoke(ref ev);
		if (checkTraversal)
		{
			_traversal.CheckTraverse(ent);
		}
	}
}

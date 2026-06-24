using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.IconSmoothing;
using Content.Shared.IconSmoothing;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;

namespace Content.Client.IconSmoothing;

public sealed class IconSmoothSystem : EntitySystem
{
	[Flags]
	private enum CardinalConnectDirs : byte
	{
		None = 0,
		North = 1,
		South = 2,
		East = 4,
		West = 8
	}

	[Flags]
	private enum CornerFill : byte
	{
		None = 0,
		CounterClockwise = 1,
		Diagonal = 2,
		Clockwise = 4
	}

	private enum CornerLayers : byte
	{
		SE,
		NE,
		NW,
		SW
	}

	private enum EdgeLayer : byte
	{
		South,
		East,
		North,
		West
	}

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private SpriteSystem _sprite;

	private readonly Queue<EntityUid> _dirtyEntities = new Queue<EntityUid>();

	private readonly Queue<EntityUid> _anchorChangedEntities = new Queue<EntityUid>();

	private int _generation;

	private EntityQuery<CMIconSmoothComponent> _cmIconSmoothQuery;

	public void SetEnabled(EntityUid uid, bool value, IconSmoothComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<IconSmoothComponent>(uid, ref component, false) && value != component.Enabled)
		{
			component.Enabled = value;
			DirtyNeighbours(uid, component);
		}
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeEdge();
		((EntitySystem)this).SubscribeLocalEvent<IconSmoothComponent, AnchorStateChangedEvent>((ComponentEventRefHandler<IconSmoothComponent, AnchorStateChangedEvent>)OnAnchorChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IconSmoothComponent, ComponentShutdown>((ComponentEventHandler<IconSmoothComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IconSmoothComponent, ComponentStartup>((ComponentEventHandler<IconSmoothComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, IconSmoothComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent val = ((EntitySystem)this).Transform(uid);
		if (val.Anchored)
		{
			MapGridComponent val2 = default(MapGridComponent);
			component.LastPosition = (((EntitySystem)this).TryComp<MapGridComponent>(val.GridUid, ref val2) ? new(EntityUid?, Vector2i)?((val.GridUid.Value, _mapSystem.TileIndicesFor(val.GridUid.Value, val2, val.Coordinates))) : new(EntityUid?, Vector2i)?(((EntityUid?)null, new Vector2i(0, 0))));
			DirtyNeighbours(uid, component);
		}
		SpriteComponent val3 = default(SpriteComponent);
		if (component.Mode == IconSmoothingMode.Corners && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val3))
		{
			SetCornerLayers(Entity<SpriteComponent>.op_Implicit((uid, val3)), component);
			if (component.Shader != null)
			{
				val3.LayerSetShader((object)CornerLayers.SE, component.Shader);
				val3.LayerSetShader((object)CornerLayers.NE, component.Shader);
				val3.LayerSetShader((object)CornerLayers.NW, component.Shader);
				val3.LayerSetShader((object)CornerLayers.SW, component.Shader);
			}
		}
	}

	public void SetStateBase(EntityUid uid, IconSmoothComponent component, string newState)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			component.StateBase = newState;
			SetCornerLayers(Entity<SpriteComponent>.op_Implicit((uid, item)), component);
		}
	}

	private void SetCornerLayers(Entity<SpriteComponent?> sprite, IconSmoothComponent component)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		_sprite.LayerMapRemove(sprite, (Enum)CornerLayers.SE);
		_sprite.LayerMapRemove(sprite, (Enum)CornerLayers.NE);
		_sprite.LayerMapRemove(sprite, (Enum)CornerLayers.NW);
		_sprite.LayerMapRemove(sprite, (Enum)CornerLayers.SW);
		string text = component.StateBase + "0";
		_sprite.LayerMapSet(sprite, (Enum)CornerLayers.SE, _sprite.AddRsiLayer(sprite, StateId.op_Implicit(text), (RSI)null, (int?)null));
		_sprite.LayerSetDirOffset(sprite, (Enum)CornerLayers.SE, (DirectionOffset)0);
		_sprite.LayerMapSet(sprite, (Enum)CornerLayers.NE, _sprite.AddRsiLayer(sprite, StateId.op_Implicit(text), (RSI)null, (int?)null));
		_sprite.LayerSetDirOffset(sprite, (Enum)CornerLayers.NE, (DirectionOffset)2);
		_sprite.LayerMapSet(sprite, (Enum)CornerLayers.NW, _sprite.AddRsiLayer(sprite, StateId.op_Implicit(text), (RSI)null, (int?)null));
		_sprite.LayerSetDirOffset(sprite, (Enum)CornerLayers.NW, (DirectionOffset)3);
		_sprite.LayerMapSet(sprite, (Enum)CornerLayers.SW, _sprite.AddRsiLayer(sprite, StateId.op_Implicit(text), (RSI)null, (int?)null));
		_sprite.LayerSetDirOffset(sprite, (Enum)CornerLayers.SW, (DirectionOffset)1);
	}

	private void OnShutdown(EntityUid uid, IconSmoothComponent component, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_dirtyEntities.Enqueue(uid);
		DirtyNeighbours(uid, component);
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		EntityQuery<TransformComponent> entityQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		EntityQuery<IconSmoothComponent> entityQuery2 = ((EntitySystem)this).GetEntityQuery<IconSmoothComponent>();
		EntityUid result;
		TransformComponent val = default(TransformComponent);
		while (_anchorChangedEntities.TryDequeue(out result))
		{
			if (entityQuery.TryGetComponent(result, ref val) && !(val.MapID == MapId.Nullspace))
			{
				DirtyNeighbours(result, null, val, entityQuery2);
			}
		}
		if (_dirtyEntities.Count != 0)
		{
			_generation++;
			EntityQuery<SpriteComponent> entityQuery3 = ((EntitySystem)this).GetEntityQuery<SpriteComponent>();
			EntityUid result2;
			while (_dirtyEntities.TryDequeue(out result2))
			{
				CalculateNewSprite(result2, entityQuery3, entityQuery2, entityQuery);
			}
		}
	}

	public void DirtyNeighbours(EntityUid uid, IconSmoothComponent? comp = null, TransformComponent? transform = null, EntityQuery<IconSmoothComponent>? smoothQuery = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery<IconSmoothComponent> valueOrDefault = smoothQuery.GetValueOrDefault();
		if (!smoothQuery.HasValue)
		{
			valueOrDefault = ((EntitySystem)this).GetEntityQuery<IconSmoothComponent>();
			smoothQuery = valueOrDefault;
		}
		if (!smoothQuery.Value.Resolve(uid, ref comp, true) || !((Component)comp).Running)
		{
			return;
		}
		_dirtyEntities.Enqueue(uid);
		if (!((EntitySystem)this).Resolve(uid, ref transform, true))
		{
			return;
		}
		MapGridComponent val = default(MapGridComponent);
		EntityUid val2;
		Vector2i val3;
		if (transform.Anchored && ((EntitySystem)this).TryComp<MapGridComponent>(transform.GridUid, ref val))
		{
			val2 = transform.GridUid.Value;
			val3 = _mapSystem.CoordinatesToTile(transform.GridUid.Value, val, transform.Coordinates);
		}
		else
		{
			(EntityUid?, Vector2i)? lastPosition = comp.LastPosition;
			if (!lastPosition.HasValue)
			{
				return;
			}
			(EntityUid?, Vector2i) valueOrDefault2 = lastPosition.GetValueOrDefault();
			var (val4, _) = valueOrDefault2;
			if (!val4.HasValue)
			{
				return;
			}
			EntityUid valueOrDefault3 = val4.GetValueOrDefault();
			Vector2i item = valueOrDefault2.Item2;
			if (!((EntitySystem)this).TryComp<MapGridComponent>(valueOrDefault3, ref val))
			{
				return;
			}
			val2 = valueOrDefault3;
			val3 = item;
		}
		DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(val2, val, val3 + new Vector2i(1, 0)));
		DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(val2, val, val3 + new Vector2i(-1, 0)));
		DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(val2, val, val3 + new Vector2i(0, 1)));
		DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(val2, val, val3 + new Vector2i(0, -1)));
		IconSmoothingMode mode = comp.Mode;
		if ((mode == IconSmoothingMode.Corners || mode - 2 <= IconSmoothingMode.CardinalFlags) ? true : false)
		{
			DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(val2, val, val3 + new Vector2i(1, 1)));
			DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(val2, val, val3 + new Vector2i(-1, -1)));
			DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(val2, val, val3 + new Vector2i(-1, 1)));
			DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(val2, val, val3 + new Vector2i(1, -1)));
		}
	}

	private void DirtyEntities(AnchoredEntitiesEnumerator entities)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = default(EntityUid?);
		while (((AnchoredEntitiesEnumerator)(ref entities)).MoveNext(ref val))
		{
			_dirtyEntities.Enqueue(val.Value);
		}
	}

	private void OnAnchorChanged(EntityUid uid, IconSmoothComponent component, ref AnchorStateChangedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Detaching)
		{
			_anchorChangedEntities.Enqueue(uid);
		}
	}

	private void CalculateNewSprite(EntityUid uid, EntityQuery<SpriteComponent> spriteQuery, EntityQuery<IconSmoothComponent> smoothQuery, EntityQuery<TransformComponent> xformQuery, IconSmoothComponent? smooth = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		Entity<MapGridComponent>? gridEntity = null;
		TransformComponent val = default(TransformComponent);
		if (!smoothQuery.Resolve(uid, ref smooth, false) || smooth.Mode == IconSmoothingMode.NoSprite || smooth.UpdateGeneration == _generation || !smooth.Enabled || !((Component)smooth).Running)
		{
			SmoothEdgeComponent component = default(SmoothEdgeComponent);
			if (smooth == null || !smooth.Enabled || !((EntitySystem)this).TryComp<SmoothEdgeComponent>(uid, ref component) || !xformQuery.TryGetComponent(uid, ref val))
			{
				return;
			}
			DirectionFlag val2 = (DirectionFlag)0;
			MapGridComponent val3 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(val.GridUid, ref val3))
			{
				EntityUid value = val.GridUid.Value;
				Vector2i val4 = _mapSystem.TileIndicesFor(value, val3, val.Coordinates);
				gridEntity = Entity<MapGridComponent>.op_Implicit((value, val3));
				if (MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(value, val3, DirectionExtensions.Offset(val4, (Direction)4)), smoothQuery))
				{
					val2 = (DirectionFlag)(val2 | 4);
				}
				if (MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(value, val3, DirectionExtensions.Offset(val4, (Direction)0)), smoothQuery))
				{
					val2 = (DirectionFlag)(val2 | 1);
				}
				if (MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(value, val3, DirectionExtensions.Offset(val4, (Direction)2)), smoothQuery))
				{
					val2 = (DirectionFlag)(val2 | 2);
				}
				if (MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(value, val3, DirectionExtensions.Offset(val4, (Direction)6)), smoothQuery))
				{
					val2 = (DirectionFlag)(val2 | 8);
				}
			}
			CalculateEdge(uid, val2, null, component);
			return;
		}
		val = xformQuery.GetComponent(uid);
		smooth.UpdateGeneration = _generation;
		SpriteComponent item = default(SpriteComponent);
		if (!spriteQuery.TryGetComponent(uid, ref item))
		{
			((EntitySystem)this).Log.Error($"Encountered a icon-smoothing entity without a sprite: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
			((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)smooth);
			return;
		}
		(EntityUid, SpriteComponent) tuple = (uid, item);
		if (val.Anchored)
		{
			MapGridComponent item2 = default(MapGridComponent);
			if (!((EntitySystem)this).TryComp<MapGridComponent>(val.GridUid, ref item2))
			{
				((EntitySystem)this).Log.Error($"Failed to calculate IconSmoothComponent sprite in {uid} because grid {val.GridUid} was missing.");
				return;
			}
			gridEntity = Entity<MapGridComponent>.op_Implicit((val.GridUid.Value, item2));
		}
		switch (smooth.Mode)
		{
		case IconSmoothingMode.Corners:
			CalculateNewSpriteCorners(uid, gridEntity, smooth, Entity<SpriteComponent>.op_Implicit(tuple), val, smoothQuery);
			break;
		case IconSmoothingMode.CardinalFlags:
			CalculateNewSpriteCardinal(uid, gridEntity, smooth, Entity<SpriteComponent>.op_Implicit(tuple), val, smoothQuery);
			break;
		case IconSmoothingMode.Diagonal:
			CalculateNewSpriteDiagonal(uid, gridEntity, smooth, Entity<SpriteComponent>.op_Implicit(tuple), val, smoothQuery);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		IconSmoothingUpdatedEvent iconSmoothingUpdatedEvent = default(IconSmoothingUpdatedEvent);
		((EntitySystem)this).RaiseLocalEvent<IconSmoothingUpdatedEvent>(uid, ref iconSmoothingUpdatedEvent, false);
	}

	private void CalculateNewSpriteDiagonal(EntityUid uid, Entity<MapGridComponent>? gridEntity, IconSmoothComponent smooth, Entity<SpriteComponent> sprite, TransformComponent xform, EntityQuery<IconSmoothComponent> smoothQuery)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		if (!gridEntity.HasValue)
		{
			_sprite.LayerSetRsiState(sprite.AsNullable(), 0, StateId.op_Implicit(smooth.StateBase + "0"));
			return;
		}
		EntityUid owner = gridEntity.Value.Owner;
		MapGridComponent comp = gridEntity.Value.Comp;
		Vector2[] array = new Vector2[3]
		{
			new Vector2(1f, 0f),
			new Vector2(1f, -1f),
			new Vector2(0f, -1f)
		};
		Vector2i val = _mapSystem.TileIndicesFor(owner, comp, xform.Coordinates);
		Angle localRotation = xform.LocalRotation;
		bool flag = true;
		for (int i = 0; i < array.Length; i++)
		{
			Vector2i val2 = (Vector2i)((Angle)(ref localRotation)).RotateVec(ref array[i]);
			flag = flag && MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, val + val2), smoothQuery);
		}
		if (flag)
		{
			_sprite.LayerSetRsiState(sprite.AsNullable(), 0, StateId.op_Implicit(smooth.StateBase + "1"));
		}
		else
		{
			_sprite.LayerSetRsiState(sprite.AsNullable(), 0, StateId.op_Implicit(smooth.StateBase + "0"));
		}
	}

	private void CalculateNewSpriteCardinal(EntityUid uid, Entity<MapGridComponent>? gridEntity, IconSmoothComponent smooth, Entity<SpriteComponent> sprite, TransformComponent xform, EntityQuery<IconSmoothComponent> smoothQuery)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		CardinalConnectDirs cardinalConnectDirs = CardinalConnectDirs.None;
		if (!gridEntity.HasValue)
		{
			_sprite.LayerSetRsiState(sprite.AsNullable(), 0, StateId.op_Implicit($"{smooth.StateBase}{(int)cardinalConnectDirs}"));
			return;
		}
		EntityUid owner = gridEntity.Value.Owner;
		MapGridComponent comp = gridEntity.Value.Comp;
		Vector2i val = _mapSystem.TileIndicesFor(owner, comp, xform.Coordinates);
		if (MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(val, (Direction)4)), smoothQuery))
		{
			cardinalConnectDirs |= CardinalConnectDirs.North;
		}
		if (MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(val, (Direction)0)), smoothQuery))
		{
			cardinalConnectDirs |= CardinalConnectDirs.South;
		}
		if (MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(val, (Direction)2)), smoothQuery))
		{
			cardinalConnectDirs |= CardinalConnectDirs.East;
		}
		if (MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(val, (Direction)6)), smoothQuery))
		{
			cardinalConnectDirs |= CardinalConnectDirs.West;
		}
		_sprite.LayerSetRsiState(sprite.AsNullable(), 0, StateId.op_Implicit($"{smooth.StateBase}{(int)cardinalConnectDirs}"));
		DirectionFlag val2 = (DirectionFlag)0;
		if ((cardinalConnectDirs & CardinalConnectDirs.South) != CardinalConnectDirs.None)
		{
			val2 = (DirectionFlag)(val2 | 1);
		}
		if ((cardinalConnectDirs & CardinalConnectDirs.East) != CardinalConnectDirs.None)
		{
			val2 = (DirectionFlag)(val2 | 2);
		}
		if ((cardinalConnectDirs & CardinalConnectDirs.North) != CardinalConnectDirs.None)
		{
			val2 = (DirectionFlag)(val2 | 4);
		}
		if ((cardinalConnectDirs & CardinalConnectDirs.West) != CardinalConnectDirs.None)
		{
			val2 = (DirectionFlag)(val2 | 8);
		}
		CalculateEdge(Entity<SpriteComponent>.op_Implicit(sprite), val2, Entity<SpriteComponent>.op_Implicit(sprite));
	}

	private bool MatchingEntity(EntityUid uid, IconSmoothComponent smooth, AnchoredEntitiesEnumerator candidates, EntityQuery<IconSmoothComponent> smoothQuery)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = default(EntityUid?);
		IconSmoothComponent iconSmoothComponent = default(IconSmoothComponent);
		CMIconSmoothComponent cMIconSmoothComponent = default(CMIconSmoothComponent);
		while (((AnchoredEntitiesEnumerator)(ref candidates)).MoveNext(ref val))
		{
			if (smoothQuery.TryGetComponent(val, ref iconSmoothComponent) && iconSmoothComponent.Enabled && ((iconSmoothComponent.SmoothKey != null && (iconSmoothComponent.SmoothKey == smooth.SmoothKey || smooth.AdditionalKeys.Contains(iconSmoothComponent.SmoothKey))) || (_cmIconSmoothQuery.TryComp(uid, ref cMIconSmoothComponent) && cMIconSmoothComponent.Smooth && _cmIconSmoothQuery.HasComp(val))))
			{
				return true;
			}
		}
		return false;
	}

	private void CalculateNewSpriteCorners(EntityUid uid, Entity<MapGridComponent>? gridEntity, IconSmoothComponent smooth, Entity<SpriteComponent> spriteEnt, TransformComponent xform, EntityQuery<IconSmoothComponent> smoothQuery)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		CornerFill cornerFill;
		CornerFill cornerFill2;
		CornerFill cornerFill3;
		CornerFill cornerFill4;
		if (gridEntity.HasValue)
		{
			(cornerFill, cornerFill2, cornerFill3, cornerFill4) = CalculateCornerFill(uid, gridEntity.Value, smooth, xform, smoothQuery);
		}
		else
		{
			cornerFill = CornerFill.None;
			cornerFill2 = CornerFill.None;
			cornerFill3 = CornerFill.None;
			cornerFill4 = CornerFill.None;
		}
		SpriteComponent comp = spriteEnt.Comp;
		_sprite.LayerSetRsiState(spriteEnt.AsNullable(), (Enum)CornerLayers.NE, StateId.op_Implicit($"{smooth.StateBase}{(int)cornerFill}"));
		_sprite.LayerSetRsiState(spriteEnt.AsNullable(), (Enum)CornerLayers.SE, StateId.op_Implicit($"{smooth.StateBase}{(int)cornerFill4}"));
		_sprite.LayerSetRsiState(spriteEnt.AsNullable(), (Enum)CornerLayers.SW, StateId.op_Implicit($"{smooth.StateBase}{(int)cornerFill3}"));
		_sprite.LayerSetRsiState(spriteEnt.AsNullable(), (Enum)CornerLayers.NW, StateId.op_Implicit($"{smooth.StateBase}{(int)cornerFill2}"));
		DirectionFlag val = (DirectionFlag)0;
		if ((cornerFill4 & cornerFill3) != CornerFill.None)
		{
			val = (DirectionFlag)(val | 1);
		}
		if ((cornerFill4 & cornerFill) != CornerFill.None)
		{
			val = (DirectionFlag)(val | 2);
		}
		if ((cornerFill & cornerFill2) != CornerFill.None)
		{
			val = (DirectionFlag)(val | 4);
		}
		if ((cornerFill2 & cornerFill3) != CornerFill.None)
		{
			val = (DirectionFlag)(val | 8);
		}
		CalculateEdge(Entity<SpriteComponent>.op_Implicit(spriteEnt), val, comp);
	}

	private (CornerFill ne, CornerFill nw, CornerFill sw, CornerFill se) CalculateCornerFill(EntityUid uid, Entity<MapGridComponent> gridEntity, IconSmoothComponent smooth, TransformComponent xform, EntityQuery<IconSmoothComponent> smoothQuery)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Invalid comparison between Unknown and I4
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Invalid comparison between Unknown and I4
		EntityUid owner = gridEntity.Owner;
		MapGridComponent comp = gridEntity.Comp;
		Vector2i val = _mapSystem.TileIndicesFor(owner, comp, xform.Coordinates);
		bool num = MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(val, (Direction)4)), smoothQuery);
		bool flag = MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(val, (Direction)3)), smoothQuery);
		bool flag2 = MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(val, (Direction)2)), smoothQuery);
		bool flag3 = MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(val, (Direction)1)), smoothQuery);
		bool flag4 = MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(val, (Direction)0)), smoothQuery);
		bool flag5 = MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(val, (Direction)7)), smoothQuery);
		bool flag6 = MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(val, (Direction)6)), smoothQuery);
		bool flag7 = MatchingEntity(uid, smooth, _mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(val, (Direction)5)), smoothQuery);
		CornerFill cornerFill = CornerFill.None;
		CornerFill cornerFill2 = CornerFill.None;
		CornerFill cornerFill3 = CornerFill.None;
		CornerFill cornerFill4 = CornerFill.None;
		if (num)
		{
			cornerFill |= CornerFill.CounterClockwise;
			cornerFill4 |= CornerFill.Clockwise;
		}
		if (flag)
		{
			cornerFill |= CornerFill.Diagonal;
		}
		if (flag2)
		{
			cornerFill |= CornerFill.Clockwise;
			cornerFill2 |= CornerFill.CounterClockwise;
		}
		if (flag3)
		{
			cornerFill2 |= CornerFill.Diagonal;
		}
		if (flag4)
		{
			cornerFill2 |= CornerFill.Clockwise;
			cornerFill3 |= CornerFill.CounterClockwise;
		}
		if (flag5)
		{
			cornerFill3 |= CornerFill.Diagonal;
		}
		if (flag6)
		{
			cornerFill3 |= CornerFill.Clockwise;
			cornerFill4 |= CornerFill.CounterClockwise;
		}
		if (flag7)
		{
			cornerFill4 |= CornerFill.Diagonal;
		}
		Angle localRotation = xform.LocalRotation;
		Direction cardinalDir = ((Angle)(ref localRotation)).GetCardinalDir();
		if ((int)cardinalDir != 0)
		{
			if ((int)cardinalDir != 4)
			{
				if ((int)cardinalDir == 6)
				{
					return (ne: cornerFill2, nw: cornerFill, sw: cornerFill4, se: cornerFill3);
				}
				return (ne: cornerFill4, nw: cornerFill3, sw: cornerFill2, se: cornerFill);
			}
			return (ne: cornerFill3, nw: cornerFill2, sw: cornerFill, se: cornerFill4);
		}
		return (ne: cornerFill, nw: cornerFill4, sw: cornerFill3, se: cornerFill2);
	}

	private void InitializeEdge()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_cmIconSmoothQuery = ((EntitySystem)this).GetEntityQuery<CMIconSmoothComponent>();
		((EntitySystem)this).SubscribeLocalEvent<SmoothEdgeComponent, ComponentStartup>((ComponentEventHandler<SmoothEdgeComponent, ComponentStartup>)OnEdgeStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SmoothEdgeComponent, ComponentShutdown>((ComponentEventHandler<SmoothEdgeComponent, ComponentShutdown>)OnEdgeShutdown, (Type[])null, (Type[])null);
	}

	private void OnEdgeStartup(EntityUid uid, SmoothEdgeComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EdgeLayer.South, new Vector2(0f, -1f));
			_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EdgeLayer.East, new Vector2(1f, 0f));
			_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EdgeLayer.North, new Vector2(0f, 1f));
			_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EdgeLayer.West, new Vector2(-1f, 0f));
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EdgeLayer.South, false);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EdgeLayer.East, false);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EdgeLayer.North, false);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EdgeLayer.West, false);
		}
	}

	private void OnEdgeShutdown(EntityUid uid, SmoothEdgeComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			_sprite.LayerMapRemove(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EdgeLayer.South);
			_sprite.LayerMapRemove(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EdgeLayer.East);
			_sprite.LayerMapRemove(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EdgeLayer.North);
			_sprite.LayerMapRemove(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)EdgeLayer.West);
		}
	}

	private void CalculateEdge(EntityUid uid, DirectionFlag directions, SpriteComponent? sprite = null, SmoothEdgeComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SpriteComponent, SmoothEdgeComponent>(uid, ref sprite, ref component, false))
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			DirectionFlag val = (DirectionFlag)(sbyte)Math.Pow(2.0, i);
			EdgeLayer edge = GetEdge(val);
			if ((val & directions) != 0)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)edge, false);
			}
			else
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)edge, true);
			}
		}
	}

	private EdgeLayer GetEdge(DirectionFlag direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected I4, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		switch (direction - 1)
		{
		default:
			if ((int)direction != 8)
			{
				break;
			}
			return EdgeLayer.West;
		case 0:
			return EdgeLayer.South;
		case 1:
			return EdgeLayer.East;
		case 3:
			return EdgeLayer.North;
		case 2:
			break;
		}
		throw new ArgumentOutOfRangeException();
	}
}

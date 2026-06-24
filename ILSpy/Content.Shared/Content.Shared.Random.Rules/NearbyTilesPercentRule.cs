using System;
using System.Collections.Generic;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Random.Rules;

public sealed class NearbyTilesPercentRule : RulesRule, ISerializationGenerated<NearbyTilesPercentRule>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool IgnoreAnchored;

	[DataField(null, false, 1, true, false, null)]
	public float Percent;

	[DataField(null, false, 1, true, false, null)]
	public List<ProtoId<ContentTileDefinition>> Tiles = new List<ProtoId<ContentTileDefinition>>();

	[DataField(null, false, 1, false, false, null)]
	public float Range = 10f;

	public override bool Check(EntityManager entManager, EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		MapGridComponent grid = default(MapGridComponent);
		if (!entManager.TryGetComponent<TransformComponent>(uid, ref xform) || !entManager.TryGetComponent<MapGridComponent>(xform.GridUid, ref grid))
		{
			return false;
		}
		SharedTransformSystem transform = entManager.System<SharedTransformSystem>();
		SharedMapSystem mapSys = entManager.System<SharedMapSystem>();
		ITileDefinitionManager tileDef = IoCManager.Resolve<ITileDefinitionManager>();
		EntityQuery<PhysicsComponent> physicsQuery = entManager.GetEntityQuery<PhysicsComponent>();
		int tileCount = 0;
		int matchingTileCount = 0;
		EntityUid? ancUid = default(EntityUid?);
		PhysicsComponent physics = default(PhysicsComponent);
		foreach (TileRef tile in mapSys.GetTilesIntersecting(xform.GridUid.Value, grid, new Circle(transform.GetWorldPosition(xform), Range), true, (Predicate<TileRef>)null))
		{
			if (IgnoreAnchored)
			{
				AnchoredEntitiesEnumerator gridEnum = mapSys.GetAnchoredEntitiesEnumerator(xform.GridUid.Value, grid, tile.GridIndices);
				bool found = false;
				while (((AnchoredEntitiesEnumerator)(ref gridEnum)).MoveNext(ref ancUid))
				{
					if (physicsQuery.TryGetComponent(ancUid, ref physics) && physics.CanCollide)
					{
						found = true;
						break;
					}
				}
				if (found)
				{
					continue;
				}
			}
			tileCount++;
			if (Tiles.Contains(ProtoId<ContentTileDefinition>.op_Implicit(((IPrototype)tileDef[tile.Tile.TypeId]).ID)))
			{
				matchingTileCount++;
			}
		}
		if (tileCount == 0 || (float)matchingTileCount / (float)tileCount < Percent)
		{
			return Inverted;
		}
		return !Inverted;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NearbyTilesPercentRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		RulesRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NearbyTilesPercentRule)definitionCast;
		if (!serialization.TryCustomCopy<NearbyTilesPercentRule>(this, ref target, hookCtx, false, context))
		{
			bool IgnoreAnchoredTemp = false;
			if (!serialization.TryCustomCopy<bool>(IgnoreAnchored, ref IgnoreAnchoredTemp, hookCtx, false, context))
			{
				IgnoreAnchoredTemp = IgnoreAnchored;
			}
			target.IgnoreAnchored = IgnoreAnchoredTemp;
			float PercentTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Percent, ref PercentTemp, hookCtx, false, context))
			{
				PercentTemp = Percent;
			}
			target.Percent = PercentTemp;
			List<ProtoId<ContentTileDefinition>> TilesTemp = null;
			if (Tiles == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<ContentTileDefinition>>>(Tiles, ref TilesTemp, hookCtx, true, context))
			{
				TilesTemp = serialization.CreateCopy<List<ProtoId<ContentTileDefinition>>>(Tiles, hookCtx, context, false);
			}
			target.Tiles = TilesTemp;
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NearbyTilesPercentRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RulesRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NearbyTilesPercentRule cast = (NearbyTilesPercentRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NearbyTilesPercentRule cast = (NearbyTilesPercentRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NearbyTilesPercentRule Instantiate()
	{
		return new NearbyTilesPercentRule();
	}
}

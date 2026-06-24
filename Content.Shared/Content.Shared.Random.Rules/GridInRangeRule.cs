using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Random.Rules;

public sealed class GridInRangeRule : RulesRule, ISerializationGenerated<GridInRangeRule>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Range = 10f;

	private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

	public override bool Check(EntityManager entManager, EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (!entManager.TryGetComponent<TransformComponent>(uid, ref xform))
		{
			return false;
		}
		if (xform.GridUid.HasValue)
		{
			return !Inverted;
		}
		SharedTransformSystem obj = entManager.System<SharedTransformSystem>();
		IMapManager mapManager = IoCManager.Resolve<IMapManager>();
		Vector2 worldPos = obj.GetWorldPosition(xform);
		Vector2 gridRange = new Vector2(Range, Range);
		_grids.Clear();
		mapManager.FindGridsIntersecting(xform.MapID, new Box2(worldPos - gridRange, worldPos + gridRange), ref _grids, false, true);
		if (_grids.Count > 0)
		{
			return !Inverted;
		}
		return Inverted;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GridInRangeRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RulesRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GridInRangeRule)definitionCast;
		if (!serialization.TryCustomCopy<GridInRangeRule>(this, ref target, hookCtx, false, context))
		{
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GridInRangeRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RulesRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GridInRangeRule cast = (GridInRangeRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GridInRangeRule cast = (GridInRangeRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GridInRangeRule Instantiate()
	{
		return new GridInRangeRule();
	}
}

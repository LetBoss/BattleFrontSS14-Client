using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Random.Rules;

public sealed class NearbyComponentsRule : RulesRule, ISerializationGenerated<NearbyComponentsRule>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Anchored;

	[DataField(null, false, 1, false, false, null)]
	public int Count;

	[DataField(null, false, 1, true, false, null)]
	public ComponentRegistry Components;

	[DataField(null, false, 1, false, false, null)]
	public float Range = 10f;

	public override bool Check(EntityManager entManager, EntityUid uid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		HashSet<Entity<IComponent>> inRange = new HashSet<Entity<IComponent>>();
		EntityQuery<TransformComponent> xformQuery = entManager.GetEntityQuery<TransformComponent>();
		TransformComponent xform = default(TransformComponent);
		if (!xformQuery.TryGetComponent(uid, ref xform) || !xform.MapUid.HasValue)
		{
			return false;
		}
		SharedTransformSystem obj = entManager.System<SharedTransformSystem>();
		EntityLookupSystem lookup = entManager.System<EntityLookupSystem>();
		bool found = false;
		Vector2 worldPos = obj.GetWorldPosition(xform);
		int count = 0;
		TransformComponent compXform = default(TransformComponent);
		foreach (ComponentRegistryEntry compType in ((Dictionary<string, ComponentRegistryEntry>)(object)Components).Values)
		{
			inRange.Clear();
			lookup.GetEntitiesInRange(((object)compType.Component).GetType(), xform.MapID, worldPos, Range, inRange, (LookupFlags)110);
			foreach (Entity<IComponent> comp in inRange)
			{
				if (!Anchored || (xformQuery.TryGetComponent(Entity<IComponent>.op_Implicit(comp), ref compXform) && compXform.Anchored))
				{
					count++;
					if (count >= Count)
					{
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				break;
			}
		}
		if (!found)
		{
			return Inverted;
		}
		return !Inverted;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NearbyComponentsRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		RulesRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NearbyComponentsRule)definitionCast;
		if (!serialization.TryCustomCopy<NearbyComponentsRule>(this, ref target, hookCtx, false, context))
		{
			bool AnchoredTemp = false;
			if (!serialization.TryCustomCopy<bool>(Anchored, ref AnchoredTemp, hookCtx, false, context))
			{
				AnchoredTemp = Anchored;
			}
			target.Anchored = AnchoredTemp;
			int CountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Count, ref CountTemp, hookCtx, false, context))
			{
				CountTemp = Count;
			}
			target.Count = CountTemp;
			ComponentRegistry ComponentsTemp = null;
			if (Components == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<ComponentRegistry>(Components, ref ComponentsTemp, hookCtx, false, context))
			{
				ComponentsTemp = serialization.CreateCopy<ComponentRegistry>(Components, hookCtx, context, false);
			}
			target.Components = ComponentsTemp;
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NearbyComponentsRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RulesRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NearbyComponentsRule cast = (NearbyComponentsRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NearbyComponentsRule cast = (NearbyComponentsRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NearbyComponentsRule Instantiate()
	{
		return new NearbyComponentsRule();
	}
}

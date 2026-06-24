using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Random.Rules;

public sealed class NearbyAccessRule : RulesRule, ISerializationGenerated<NearbyAccessRule>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Anchored = true;

	[DataField(null, false, 1, false, false, null)]
	public int Count = 1;

	[DataField(null, false, 1, true, false, null)]
	public List<ProtoId<AccessLevelPrototype>> Access = new List<ProtoId<AccessLevelPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public float Range = 10f;

	public override bool Check(EntityManager entManager, EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery<TransformComponent> xformQuery = entManager.GetEntityQuery<TransformComponent>();
		TransformComponent xform = default(TransformComponent);
		if (!xformQuery.TryGetComponent(uid, ref xform) || !xform.MapUid.HasValue)
		{
			return false;
		}
		SharedTransformSystem obj = entManager.System<SharedTransformSystem>();
		EntityLookupSystem lookup = entManager.System<EntityLookupSystem>();
		AccessReaderSystem reader = entManager.System<AccessReaderSystem>();
		bool found = false;
		Vector2 worldPos = obj.GetWorldPosition(xform, xformQuery);
		int count = 0;
		HashSet<Entity<AccessReaderComponent>> entities = new HashSet<Entity<AccessReaderComponent>>();
		lookup.GetEntitiesInRange<AccessReaderComponent>(xform.MapID, worldPos, Range, entities, (LookupFlags)110);
		TransformComponent compXform = default(TransformComponent);
		foreach (Entity<AccessReaderComponent> comp in entities)
		{
			if (reader.AreAccessTagsAllowed(Access, Entity<AccessReaderComponent>.op_Implicit(comp)) && (!Anchored || (xformQuery.TryGetComponent(Entity<AccessReaderComponent>.op_Implicit(comp), ref compXform) && compXform.Anchored)))
			{
				count++;
				if (count >= Count)
				{
					found = true;
					break;
				}
			}
		}
		if (!found)
		{
			return Inverted;
		}
		return !Inverted;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NearbyAccessRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		RulesRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NearbyAccessRule)definitionCast;
		if (!serialization.TryCustomCopy<NearbyAccessRule>(this, ref target, hookCtx, false, context))
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
			List<ProtoId<AccessLevelPrototype>> AccessTemp = null;
			if (Access == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<AccessLevelPrototype>>>(Access, ref AccessTemp, hookCtx, true, context))
			{
				AccessTemp = serialization.CreateCopy<List<ProtoId<AccessLevelPrototype>>>(Access, hookCtx, context, false);
			}
			target.Access = AccessTemp;
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NearbyAccessRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RulesRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NearbyAccessRule cast = (NearbyAccessRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NearbyAccessRule cast = (NearbyAccessRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NearbyAccessRule Instantiate()
	{
		return new NearbyAccessRule();
	}
}

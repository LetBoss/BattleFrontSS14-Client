using System;
using System.Numerics;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Random.Rules;

public sealed class NearbyEntitiesRule : RulesRule, ISerializationGenerated<NearbyEntitiesRule>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int Count = 1;

	[DataField(null, false, 1, true, false, null)]
	public EntityWhitelist Whitelist = new EntityWhitelist();

	[DataField(null, false, 1, false, false, null)]
	public float Range = 10f;

	public override bool Check(EntityManager entManager, EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (!entManager.TryGetComponent<TransformComponent>(uid, ref xform) || !xform.MapUid.HasValue)
		{
			return false;
		}
		SharedTransformSystem obj = entManager.System<SharedTransformSystem>();
		EntityLookupSystem lookup = entManager.System<EntityLookupSystem>();
		EntityWhitelistSystem whitelistSystem = entManager.System<EntityWhitelistSystem>();
		bool found = false;
		Vector2 worldPos = obj.GetWorldPosition(xform);
		int count = 0;
		foreach (EntityUid ent in lookup.GetEntitiesInRange(xform.MapID, worldPos, Range, (LookupFlags)110))
		{
			if (!whitelistSystem.IsWhitelistFail(Whitelist, ent))
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
	public void InternalCopy(ref NearbyEntitiesRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		RulesRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NearbyEntitiesRule)definitionCast;
		if (serialization.TryCustomCopy<NearbyEntitiesRule>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		int CountTemp = 0;
		if (!serialization.TryCustomCopy<int>(Count, ref CountTemp, hookCtx, false, context))
		{
			CountTemp = Count;
		}
		target.Count = CountTemp;
		EntityWhitelist WhitelistTemp = null;
		if (Whitelist == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, true);
			}
		}
		target.Whitelist = WhitelistTemp;
		float RangeTemp = 0f;
		if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
		{
			RangeTemp = Range;
		}
		target.Range = RangeTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NearbyEntitiesRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RulesRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NearbyEntitiesRule cast = (NearbyEntitiesRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NearbyEntitiesRule cast = (NearbyEntitiesRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NearbyEntitiesRule Instantiate()
	{
		return new NearbyEntitiesRule();
	}
}

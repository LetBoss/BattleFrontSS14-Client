using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Random.Rules;

public sealed class OnMapGridRule : RulesRule, ISerializationGenerated<OnMapGridRule>, ISerializationGenerated
{
	public override bool Check(EntityManager entManager, EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (entManager.TryGetComponent<TransformComponent>(uid, ref xform))
		{
			EntityUid? gridUid = xform.GridUid;
			EntityUid? mapUid = xform.MapUid;
			if (gridUid.HasValue == mapUid.HasValue && (!gridUid.HasValue || !(gridUid.GetValueOrDefault() != mapUid.GetValueOrDefault())) && xform.MapUid.HasValue)
			{
				return !Inverted;
			}
		}
		return Inverted;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref OnMapGridRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RulesRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (OnMapGridRule)definitionCast;
		serialization.TryCustomCopy<OnMapGridRule>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref OnMapGridRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RulesRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OnMapGridRule cast = (OnMapGridRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OnMapGridRule cast = (OnMapGridRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override OnMapGridRule Instantiate()
	{
		return new OnMapGridRule();
	}
}

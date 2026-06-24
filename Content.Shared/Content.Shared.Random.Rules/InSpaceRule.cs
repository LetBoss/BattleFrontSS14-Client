using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Random.Rules;

public sealed class InSpaceRule : RulesRule, ISerializationGenerated<InSpaceRule>, ISerializationGenerated
{
	public override bool Check(EntityManager entManager, EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (!entManager.TryGetComponent<TransformComponent>(uid, ref xform) || xform.GridUid.HasValue)
		{
			return Inverted;
		}
		return !Inverted;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InSpaceRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RulesRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (InSpaceRule)definitionCast;
		serialization.TryCustomCopy<InSpaceRule>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InSpaceRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RulesRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InSpaceRule cast = (InSpaceRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InSpaceRule cast = (InSpaceRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override InSpaceRule Instantiate()
	{
		return new InSpaceRule();
	}
}

using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Random.Rules;

public sealed class AlwaysTrueRule : RulesRule, ISerializationGenerated<AlwaysTrueRule>, ISerializationGenerated
{
	public override bool Check(EntityManager entManager, EntityUid uid)
	{
		return !Inverted;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AlwaysTrueRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RulesRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AlwaysTrueRule)definitionCast;
		serialization.TryCustomCopy<AlwaysTrueRule>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AlwaysTrueRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RulesRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AlwaysTrueRule cast = (AlwaysTrueRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AlwaysTrueRule cast = (AlwaysTrueRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AlwaysTrueRule Instantiate()
	{
		return new AlwaysTrueRule();
	}
}

using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Random.Rules;

[ImplicitDataDefinitionForInheritors]
public abstract class RulesRule : ISerializationGenerated<RulesRule>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Inverted;

	public abstract bool Check(EntityManager entManager, EntityUid uid);

	public RulesRule()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref RulesRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<RulesRule>(this, ref target, hookCtx, false, context))
		{
			bool InvertedTemp = false;
			if (!serialization.TryCustomCopy<bool>(Inverted, ref InvertedTemp, hookCtx, false, context))
			{
				InvertedTemp = Inverted;
			}
			target.Inverted = InvertedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref RulesRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RulesRule cast = (RulesRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual RulesRule Instantiate()
	{
		throw new NotImplementedException();
	}
}

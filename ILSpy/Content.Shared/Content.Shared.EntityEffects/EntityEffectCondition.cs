using System;
using System.Text.Json.Serialization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects;

[ImplicitDataDefinitionForInheritors]
public abstract class EntityEffectCondition : ISerializationGenerated<EntityEffectCondition>, ISerializationGenerated
{
	[JsonPropertyName("id")]
	private protected string _id => GetType().Name;

	public abstract bool Condition(EntityEffectBaseArgs args);

	public abstract string GuidebookExplanation(IPrototypeManager prototype);

	public EntityEffectCondition()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref EntityEffectCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<EntityEffectCondition>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref EntityEffectCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffectCondition cast = (EntityEffectCondition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual EntityEffectCondition Instantiate()
	{
		throw new NotImplementedException();
	}
}

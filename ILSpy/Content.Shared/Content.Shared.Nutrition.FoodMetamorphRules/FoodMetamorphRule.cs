using System;
using System.Collections.Generic;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Nutrition.FoodMetamorphRules;

[Serializable]
[ImplicitDataDefinitionForInheritors]
[NetSerializable]
public abstract class FoodMetamorphRule : ISerializationGenerated<FoodMetamorphRule>, ISerializationGenerated
{
	public abstract bool Check(IPrototypeManager protoMan, EntityManager entMan, EntityUid food, List<FoodSequenceVisualLayer> ingredients);

	public FoodMetamorphRule()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref FoodMetamorphRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<FoodMetamorphRule>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref FoodMetamorphRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodMetamorphRule cast = (FoodMetamorphRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual FoodMetamorphRule Instantiate()
	{
		throw new NotImplementedException();
	}
}

using System;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedFoodSequenceSystem) })]
public sealed class FoodMetamorphableByAddingComponent : Component, ISerializationGenerated<FoodMetamorphableByAddingComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool OnlyFinal = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FoodMetamorphableByAddingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FoodMetamorphableByAddingComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FoodMetamorphableByAddingComponent>(this, ref target, hookCtx, false, context))
		{
			bool OnlyFinalTemp = false;
			if (!serialization.TryCustomCopy<bool>(OnlyFinal, ref OnlyFinalTemp, hookCtx, false, context))
			{
				OnlyFinalTemp = OnlyFinal;
			}
			target.OnlyFinal = OnlyFinalTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FoodMetamorphableByAddingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodMetamorphableByAddingComponent cast = (FoodMetamorphableByAddingComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodMetamorphableByAddingComponent cast = (FoodMetamorphableByAddingComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodMetamorphableByAddingComponent def = (FoodMetamorphableByAddingComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FoodMetamorphableByAddingComponent Instantiate()
	{
		return new FoodMetamorphableByAddingComponent();
	}
}

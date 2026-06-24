using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Kitchen.Components;

[RegisterComponent]
public sealed class FoodRecipeProviderComponent : Component, ISerializationGenerated<FoodRecipeProviderComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables]
	public List<ProtoId<FoodRecipePrototype>> ProvidedRecipes = new List<ProtoId<FoodRecipePrototype>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FoodRecipeProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FoodRecipeProviderComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FoodRecipeProviderComponent>(this, ref target, hookCtx, false, context))
		{
			List<ProtoId<FoodRecipePrototype>> ProvidedRecipesTemp = null;
			if (ProvidedRecipes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<FoodRecipePrototype>>>(ProvidedRecipes, ref ProvidedRecipesTemp, hookCtx, true, context))
			{
				ProvidedRecipesTemp = serialization.CreateCopy<List<ProtoId<FoodRecipePrototype>>>(ProvidedRecipes, hookCtx, context, false);
			}
			target.ProvidedRecipes = ProvidedRecipesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FoodRecipeProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodRecipeProviderComponent cast = (FoodRecipeProviderComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodRecipeProviderComponent cast = (FoodRecipeProviderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodRecipeProviderComponent def = (FoodRecipeProviderComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FoodRecipeProviderComponent Instantiate()
	{
		return new FoodRecipeProviderComponent();
	}
}

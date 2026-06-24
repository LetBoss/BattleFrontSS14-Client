using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Kitchen;

[ByRefEvent]
public struct GetSecretRecipesEvent
{
	public List<FoodRecipePrototype> Recipes = new List<FoodRecipePrototype>();

	public GetSecretRecipesEvent()
	{
	}
}

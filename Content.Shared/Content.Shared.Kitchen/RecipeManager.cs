using System.Collections.Generic;
using System.Linq;
using Content.Shared.FixedPoint;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Kitchen;

public sealed class RecipeManager
{
	private sealed class RecipeComparer : Comparer<FoodRecipePrototype>
	{
		public override int Compare(FoodRecipePrototype? x, FoodRecipePrototype? y)
		{
			if (x == null || y == null)
			{
				return 0;
			}
			FixedPoint2 nx = x.IngredientCount();
			FixedPoint2 ny = y.IngredientCount();
			return -nx.CompareTo(ny);
		}
	}

	[Dependency]
	private IPrototypeManager _prototypeManager;

	public List<FoodRecipePrototype> Recipes { get; private set; } = new List<FoodRecipePrototype>();

	public void Initialize()
	{
		Recipes = new List<FoodRecipePrototype>();
		foreach (FoodRecipePrototype item in _prototypeManager.EnumeratePrototypes<FoodRecipePrototype>())
		{
			if (!item.SecretRecipe)
			{
				Recipes.Add(item);
			}
		}
		Recipes.Sort(new RecipeComparer());
	}

	public bool SolidAppears(string solidId)
	{
		return Recipes.Any((FoodRecipePrototype recipe) => recipe.IngredientsSolids.ContainsKey(solidId));
	}
}

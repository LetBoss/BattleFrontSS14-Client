// Decompiled with JetBrains decompiler
// Type: Content.Shared.Kitchen.RecipeManager
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Kitchen;

public sealed class RecipeManager
{
  [Dependency]
  private IPrototypeManager _prototypeManager;

  public List<FoodRecipePrototype> Recipes { get; private set; } = new List<FoodRecipePrototype>();

  public void Initialize()
  {
    this.Recipes = new List<FoodRecipePrototype>();
    foreach (FoodRecipePrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<FoodRecipePrototype>())
    {
      if (!enumeratePrototype.SecretRecipe)
        this.Recipes.Add(enumeratePrototype);
    }
    this.Recipes.Sort((IComparer<FoodRecipePrototype>) new RecipeManager.RecipeComparer());
  }

  public bool SolidAppears(string solidId)
  {
    return this.Recipes.Any<FoodRecipePrototype>((Func<FoodRecipePrototype, bool>) (recipe => recipe.IngredientsSolids.ContainsKey(solidId)));
  }

  private sealed class RecipeComparer : Comparer<FoodRecipePrototype>
  {
    public override int Compare(FoodRecipePrototype? x, FoodRecipePrototype? y)
    {
      return x == null || y == null ? 0 : -x.IngredientCount().CompareTo(y.IngredientCount());
    }
  }
}

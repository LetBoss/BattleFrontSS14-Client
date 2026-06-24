using System;
using Content.Client.UserInterface.Controls;
using Content.Shared.Construction.Prototypes;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Construction.UI;

public interface IConstructionMenuView : IDisposable
{
	string[] Categories { get; set; }

	OptionButton OptionCategories { get; }

	bool EraseButtonPressed { get; set; }

	bool GridViewButtonPressed { get; set; }

	bool BuildButtonPressed { get; set; }

	ListContainer Recipes { get; }

	ItemList RecipeStepList { get; }

	ScrollContainer RecipesGridScrollContainer { get; }

	GridContainer RecipesGrid { get; }

	bool IsOpen { get; }

	event EventHandler<(string search, string catagory)> PopulateRecipes;

	event EventHandler<ConstructionMenu.ConstructionMenuListData?> RecipeSelected;

	event EventHandler RecipeFavorited;

	event EventHandler<bool> BuildButtonToggled;

	event EventHandler<bool> EraseButtonToggled;

	event EventHandler ClearAllGhosts;

	event Action? OnClose;

	void ClearRecipeInfo();

	void SetRecipeInfo(string name, string description, EntityPrototype? targetPrototype, Color iconColor, bool isItem, bool isFavorite, ConstructionPrototype prototype);

	void ResetPlacement();

	void OpenCentered();

	void MoveToFront();

	bool IsAtFront();

	void Close();
}

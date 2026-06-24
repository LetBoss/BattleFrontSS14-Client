using System;
using Content.Shared.Lathe;
using Content.Shared.Research.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Lathe.UI;

public sealed class LatheBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private LatheMenu? _menu;

	public LatheBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindowCenteredRight<LatheMenu>((BoundUserInterface)(object)this);
		_menu.SetEntity(((BoundUserInterface)this).Owner);
		_menu.OnServerListButtonPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ConsoleServerSelectionMessage());
		};
		_menu.RecipeQueueAction += delegate(string recipe, int amount)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new LatheQueueRecipeMessage(recipe, amount));
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is LatheUpdateState latheUpdateState)
		{
			if (_menu != null)
			{
				_menu.Recipes = latheUpdateState.Recipes;
			}
			_menu?.PopulateRecipes();
			_menu?.UpdateCategories();
			_menu?.PopulateQueueList(latheUpdateState.Queue);
			_menu?.SetQueueInfo(latheUpdateState.CurrentlyProducing);
		}
	}
}

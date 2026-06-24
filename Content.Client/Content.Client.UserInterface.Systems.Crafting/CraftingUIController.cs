using Content.Client.Construction.UI;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Crafting;

public sealed class CraftingUIController : UIController, IOnStateChanged<GameplayState>, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	private ConstructionMenuPresenter? _presenter;

	private MenuButton? CraftingButton => base.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.CraftingButton;

	public void OnStateEntered(GameplayState state)
	{
		_presenter = new ConstructionMenuPresenter();
	}

	public void OnStateExited(GameplayState state)
	{
		if (_presenter != null)
		{
			UnloadButton(_presenter);
			_presenter.Dispose();
			_presenter = null;
		}
	}

	internal void UnloadButton(ConstructionMenuPresenter? presenter = null)
	{
		if (CraftingButton == null)
		{
			return;
		}
		if (presenter == null)
		{
			if (presenter == null)
			{
				presenter = _presenter;
			}
			if (presenter == null)
			{
				return;
			}
		}
		((BaseButton)CraftingButton).Pressed = false;
		((BaseButton)CraftingButton).OnToggled -= presenter.OnHudCraftingButtonToggled;
	}

	public void LoadButton()
	{
		if (CraftingButton != null)
		{
			((BaseButton)CraftingButton).OnToggled += ButtonToggled;
		}
	}

	private void ButtonToggled(ButtonToggledEventArgs obj)
	{
		_presenter?.OnHudCraftingButtonToggled(obj);
	}
}

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using Content.Client.CombatMode;
using Content.Client.Gameplay;
using Content.Client.Mapping;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.ContextMenu.UI;

public sealed class ContextMenuUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>, IOnSystemChanged<CombatModeSystem>, IOnSystemLoaded<CombatModeSystem>, IOnSystemUnloaded<CombatModeSystem>, IOnStateEntered<MappingState>, IOnStateExited<MappingState>
{
	public static readonly TimeSpan HoverDelay = TimeSpan.FromSeconds(0.2);

	public ContextMenuPopup RootMenu;

	public CancellationTokenSource? CancelOpen;

	public CancellationTokenSource? CancelClose;

	public Action? OnContextClosed;

	public Action<ContextMenuElement>? OnContextMouseEntered;

	public Action<ContextMenuElement>? OnContextMouseExited;

	public Action<ContextMenuElement>? OnSubMenuOpened;

	public Action<ContextMenuElement, GUIBoundKeyEventArgs>? OnContextKeyEvent;

	private bool _setup;

	public Stack<ContextMenuPopup> Menus { get; } = new Stack<ContextMenuPopup>();

	public void OnStateEntered(GameplayState state)
	{
		Setup();
	}

	public void OnStateExited(GameplayState state)
	{
		Shutdown();
	}

	public void OnStateEntered(MappingState state)
	{
		Setup();
	}

	public void OnStateExited(MappingState state)
	{
		Shutdown();
	}

	public void Setup()
	{
		if (!_setup)
		{
			_setup = true;
			RootMenu = new ContextMenuPopup(this, null);
			((Popup)RootMenu).OnPopupHide += Close;
			Menus.Push(RootMenu);
		}
	}

	public void Shutdown()
	{
		if (_setup)
		{
			_setup = false;
			Close();
			((Popup)RootMenu).OnPopupHide -= Close;
			((Control)RootMenu).Orphan();
			RootMenu = null;
		}
	}

	public void Close()
	{
		((Control)RootMenu.MenuBody).DisposeAllChildren();
		CancelOpen?.Cancel();
		CancelClose?.Cancel();
		OnContextClosed?.Invoke();
		((Popup)RootMenu).Close();
	}

	public void CloseSubMenus(ContextMenuPopup? menu)
	{
		if (menu != null && ((Control)menu).Visible)
		{
			ContextMenuPopup result;
			while (Menus.TryPeek(out result) && result != menu)
			{
				((Popup)Menus.Pop()).Close();
			}
			CancelClose?.Cancel();
			CancelClose = null;
		}
	}

	private void OnMouseEntered(ContextMenuElement element)
	{
		if (!Menus.TryPeek(out ContextMenuPopup result))
		{
			((UIController)this).Log.Error("Context Menu: Mouse entered menu without any open menus?");
			return;
		}
		if (element.ParentMenu == result || element.SubMenu == result)
		{
			CancelClose?.Cancel();
		}
		if (element.SubMenu != result)
		{
			CancelOpen?.Cancel();
			CancelOpen = new CancellationTokenSource();
			Timer.Spawn(HoverDelay, (Action)delegate
			{
				OpenSubMenu(element);
			}, CancelOpen.Token);
		}
	}

	private void OnMouseExited(ContextMenuElement element)
	{
		CancelOpen?.Cancel();
		if (element.SubMenu != null)
		{
			CancelClose?.Cancel();
			CancelClose = new CancellationTokenSource();
			Timer.Spawn(HoverDelay, (Action)delegate
			{
				CloseSubMenus(element.ParentMenu);
			}, CancelClose.Token);
			OnContextMouseExited?.Invoke(element);
		}
	}

	private void OnKeyBindDown(ContextMenuElement element, GUIBoundKeyEventArgs args)
	{
		OnContextKeyEvent?.Invoke(element, args);
	}

	public void OpenSubMenu(ContextMenuElement element)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (!Menus.TryPeek(out ContextMenuPopup result))
		{
			((UIController)this).Log.Error("Context Menu: Attempting to open sub menu without any open menus?");
		}
		else if (element.SubMenu != result && !((Control)element).Disposed && element.ParentMenu != null && ((Control)element.ParentMenu).Visible)
		{
			CloseSubMenus(element.ParentMenu);
			CancelOpen?.Cancel();
			CancelOpen = null;
			if (element.SubMenu != null)
			{
				Vector2 globalPosition = ((Control)element).GlobalPosition;
				Vector2 vector = globalPosition + new Vector2(((Control)element).Width + 4f, -4f);
				((Popup)element.SubMenu).Open((UIBox2?)UIBox2.FromDimensions(vector, new Vector2(1f, 1f)), (Vector2?)globalPosition, (Vector2?)null);
				((Control)element.SubMenu).SetPositionLast();
				Menus.Push(element.SubMenu);
				OnSubMenuOpened?.Invoke(element);
			}
		}
	}

	public void AddElement(ContextMenuPopup menu, ContextMenuElement element)
	{
		((Control)element).OnMouseEntered += delegate
		{
			OnMouseEntered(element);
		};
		((Control)element).OnMouseExited += delegate
		{
			OnMouseExited(element);
		};
		((Control)element).OnKeyBindDown += delegate(GUIBoundKeyEventArgs args)
		{
			OnKeyBindDown(element, args);
		};
		element.ParentMenu = menu;
		((Control)menu.MenuBody).AddChild((Control)(object)element);
		((Control)menu).InvalidateMeasure();
	}

	public void OnRemoveElement(ContextMenuPopup menu, Control control)
	{
		ContextMenuElement element = control as ContextMenuElement;
		if (element != null)
		{
			((Control)element).OnMouseEntered -= delegate
			{
				OnMouseEntered(element);
			};
			((Control)element).OnMouseExited -= delegate
			{
				OnMouseExited(element);
			};
			((Control)element).OnKeyBindDown -= delegate(GUIBoundKeyEventArgs args)
			{
				OnKeyBindDown(element, args);
			};
			((Control)menu).InvalidateMeasure();
		}
	}

	private void OnCombatModeUpdated(bool inCombatMode)
	{
		if (inCombatMode)
		{
			Close();
		}
	}

	public void OnSystemLoaded(CombatModeSystem system)
	{
		system.LocalPlayerCombatModeUpdated += OnCombatModeUpdated;
	}

	public void OnSystemUnloaded(CombatModeSystem system)
	{
		system.LocalPlayerCombatModeUpdated -= OnCombatModeUpdated;
	}
}

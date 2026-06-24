using System;
using Content.Shared._RMC14.Requisitions;
using Content.Shared._RMC14.Requisitions.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Requisitions;

public sealed class RequisitionsBui : BoundUserInterface
{
	[Dependency]
	private IEntityManager _entities;

	[Dependency]
	private IPrototypeManager _prototypes;

	[ViewVariables]
	private RequisitionsWindow? _window;

	public RequisitionsBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<RequisitionsWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.MainView.OrderItemsButton).OnPressed += delegate
		{
			ShowView(_window, (Control)(object)_window.OrderCategoriesView);
		};
		((BaseButton)_window.MainView.ViewRequestsButton).OnPressed += delegate
		{
		};
		((BaseButton)_window.MainView.ViewOrdersButton).OnPressed += delegate
		{
		};
		((BaseButton)_window.OrderCategoriesView.MainMenuButton).OnPressed += delegate
		{
			ShowView(_window, (Control)(object)_window.MainView);
		};
		((BaseButton)_window.OrderCategoriesView.SearchMenuButton).OnPressed += delegate
		{
			ShowView(_window, (Control)(object)_window.OrderSearchView);
		};
		((BaseButton)_window.OrderSearchView.BackButton).OnPressed += delegate
		{
			ShowView(_window, (Control)(object)_window.OrderCategoriesView);
		};
		_window.OrderSearchView.SearchBar.OnTextChanged += delegate
		{
			UpdateItemSearch(_window.OrderSearchView.SearchBar.Text);
		};
		((BaseButton)_window.CategoryView.BackButton).OnPressed += delegate
		{
			ShowView(_window, (Control)(object)_window.OrderCategoriesView);
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is RequisitionsBuiState uiState)
		{
			UpdateState(uiState);
		}
	}

	private unsafe void UpdateState(RequisitionsBuiState uiState)
	{
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected O, but got Unknown
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Expected O, but got Unknown
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Expected O, but got Unknown
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			_window = BoundUserInterfaceExt.CreateWindow<RequisitionsWindow>((BoundUserInterface)(object)this);
		}
		string text = "No platform";
		string text2 = "No platform";
		bool disabled = false;
		bool? raise = null;
		int num;
		switch (uiState.PlatformLowered)
		{
		case RequisitionsElevatorMode.Lowered:
			num = 0;
			goto IL_007c;
		case RequisitionsElevatorMode.Raised:
			num = 1;
			goto IL_007c;
		case RequisitionsElevatorMode.Lowering:
			text2 = "Please wait";
			text = "Platform lowering...";
			disabled = true;
			break;
		case RequisitionsElevatorMode.Raising:
			text2 = "Please wait";
			text = "Platform raising...";
			disabled = true;
			break;
		case null:
			{
				disabled = true;
				break;
			}
			IL_007c:
			if (!uiState.Busy)
			{
				if (num == 0)
				{
					text2 = "Raise platform";
					text = "Platform position: Lowered";
					raise = true;
					break;
				}
				if (num == 1)
				{
					text2 = "Lower platform";
					text = "Platform position: Raised";
					raise = false;
					break;
				}
			}
			text = $"Platform position: {uiState.PlatformLowered}";
			text2 = "ASRS is busy";
			disabled = true;
			break;
		}
		_window.MainView.PlatformLabel.SetMessage(text, (Color?)null);
		_window.MainView.PlatformButton.Text = text2;
		((BaseButton)_window.MainView.PlatformButton).Disabled = disabled;
		if (raise.HasValue)
		{
			((BaseButton)_window.MainView.PlatformButton).OnPressed += delegate
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new RequisitionsPlatformMsg(raise.Value));
			};
		}
		FormattedMessage val = new FormattedMessage();
		val.AddMarkupOrThrow($"[bold]Supply budget: ${uiState.Balance}[/bold]");
		_window.MainView.BudgetLabel.SetMessage(val, (Color?)null);
		_window.OrderCategoriesView.BudgetLabel.SetMessage(val, (Color?)null);
		_window.CategoryView.BudgetLabel.SetMessage(val, (Color?)null);
		_window.OrderSearchView.BudgetLabel.SetMessage(val, (Color?)null);
		FormattedMessage val2 = new FormattedMessage();
		val2.AddMarkupOrThrow("[bold]Select a category[/bold]");
		_window.OrderCategoriesView.CategoryHeaderLabel.SetMessage(val2, (Color?)null);
		((Control)_window.OrderCategoriesView.CategoriesContainer).DisposeAllChildren();
		RequisitionsComputerComponent requisitionsComputerComponent = default(RequisitionsComputerComponent);
		if (_entities.TryGetComponent<RequisitionsComputerComponent>(((BoundUserInterface)this).Owner, ref requisitionsComputerComponent))
		{
			for (int num2 = 0; num2 < requisitionsComputerComponent.Categories.Count; num2++)
			{
				RequisitionsCategory requisitionsCategory = requisitionsComputerComponent.Categories[num2];
				Button val3 = new Button
				{
					Text = requisitionsCategory.Name,
					StyleClasses = { "ButtonSquare" }
				};
				int categoryIndex = num2;
				((BaseButton)val3).OnPressed += delegate
				{
					ChangeOrderCategory(categoryIndex);
				};
				((Control)_window.OrderCategoriesView.CategoriesContainer).AddChild((Control)(object)val3);
			}
		}
		Enumerator enumerator = ((Control)_window.CategoryView.OrdersContainer).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (((Enumerator)(ref enumerator)).Current is RequisitionsOrderButton order)
				{
					UpdateOrderButton(order, uiState);
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = ((Control)_window.OrderSearchView.ResultContainer).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (!(((Enumerator)(ref enumerator)).Current is RequisitionsOrderSearchGroup requisitionsOrderSearchGroup))
				{
					continue;
				}
				Enumerator enumerator2 = ((Control)requisitionsOrderSearchGroup.GroupItems).Children.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						if (((Enumerator)(ref enumerator2)).Current is RequisitionsOrderButton order2)
						{
							UpdateOrderButton(order2, uiState);
						}
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		if (!((BaseWindow)_window).IsOpen)
		{
			((BaseWindow)_window).OpenCentered();
		}
	}

	private unsafe void ShowView(RequisitionsWindow window, Control view)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = ((DefaultWindow)window).Contents.Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				current.Visible = current == view;
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private void ChangeOrderCategory(int categoryIndex)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		RequisitionsComputerComponent requisitionsComputerComponent = default(RequisitionsComputerComponent);
		if (_window == null || !_entities.TryGetComponent<RequisitionsComputerComponent>(((BoundUserInterface)this).Owner, ref requisitionsComputerComponent) || categoryIndex >= requisitionsComputerComponent.Categories.Count)
		{
			return;
		}
		ShowView(_window, (Control)(object)_window.CategoryView);
		((Control)_window.CategoryView.OrdersContainer).DisposeAllChildren();
		RequisitionsCategory requisitionsCategory = requisitionsComputerComponent.Categories[categoryIndex];
		FormattedMessage val = new FormattedMessage();
		val.AddMarkupOrThrow("[bold]Request from: " + requisitionsCategory.Name + "[/bold]");
		_window.CategoryView.RequestFromLabel.SetMessage(val, (Color?)null);
		RequisitionsBuiState state = ((BoundUserInterface)this).State as RequisitionsBuiState;
		for (int i = 0; i < requisitionsCategory.Entries.Count; i++)
		{
			RequisitionsEntry requisitionsEntry = requisitionsCategory.Entries[i];
			RequisitionsOrderButton requisitionsOrderButton = new RequisitionsOrderButton();
			int orderIndex = i;
			requisitionsOrderButton.Button.Text = requisitionsEntry.Name ?? _prototypes.Index<EntityPrototype>(EntProtoId.op_Implicit(requisitionsEntry.Crate)).Name;
			((BaseButton)requisitionsOrderButton.Button).OnPressed += delegate
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new RequisitionsBuyMsg(categoryIndex, orderIndex));
			};
			requisitionsOrderButton.SetCost(requisitionsEntry.Cost);
			UpdateOrderButton(requisitionsOrderButton, state);
			((Control)_window.CategoryView.OrdersContainer).AddChild((Control)(object)requisitionsOrderButton);
		}
	}

	private void UpdateItemSearch(string? filter = null)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		RequisitionsComputerComponent requisitionsComputerComponent = default(RequisitionsComputerComponent);
		if (_window == null || !_entities.TryGetComponent<RequisitionsComputerComponent>(((BoundUserInterface)this).Owner, ref requisitionsComputerComponent))
		{
			return;
		}
		((Control)_window.OrderSearchView.ResultContainer).DisposeAllChildren();
		if (string.IsNullOrEmpty(filter))
		{
			return;
		}
		RequisitionsBuiState state = ((BoundUserInterface)this).State as RequisitionsBuiState;
		for (int i = 0; i < requisitionsComputerComponent.Categories.Count; i++)
		{
			int num = 0;
			RequisitionsCategory requisitionsCategory = requisitionsComputerComponent.Categories[i];
			RequisitionsOrderSearchGroup requisitionsOrderSearchGroup = new RequisitionsOrderSearchGroup();
			for (int j = 0; j < requisitionsCategory.Entries.Count; j++)
			{
				RequisitionsEntry requisitionsEntry = requisitionsCategory.Entries[j];
				string text = requisitionsEntry.Name ?? _prototypes.Index<EntityPrototype>(EntProtoId.op_Implicit(requisitionsEntry.Crate)).Name;
				if (text.ToLowerInvariant().Contains(filter.Trim().ToLowerInvariant()))
				{
					RequisitionsOrderButton requisitionsOrderButton = new RequisitionsOrderButton();
					int orderIndex = j;
					int categoryIndex = i;
					requisitionsOrderButton.Button.Text = text;
					((BaseButton)requisitionsOrderButton.Button).OnPressed += delegate
					{
						((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new RequisitionsBuyMsg(categoryIndex, orderIndex));
					};
					requisitionsOrderButton.SetCost(requisitionsEntry.Cost);
					UpdateOrderButton(requisitionsOrderButton, state);
					((Control)requisitionsOrderSearchGroup.GroupItems).AddChild((Control)(object)requisitionsOrderButton);
					num++;
				}
			}
			if (num >= 1)
			{
				FormattedMessage val = new FormattedMessage();
				val.AddMarkupOrThrow("[bold]Request from: " + requisitionsCategory.Name + "[/bold]");
				requisitionsOrderSearchGroup.GroupLabel.SetMessage(val, (Color?)null);
				((Control)_window.OrderSearchView.ResultContainer).AddChild((Control)(object)requisitionsOrderSearchGroup);
			}
		}
	}

	private void UpdateOrderButton(RequisitionsOrderButton order, RequisitionsBuiState? state)
	{
		((BaseButton)order.Button).Disabled = state == null || state.Balance < order.Cost || state.Full;
	}
}

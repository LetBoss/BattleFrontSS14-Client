// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Requisitions.RequisitionsBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;

#nullable enable
namespace Content.Client._RMC14.Requisitions;

public sealed class RequisitionsBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Dependency]
  private IEntityManager _entities;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Robust.Shared.ViewVariables.ViewVariables]
  private RequisitionsWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<RequisitionsWindow>((BoundUserInterface) this);
    ((BaseButton) this._window.MainView.OrderItemsButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ShowView(this._window, (Control) this._window.OrderCategoriesView));
    ((BaseButton) this._window.MainView.ViewRequestsButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => { });
    ((BaseButton) this._window.MainView.ViewOrdersButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => { });
    ((BaseButton) this._window.OrderCategoriesView.MainMenuButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ShowView(this._window, (Control) this._window.MainView));
    ((BaseButton) this._window.OrderCategoriesView.SearchMenuButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ShowView(this._window, (Control) this._window.OrderSearchView));
    ((BaseButton) this._window.OrderSearchView.BackButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ShowView(this._window, (Control) this._window.OrderCategoriesView));
    this._window.OrderSearchView.SearchBar.OnTextChanged += (Action<LineEdit.LineEditEventArgs>) (_ => this.UpdateItemSearch(this._window.OrderSearchView.SearchBar.Text));
    ((BaseButton) this._window.CategoryView.BackButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ShowView(this._window, (Control) this._window.OrderCategoriesView));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is RequisitionsBuiState uiState))
      return;
    this.UpdateState(uiState);
  }

  private void UpdateState(RequisitionsBuiState uiState)
  {
    if (this._window == null)
      this._window = BoundUserInterfaceExt.CreateWindow<RequisitionsWindow>((BoundUserInterface) this);
    string str1 = "No platform";
    string str2 = "No platform";
    bool flag = false;
    bool? raise = new bool?();
    RequisitionsElevatorMode? platformLowered = uiState.PlatformLowered;
    if (platformLowered.HasValue)
    {
      int num;
      switch (platformLowered.GetValueOrDefault())
      {
        case RequisitionsElevatorMode.Lowered:
          num = 0;
          break;
        case RequisitionsElevatorMode.Raised:
          num = 1;
          break;
        case RequisitionsElevatorMode.Lowering:
          str2 = "Please wait";
          str1 = "Platform lowering...";
          flag = true;
          goto label_14;
        case RequisitionsElevatorMode.Raising:
          str2 = "Please wait";
          str1 = "Platform raising...";
          flag = true;
          goto label_14;
        default:
          goto label_14;
      }
      if (!uiState.Busy)
      {
        switch (num)
        {
          case 0:
            str2 = "Raise platform";
            str1 = "Platform position: Lowered";
            raise = new bool?(true);
            goto label_14;
          case 1:
            str2 = "Lower platform";
            str1 = "Platform position: Raised";
            raise = new bool?(false);
            goto label_14;
        }
      }
      str1 = $"Platform position: {uiState.PlatformLowered}";
      str2 = "ASRS is busy";
      flag = true;
    }
    else
      flag = true;
label_14:
    this._window.MainView.PlatformLabel.SetMessage(str1, new Color?());
    this._window.MainView.PlatformButton.Text = str2;
    ((BaseButton) this._window.MainView.PlatformButton).Disabled = flag;
    if (raise.HasValue)
      ((BaseButton) this._window.MainView.PlatformButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new RequisitionsPlatformMsg(raise.Value)));
    FormattedMessage formattedMessage1 = new FormattedMessage();
    formattedMessage1.AddMarkupOrThrow($"[bold]Supply budget: ${uiState.Balance}[/bold]");
    this._window.MainView.BudgetLabel.SetMessage(formattedMessage1, new Color?());
    this._window.OrderCategoriesView.BudgetLabel.SetMessage(formattedMessage1, new Color?());
    this._window.CategoryView.BudgetLabel.SetMessage(formattedMessage1, new Color?());
    this._window.OrderSearchView.BudgetLabel.SetMessage(formattedMessage1, new Color?());
    FormattedMessage formattedMessage2 = new FormattedMessage();
    formattedMessage2.AddMarkupOrThrow("[bold]Select a category[/bold]");
    this._window.OrderCategoriesView.CategoryHeaderLabel.SetMessage(formattedMessage2, new Color?());
    ((Control) this._window.OrderCategoriesView.CategoriesContainer).DisposeAllChildren();
    RequisitionsComputerComponent computerComponent;
    if (this._entities.TryGetComponent<RequisitionsComputerComponent>(this.Owner, ref computerComponent))
    {
      for (int index = 0; index < computerComponent.Categories.Count; ++index)
      {
        RequisitionsCategory category = computerComponent.Categories[index];
        Button button1 = new Button();
        button1.Text = category.Name;
        ((Control) button1).StyleClasses.Add("ButtonSquare");
        Button button2 = button1;
        int categoryIndex = index;
        ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ChangeOrderCategory(categoryIndex));
        ((Control) this._window.OrderCategoriesView.CategoriesContainer).AddChild((Control) button2);
      }
    }
    foreach (Control child in ((Control) this._window.CategoryView.OrdersContainer).Children)
    {
      if (child is RequisitionsOrderButton order)
        this.UpdateOrderButton(order, uiState);
    }
    foreach (Control child1 in ((Control) this._window.OrderSearchView.ResultContainer).Children)
    {
      if (child1 is RequisitionsOrderSearchGroup orderSearchGroup)
      {
        foreach (Control child2 in ((Control) orderSearchGroup.GroupItems).Children)
        {
          if (child2 is RequisitionsOrderButton order)
            this.UpdateOrderButton(order, uiState);
        }
      }
    }
    if (((BaseWindow) this._window).IsOpen)
      return;
    ((BaseWindow) this._window).OpenCentered();
  }

  private void ShowView(RequisitionsWindow window, Control view)
  {
    foreach (Control child in window.Contents.Children)
      child.Visible = child == view;
  }

  private void ChangeOrderCategory(int categoryIndex)
  {
    RequisitionsComputerComponent computerComponent;
    if (this._window == null || !this._entities.TryGetComponent<RequisitionsComputerComponent>(this.Owner, ref computerComponent) || categoryIndex >= computerComponent.Categories.Count)
      return;
    this.ShowView(this._window, (Control) this._window.CategoryView);
    ((Control) this._window.CategoryView.OrdersContainer).DisposeAllChildren();
    RequisitionsCategory category = computerComponent.Categories[categoryIndex];
    FormattedMessage formattedMessage = new FormattedMessage();
    formattedMessage.AddMarkupOrThrow($"[bold]Request from: {category.Name}[/bold]");
    this._window.CategoryView.RequestFromLabel.SetMessage(formattedMessage, new Color?());
    RequisitionsBuiState state = this.State as RequisitionsBuiState;
    for (int index = 0; index < category.Entries.Count; ++index)
    {
      RequisitionsEntry entry = category.Entries[index];
      RequisitionsOrderButton order = new RequisitionsOrderButton();
      int orderIndex = index;
      order.Button.Text = entry.Name ?? this._prototypes.Index<EntityPrototype>(EntProtoId.op_Implicit(entry.Crate)).Name;
      ((BaseButton) order.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new RequisitionsBuyMsg(categoryIndex, orderIndex)));
      order.SetCost(entry.Cost);
      this.UpdateOrderButton(order, state);
      ((Control) this._window.CategoryView.OrdersContainer).AddChild((Control) order);
    }
  }

  private void UpdateItemSearch(string? filter = null)
  {
    RequisitionsComputerComponent computerComponent;
    if (this._window == null || !this._entities.TryGetComponent<RequisitionsComputerComponent>(this.Owner, ref computerComponent))
      return;
    ((Control) this._window.OrderSearchView.ResultContainer).DisposeAllChildren();
    if (string.IsNullOrEmpty(filter))
      return;
    RequisitionsBuiState state = this.State as RequisitionsBuiState;
    for (int index1 = 0; index1 < computerComponent.Categories.Count; ++index1)
    {
      int num = 0;
      RequisitionsCategory category = computerComponent.Categories[index1];
      RequisitionsOrderSearchGroup orderSearchGroup = new RequisitionsOrderSearchGroup();
      for (int index2 = 0; index2 < category.Entries.Count; ++index2)
      {
        RequisitionsEntry entry = category.Entries[index2];
        string str = entry.Name ?? this._prototypes.Index<EntityPrototype>(EntProtoId.op_Implicit(entry.Crate)).Name;
        if (str.ToLowerInvariant().Contains(filter.Trim().ToLowerInvariant()))
        {
          RequisitionsOrderButton order = new RequisitionsOrderButton();
          int orderIndex = index2;
          int categoryIndex = index1;
          order.Button.Text = str;
          ((BaseButton) order.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new RequisitionsBuyMsg(categoryIndex, orderIndex)));
          order.SetCost(entry.Cost);
          this.UpdateOrderButton(order, state);
          ((Control) orderSearchGroup.GroupItems).AddChild((Control) order);
          ++num;
        }
      }
      if (num >= 1)
      {
        FormattedMessage formattedMessage = new FormattedMessage();
        formattedMessage.AddMarkupOrThrow($"[bold]Request from: {category.Name}[/bold]");
        orderSearchGroup.GroupLabel.SetMessage(formattedMessage, new Color?());
        ((Control) this._window.OrderSearchView.ResultContainer).AddChild((Control) orderSearchGroup);
      }
    }
  }

  private void UpdateOrderButton(RequisitionsOrderButton order, RequisitionsBuiState? state)
  {
    ((BaseButton) order.Button).Disabled = state == null || state.Balance < order.Cost || state.Full;
  }
}

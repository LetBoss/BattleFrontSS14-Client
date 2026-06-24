// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Crafting.CraftingUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Construction.UI;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.Crafting;

public sealed class CraftingUIController : 
  UIController,
  IOnStateChanged<GameplayState>,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  private ConstructionMenuPresenter? _presenter;

  private MenuButton? CraftingButton
  {
    get => this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.CraftingButton;
  }

  public void OnStateEntered(GameplayState state)
  {
    this._presenter = new ConstructionMenuPresenter();
  }

  public void OnStateExited(GameplayState state)
  {
    if (this._presenter == null)
      return;
    this.UnloadButton(this._presenter);
    this._presenter.Dispose();
    this._presenter = (ConstructionMenuPresenter) null;
  }

  internal void UnloadButton(ConstructionMenuPresenter? presenter = null)
  {
    if (this.CraftingButton == null)
      return;
    if (presenter == null)
    {
      if (presenter == null)
        presenter = this._presenter;
      if (presenter == null)
        return;
    }
    ((BaseButton) this.CraftingButton).Pressed = false;
    ((BaseButton) this.CraftingButton).OnToggled -= new Action<BaseButton.ButtonToggledEventArgs>(presenter.OnHudCraftingButtonToggled);
  }

  public void LoadButton()
  {
    if (this.CraftingButton == null)
      return;
    ((BaseButton) this.CraftingButton).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(this.ButtonToggled);
  }

  private void ButtonToggled(BaseButton.ButtonToggledEventArgs obj)
  {
    this._presenter?.OnHudCraftingButtonToggled(obj);
  }
}

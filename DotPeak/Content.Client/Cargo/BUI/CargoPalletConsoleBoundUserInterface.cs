// Decompiled with JetBrains decompiler
// Type: Content.Client.Cargo.BUI.CargoPalletConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Cargo.UI;
using Content.Shared.Cargo.BUI;
using Content.Shared.Cargo.Events;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Cargo.BUI;

public sealed class CargoPalletConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private CargoPalletMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<CargoPalletMenu>((BoundUserInterface) this);
    this._menu.AppraiseRequested += new Action(this.OnAppraisal);
    this._menu.SellRequested += new Action(this.OnSell);
  }

  private void OnAppraisal()
  {
    this.SendMessage((BoundUserInterfaceMessage) new CargoPalletAppraiseMessage());
  }

  private void OnSell()
  {
    this.SendMessage((BoundUserInterfaceMessage) new CargoPalletSellMessage());
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is CargoPalletConsoleInterfaceState consoleInterfaceState))
      return;
    this._menu?.SetEnabled(consoleInterfaceState.Enabled);
    this._menu?.SetAppraisal(consoleInterfaceState.Appraisal);
    this._menu?.SetCount(consoleInterfaceState.Count);
  }
}

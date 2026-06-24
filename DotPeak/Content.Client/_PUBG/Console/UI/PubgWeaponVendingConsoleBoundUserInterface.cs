// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Console.UI.PubgWeaponVendingConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client._PUBG.Console.UI;

public sealed class PubgWeaponVendingConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private PubgWeaponVendingConsoleWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = new PubgWeaponVendingConsoleWindow();
    ((BaseWindow) this._window).OpenCentered();
    ((BaseWindow) this._window).OnClose += new Action(((BoundUserInterface) this).Close);
    this._window.OnItemSelected += (Action<string>) (itemId => this.SendMessage((BoundUserInterfaceMessage) new PubgWeaponVendingDispenseMessage(itemId)));
    PubgWeaponVendingConsoleComponent consoleComponent;
    PubgWeaponVendingInventoryPrototype inventory;
    if (!this.EntMan.TryGetComponent<PubgWeaponVendingConsoleComponent>(this.Owner, ref consoleComponent) || !IoCManager.Resolve<IPrototypeManager>().TryIndex<PubgWeaponVendingInventoryPrototype>(consoleComponent.InventoryPrototype, ref inventory))
      return;
    this._window.Populate(inventory);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._window)?.Dispose();
    this._window = (PubgWeaponVendingConsoleWindow) null;
  }
}

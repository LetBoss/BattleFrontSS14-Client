// Decompiled with JetBrains decompiler
// Type: Content.Client.Cargo.BUI.CargoOrderConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Cargo.UI;
using Content.Shared.Cargo;
using Content.Shared.Cargo.BUI;
using Content.Shared.Cargo.Components;
using Content.Shared.Cargo.Events;
using Content.Shared.Cargo.Prototypes;
using Content.Shared.IdentityManagement;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Cargo.BUI;

public sealed class CargoOrderConsoleBoundUserInterface : BoundUserInterface
{
  private readonly SharedCargoSystem _cargoSystem;
  [Robust.Shared.ViewVariables.ViewVariables]
  private CargoConsoleMenu? _menu;
  [Robust.Shared.ViewVariables.ViewVariables]
  private CargoConsoleOrderMenu? _orderMenu;
  [Robust.Shared.ViewVariables.ViewVariables]
  private CargoProductPrototype? _product;

  [Robust.Shared.ViewVariables.ViewVariables]
  public string? AccountName { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public int BankBalance { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public int OrderCapacity { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public int OrderCount { get; private set; }

  public CargoOrderConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._cargoSystem = this.EntMan.System<SharedCargoSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    SpriteSystem spriteSystem = this.EntMan.System<SpriteSystem>();
    IDependencyCollection instance = IoCManager.Instance;
    this._menu = new CargoConsoleMenu(this.Owner, this.EntMan, instance.Resolve<IPrototypeManager>(), spriteSystem);
    EntityUid? localEntity = ((ISharedPlayerManager) instance.Resolve<IPlayerManager>()).LocalEntity;
    FormattedMessage description = new FormattedMessage();
    string orderRequester = !this.EntMan.EntityExists(localEntity) ? string.Empty : (string) Identity.Name(localEntity.Value, this.EntMan);
    this._orderMenu = new CargoConsoleOrderMenu();
    this._menu.OnClose += new Action(((BoundUserInterface) this).Close);
    this._menu.OnItemSelected += (Action<BaseButton.ButtonEventArgs>) (args =>
    {
      if (!(((Control) args.Button).Parent is CargoProductRow parent2))
        return;
      description.Clear();
      description.PushColor(Color.White);
      if (((Control) parent2.MainButton).ToolTip != null)
        description.AddText(((Control) parent2.MainButton).ToolTip);
      this._orderMenu.Description.SetMessage(description, new Color?());
      this._product = parent2.Product;
      this._orderMenu.ProductName.Text = parent2.ProductName.Text;
      this._orderMenu.PointCost.Text = parent2.PointCost.Text;
      this._orderMenu.Requester.Text = orderRequester;
      this._orderMenu.Reason.Text = "";
      this._orderMenu.Amount.Value = 1;
      ((BaseWindow) this._orderMenu).OpenCentered();
    });
    this._menu.OnOrderApproved += new Action<BaseButton.ButtonEventArgs>(this.ApproveOrder);
    this._menu.OnOrderCanceled += new Action<BaseButton.ButtonEventArgs>(this.RemoveOrder);
    ((BaseButton) this._orderMenu.SubmitButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      if (!this.AddOrder())
        return;
      ((BaseWindow) this._orderMenu).Close();
    });
    this._menu.OnAccountAction += (Action<ProtoId<CargoAccountPrototype>?, int>) ((account, amount) => this.SendMessage((BoundUserInterfaceMessage) new CargoConsoleWithdrawFundsMessage(account, amount)));
    this._menu.OnToggleUnboundedLimit += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new CargoConsoleToggleLimitMessage()));
    this._menu.OpenCentered();
  }

  private void Populate(List<CargoOrderData> orders)
  {
    if (this._menu == null)
      return;
    this._menu.PopulateProducts();
    this._menu.PopulateCategories();
    this._menu.PopulateOrders((IEnumerable<CargoOrderData>) orders);
    this._menu.PopulateAccountActions();
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    CargoOrderConsoleComponent consoleComponent;
    if (!(state is CargoConsoleInterfaceState consoleInterfaceState) || !this.EntMan.TryGetComponent<CargoOrderConsoleComponent>(this.Owner, ref consoleComponent))
      return;
    EntityUid entity = this.EntMan.GetEntity(consoleInterfaceState.Station);
    this.OrderCapacity = consoleInterfaceState.Capacity;
    this.OrderCount = consoleInterfaceState.Count;
    this.BankBalance = this._cargoSystem.GetBalanceFromAccount(Entity<StationBankAccountComponent>.op_Implicit(entity), consoleComponent.Account);
    this.AccountName = consoleInterfaceState.Name;
    if (this._menu == null)
      return;
    this._menu.ProductCatalogue = consoleInterfaceState.Products;
    this._menu?.UpdateStation(entity);
    this.Populate(consoleInterfaceState.Orders);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._menu)?.Orphan();
    ((Control) this._orderMenu)?.Orphan();
  }

  private bool AddOrder()
  {
    CargoConsoleOrderMenu orderMenu = this._orderMenu;
    int amount = orderMenu != null ? orderMenu.Amount.Value : 0;
    if (amount < 1 || amount > this.OrderCapacity)
      return false;
    this.SendMessage((BoundUserInterfaceMessage) new CargoConsoleAddOrderMessage(this._orderMenu?.Requester.Text ?? "", this._orderMenu?.Reason.Text ?? "", this._product?.ID ?? "", amount));
    return true;
  }

  private void RemoveOrder(BaseButton.ButtonEventArgs args)
  {
    if (!(((Control) args.Button).Parent?.Parent is CargoOrderRow parent) || parent.Order == null)
      return;
    this.SendMessage((BoundUserInterfaceMessage) new CargoConsoleRemoveOrderMessage(parent.Order.OrderId));
  }

  private void ApproveOrder(BaseButton.ButtonEventArgs args)
  {
    if (!(((Control) args.Button).Parent?.Parent is CargoOrderRow parent) || parent.Order == null || this.OrderCount >= this.OrderCapacity)
      return;
    this.SendMessage((BoundUserInterfaceMessage) new CargoConsoleApproveOrderMessage(parent.Order.OrderId));
  }
}
